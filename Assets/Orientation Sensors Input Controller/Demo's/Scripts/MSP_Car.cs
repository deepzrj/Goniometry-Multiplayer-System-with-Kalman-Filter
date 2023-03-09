using UnityEngine;

namespace MSP_Input.Demo
{
    [RequireComponent(typeof(Rigidbody))]
    public class MSP_Car : MonoBehaviour
    {
        [Header("Wheels")]

        [SerializeField]
        private WheelData[] wheels = new WheelData[0];
        [System.Serializable]
        public struct WheelData
        {
            public Transform wheelModel;
            public WheelCollider wheelCollider;
            [Range(-45f, 45f)]
            public float maxSteeringAngle;
            [Range(0f, 1f)]
            public float tractionStrength;
            [Range(0f, 1f)]
            public float brakeStrength;
            [Range(0f, 1f)]
            public float handbrakeStrength;
            [HideInInspector]
            public bool isGrounded;
            [HideInInspector]
            public float speed;
            [HideInInspector]
            public float rotationAngle;
            [HideInInspector]
            public Quaternion defaultRotation;
        }

        [Header("Center of Mass / Stability")]

        [SerializeField]
        private Transform COM = null;

        [Header("Engine")]
        [SerializeField]
        private AudioClip engineAudioClip = null;
        private AudioSource engineAudioSource = null;
        [SerializeField]
        private float minimumEngineRPM = 500f;
        [SerializeField]
        private float maximumEngineRPM = 7000f;
        [Space]
        [SerializeField]
        private float engineTorque = 3000f;
        [SerializeField]
        private float brakeTorque = 5000f;
        [SerializeField]
        [Range(2f, 5f)]
        private float handBrakeMultiplier = 4f;
        [Space]
        [SerializeField]
        private float maximumSpeedForward = 25f;
        [SerializeField]
        private float maximumSpeedBackward = 5f;

        [Header("Downforce / Self Righting")]

        [Range(0f, 2f)]
        public float downforceStrength = 0f;
        [SerializeField]
        private float selfRightingSpeedThreshold = 0.5f;
        [SerializeField]
        private float selfRightingWaitTime = 3f;
        private float selfRightingLastOkTime = 0f;

        [Header("Debug")]

        [SerializeField]
        private Vector3 velocity = Vector3.zero;
        [SerializeField]
        private float speed = 0f;
        [SerializeField]
        private float engineRPM = 0f;
        [SerializeField]
        [Range(-1f, 1f)]
        private float inputSteering;
        [SerializeField]
        [Range(0f, 1f)]
        private float inputGas;
        [SerializeField]
        [Range(0f, 1f)]
        private float inputBrake;
        [SerializeField]
        [Range(0f, 1f)]
        private float inputHandbrake;

        //-------------

        private Rigidbody rb;
        private float wheelsSpeedAvg = 0f;
        private float brakeStrengthTotal = 0f;
        private float tractionStrengthTotal = 0f;
        private float handbrakeStrengthTotal = 0f;
        private bool isGroundedTotal = false;
        private bool reverseGear = false;
        private float reverseGearTimer = 0f;
        private RaycastHit raycastHit;
        private WheelHit wheelHit;

        //====================================================================

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = COM.localPosition;
            rb.maxAngularVelocity = 2f * Mathf.PI;

            for (int i = 0; i < wheels.Length; i++)
            {
                //wheels[i].wheelCollider.mass = rb.mass * 0.025f;
                wheels[i].defaultRotation = wheels[i].wheelModel.localRotation;
            }

            engineAudioSource = gameObject.AddComponent<AudioSource>();
            engineAudioSource.clip = engineAudioClip;
            engineAudioSource.loop = true;
            engineAudioSource.spatialBlend = 1f;
            engineAudioSource.volume = 1f;
            engineAudioSource.pitch = 1f;
            engineAudioSource.Play();

            selfRightingLastOkTime = Time.time;
        }

        //====================================================================

        //private void Update()
        //{
        //    inputSteering = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);
        //    inputGas = Mathf.Clamp01(Input.GetAxis("Vertical"));
        //    inputBrake = Mathf.Abs(Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 0f));
        //    inputHandbrake = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        //}

        //====================================================================

        void FixedUpdate()
        {
            inputSteering = Mathf.Clamp(inputSteering, -1f, 1f);
            inputGas = Mathf.Clamp(inputGas, 0f, 1f);
            inputBrake = Mathf.Clamp(inputBrake, 0f, 1f);
            inputHandbrake = Mathf.Clamp(inputHandbrake, 0f, 1f);

            // Velocity / Speed / WheelSpeed(Average) / total traction & brake & handbrake strengths / isGroundedTotal
            velocity = rb.velocity;
            speed = velocity.magnitude;

            wheelsSpeedAvg = 0f;
            brakeStrengthTotal = 0f;
            tractionStrengthTotal = 0f;
            handbrakeStrengthTotal = 0f;
            isGroundedTotal = false;
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].isGrounded = wheels[i].wheelCollider.GetGroundHit(out WheelHit hit);
                if (wheels[i].isGrounded)
                {
                    isGroundedTotal = true;
                }
                wheels[i].speed = wheels[i].wheelCollider.rpm;
                wheels[i].speed *= wheels[i].wheelCollider.radius * 0.1047197551f;
                wheels[i].speed *= Mathf.Lerp(1f, 0f, hit.forwardSlip);
                wheels[i].speed *= rb.transform.lossyScale.z;

                wheelsSpeedAvg += wheels[i].speed * wheels[i].tractionStrength;

                tractionStrengthTotal += wheels[i].tractionStrength;
                brakeStrengthTotal += wheels[i].brakeStrength;
                handbrakeStrengthTotal += wheels[i].handbrakeStrength;
            }
            if (tractionStrengthTotal > 0f)
            {
                wheelsSpeedAvg /= tractionStrengthTotal;
            }

            // Drive Direction
            if (!reverseGear)
            {
                if (speed <= 0.2f && inputBrake >= 0.5f)
                {
                    reverseGearTimer += Time.fixedDeltaTime;
                }
                else
                {
                    reverseGearTimer = 0f;
                }
                if (reverseGearTimer >= 0.5f)
                {
                    reverseGearTimer = 0f;
                    reverseGear = true;
                }
            }
            if (reverseGear)
            {
                if (speed <= 0.2f && inputBrake <= 0.2f)
                {
                    reverseGear = false;
                }
            }
            float maximumSpeed = (!reverseGear) ? maximumSpeedForward : maximumSpeedBackward;

            // EngineRPM & Engine Sound
            engineRPM = Mathf.Lerp(minimumEngineRPM, maximumEngineRPM, Mathf.Abs(wheelsSpeedAvg / maximumSpeed));
            engineAudioSource.pitch = Mathf.Lerp(0.8f, 1.6f, engineRPM / maximumEngineRPM);

            // Steering & Traction
            for (int i = 0; i < wheels.Length; i++)
            {
                // Steering
                wheels[i].wheelCollider.steerAngle = (wheels[i].maxSteeringAngle * inputSteering) * Mathf.Lerp(1f, .25f, speed * 0.04f);

                // Traction
                wheels[i].wheelCollider.motorTorque = 0f;
                if (wheels[i].tractionStrength > 0f)
                {
                    if (speed <= maximumSpeed && wheels[i].speed <= maximumSpeed)
                    {
                        if (!reverseGear)
                        {
                            wheels[i].wheelCollider.motorTorque = engineTorque * Mathf.Clamp01(inputGas) / tractionStrengthTotal;

                        }
                        else
                        {
                            wheels[i].wheelCollider.motorTorque = -engineTorque * Mathf.Clamp01(inputBrake) / tractionStrengthTotal;
                        }
                    }
                }

                // Braking
                wheels[i].wheelCollider.brakeTorque = 0f;
                if (wheels[i].brakeStrength > 0f)
                {
                    if (!reverseGear)
                    {
                        wheels[i].wheelCollider.brakeTorque += Mathf.Clamp01(inputBrake) * brakeTorque * wheels[i].brakeStrength / brakeStrengthTotal;
                    }
                    else
                    {
                        wheels[i].wheelCollider.brakeTorque += Mathf.Clamp01(inputGas) * brakeTorque * wheels[i].brakeStrength / brakeStrengthTotal;
                    }
                }
                if (wheels[i].handbrakeStrength > 0f)
                {
                    wheels[i].wheelCollider.brakeTorque += inputHandbrake * handBrakeMultiplier * brakeTorque * wheels[i].handbrakeStrength / handbrakeStrengthTotal;
                }

                // Align wheelmodels with wheelcolliders
                wheels[i].wheelCollider.GetGroundHit(out wheelHit);
                Vector3 wheelColliderCenter = wheels[i].wheelCollider.transform.TransformPoint(wheels[i].wheelCollider.center);
                float maxRaycastDistance = (wheels[i].wheelCollider.suspensionDistance + wheels[i].wheelCollider.radius) * transform.localScale.y;
                if (Physics.Raycast(wheelColliderCenter, -wheels[i].wheelCollider.transform.up, out raycastHit, maxRaycastDistance))
                {
                    if (!raycastHit.transform.IsChildOf(rb.transform) && !raycastHit.collider.isTrigger)
                    {
                        wheels[i].wheelModel.position = raycastHit.point;
                        wheels[i].wheelModel.position += (wheels[i].wheelCollider.transform.up * wheels[i].wheelCollider.radius) * transform.localScale.y;
                    }
                }
                else
                {
                    wheels[i].wheelModel.position = Vector3.Lerp(wheels[i].wheelModel.position, wheelColliderCenter - (wheels[i].wheelCollider.transform.up * wheels[i].wheelCollider.suspensionDistance) * transform.localScale.y, Time.fixedDeltaTime * 10f);
                }
                wheels[i].rotationAngle += wheels[i].wheelCollider.rpm * 6f * Time.fixedDeltaTime;
                while (wheels[i].rotationAngle > 360f)
                {
                    wheels[i].rotationAngle -= 360f;
                }
                while (wheels[i].rotationAngle < 0f)
                {
                    wheels[i].rotationAngle += 360f;
                }
                wheels[i].wheelModel.rotation = wheels[i].wheelCollider.transform.rotation;
                wheels[i].wheelModel.rotation *= Quaternion.Euler(wheels[i].rotationAngle, wheels[i].wheelCollider.steerAngle, wheels[i].wheelCollider.transform.rotation.z);
                wheels[i].wheelModel.rotation *= wheels[i].defaultRotation;

                // stabilize car by adding downforce
                if (!wheels[i].isGrounded && isGroundedTotal)
                {
                    rb.AddForce(-downforceStrength * wheels[i].wheelCollider.transform.up * rb.mass * 9.81f / wheels.Length, ForceMode.Force);
                }

                // Self Righting
                if (rb.transform.up.y > 0f || speed > selfRightingSpeedThreshold)
                {
                    selfRightingWaitTime = Time.time;
                }
                if (Time.time > selfRightingLastOkTime + selfRightingWaitTime)
                {
                    transform.position += Vector3.up;
                    transform.rotation = Quaternion.LookRotation(transform.forward);
                }

            }
        }

        //====================================================================

        public void Input(float steer, float gas, float brake, float handbrake)
        {
            inputSteering = Mathf.Clamp(steer, -1f, 1f);
            inputGas = Mathf.Clamp01(gas);
            inputBrake = Mathf.Clamp01(brake);
            inputHandbrake = handbrake;
        }

        //====================================================================

    }
}