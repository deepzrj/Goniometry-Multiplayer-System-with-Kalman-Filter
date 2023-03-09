using UnityEngine;
using MSP_Input;

namespace MSP_Input.Demo
{
    public class MSP_TurretCamera : MonoBehaviour
    {
        const float fieldOfViewMin = 20f;
        const float fieldOfViewMax = 70f;
        private float fieldOfView = 70f;
        private Camera mainCamera;
        private AnimationCurve gyroAmplifierCurve;

        //====================================================================

        void Start()
        {
            mainCamera = gameObject.GetComponent<Camera>() as Camera;
            gyroAmplifierCurve = new AnimationCurve(new Keyframe(fieldOfViewMin, 0.1f), new Keyframe(fieldOfViewMax, 1.3f));
        }

        //====================================================================

        void Update()
        {
            if (TouchInterface.GetButtonDown("ZoomButton"))
            {
                fieldOfView = fieldOfViewMin;
            }

            if (TouchInterface.GetButtonUp("ZoomButton"))
            {
                fieldOfView = fieldOfViewMax;
            }

            // smoothly lerp FOV towards target value
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, fieldOfView, Time.deltaTime * 5f);

            // when zoomed in, the gyro movements will be slowed down, for more accurate aiming
            float gyroAmplifier = gyroAmplifierCurve.Evaluate(fieldOfView);
            OrientationSensors.SetGyroHeadingAmplifier(gyroAmplifier);
            OrientationSensors.SetGyroPitchAmplifier(gyroAmplifier);

            // Use the current value of the pitch to adjust the camera's position, relative to the turret 
            transform.localPosition = new Vector3(0f, 1.7f, -Mathf.Cos(Mathf.Deg2Rad * OrientationSensors.GetPitch()));
        }

        //====================================================================

    }
}