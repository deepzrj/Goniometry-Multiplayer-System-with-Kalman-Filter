using UnityEngine;
using MSP_Input;

namespace MSP_Input.Demo
{
    public class MSP_Sword : MonoBehaviour
    {
        private Vector3 originalPos;
        private int resetButtonID;

        //====================================================================

        private void Start()
        {
            originalPos = transform.position;

            // cache the button id 
            resetButtonID = TouchInterface.GetID("ResetButton");
        }

        //====================================================================

        void Update()
        {
            // Get the full rotation from the OrientationSensors script and directly apply it to this transform
            transform.rotation = OrientationSensors.GetRotation();

            // at reset: force the heading 0 degrees
            if (TouchInterface.GetButtonDown(resetButtonID))
            {
                OrientationSensors.SetHeading(0f);
            }
        }

        //====================================================================

    }
}