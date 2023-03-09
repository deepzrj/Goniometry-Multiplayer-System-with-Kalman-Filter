using UnityEngine;
using MSP_Input;

namespace MSP_Input.Demo
{
    public class MSP_TurretRotation : MonoBehaviour
    {
        public Transform playerCamera;
        public Transform turret;
        public Transform gun;

        //====================================================================

        void Update()
        {
            // the base of the turret only copy's the gyro's heading
            float heading = OrientationSensors.GetHeading();
            turret.rotation = OrientationSensors.GetQuaternionFromHeadingPitchRoll(heading, 0f, 0f);

            // the guns copy heading and pitch from the gyro (not roll)
            float pitch = OrientationSensors.GetPitch();
            gun.rotation = OrientationSensors.GetQuaternionFromHeadingPitchRoll(heading, pitch, 0f);

            // the camera uses the full gyro's rotation
            playerCamera.rotation = OrientationSensors.GetRotation();

            // place the camera directly behind the gun barrels
            playerCamera.localPosition = new Vector3(0f, 1.7f, -2.2f * Mathf.Cos(Mathf.Deg2Rad * pitch));
        }

        //====================================================================

    }
}