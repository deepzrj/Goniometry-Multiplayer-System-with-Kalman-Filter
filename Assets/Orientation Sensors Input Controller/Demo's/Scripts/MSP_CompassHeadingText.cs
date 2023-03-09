using UnityEngine;
using UnityEngine.UI;
using MSP_Input;

//====================================================================

public class MSP_CompassHeadingText : MonoBehaviour
{
    [SerializeField]
    private float updateTime = 0.25f;
    private Text text;
    private float lastUpdateTime = 0f;

    //====================================================================

    void Awake()
    {
        // Cach the text component
        text = gameObject.GetComponent<Text>();
    }

    //====================================================================

    void Update()
    {
        if (Time.time > lastUpdateTime + updateTime)
        {
            lastUpdateTime = Time.time;
            // get the magntetic heading, convert it to a string and feed it to the text component
            text.text = OrientationSensors.GetCompassHeading360().ToString("#.0");
        }
    }
}