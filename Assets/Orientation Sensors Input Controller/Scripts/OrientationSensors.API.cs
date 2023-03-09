using System.Collections;
using UnityEngine;

namespace MSP_Input
{
    public partial class OrientationSensors : MonoBehaviour
    {
        //====================================================================

        static public bool IsGyroscopeEnabled()
        {
            return Singleton.gyroscopeEnabled;
        }

        //====================================================================

        static public bool IsAccelerometerEnabled()
        {
            return Singleton.accelerometerEnabled;
        }

        //====================================================================

        static public bool IsGyroDriftCompensationEnabled()
        {
            return Singleton.gyroDriftCompensation;
        }

        //====================================================================

        static public bool IsMagnetometerEnabled()
        {
            return Singleton.magnetometerEnabled;
        }

        //====================================================================

        // ENABLE / DISABLE

        //====================================================================

        static public void GyroscopeEnabled(bool enabled)
	    {
			if (enabled && !Singleton.gyroscopeEnabled)
			{
				Singleton.accelerometerEnabled = false; // Enabling Gyro (Disabling Accelerometer)
				Singleton.gyroscopeEnabled = true;
				HeadingPitchRoll gyroHPR = Singleton.GetDeviceHeadingPitchRollFromGyroscope();
				AddFloatToHeadingOffset(-gyroHPR.heading);
            }
            if (!enabled && Singleton.gyroscopeEnabled)
            {
	            Singleton.accelerometerEnabled = true; // Disabling Gyro (Enabling Accelerometer)
	            Singleton.gyroscopeEnabled = false;
	            SetHeadingOffset(Singleton.gameWorldHPR_Unclamped.heading);
			}
            Singleton.gyroscopeEnabled = enabled;
		    Singleton.accelerometerEnabled = !enabled;
        }

        //====================================================================

        static public void AccelerometerEnabled(bool enabled)
        {
            if (enabled && !Singleton.accelerometerEnabled)
            {
	            Singleton.accelerometerEnabled = true; // Enabling Accelerometer (Disabling Gyro)
                Singleton.gyroscopeEnabled = false;
                SetHeadingOffset(Singleton.gameWorldHPR_Unclamped.heading);
            }
            if (!enabled && Singleton.accelerometerEnabled)
            {
	            Singleton.accelerometerEnabled = false; // Disabling Accelerometer (Enabling Gyro)
                Singleton.gyroscopeEnabled = true;
                HeadingPitchRoll gyroHPR = Singleton.GetDeviceHeadingPitchRollFromGyroscope();
                AddFloatToHeadingOffset(-gyroHPR.heading);
            }
            Singleton.accelerometerEnabled = enabled;
            Singleton.gyroscopeEnabled = !enabled;
        }

        //====================================================================

        static public void MagnetometerEnabled(bool enabled)
        {
            if (enabled && !Singleton.magnetometerEnabled)
            {
                Singleton.magnetometerEnabled = true; // starting up compass
                Input.location.Start();
                Input.compass.enabled = true;
            }
            if (!enabled && Singleton.magnetometerEnabled)
            {
                Singleton.magnetometerEnabled = false; // switching off compass
                Input.location.Stop();
                Input.compass.enabled = false;
                if (Singleton.setGameNorthToCompassNorth)
                {
                    SetHeadingOffset(Singleton.compassHeading);
                }
            }
            Singleton.magnetometerEnabled = enabled;
            Input.compass.enabled = enabled;
        }

        //====================================================================

        // SET / ADD Functions

        //====================================================================

        static public void SetBaseOrientation(BaseOrientation newBaseOrientation)
	    {
		    Singleton.baseOrientation = newBaseOrientation;
		    switch (Singleton.baseOrientation)
            {
                case BaseOrientation.Portrait:
                    Screen.orientation = ScreenOrientation.Portrait;
                    Screen.autorotateToPortrait = true;
                    Screen.autorotateToLandscapeLeft = false;
                    Screen.autorotateToLandscapeRight = false;
                    Screen.autorotateToPortraitUpsideDown = false;
                    break;
                case BaseOrientation.LandscapeLeft:
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    Screen.autorotateToPortrait = false;
                    Screen.autorotateToLandscapeLeft = true;
                    Screen.autorotateToLandscapeRight = false;
                    Screen.autorotateToPortraitUpsideDown = false;
                    break;
                case BaseOrientation.LandscapeRight:
                    Screen.orientation = ScreenOrientation.LandscapeRight;
                    Screen.autorotateToPortrait = false;
                    Screen.autorotateToLandscapeLeft = false;
                    Screen.autorotateToLandscapeRight = true;
                    Screen.autorotateToPortraitUpsideDown = false;
                    break;
                case BaseOrientation.Portrait_FaceUp:
                    Screen.orientation = ScreenOrientation.Portrait;
                    Screen.autorotateToPortrait = true;
                    Screen.autorotateToLandscapeLeft = false;
                    Screen.autorotateToLandscapeRight = false;
                    Screen.autorotateToPortraitUpsideDown = false;
                    break;
                case BaseOrientation.LandscapeLeft_FaceUp:
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    Screen.autorotateToPortrait = false;
                    Screen.autorotateToLandscapeLeft = true;
                    Screen.autorotateToLandscapeRight = false;
                    Screen.autorotateToPortraitUpsideDown = false;
                    break;
                case BaseOrientation.LandscapeRight_FaceUp:
                    Screen.orientation = ScreenOrientation.LandscapeRight;
                    Screen.autorotateToPortrait = false;
                    Screen.autorotateToLandscapeLeft = false;
                    Screen.autorotateToLandscapeRight = true;
                    Screen.autorotateToPortraitUpsideDown = false;
                    break;
                default:
                    Screen.orientation = ScreenOrientation.Portrait;
                    Screen.autorotateToPortrait = true;
                    Screen.autorotateToLandscapeLeft = false;
                    Screen.autorotateToLandscapeRight = false;
                    Screen.autorotateToPortraitUpsideDown = false;
                    break;
            }
#if UNITY_EDITOR
            switch (Singleton.baseOrientation)
            {
                case BaseOrientation.Portrait:
                    UnityEditor.PlayerSettings.defaultInterfaceOrientation = UnityEditor.UIOrientation.Portrait;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortrait = true;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeLeft = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeRight = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                    break;
                case BaseOrientation.LandscapeLeft:
                    UnityEditor.PlayerSettings.defaultInterfaceOrientation = UnityEditor.UIOrientation.LandscapeLeft;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortrait = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeLeft = true;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeRight = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                    break;
                case BaseOrientation.LandscapeRight:
                    UnityEditor.PlayerSettings.defaultInterfaceOrientation = UnityEditor.UIOrientation.LandscapeRight;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortrait = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeLeft = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeRight = true;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                    break;
                case BaseOrientation.Portrait_FaceUp:
                    UnityEditor.PlayerSettings.defaultInterfaceOrientation = UnityEditor.UIOrientation.Portrait;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortrait = true;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeLeft = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeRight = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                    break;
                case BaseOrientation.LandscapeLeft_FaceUp:
                    UnityEditor.PlayerSettings.defaultInterfaceOrientation = UnityEditor.UIOrientation.LandscapeLeft;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortrait = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeLeft = true;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeRight = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                    break;
                case BaseOrientation.LandscapeRight_FaceUp:
                    UnityEditor.PlayerSettings.defaultInterfaceOrientation = UnityEditor.UIOrientation.LandscapeRight;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortrait = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeLeft = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeRight = true;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                    break;
                default:
                    UnityEditor.PlayerSettings.defaultInterfaceOrientation = UnityEditor.UIOrientation.Portrait;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortrait = true;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeLeft = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToLandscapeRight = false;
                    UnityEditor.PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                    break;
            }
#endif
        }

        //====================================================================

        static public void SetHeading(float newHeading)
        {
            Singleton.gameWorldHPR_Unclamped.heading = newHeading;
            while (newHeading >= 180f) newHeading -= 360f;
            while (newHeading < -180f) newHeading += 360f;
            SetHeadingOffset(Singleton.headingOffset - Singleton.gameWorldHPR_Clamped.heading + newHeading);
            Singleton.gameWorldHPR_Clamped.heading = newHeading;
        }

        //====================================================================

        static private void SetHeadingOffset(float newHeadingOffset)
        {
            Singleton.headingOffset = newHeadingOffset;
        }

        //====================================================================

        static public void SetPitch(float newPitch)
        {
            newPitch = Mathf.Clamp(newPitch, -90f, 90f);
            SetPitchOffset(Singleton.pitchOffset.value + Singleton.gameWorldHPR_Clamped.pitch - newPitch);
            Singleton.gameWorldHPR_Clamped.pitch = newPitch;
            Singleton.gameWorldHPR_Unclamped.pitch = newPitch;
        }

        //====================================================================

        static private void SetPitchOffset(float newPitchOffset)
        {
            Singleton.pitchOffset.value = newPitchOffset;
            Singleton.pitchOffset.ValidateData();
        }

        //====================================================================

        static private void SetPitchOffsetMinumumMaximum(float newPitchOffsetMinimum, float newPitchOffsetMaximum)
        {
            Singleton.pitchOffset.minValue = newPitchOffsetMinimum;
            Singleton.pitchOffset.maxValue = newPitchOffsetMaximum;
            Singleton.pitchOffset.ValidateData();
        }

        //====================================================================

        static public void SetGyroHeadingAmplifier(float newValue)
        {
            Singleton.gyroHeadingAmplifier = Mathf.Clamp(newValue, 0.1f, 4f);
        }

        //====================================================================

        static public void SetGyroPitchAmplifier(float newValue)
        {
            Singleton.gyroPitchAmplifier = Mathf.Clamp(newValue, 0.1f, 4f);
        }

        //====================================================================

        static public void SetSmoothingTime(float newSmoothTime)
        {
            Singleton.smoothingTime = Mathf.Max(0f, newSmoothTime);
        }

        //====================================================================

        static public void AddFloatToHeadingOffset(float extraHeadingOffset)
        {
            Singleton.headingOffset += extraHeadingOffset;
        }

        //====================================================================

        static public void AddFloatToPitchOffset(float extraPitchOffset)
        {
            Singleton.pitchOffset.value += extraPitchOffset;
            Singleton.pitchOffset.ValidateData();
        }

        //====================================================================

        static public void SetGyroDriftCompensation(bool enabled)
        {
            Singleton.gyroDriftCompensation = enabled;
        }

        //====================================================================

        static public void SetGameNorthToCompassNorth()
        {
            SetHeading(Singleton.compassHeading);
        }

        //====================================================================

        static public void SetGameNorthToCompassNorth(float lerpFactor)
        {
            float newHeading = Mathf.LerpAngle(Singleton.gameWorldHPR_Clamped.heading, Singleton.compassHeading, lerpFactor);
            SetHeading(newHeading);
        }

        //====================================================================

        static public void SetGameNorthToCompassNorth(bool setAutomatically)
        {
            if (setAutomatically && !Singleton.setGameNorthToCompassNorth)
            {
                Singleton.setGameNorthToCompassNorth = true; // starting up
                if (Singleton.accelerometerEnabled)
                {
                    SetHeadingOffset(Singleton.deviceHPR.heading);
                }
            }
            if (!setAutomatically && Singleton.setGameNorthToCompassNorth)
            {
                Singleton.setGameNorthToCompassNorth = false; // switching off
                if (Singleton.gyroscopeEnabled)
                {
                    HeadingPitchRoll gyroHPR = Singleton.GetDeviceHeadingPitchRollFromGyroscope();
                    AddFloatToHeadingOffset(-gyroHPR.heading);
                }
            }
            Singleton.setGameNorthToCompassNorth = setAutomatically;
        }

        //====================================================================

        static public void SetGameNorthToCompassNorth(float lerpFactor, bool setAutomatically)
        {
            float newHeading = Mathf.LerpAngle(Singleton.gameWorldHPR_Clamped.heading, Singleton.compassHeading, lerpFactor);
            SetHeading(newHeading);
            Singleton.setGameNorthToCompassNorth = setAutomatically;
        }

        //====================================================================

        // GET Functions

        //====================================================================

        static public Quaternion GetRotation()
        {
            return Singleton.gameWorldRotation;
        }

        //====================================================================

        static public float GetHeading()
        {
            return Singleton.gameWorldHPR_Clamped.heading;
        }

        //====================================================================

        static public float GetHeadingUnclamped()
        {
            return Singleton.gameWorldHPR_Unclamped.heading;
        }

        //====================================================================

        static public float GetPitch()
        {
            return Singleton.gameWorldHPR_Clamped.pitch;
        }

        //====================================================================

        static public float GetRoll()
        {
            return Singleton.gameWorldHPR_Clamped.roll;
        }

        //====================================================================

        static public float GetRollUnclamped()
        {
            return Singleton.gameWorldHPR_Unclamped.roll;
        }

        //====================================================================

        static public void GetHeadingPitchRoll(out float h, out float p, out float r)
        {
            h = Singleton.gameWorldHPR_Clamped.heading;
            p = Singleton.gameWorldHPR_Clamped.pitch;
            r = Singleton.gameWorldHPR_Clamped.roll;
        }

        //====================================================================

        static public void GetHeadingPitchRollUnclamped(out float h, out float p, out float r)
        {
            h = Singleton.gameWorldHPR_Unclamped.heading;
            p = Singleton.gameWorldHPR_Unclamped.pitch;
            r = Singleton.gameWorldHPR_Unclamped.roll;
        }

	    //====================================================================

	    static public void GetDeviceHeadingPitchRoll(out float h, out float p, out float r)
	    {
		    h = Singleton.deviceHPR.heading;
		    p = Singleton.deviceHPR.pitch;
		    r = Singleton.deviceHPR.roll;
	    }
	    
        //====================================================================

        static public float GetHeadingOffset()
        {
            return Singleton.headingOffset;
        }

        //====================================================================

        static public float GetPitchOffset()
        {
            return Singleton.pitchOffset.value;
        }

        //====================================================================

        static public float GetSmoothingTime()
        {
            return Singleton.smoothingTime;
        }

        //====================================================================

        static public float GetGyroHeadingAmplifier()
        {
            return Singleton.gyroHeadingAmplifier;
        }

        //====================================================================

        static public float GetGyroPitchAmplifier()
        {
            return Singleton.gyroPitchAmplifier;
        }

        //====================================================================

        static public float GetCompassHeading360()
        {
            return ConvertAngle180To360(Singleton.compassHeading);
        }

        //====================================================================

        static public float GetCompassHeading180()
        {
            return Singleton.compassHeading;
        }

        //====================================================================

        static public float GetMagneticDeclinationAtCurrentLocation()
        {
            float declination = Input.compass.trueHeading - Input.compass.magneticHeading;
            while (declination >= 360f) declination -= 360f;
            while (declination < 0f) declination += 360f;
            return declination;
        }

        //====================================================================

        // Misc

        //====================================================================

        static public float ConvertAngle180To360(float angle180)
        {
            while (angle180 >= 180f) angle180 -= 360f;
            while (angle180 < -180f) angle180 += 360f;
            if (angle180 >= 0f)
            {
                return angle180;
            }
            else
            {
                return angle180 + 360f;
            }
        }

        //====================================================================

        static public float ConvertAngle360To180(float angle360)
        {
            while (angle360 >= 360f) angle360 -= 360f;
            while (angle360 < 0f) angle360 += 360f;
            if (angle360 >= 180f)
            {
                return angle360 - 360f;
            }
            else
            {
                return angle360;
            }
        }

        //====================================================================

        static public Quaternion GetQuaternionFromHeadingPitchRoll(float inputHeading, float inputPitch, float inputRoll)
        {
            return HeadingPitchRoll.ToQuaternion(inputHeading, inputPitch, inputRoll);
        }


        //====================================================================

        // Transform Updater Functions

        //====================================================================

        static private bool ValidTransformUpdaterID(int id)
        {
            if (id >= 0 && id < Singleton.transformUpdateList.Count)
            {
                return true;
            }
            return false;
        }

        //====================================================================

        static public int GetTransformUpdaterID(Transform sourceTransform)
        {
            for (int id = 0; id < Singleton.transformUpdateList.Count; id++)
            {
                if (Singleton.transformUpdateList[id].targetTransform.Equals(sourceTransform))
                {
                    return id;
                }
            }
            return -1;
        }

        //====================================================================

        static public bool EnableTransformUpdate(int id)
        {
            if (!ValidTransformUpdaterID(id))
            {
                return false;
            }
            Singleton.transformUpdateList[id] = new TransformUpdateData()
            {
                enabled = true,
                targetTransform = Singleton.transformUpdateList[id].targetTransform,
                smoothingTime = Singleton.transformUpdateList[id].smoothingTime,
                copyHeading = Singleton.transformUpdateList[id].copyHeading,
                heading = Singleton.transformUpdateList[id].heading,
                copyPitch = Singleton.transformUpdateList[id].copyPitch,
                pitch = Singleton.transformUpdateList[id].pitch,
                copyRoll = Singleton.transformUpdateList[id].copyRoll,
                roll = Singleton.transformUpdateList[id].roll,
                canPushEdge = Singleton.transformUpdateList[id].canPushEdge
            };
            return true;
        }

        //====================================================================

        static public bool EnableTransformUpdate(Transform targetTransform)
        {
            return EnableTransformUpdate(GetTransformUpdaterID(targetTransform));
        }

        //====================================================================

        static public bool DisableTransformUpdate(int id)
        {
            if (!ValidTransformUpdaterID(id))
            {
                return false;
            }
            Singleton.transformUpdateList[id] = new TransformUpdateData()
            {
                enabled = false,
                targetTransform = Singleton.transformUpdateList[id].targetTransform,
                smoothingTime = Singleton.transformUpdateList[id].smoothingTime,
                copyHeading = Singleton.transformUpdateList[id].copyHeading,
                heading = Singleton.transformUpdateList[id].heading,
                copyPitch = Singleton.transformUpdateList[id].copyPitch,
                pitch = Singleton.transformUpdateList[id].pitch,
                copyRoll = Singleton.transformUpdateList[id].copyRoll,
                roll = Singleton.transformUpdateList[id].roll,
                canPushEdge = Singleton.transformUpdateList[id].canPushEdge
            }; return true;
        }

        //====================================================================

        static public bool DisableTransformUpdate(Transform targetTransform)
        {
            return DisableTransformUpdate(GetTransformUpdaterID(targetTransform));
        }

        //====================================================================

    }
}