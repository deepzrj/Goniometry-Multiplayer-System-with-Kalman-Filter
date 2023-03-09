using UnityEngine;

namespace MSP_Input
{
    public partial class OrientationSensors : MonoBehaviour
    {
        //====================================================================

        private void OnValidate()
        {
            Singleton = this;
            SetBaseOrientation(baseOrientation);  
        }

        //====================================================================

        //private void Start()
        //{
        //    Input.gyro.enabled = true;
        //}

        //====================================================================

        private void Awake()
        {
            Singleton = this;
            Input.gyro.enabled = true;
            Input.gyro.updateInterval = 1f / gyroscopeUpdateFrequency;
            //Input.location.Start();
            SetBaseOrientation(baseOrientation);

#if UNITY_EDITOR
            UnityEditor.PlayerSettings.accelerometerFrequency = accelerometerUpdateFrequency;
#endif

#if UNITY_EDITOR
            if (editorSimulation == EditorSimulationMode.Mouse)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
#endif
            gameWorldHPR_Unclamped = HeadingPitchRoll.Zero();
            gameWorldHPR_Clamped = HeadingPitchRoll.Zero();
            deviceHPR = HeadingPitchRoll.Zero();

            pitchOffset.ValidateData();
        }

        //====================================================================

        private void OnApplicationQuit()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //====================================================================

		private void Update()
        {
	        Singleton = this;
            if (!Input.gyro.enabled)
            {
                Input.gyro.enabled = true;
            }
            MagnetometerEnabled(magnetometerEnabled || gyroDriftCompensation);

#if UNITY_EDITOR
            // USE CURSORKEYS/MOUSE IN EDITOR
            if (editorSimulation != EditorSimulationMode.None)
            {
                SimulationMode();
            }
#endif

            if (BaseOrientationIsFaceUp())
            {
                pitchOffset.value = 0f;
            }

            // DEVICE Heading, Pitch & Roll: accelerometer
            accelerometerEnabled = !gyroscopeEnabled;
            if (accelerometerEnabled)
            {
                deviceHPR = GetDeviceHeadingPitchRollFromAccelerometer();
            }

            // DEVICE Heading, Pitch & Roll: magnetometer
            bool compassHasBeenUpdated = false;
            if (magnetometerEnabled)
            {
                compassHasBeenUpdated = GetCompassHeadingFromMagnetometer(ref compassHeading, magneticDeclination);
                deviceHPR.heading = compassHeading;
                // [Optional]: set game north to compass north
                if (setGameNorthToCompassNorth)
                {
                    gyroDriftCompensation = false;
                    gyroHeadingAmplifier = 1f;
                    gyroPitchAmplifier = 1f;
                    SetGameNorthToCompassNorth(smoothFactor);
                    deviceHPR.heading = gameWorldHPR_Clamped.heading;  //compassHeading;
                }
                else
                {
                    deviceHPR.heading = 0f;
                }
            }

            // DEVICE Heading, Pitch & Roll: gyroscope
            Quaternion gyroQuat = Quaternion.identity;
            if (gyroscopeEnabled)
            {
                deviceHPR = GetDeviceHeadingPitchRollFromGyroscope(out gyroQuat);
                //apply gyro heading and/or pitch amplification
                float rcosin = Mathf.Cos(Mathf.Deg2Rad * deviceHPR.roll);
                float rsinus = Mathf.Sin(Mathf.Deg2Rad * deviceHPR.roll);
                //
                gyroHeadingAmplifier = Mathf.Clamp(gyroHeadingAmplifier, 0.1f, 4f);
                float deltaHeading = (-Input.gyro.rotationRateUnbiased.x * rsinus - Input.gyro.rotationRateUnbiased.y * rcosin);
                deltaHeading *= (gyroHeadingAmplifier - 1f);
                AddFloatToHeadingOffset(deltaHeading);
                //
                gyroPitchAmplifier = Mathf.Clamp(gyroPitchAmplifier, 0.1f, 4f);
                float deltaPitch = (-Input.gyro.rotationRateUnbiased.y * rsinus + Input.gyro.rotationRateUnbiased.x * rcosin);
                deltaPitch *= (gyroPitchAmplifier - 1f);
                AddFloatToPitchOffset(deltaPitch);
                // [Optional]: gyro drift compensation (experimental)
                if (gyroDriftCompensation)
                {
                    if (compassHasBeenUpdated)
                    {
                        deviceHPRold = deviceHPR;
                        deviceDeltaHPRlog.Clear();
                    }
                    else
                    {
                        HeadingPitchRoll deltaHPR = deviceHPR - deviceHPRold;
                        deltaHPR /= Time.deltaTime;
                        deviceHPRold = deviceHPR;

                        deviceDeltaHPRlog.Add(deltaHPR);
                        if (deviceDeltaHPRlog.Count >= 32)
                        {
                            HeadingPitchRoll averageHPR = HeadingPitchRoll.Zero();
                            for (int i = 0; i < deviceDeltaHPRlog.Count - 1; i++)
                            {
                                averageHPR += deviceDeltaHPRlog[i];
                            }
                            averageHPR /= deviceDeltaHPRlog.Count;

                            HeadingPitchRoll stdevHPR = HeadingPitchRoll.Zero();
                            for (int i = 0; i < deviceDeltaHPRlog.Count - 1; i++)
                            {
                                stdevHPR += HeadingPitchRoll.Square(deviceDeltaHPRlog[i] - averageHPR);
                            }
                            stdevHPR = HeadingPitchRoll.Sqrt(stdevHPR / deviceDeltaHPRlog.Count);

                            gyroDriftHPR = HeadingPitchRoll.Lerp(averageHPR, gyroDriftHPR, stdevHPR * 2f);
                            deviceDeltaHPRlog.RemoveRange(0, 16);
                            //deviceDeltaHPRlog.Clear();
                        }
                    }
                    AddFloatToHeadingOffset(-gyroDriftHPR.heading * Time.deltaTime);
                    AddFloatToPitchOffset(-gyroDriftHPR.pitch * Time.deltaTime);
                }
            }

            // GAMEWORLD Rotation, Heading, Pitch & Roll: validate and apply heading and pitch offset
            Quaternion newRotation;
            if (gyroscopeEnabled)
            {
                newRotation = gyroQuat;
            }
            else
            {
                if (!BaseOrientationIsFaceUp())
                {
                    newRotation = deviceHPR.ToQuaternion();
	            }
	            else
	            {
		            newRotation = deviceHPR.ToQuaternionTopDownView();
	            }
            }
            while (headingOffset >= 180f) headingOffset -= 360f;
            while (headingOffset < -180f) headingOffset += 360f;
            pitchOffset.ValidateData();
            AnimationCurve devicePitchAdjustmentCurve = new AnimationCurve(new Keyframe(-90f, 0f), new Keyframe(pitchOffset.value, -pitchOffset.value), new Keyframe(90f, 0f));
            Vector3 rotAxis = Vector3.Cross(Vector3.up, newRotation * Vector3.forward);
            newRotation = Quaternion.AngleAxis(devicePitchAdjustmentCurve.Evaluate(deviceHPR.pitch), rotAxis) * newRotation;
            newRotation = Quaternion.AngleAxis(headingOffset, Vector3.up) * newRotation;

            // GAMEWORLD ROTATION & HPR: set final rotation and heading, pitch & roll
            gameWorldRotation = Quaternion.Slerp(gameWorldRotation, newRotation, smoothFactor);
            HeadingPitchRoll newGameWorldHPR;
            if (!BaseOrientationIsFaceUp())
            {
                newGameWorldHPR = HeadingPitchRoll.FromQuaternion(newRotation, true);
            }
            else
            {
	            newGameWorldHPR = HeadingPitchRoll.FromQuaternionTopDownView(newRotation, true);
            }

            AnimationCurve headingSmoothCurve = new AnimationCurve(
                new Keyframe(-90f, 0f, 0f, 0f),
                new Keyframe(-85, smoothFactor, 0f, 0f),
                new Keyframe(85f, smoothFactor, 0f, 0f),
                new Keyframe(90f, 0f, 0f, 0f));
            HeadingPitchRoll smoothFactorHPR = new HeadingPitchRoll()
            {
                heading = headingSmoothCurve.Evaluate(deviceHPR.pitch),
                pitch = smoothFactor,
                roll = smoothFactor
            };
            gameWorldHPR_Unclamped = HeadingPitchRoll.Lerp(gameWorldHPR_Unclamped, newGameWorldHPR, smoothFactorHPR);
            gameWorldHPR_Clamped = HeadingPitchRoll.Clamp(gameWorldHPR_Unclamped);

            UpdateTransforms();
        }

        //====================================================================

        private HeadingPitchRoll GetDeviceHeadingPitchRollFromAccelerometer()
        {
            bool compensateSensorsCachedValue = Input.compensateSensors;
            Input.compensateSensors = true;

	        Vector3 gravity = (SystemInfo.supportsGyroscope)? Input.gyro.gravity : Input.acceleration;
            HeadingPitchRoll accelHPR = HeadingPitchRoll.Zero();

            if (!BaseOrientationIsFaceUp())
            {
                accelHPR.pitch = Vector3.Angle(gravity, Vector3.forward) - 90f;
		        Vector3 gravityProjectedOnXY = Vector3.ProjectOnPlane(gravity, Vector3.forward);
		        accelHPR.roll = Vector3.SignedAngle(gravityProjectedOnXY, Vector3.down, Vector3.forward);
				AnimationCurve rollAdjustmentCurve = new AnimationCurve(new Keyframe(-90f, 0f), new Keyframe(-80f, 1f), new Keyframe(80f, 1f), new Keyframe(90f, 0f));
		        accelHPR.roll *= rollAdjustmentCurve.Evaluate(accelHPR.pitch);
	        }
	        else
	        {
		        accelHPR.pitch = 90f - Vector3.Angle(gravity, Vector3.up);
		        Vector3 gravityProjectedOnXY = Vector3.ProjectOnPlane(gravity, Vector3.up);
		        accelHPR.roll = Vector3.SignedAngle(gravityProjectedOnXY, -Vector3.forward, Vector3.down);
		        AnimationCurve rollAdjustmentCurve = new AnimationCurve(new Keyframe(-90f, 0f), new Keyframe(-80f, 1f), new Keyframe(80f, 1f), new Keyframe(90f, 0f));
		        accelHPR.roll *= rollAdjustmentCurve.Evaluate(accelHPR.pitch);
	        }
	        
            Input.compensateSensors = compensateSensorsCachedValue;
            return accelHPR;
        }

        //====================================================================

        private HeadingPitchRoll GetDeviceHeadingPitchRollFromGyroscope()
        {
            return GetDeviceHeadingPitchRollFromGyroscope(out Quaternion tempQuat);
        }

        //====================================================================

        private HeadingPitchRoll GetDeviceHeadingPitchRollFromGyroscope(out Quaternion gyroRot)
        {
            if (!SystemInfo.supportsGyroscope)
            {
                gyroRot = Quaternion.identity;
                return HeadingPitchRoll.Zero();
            }

            if (!Input.gyro.enabled)
            {
                Input.gyro.enabled = true;
            }

            bool compensateSensorsCachedValue = Input.compensateSensors;
            Input.compensateSensors = false;

            // LandscapeLeft --> gyroRot = new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
            // LandscapeRight --> gyroRot = new Quaternion(-0.5f, 0.5f, -0.5f, -0.5f) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
            // Portrait --> gyroRot = new Quaternion(0.0f, sqrthalf, -sqrthalf, 0.0f) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
            // LandscapeLeft_FaceUp --> gyroRot = new Quaternion(0.0f, 0.0f, -sqrthalf, sqrthalf) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
            // LandscapeRight_FaceUp --> gyroRot = new Quaternion(0.0f, 0.0f, sqrthalf, sqrthalf) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
            // Portrait_FaceUp --> gyroRot = new Quaternion(0f, 0f, 1f, 0f) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);

            HeadingPitchRoll gyroHPR;

            switch (baseOrientation)
            {
                case BaseOrientation.LandscapeLeft:
                    gyroRot = gyroRot = new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
                    gyroHPR = HeadingPitchRoll.FromQuaternion(gyroRot, true);
                    break;
                case BaseOrientation.Portrait:
                    gyroRot = new Quaternion(0.0f, sqrthalf, -sqrthalf, 0.0f) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
                    gyroHPR = HeadingPitchRoll.FromQuaternion(gyroRot, true);
                    break;
                case BaseOrientation.LandscapeRight:
                    gyroRot = new Quaternion(-0.5f, 0.5f, -0.5f, -0.5f) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
                    gyroHPR = HeadingPitchRoll.FromQuaternion(gyroRot, true);
                    break;
                case BaseOrientation.LandscapeLeft_FaceUp:
                    gyroRot = gyroRot = new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
                    gyroHPR = HeadingPitchRoll.FromQuaternionTopDownView(gyroRot, true);
                    break;
                case BaseOrientation.Portrait_FaceUp:
                    gyroRot = new Quaternion(0.0f, sqrthalf, -sqrthalf, 0.0f) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
                    gyroHPR = HeadingPitchRoll.FromQuaternionTopDownView(gyroRot, true);
                    break;
                case BaseOrientation.LandscapeRight_FaceUp:
                    gyroRot = new Quaternion(-0.5f, 0.5f, -0.5f, -0.5f) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
                    gyroHPR = HeadingPitchRoll.FromQuaternionTopDownView(gyroRot, true);
                    break;
                default:
                    gyroRot = new Quaternion(0.0f, sqrthalf, -sqrthalf, 0.0f) * Input.gyro.attitude * new Quaternion(0f, 0f, 1f, 0f);
                    gyroHPR = HeadingPitchRoll.FromQuaternion(gyroRot, true);
                    break;
            }

            Input.compensateSensors = compensateSensorsCachedValue;

            return gyroHPR;
        }

        //====================================================================

        private bool GetCompassHeadingFromMagnetometer(ref float magHeading, float magDeclination)
        {
            Input.compass.enabled = true;
            bool compensateSensorsCachedValue = Input.compensateSensors;
            Input.compensateSensors = true;

            if (Input.compass.timestamp <= lastCompassUpdateTime)
            {
                return false;
            }
            lastCompassUpdateTime = Input.compass.timestamp;

            Vector3 gravity = SystemInfo.supportsGyroscope ? Input.gyro.gravity : Input.acceleration.normalized;
            gravity.z *= -1f;

            Vector3 magneticField = Input.compass.rawVector;
            magneticField.z *= -1f;

            Vector3 deviceForward;
            if (!BaseOrientationIsFaceUp())
            {
                deviceForward = Vector3.forward;
            }
            else
            {
                deviceForward = Vector3.up;
            }

            Vector3 magneticFieldHorizontal = Vector3.ProjectOnPlane(magneticField, -gravity);
            Vector3 deviceForwardHorizontal = Vector3.ProjectOnPlane(deviceForward, -gravity);
            magHeading = Vector3.SignedAngle(magneticFieldHorizontal, deviceForwardHorizontal, -gravity);
            magHeading += magDeclination;
            while (magHeading >= 180f) magHeading -= 360f;
            while (magHeading < -180f) magHeading += 360f;

            Input.compensateSensors = compensateSensorsCachedValue;
            return true;
        }

        //====================================================================

        public bool BaseOrientationIsFaceUp() =>
            baseOrientation == BaseOrientation.Portrait_FaceUp ||
            baseOrientation == BaseOrientation.LandscapeLeft_FaceUp ||
            baseOrientation == BaseOrientation.LandscapeRight_FaceUp;

        //====================================================================

#if UNITY_EDITOR
        private void SimulationMode()
        {
            switch (editorSimulation)
            {
                case EditorSimulationMode.None:
                    break;
                case EditorSimulationMode.Mouse:
                    AddFloatToHeadingOffset(Input.GetAxis("Mouse X") * simulationSensitivity.x * Time.deltaTime);
                    AddFloatToPitchOffset(Input.GetAxis("Mouse Y") * simulationSensitivity.y * Time.deltaTime);
                    break;
                case EditorSimulationMode.CursorKeys:
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        AddFloatToHeadingOffset(-simulationSensitivity.x * Time.deltaTime);
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        AddFloatToHeadingOffset(simulationSensitivity.x * Time.deltaTime);
                    }

                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        AddFloatToPitchOffset(-simulationSensitivity.y * Time.deltaTime);
                    }
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        AddFloatToPitchOffset(simulationSensitivity.y * Time.deltaTime);
                    }
                    break;
                default:
                    break;
            }
        }
#endif

        //====================================================================
    }
}