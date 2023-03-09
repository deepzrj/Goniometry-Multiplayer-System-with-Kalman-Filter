using UnityEngine;
using UnityEditor;
using MSP_Input;

namespace MSP_Mobile
{
    [CustomEditor(typeof(MSP_Input.OrientationSensors))]
    public class OrientationEditor : Editor
    {
        const float buttonWidth = 40f;
        //  http://www.unicode.org/charts/
        const char upArrow = '\u25B2';
        const char downArrow = '\u25BC';

        private SerializedProperty accelerometerEnabled;
        private SerializedProperty gyroscopeEnabled;
        private bool _gyroscopeEnabled;
        private SerializedProperty gyroDriftCompensation;

        //====================================================================

        public override void OnInspectorGUI()
        {
            GUILayout.Space(4);

            //EditorGUILayout.LabelField("=====  DEFAULT INSPECTOR (START) ====", EditorStyles.boldLabel);
            //DrawDefaultInspector();
            //EditorGUILayout.LabelField("======  DEFAULT INSPECTOR (END) =====", EditorStyles.boldLabel);
            //EditorGUILayout.Space();

            if (Application.isPlaying)
            {
                GUILayout.Space(4);
                ShowRuntimeValues();
                GUILayout.Space(4);
                //return;
            }

	        if (!Application.isPlaying)
	        {
		        serializedObject.Update();

		        accelerometerEnabled = serializedObject.FindProperty("accelerometerEnabled");
		        gyroscopeEnabled = serializedObject.FindProperty("gyroscopeEnabled");
		        _gyroscopeEnabled = gyroscopeEnabled.boolValue;
		        gyroDriftCompensation = serializedObject.FindProperty("gyroDriftCompensation");

		        GUILayout.Space(4);
		        ShowGeneralSettings();
		        GUILayout.Space(2);
		        ShowAccelerometerSettings();
		        GUILayout.Space(2);
		        ShowGyroscopeSettings();
		        GUILayout.Space(2);
		        ShowMagnetometerSettings();
		        GUILayout.Space(2);
		        ShowTransformUpdaterSettings();

		        serializedObject.ApplyModifiedProperties();
	        }

        }

        //====================================================================

        void ShowGeneralSettings()
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

            SerializedProperty baseOrientation = serializedObject.FindProperty("baseOrientation");
            EditorGUILayout.PropertyField(baseOrientation);

            EditorGUILayout.Space();

            SerializedProperty smoothingTime = serializedObject.FindProperty("smoothingTime");
            EditorGUILayout.PropertyField(smoothingTime);

            EditorGUILayout.Space();

            SerializedProperty headingOffset = serializedObject.FindProperty("headingOffset");
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Heading Offset");
                EditorGUILayout.LabelField("", GUILayout.Width(43));
                headingOffset.floatValue = EditorGUILayout.Slider(GUIContent.none, headingOffset.floatValue, -180f, 180f, GUILayout.MinWidth(100));
            }
            EditorGUILayout.EndHorizontal();

            OrientationSensors myScript = (OrientationSensors)target;
            if (!myScript.BaseOrientationIsFaceUp())
            {
                SerializedProperty pitchOffset = serializedObject.FindProperty("pitchOffset");
                EditorGUILayout.PropertyField(pitchOffset);
            }

            EditorGUILayout.Space();

            SerializedProperty editorSimulation = serializedObject.FindProperty("editorSimulation");
            EditorGUILayout.PropertyField(editorSimulation, new GUIContent("Editor Simulation Mode"));

            if (editorSimulation.enumValueIndex != 0)
            {
                SerializedProperty simulationSensitivity = serializedObject.FindProperty("simulationSensitivity");
                EditorGUILayout.PropertyField(simulationSensitivity, new GUIContent("  Sensitivity"));
            }

            EditorGUILayout.EndVertical();
        }

        //====================================================================

        void ShowAccelerometerSettings()
        {
            EditorGUILayout.BeginVertical("Box");

	        _gyroscopeEnabled = !EditorGUILayout.ToggleLeft("Accelerometer", !_gyroscopeEnabled, EditorStyles.boldLabel);
	        if (Application.isPlaying && _gyroscopeEnabled != gyroscopeEnabled.boolValue)
	        {
	        	OrientationSensors.GyroscopeEnabled(_gyroscopeEnabled);
	        }
            gyroscopeEnabled.boolValue = _gyroscopeEnabled;
            accelerometerEnabled.boolValue = !gyroscopeEnabled.boolValue;

            GUI.enabled = accelerometerEnabled.boolValue;

            SerializedProperty accelerometerUpdateFrequency = serializedObject.FindProperty("accelerometerUpdateFrequency");
            EditorGUILayout.PropertyField(accelerometerUpdateFrequency, new GUIContent("  Sensor Update Frequency"));

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        //====================================================================

        void ShowGyroscopeSettings()
        {
            EditorGUILayout.BeginVertical("Box");

	        _gyroscopeEnabled = EditorGUILayout.ToggleLeft("Gyroscope", _gyroscopeEnabled, EditorStyles.boldLabel);
	        if (Application.isPlaying && _gyroscopeEnabled != gyroscopeEnabled.boolValue)
	        {
	        	OrientationSensors.GyroscopeEnabled(_gyroscopeEnabled);
	        }
            gyroscopeEnabled.boolValue = _gyroscopeEnabled;
            accelerometerEnabled.boolValue = !gyroscopeEnabled.boolValue;

            GUI.enabled = gyroscopeEnabled.boolValue;

            SerializedProperty gyroscopeUpdateFrequency = serializedObject.FindProperty("gyroscopeUpdateFrequency");
            EditorGUILayout.PropertyField(gyroscopeUpdateFrequency, new GUIContent("  Sensor Update Frequency"));

            EditorGUILayout.Space();

            SerializedProperty gyroHeadingAmplifier = serializedObject.FindProperty("gyroHeadingAmplifier");
            EditorGUILayout.PropertyField(gyroHeadingAmplifier, new GUIContent("  Heading Amplifier"));

            SerializedProperty gyroPitchAmplifier = serializedObject.FindProperty("gyroPitchAmplifier");
            EditorGUILayout.PropertyField(gyroPitchAmplifier, new GUIContent("  Pitch Amplifier"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(gyroDriftCompensation, new GUIContent("  Drift Compensation"));

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        //====================================================================

        void ShowMagnetometerSettings()
        {
            EditorGUILayout.BeginVertical("Box");

            SerializedProperty magnetometerEnabled = serializedObject.FindProperty("magnetometerEnabled");

            if (gyroDriftCompensation.boolValue && gyroscopeEnabled.boolValue)
            {
                EditorGUILayout.ToggleLeft("Magnetometer", true, EditorStyles.boldLabel);
                magnetometerEnabled.boolValue = true;
            }
            else
            {
                bool _magnetometerEnabled = magnetometerEnabled.boolValue;
                _magnetometerEnabled = EditorGUILayout.ToggleLeft("Magnetometer", _magnetometerEnabled, EditorStyles.boldLabel);
                magnetometerEnabled.boolValue = _magnetometerEnabled;
            }

            GUI.enabled = magnetometerEnabled.boolValue;

            SerializedProperty magneticDeclination = serializedObject.FindProperty("magneticDeclination");
            EditorGUILayout.PropertyField(magneticDeclination, new GUIContent("  Magnetic Declination"));

            SerializedProperty setGameNorthToCompassNorth = serializedObject.FindProperty("setGameNorthToCompassNorth");
            EditorGUILayout.PropertyField(setGameNorthToCompassNorth, new GUIContent("  Game North = Compass North"));

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        //====================================================================

        void ShowTransformUpdaterSettings()
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.LabelField("Transform Updater", EditorStyles.boldLabel);

            SerializedProperty transformUpdateList = serializedObject.FindProperty("transformUpdateList");
            int listSize = transformUpdateList.arraySize;
            OrientationSensors myScript = (OrientationSensors)target;
            SerializedProperty inspTransformIndex = serializedObject.FindProperty("inspTransformIndex");

            //---------------------

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Add", GUILayout.Width(buttonWidth)))
            {
                myScript.AddTransform();
            }

            if (GUILayout.Button("Ins", GUILayout.Width(buttonWidth)))
            {
                myScript.AddTransform(inspTransformIndex.intValue);
            }

            if (GUILayout.Button("Del", GUILayout.Width(buttonWidth)))
            {
                myScript.DeleteTransform(inspTransformIndex.intValue);
            }

            EditorGUILayout.LabelField("", GUILayout.MinWidth(1f));

            if (GUILayout.Button(downArrow.ToString(), GUILayout.Width(buttonWidth)))
            {
                myScript.MoveUpTransform(inspTransformIndex.intValue);
            }

            if (GUILayout.Button(upArrow.ToString(), GUILayout.Width(buttonWidth)))
            {
                myScript.MoveDownTransform(inspTransformIndex.intValue);
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();

            //---------------------

            int index = inspTransformIndex.intValue;
            if (index >= 0)
            {
                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical("Box");
                string[] transformNames = myScript.GetTransformNames();
                index = GUILayout.SelectionGrid(index, transformNames, 1);
                EditorGUILayout.EndVertical();

                //---------------------

                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.BeginHorizontal();
                SerializedProperty targetTransform = transformUpdateList.GetArrayElementAtIndex(index).FindPropertyRelative("targetTransform");
                EditorGUILayout.PropertyField(targetTransform);
                if (GUILayout.Button("parent", GUILayout.Width(56f)))
                {
                    myScript.SetParentAsTargetTransform(index);
                }
                EditorGUILayout.EndHorizontal();

                SerializedProperty enabled = transformUpdateList.GetArrayElementAtIndex(index).FindPropertyRelative("enabled");
                EditorGUILayout.PropertyField(enabled);

                GUILayout.Space(4);

                SerializedProperty smoothingTimeExtra = transformUpdateList.GetArrayElementAtIndex(index).FindPropertyRelative("smoothingTime");
                EditorGUILayout.PropertyField(smoothingTimeExtra);

                GUILayout.Space(4);

                SerializedProperty copyHeading = transformUpdateList.GetArrayElementAtIndex(index).FindPropertyRelative("copyHeading");
                SerializedProperty heading = transformUpdateList.GetArrayElementAtIndex(index).FindPropertyRelative("heading");
                float headingValue = heading.FindPropertyRelative("value").floatValue;
                float headingMinValue = heading.FindPropertyRelative("minValue").floatValue;
                float headingMaxValue = heading.FindPropertyRelative("maxValue").floatValue;
                EditorGUILayout.BeginHorizontal();
                copyHeading.boolValue = EditorGUILayout.Toggle("Copy Heading", copyHeading.boolValue);
                if (copyHeading.boolValue)
                {
                    headingMinValue = EditorGUILayout.FloatField(headingMinValue, GUILayout.MaxWidth(50f));
                    EditorGUILayout.MinMaxSlider(ref headingMinValue, ref headingMaxValue, -180f, 180f);
                    headingMaxValue = EditorGUILayout.FloatField(headingMaxValue, GUILayout.MaxWidth(50f));
                }
                else
                {
                    EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(50f));
                    headingValue = EditorGUILayout.Slider(headingValue, -180f, 180f);
                }
                EditorGUILayout.EndHorizontal();
                heading.FindPropertyRelative("value").floatValue = Mathf.Round(headingValue);
                heading.FindPropertyRelative("minValue").floatValue = Mathf.Round(headingMinValue);
                heading.FindPropertyRelative("maxValue").floatValue = Mathf.Round(headingMaxValue);
                //EditorGUILayout.PropertyField(copyHeading);
                //EditorGUILayout.PropertyField(heading);

                SerializedProperty copyPitch = transformUpdateList.GetArrayElementAtIndex(index).FindPropertyRelative("copyPitch");
                SerializedProperty pitch = transformUpdateList.GetArrayElementAtIndex(index).FindPropertyRelative("pitch");
                float pitchValue = pitch.FindPropertyRelative("value").floatValue;
                float pitchMinValue = pitch.FindPropertyRelative("minValue").floatValue;
                float pitchMaxValue = pitch.FindPropertyRelative("maxValue").floatValue;
                EditorGUILayout.BeginHorizontal();
                copyPitch.boolValue = EditorGUILayout.Toggle("Copy Pitch", copyPitch.boolValue);
                if (copyPitch.boolValue)
                {
                    pitchMinValue = EditorGUILayout.FloatField(pitchMinValue, GUILayout.MaxWidth(50f));
                    EditorGUILayout.MinMaxSlider(ref pitchMinValue, ref pitchMaxValue, -90f, 90f);
                    pitchMaxValue = EditorGUILayout.FloatField(pitchMaxValue, GUILayout.MaxWidth(50f));
                }
                else
                {
                    EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(50f));
                    pitchValue = EditorGUILayout.Slider(pitchValue, -90f, 90f);
                }
                EditorGUILayout.EndHorizontal();
                pitch.FindPropertyRelative("value").floatValue = Mathf.Round(pitchValue);
                pitch.FindPropertyRelative("minValue").floatValue = Mathf.Round(pitchMinValue);
                pitch.FindPropertyRelative("maxValue").floatValue = Mathf.Round(pitchMaxValue);
                //EditorGUILayout.PropertyField(copyPitch);
                //EditorGUILayout.PropertyField(pitch);

                SerializedProperty copyRoll = transformUpdateList.GetArrayElementAtIndex(index).FindPropertyRelative("copyRoll");
                SerializedProperty roll = transformUpdateList.GetArrayElementAtIndex(index).FindPropertyRelative("roll");
                float rollValue = roll.FindPropertyRelative("value").floatValue;
                float rollMinValue = roll.FindPropertyRelative("minValue").floatValue;
                float rollMaxValue = roll.FindPropertyRelative("maxValue").floatValue;
                EditorGUILayout.BeginHorizontal();
                copyRoll.boolValue = EditorGUILayout.Toggle("Copy Roll", copyRoll.boolValue);
                if (copyRoll.boolValue)
                {
                    rollMinValue = EditorGUILayout.FloatField(rollMinValue, GUILayout.MaxWidth(50f));
                    EditorGUILayout.MinMaxSlider(ref rollMinValue, ref rollMaxValue, -180f, 180f);
                    rollMaxValue = EditorGUILayout.FloatField(rollMaxValue, GUILayout.MaxWidth(50f));
                }
                else
                {
                    EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(50f));
                    rollValue = EditorGUILayout.Slider(rollValue, -180f, 180f);
                }
                EditorGUILayout.EndHorizontal();
                roll.FindPropertyRelative("value").floatValue = Mathf.Round(rollValue);
                roll.FindPropertyRelative("minValue").floatValue = Mathf.Round(rollMinValue);
                roll.FindPropertyRelative("maxValue").floatValue = Mathf.Round(rollMaxValue);
                //EditorGUILayout.PropertyField(copyRoll);
                //EditorGUILayout.PropertyField(roll);


                SerializedProperty canPushEdge = transformUpdateList.GetArrayElementAtIndex(index).FindPropertyRelative("canPushEdge");
                float copyHeadingMinValue = heading.FindPropertyRelative("minValue").floatValue;
                float copyHeadingMaxValue = heading.FindPropertyRelative("maxValue").floatValue;
                float copyPitchMinValue = pitch.FindPropertyRelative("minValue").floatValue;
                float copyPitchMaxValue = pitch.FindPropertyRelative("maxValue").floatValue;
                if ((copyHeading.boolValue && (copyHeadingMinValue > -180f || copyHeadingMaxValue < 180f)) ||
                    (copyPitch.boolValue && (copyPitchMinValue > -90f || copyPitchMaxValue < 90f)))
                {
                    GUILayout.Space(4);
                    EditorGUILayout.PropertyField(canPushEdge);
                }
                //EditorGUILayout.PropertyField(canPushEdge);

                GUILayout.Space(4);

                EditorGUILayout.EndVertical();

            }

            inspTransformIndex.intValue = index;

            GUILayout.Space(4);

            EditorGUILayout.EndVertical();
        }



        //====================================================================

        void ShowRuntimeValues()
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("--== RUNTIME VALUES ==--", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            GUI.enabled = false;

            EditorGUILayout.LabelField("Device Orientation", EditorStyles.boldLabel);
            SerializedProperty gyroscopeEnabled = serializedObject.FindProperty("gyroscopeEnabled");
            SerializedProperty accelerometerEnabled = serializedObject.FindProperty("accelerometerEnabled");
            SerializedProperty magnetometerEnabled = serializedObject.FindProperty("magnetometerEnabled");

            SerializedProperty deviceHPR = serializedObject.FindProperty("deviceHPR");
            if (gyroscopeEnabled.boolValue)
            {
                EditorGUILayout.PropertyField(deviceHPR, new GUIContent("  Gyroscope"));
                SerializedProperty gyroDriftCompensation = serializedObject.FindProperty("gyroDriftCompensation");
                if (gyroDriftCompensation.boolValue)
                {
                    SerializedProperty gyroDriftHPR = serializedObject.FindProperty("gyroDriftHPR");
                    EditorGUILayout.PropertyField(gyroDriftHPR, new GUIContent("  Gyro Drift"));
                }
            }

            if (accelerometerEnabled.boolValue)
            {
                EditorGUILayout.PropertyField(deviceHPR, new GUIContent("  Accelerometer"));
            }

            if (magnetometerEnabled.boolValue)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("  Compass Heading");
                SerializedProperty compassHeading = serializedObject.FindProperty("compassHeading");
                EditorGUILayout.Slider(MSP_Input.OrientationSensors.ConvertAngle180To360(compassHeading.floatValue), 0f, 360f);

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(4);

            EditorGUILayout.LabelField("GameWorld Orientation", EditorStyles.boldLabel);

            SerializedProperty gameWorldHPR_Clamped = serializedObject.FindProperty("gameWorldHPR_Clamped");
            EditorGUILayout.PropertyField(gameWorldHPR_Clamped, new GUIContent("  Clamped"));

            SerializedProperty gameWorldHPR_Unclamped = serializedObject.FindProperty("gameWorldHPR_Unclamped");
            EditorGUILayout.PropertyField(gameWorldHPR_Unclamped, new GUIContent("  Unclamped"));

            GUI.enabled = true;

            GUILayout.Space(4);

            EditorGUILayout.EndVertical();
        }

        //====================================================================

    }
}