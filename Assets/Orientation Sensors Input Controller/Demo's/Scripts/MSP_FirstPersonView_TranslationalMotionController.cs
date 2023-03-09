using UnityEngine;
using System;
using MSP_Input;

namespace MSP_Input.Demo
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class MSP_FirstPersonView_TranslationalMotionController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float forwardSpeed = 4.0f;   // Speed when walking forward
            public float strafeSpeed = 3f;    // Speed when walking sideways
            public float backwardSpeed = 2.0f;  // Speed when walking backwards
            public float runMultiplier = 2.0f;   // Speed when sprinting
            public float maxVelocityChange = 1.0f; // Maximum velocity change during one frame
            public float jumpHeight = 2.0f; // The (maximum) jumpheight.
        }
        public MovementSettings movementSettings = new MovementSettings();

        [Serializable]
        public class InputSettings
        {
            public string movementInputName;
            public string runInputName;
            public string jumpInputName;
        }
        public InputSettings inputSettings = new InputSettings();

        [Serializable]
        public class AdvancedSettings
        {
            public float gravity = -9.8f;
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded
            public float slopeLimitMin = 0f;
            public float slopeLimitMax = 60f;
            public bool allowAirControl = false; // can the user control the direction that is being moved in the air
        }
        public AdvancedSettings advancedSettings = new AdvancedSettings();

        //

        private Rigidbody player_rigidbody;
        private CapsuleCollider player_capsule;

        private AnimationCurve maxSpeedCurve;
        private AnimationCurve slopeCurveModifier;

        private Vector3 groundContactNormal = Vector3.up;
        private bool isGrounded = false;
        private bool isJumping = false;
        private bool isRunning = false;

        private int movementInputID;
        private int runInputID;
        private int jumpInputID;

        private Vector2 joystickAxis = Vector2.zero;
        private float joystickAngle = 0f;
        private float joystickMagnitude = 0f;

        //====================================================================

        void Start()
        {
            player_rigidbody = gameObject.GetComponent<Rigidbody>();
            player_rigidbody.constraints = RigidbodyConstraints.None;
            player_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            player_rigidbody.useGravity = false;

            player_capsule = gameObject.GetComponent<CapsuleCollider>();

            maxSpeedCurve = new AnimationCurve(new Keyframe(-180.0f, movementSettings.backwardSpeed),
                                               new Keyframe(-90.0f, movementSettings.strafeSpeed),
                                               new Keyframe(0.0f, movementSettings.forwardSpeed),
                                               new Keyframe(90.0f, movementSettings.strafeSpeed),
                                               new Keyframe(180.0f, movementSettings.backwardSpeed));

            advancedSettings.slopeLimitMin = Mathf.Abs(advancedSettings.slopeLimitMin);
            advancedSettings.slopeLimitMax = Mathf.Abs(advancedSettings.slopeLimitMax);
            slopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f),
                                                    new Keyframe(advancedSettings.slopeLimitMin, 1.0f),
                                                    new Keyframe(advancedSettings.slopeLimitMax, 0.0f));

            advancedSettings.gravity = -Mathf.Abs(advancedSettings.gravity);

            // Cache all input id's (joystick/touchpad/button's)
            movementInputID = TouchInterface.GetID(inputSettings.movementInputName);
            if (movementInputID == -1)
            {
                Debug.Log("Warning [FirstPersonView_TranslationalMotionController] : " +
                           "Please specify the (correct) name of the movement \"Movement Input Name\".");
            }
            runInputID = TouchInterface.GetID(inputSettings.runInputName);
            if (runInputID == -1)
            {
                Debug.Log("Warning [FirstPersonView_TranslationalMotionController] : " +
                           "Please specify the (correct) name of the \"Run Input Name\".");
            }
            jumpInputID = TouchInterface.GetID(inputSettings.jumpInputName);
            if (jumpInputID == -1)
            {
                Debug.Log("Warning [FirstPersonView_TranslationalMotionController] : " +
                           "Please specify the (correct) name of the jump \"Jump Input Name\".");
            }
        }

        //====================================================================

        void Update()
        {
            // get the joysticks axis, angle and magnitude; these will be later used in FixedUpdate()
            joystickAxis = TouchInterface.GetAxis(movementInputID);
            TouchInterface.GetAngleAndMagnitude(inputSettings.movementInputName, out joystickAngle, out joystickMagnitude);

#if UNITY_EDITOR
            isRunning = (TouchInterface.GetButton(runInputID) || TouchInterface.GetDoubleTapHold(runInputID));
            isJumping = (TouchInterface.GetButtonDown(jumpInputID) || TouchInterface.GetButtonDown(jumpInputID));
#else
        isRunning = TouchInterface.GetDoubleTapHold(runInputID);
        isJumping = TouchInterface.GetButtonDown(jumpInputID);
#endif
        }

        //====================================================================

        void FixedUpdate()
        {
            Vector3 velocity = player_rigidbody.velocity;

            // is grounded?
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, player_capsule.radius, Vector3.down, out hitInfo,
                                   ((player_capsule.height * 0.5f) - player_capsule.radius) + advancedSettings.groundCheckDistance))
            {
                isGrounded = true;
                groundContactNormal = hitInfo.normal;
            }
            else
            {
                isGrounded = false;
                groundContactNormal = Vector3.up;
            }

            // walking/running
            if (isGrounded || advancedSettings.allowAirControl)
            {
                // Calculate how fast we should be moving
                float speed = joystickMagnitude;
                speed *= maxSpeedCurve.Evaluate(joystickAngle);
                if (isRunning)
                {
                    speed *= movementSettings.runMultiplier;
                }

                // Calculate the requested target velocity vector
                Vector3 targetVelocity = new Vector3(joystickAxis.x, 0, joystickAxis.y);
                targetVelocity = transform.TransformDirection(targetVelocity);
                targetVelocity *= speed;

                // Compensate target velocity vector for the angle of slope
                if (targetVelocity.sqrMagnitude > 0f)
                {
                    Vector3 v = Vector3.ProjectOnPlane(targetVelocity, Vector3.up);
                    Vector3 vp = Vector3.ProjectOnPlane(v, groundContactNormal);
                    float slopeAngle = Vector3.Angle(v, vp) * Mathf.Sign(vp.y - v.y);
                    float slopeModifier = slopeCurveModifier.Evaluate(slopeAngle);
                    speed *= slopeModifier;
                    targetVelocity *= slopeModifier;
                }

                // Apply a force that attempts to reach our target velocity
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -movementSettings.maxVelocityChange, movementSettings.maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -movementSettings.maxVelocityChange, movementSettings.maxVelocityChange);
                velocityChange.y = 0f;
                player_rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            }

            // Jump
            if (isGrounded && isJumping)
            {
                float jumpVerticalSpeed = Mathf.Sqrt(2f * movementSettings.jumpHeight * -advancedSettings.gravity);
                player_rigidbody.velocity = new Vector3(velocity.x, jumpVerticalSpeed, velocity.z);
            }

            // Apply gravity
            player_rigidbody.AddForce(new Vector3(0f, advancedSettings.gravity * player_rigidbody.mass, 0f));

            isGrounded = false;
        }

        //====================================================================

    }
}