using UnityEngine;
using System.Collections.Generic;

namespace MSP_Input
{
    [DisallowMultipleComponent]
    [AddComponentMenu("MSP_Input/Orientation Sensors")]
    public partial class OrientationSensors : MonoBehaviour
    {
        public static OrientationSensors Singleton;

        //---------------------------------------------------------

        public BaseOrientation baseOrientation = BaseOrientation.LandscapeLeft;
        public enum BaseOrientation
        {
            Portrait,
            LandscapeLeft,
            LandscapeRight,
            Portrait_FaceUp,
            LandscapeLeft_FaceUp,
            LandscapeRight_FaceUp
        }

        [Space(4)]

        [SerializeField]
        [Range(0f, 2f)]
        private float smoothingTime = 0.1f;
        private float smoothFactor
        {
            get
            {
                if (smoothingTime > Time.deltaTime)
                {
                    return Time.deltaTime / smoothingTime;
                }
                else
                {
                    return 1f;
                }
            }
        }

        [Space(4)]

        [SerializeField]
        [Range(-180f, 180f)]
        private float headingOffset = 0f;

        [SerializeField]
        private FloatMinMax pitchOffset = new FloatMinMax(0f, -70f, 70f, -90f, 90f);

        [Space(4)]
        [SerializeField]
        private HeadingPitchRoll gameWorldHPR_Unclamped = HeadingPitchRoll.Zero();
        [SerializeField]
        private HeadingPitchRoll gameWorldHPR_Clamped = HeadingPitchRoll.Zero();
        private Quaternion gameWorldRotation = Quaternion.identity;

        [SerializeField]
        private HeadingPitchRoll deviceHPR = HeadingPitchRoll.Zero();

        //---------------------------------------------------------     

        [SerializeField]
        private bool gyroscopeEnabled = true;

        [SerializeField]
        [Range(10f, 100f)]
        private int gyroscopeUpdateFrequency = 50;

        [SerializeField]
        [Range(0.1f, 4f)]
        private float gyroHeadingAmplifier = 1f;

        [SerializeField]
        [Range(0.1f, 4f)]
        private float gyroPitchAmplifier = 1f;

        [SerializeField]
        private bool gyroDriftCompensation = false;
        private HeadingPitchRoll deviceHPRold = HeadingPitchRoll.Zero();
        private List<HeadingPitchRoll> deviceDeltaHPRlog = new List<HeadingPitchRoll>(0);
        [HideInInspector]
        [SerializeField]
        private HeadingPitchRoll gyroDriftHPR = HeadingPitchRoll.Zero();

        //---------------------------------------------------------

        [SerializeField]
        private bool accelerometerEnabled = false;

#if UNITY_EDITOR
        [SerializeField]
        [Range(10f, 100f)]
        private int accelerometerUpdateFrequency = 60;
#endif

        //---------------------------------------------------------

        [SerializeField]
        private bool magnetometerEnabled = false;

        [SerializeField]
        [Range(-90f, 90f)]
        private float magneticDeclination = 0f;

        [SerializeField]
        private bool setGameNorthToCompassNorth = false;

        [SerializeField]
        private float compassHeading = 0f;

        private double lastCompassUpdateTime = 0d;

        //---------------------------------------------------------

#if UNITY_EDITOR
        [SerializeField]
        private EditorSimulationMode editorSimulation = EditorSimulationMode.None;
        private enum EditorSimulationMode { None, Mouse, CursorKeys };

        [SerializeField]
        private Vector2 simulationSensitivity = new Vector2(20f, 10f);
#endif

        //---------------------------------------------------------

        private const float sqrthalf = 0.707106781186548f;

        //---------------------------------------------------------

    }
}