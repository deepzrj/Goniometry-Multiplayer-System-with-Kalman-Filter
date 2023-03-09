using UnityEngine;
using MSP_Input;

namespace MSP_Input.Demo
{
    [RequireComponent(typeof(MSP_Car))]
    [RequireComponent(typeof(Rigidbody))]
    public class MSP_CarPlayerController : MonoBehaviour
    {
        [SerializeField]
        private Camera playerCamera;

        private Rigidbody rb;

        [SerializeField]
        private string joystickName = "Joystick";
        [SerializeField]
        private string touchpadName = "Touchpad";
        [SerializeField]
        private string handbrakeName = "Handbrake";

        private int joystickID;
        private int touchpadID;
        private int handbrakeID;

        private MSP_Car carScript;

        private AnimationCurve angleSteerCurve = new AnimationCurve(new Keyframe(-180f, 0f, 0f, 0f),
                                                                      new Keyframe(-160f, -1f, 0f, 0f),
                                                                        new Keyframe(-20f, -1f, 0f, 0f),
                                                                        new Keyframe(0f, 0f, 0f, 0f),
                                                                        new Keyframe(20f, 1f, 0f, 0f),
                                                                        new Keyframe(160f, 1f, 0f, 0f),
                                                                        new Keyframe(180f, 0f, 0f, 0f));

        private AnimationCurve angleGasCurve = new AnimationCurve(new Keyframe(-180f, 0f, 0f, 0f),
                                                                  new Keyframe(-100f, 0f, 0f, 0f),
                                                                    new Keyframe(-80f, 1f, 0f, 0f),
                                                                    new Keyframe(0f, 1f, 0f, 0f),
                                                                    new Keyframe(80f, 1f, 0f, 0f),
                                                                    new Keyframe(100f, 0f, 0f, 0f),
                                                                    new Keyframe(180f, 0f, 0f, 0f));

        private AnimationCurve anglebrakeCurve = new AnimationCurve(new Keyframe(-180f, 1f, 0f, 0f),
                                                              new Keyframe(-100f, 1f, 0f, 0f),
                                                                new Keyframe(-80f, 0f, 0f, 0f),
                                                                new Keyframe(0f, 0f, 0f, 0f),
                                                                new Keyframe(80f, 0f, 0f, 0f),
                                                                new Keyframe(100f, 1f, 0f, 0f),
                                                                new Keyframe(180f, 1f, 0f, 0f));

        //=====================================================================

        private void Start()
        {
            // Get the ID's, associated with the joystick, touchpad and handbrake-button
            joystickID = TouchInterface.GetID(joystickName);
            touchpadID = TouchInterface.GetID(touchpadName);
            handbrakeID = TouchInterface.GetID(handbrakeName);

            // The MSP_Car script handles the actual handling of the car. So better cache it's component
            carScript = GetComponent<MSP_Car>();

            // A camera is required
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }

            // cache the rigidbody's component
            rb = GetComponent<Rigidbody>();
        }

        //=====================================================================

        private void Update()
        {
            // Get the angle and magnitude of the joystick
            TouchInterface.GetAngleAndMagnitude(joystickID, out float alpha, out float strength);
            // and use this to calculate the angle between the playerCamera's looking direction and the rigidbody's velocity (move direction of the car)
            alpha -= Vector3.SignedAngle(Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up), Vector3.ProjectOnPlane(rb.transform.forward, Vector3.up), Vector3.up);

            // nicely put this angle between -180 and 180 degrees
            if (alpha > 180f)
            {
                alpha -= 360f;
            }
            if (alpha <= -180f)
            {
                alpha += 360f;
            }

            // Convert angle and input strength to steer, gas and brake, by applying the associated curve
            float steer = angleSteerCurve.Evaluate(alpha) * strength;
            float gas = angleGasCurve.Evaluate(alpha) * strength;
            float brake = anglebrakeCurve.Evaluate(alpha) * strength;
            // Is the handbrake button being pressed? 
            float handbrake = (TouchInterface.GetButton(handbrakeID)) ? 1f : 0f;

            // feed everything to the MSP_Car script we cached before
            carScript.Input(steer, gas, brake, handbrake);
        }

        //=====================================================================

        private void LateUpdate()
        {
            // First: place the camera's position straight above the car
            playerCamera.transform.position = rb.transform.position + Vector3.up * 3f;
            // Give it the full rotation obtained from the OrientationSensors
            playerCamera.transform.rotation = OrientationSensors.GetRotation();
            // Move the camera 10 meters backwords, along it's own z-axis
            playerCamera.transform.Translate(0f, 0f, -10f, Space.Self);
        }

        //=====================================================================

    }
}