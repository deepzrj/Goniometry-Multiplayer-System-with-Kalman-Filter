using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSP_SwitchInputMethod : MonoBehaviour
{
	[SerializeField]
	private KeyCode inputKey = KeyCode.Tab;
	
    void Update()
	{
		if (Input.GetKeyDown(inputKey))
	    {
	    	Debug.Log("==== Switching Orientation Input Method ====");
	    	bool gyroEnabled = MSP_Input.OrientationSensors.IsGyroscopeEnabled();
	    	MSP_Input.OrientationSensors.GyroscopeEnabled(!gyroEnabled);
	    }
    }
}
