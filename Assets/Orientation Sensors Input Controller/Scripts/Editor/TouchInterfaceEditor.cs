using UnityEngine;
using UnityEditor;
using MSP_Input;

[CustomEditor(typeof(TouchInterface))]
public class TouchInterfaceEditor : Editor
{
    const float buttonWidth = 40f;
    //  http://www.unicode.org/charts/
    const char upArrow = '\u25B2';
    const char downArrow = '\u25BC';
    const string touchpadStr = "Touchpad";
    const string joystickStr = "Joystick";
    const string buttonStr = "Button";
    const string textureStr = "Texture";

    SerializedProperty uiList;
    int index;

    //====================================================================

    public override void OnInspectorGUI()
    {
        GUILayout.Space(4);

        serializedObject.Update();
        uiList = serializedObject.FindProperty("uiList");

        //EditorGUILayout.LabelField("=====  DEFAULT INSPECTOR (START) ====", EditorStyles.boldLabel);
        //DrawDefaultInspector();
        //EditorGUILayout.LabelField("======  DEFAULT INSPECTOR (END) =====", EditorStyles.boldLabel);
        //EditorGUILayout.Space();

        if (Application.isPlaying)
        {
            ShowRuntimeValues();
        }
        else
        {
            ShowEditorMenu();
        }
    }

    //====================================================================

    private void ShowEditorMenu()
    {
        int listSize = uiList.arraySize;
        TouchInterface myScript = (TouchInterface)target;
        SerializedProperty inspIndex = serializedObject.FindProperty("inspIndex");

        //---------------------

        EditorGUILayout.BeginVertical("Box");
        GUILayout.Space(4);

        SerializedProperty showInEditMode = serializedObject.FindProperty("showInEditMode");
        EditorGUILayout.PropertyField(showInEditMode);

        SerializedProperty material = serializedObject.FindProperty("material");
        EditorGUILayout.PropertyField(material);

        GUILayout.Space(4);
        EditorGUILayout.EndVertical();

        //---------------------

        EditorGUILayout.BeginVertical("Box");
        GUILayout.Space(2);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add", GUILayout.Width(buttonWidth)))
        {
            myScript.Add();
        }

        if (GUILayout.Button("Ins", GUILayout.Width(buttonWidth)))
        {
            myScript.Add(inspIndex.intValue);
        }

        if (GUILayout.Button("Del", GUILayout.Width(buttonWidth)))
        {
            myScript.Delete(inspIndex.intValue);
        }

        EditorGUILayout.LabelField("", GUILayout.MinWidth(1f));

        if (GUILayout.Button(downArrow.ToString(), GUILayout.Width(buttonWidth)))
        {
            myScript.MoveUp(inspIndex.intValue);
        }

        if (GUILayout.Button(upArrow.ToString(), GUILayout.Width(buttonWidth)))
        {
            myScript.MoveDown(inspIndex.intValue);
        }

        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(4);
        EditorGUILayout.EndVertical();

        //---------------------

        serializedObject.Update();
        index = inspIndex.intValue;
        if (index >= 0)
        {
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Space(4);

            string[] transformNames = myScript.GetNamesList();
            index = GUILayout.SelectionGrid(index, transformNames, 1);

            GUILayout.Space(4);
            EditorGUILayout.EndVertical();
        }

        //---------------------

        if (index >= 0)
        {
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Space(4);

            SerializedProperty type = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("type");
            EditorGUILayout.PropertyField(type);

            switch (type.enumDisplayNames[type.enumValueIndex])
            {
                case touchpadStr:
                    TouchpadSettings();
                    break;
                case joystickStr:
                    JoystickSettings();
                    break;
                case buttonStr:
                    ButtonSettings();
                    break;
                case textureStr:
                    TextureSettings();
                    break;
                default:
                    break;
            }

            GUILayout.Space(4);
            EditorGUILayout.EndVertical();
        }

        //---------------------

        GUILayout.Space(4);

        inspIndex.intValue = index;
        serializedObject.ApplyModifiedProperties();
    }

    //====================================================================

    private void TouchpadSettings()
    {
        SerializedProperty enabled = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("enabled");
        EditorGUILayout.PropertyField(enabled);

        SerializedProperty nameProp = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("name");
        EditorGUILayout.PropertyField(nameProp);

        GUILayout.Space(4);

        SerializedProperty textureA = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("textureA");
        SerializedProperty alwaysVisible = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("alwaysVisible");
        float textureSize = Mathf.Clamp(EditorGUIUtility.currentViewWidth * 0.25f, 0f, 80f);
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Texture");

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Background", GUILayout.MinWidth(textureSize));
                    textureA.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField(textureA.objectReferenceValue, typeof(Texture2D), true, GUILayout.Height(textureSize), GUILayout.Width(textureSize));
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                {
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            //
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel(" ");
                alwaysVisible.boolValue = !EditorGUILayout.ToggleLeft("Hide during runtime", !alwaysVisible.boolValue);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(4);

        if (textureA.objectReferenceValue != null && textureA.objectReferenceValue != null)
        {
            SerializedProperty rectA = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("rectA");
            //EditorGUILayout.PropertyField(rectA);
            float xMin = rectA.rectValue.xMin;
            float xMax = rectA.rectValue.xMax;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Horizontal");
                EditorGUILayout.MinMaxSlider(ref xMin, ref xMax, 0f, 1f);
                xMin = Mathf.Round(xMin * 100f) * 0.01f;
                xMax = Mathf.Round(xMax * 100f) * 0.01f;
                EditorGUILayout.LabelField("" + xMin.ToString("F2") + "/" + xMax.ToString("F2"), GUILayout.Width(60));
            }
            EditorGUILayout.EndHorizontal();
            float yMin = rectA.rectValue.yMin;
            float yMax = rectA.rectValue.yMax;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Vertical");
                EditorGUILayout.MinMaxSlider(ref yMin, ref yMax, 0f, 1f);
                yMin = Mathf.Round(yMin * 100f) * 0.01f;
                yMax = Mathf.Round(yMax * 100f) * 0.01f;
                EditorGUILayout.LabelField("" + yMin.ToString("F2") + "/" + yMax.ToString("F2"), GUILayout.Width(60));
            }
            EditorGUILayout.EndHorizontal();
            rectA.rectValue = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);

            GUILayout.Space(8);

            SerializedProperty axisMultiplier = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("axisMultiplier");
            EditorGUILayout.PropertyField(axisMultiplier);

            GUILayout.Space(4);

            SerializedProperty compensateForDeviceRoll = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("compensateForDeviceRoll");
            EditorGUILayout.PropertyField(compensateForDeviceRoll, new GUIContent("Device Roll Compensation"));

            SerializedProperty updateOrientationSensorsScript = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("updateOrientationSensorsScript");
            EditorGUILayout.PropertyField(updateOrientationSensorsScript);

            GUILayout.Space(8);

            SerializedProperty editorSimulationAxisKeys = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("editorSimulationAxisKeys");
            SerializedProperty editorSimulationAxisKeys_simulationMode = editorSimulationAxisKeys.FindPropertyRelative("simulationMode");
            EditorGUILayout.PropertyField(editorSimulationAxisKeys_simulationMode, new GUIContent("Editor Simulation"));

            if (editorSimulationAxisKeys_simulationMode.enumDisplayNames[editorSimulationAxisKeys_simulationMode.enumValueIndex] == "Keyboard_Custom")
            {
                GUILayout.Space(4);

                SerializedProperty editorSimulationAxisKeys_keyCode_Left = editorSimulationAxisKeys.FindPropertyRelative("keyCode_Left");
                EditorGUILayout.PropertyField(editorSimulationAxisKeys_keyCode_Left, new GUIContent("  - Left"));

                SerializedProperty editorSimulationAxisKeys_keyCode_Right = editorSimulationAxisKeys.FindPropertyRelative("keyCode_Right");
                EditorGUILayout.PropertyField(editorSimulationAxisKeys_keyCode_Right, new GUIContent("  - Right"));

                SerializedProperty editorSimulationAxisKeys_keyCode_Up = editorSimulationAxisKeys.FindPropertyRelative("keyCode_Up");
                EditorGUILayout.PropertyField(editorSimulationAxisKeys_keyCode_Up, new GUIContent("  - Up"));

                SerializedProperty editorSimulationAxisKeys_keyCode_Down = editorSimulationAxisKeys.FindPropertyRelative("keyCode_Down");
                EditorGUILayout.PropertyField(editorSimulationAxisKeys_keyCode_Down, new GUIContent("  - Down"));

                GUILayout.Space(4);

                SerializedProperty editorSimulationAxisKeys_keyCode_DoubleTap = editorSimulationAxisKeys.FindPropertyRelative("keyCode_DoubleTap");
                EditorGUILayout.PropertyField(editorSimulationAxisKeys_keyCode_DoubleTap, new GUIContent("  - DoubleTap"));
            }
        }
    }

    //====================================================================

    private void JoystickSettings()
    {
        SerializedProperty enabled = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("enabled");
        EditorGUILayout.PropertyField(enabled);

        SerializedProperty nameProp = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("name");
        EditorGUILayout.PropertyField(nameProp);

        GUILayout.Space(4);

        SerializedProperty textureA = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("textureA");
        SerializedProperty textureB = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("textureB");
        SerializedProperty alwaysVisible = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("alwaysVisible");
        float textureSize = Mathf.Clamp(EditorGUIUtility.currentViewWidth * 0.25f, 0f, 80f);
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {

                EditorGUILayout.PrefixLabel("Textures");

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Background", GUILayout.MinWidth(textureSize));
                    textureA.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField(textureA.objectReferenceValue, typeof(Texture2D), true, GUILayout.Height(textureSize), GUILayout.Width(textureSize));
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Button", GUILayout.MinWidth(textureSize));
                    textureB.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField(textureB.objectReferenceValue, typeof(Texture2D), true, GUILayout.Height(textureSize), GUILayout.Width(textureSize));
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            //
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel(" ");
                alwaysVisible.boolValue = !EditorGUILayout.ToggleLeft("Hide Joystick when not active", !alwaysVisible.boolValue);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        if (textureA.objectReferenceValue != null && textureB.objectReferenceValue != null)
        {
            GUILayout.Space(8);

            SerializedProperty sizeA = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("sizeA");
            SerializedProperty sizeB = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("sizeB");
            Vector2 _sizeA = sizeA.vector2Value;
            Vector2 _sizeB = sizeB.vector2Value;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Background Size");
                _sizeA.x = EditorGUILayout.Slider(_sizeA.x, 0f, 1f);
                _sizeA.y = _sizeA.x;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Button Size");
                _sizeB.x = EditorGUILayout.Slider(_sizeB.x, 0f, 1f);
                _sizeB.y = _sizeB.x;
            }
            EditorGUILayout.EndHorizontal();
            sizeA.vector2Value = _sizeA;
            sizeB.vector2Value = _sizeB;

            GUILayout.Space(8);

            SerializedProperty centerDefault = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("centerDefault");
            SerializedProperty centerA = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("centerA");
            SerializedProperty centerAnew = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("centerAnew");
            SerializedProperty centerB = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("centerB");
            SerializedProperty centerBnew = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("centerBnew");
            Vector2 _centerDefault = centerDefault.vector2Value;
            EditorGUILayout.LabelField("Center");
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("   - X");
                _centerDefault.x = EditorGUILayout.Slider(_centerDefault.x, 0f, 1f);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("   - Y");
                _centerDefault.y = EditorGUILayout.Slider(_centerDefault.y, 0f, 1f);
            }
            EditorGUILayout.EndHorizontal();
            centerDefault.vector2Value = _centerDefault;
            if (!Application.isPlaying)
            {
                centerA.vector2Value = _centerDefault;
                centerAnew.vector2Value = _centerDefault;
                centerB.vector2Value = _centerDefault;
                centerBnew.vector2Value = _centerDefault;
            }

            GUILayout.Space(8);

            SerializedProperty axisMultiplier = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("axisMultiplier");
            EditorGUILayout.PropertyField(axisMultiplier);

            GUILayout.Space(4);

            SerializedProperty sensitivity = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("sensitivity");
            EditorGUILayout.PropertyField(sensitivity);

            SerializedProperty smoothingTime = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("smoothingTime");
            EditorGUILayout.PropertyField(smoothingTime);

            SerializedProperty updateOrientationSensorsScript = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("updateOrientationSensorsScript");
            EditorGUILayout.PropertyField(updateOrientationSensorsScript);

            GUILayout.Space(8);

            SerializedProperty editorSimulationAxisKeys = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("editorSimulationAxisKeys");
            SerializedProperty editorSimulationAxisKeys_simulationMode = editorSimulationAxisKeys.FindPropertyRelative("simulationMode");
            EditorGUILayout.PropertyField(editorSimulationAxisKeys_simulationMode, new GUIContent("Editor Simulation"));

            if (editorSimulationAxisKeys_simulationMode.enumDisplayNames[editorSimulationAxisKeys_simulationMode.enumValueIndex] == "Keyboard_Custom")
            {
                GUILayout.Space(4);

                SerializedProperty editorSimulationAxisKeys_keyCode_Left = editorSimulationAxisKeys.FindPropertyRelative("keyCode_Left");
                EditorGUILayout.PropertyField(editorSimulationAxisKeys_keyCode_Left, new GUIContent("  - Left"));

                SerializedProperty editorSimulationAxisKeys_keyCode_Right = editorSimulationAxisKeys.FindPropertyRelative("keyCode_Right");
                EditorGUILayout.PropertyField(editorSimulationAxisKeys_keyCode_Right, new GUIContent("  - Right"));

                SerializedProperty editorSimulationAxisKeys_keyCode_Up = editorSimulationAxisKeys.FindPropertyRelative("keyCode_Up");
                EditorGUILayout.PropertyField(editorSimulationAxisKeys_keyCode_Up, new GUIContent("  - Up"));

                SerializedProperty editorSimulationAxisKeys_keyCode_Down = editorSimulationAxisKeys.FindPropertyRelative("keyCode_Down");
                EditorGUILayout.PropertyField(editorSimulationAxisKeys_keyCode_Down, new GUIContent("  - Down"));

                GUILayout.Space(4);

                SerializedProperty editorSimulationAxisKeys_keyCode_DoubleTap = editorSimulationAxisKeys.FindPropertyRelative("keyCode_DoubleTap");
                EditorGUILayout.PropertyField(editorSimulationAxisKeys_keyCode_DoubleTap, new GUIContent("  - DoubleTap"));
            }
        }
    }

    //====================================================================

    private void ButtonSettings()
    {
        SerializedProperty enabled = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("enabled");
        EditorGUILayout.PropertyField(enabled);

        SerializedProperty nameProp = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("name");
        EditorGUILayout.PropertyField(nameProp);

        GUILayout.Space(4);

        SerializedProperty textureA = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("textureA");
        SerializedProperty textureB = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("textureB");
        float textureSize = Mathf.Clamp(EditorGUIUtility.currentViewWidth * 0.25f, 0f, 80f);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Textures");
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Unpressed", GUILayout.MinWidth(textureSize));
                textureA.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField(textureA.objectReferenceValue, typeof(Texture2D), true, GUILayout.Height(textureSize), GUILayout.Width(textureSize));
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Pressed", GUILayout.MinWidth(textureSize));
                textureB.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField(textureB.objectReferenceValue, typeof(Texture2D), true, GUILayout.Height(textureSize), GUILayout.Width(textureSize));
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();

        if (textureA.objectReferenceValue != null && textureB.objectReferenceValue != null)
        {
            GUILayout.Space(8);

            SerializedProperty forceSquareButton = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("forceSquareButton");
            SerializedProperty sizeA = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("sizeA");
            Vector2 _sizeA = sizeA.vector2Value;
            if (forceSquareButton.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Size");
                    _sizeA.x = EditorGUILayout.Slider(_sizeA.x, 0f, 1f, GUILayout.MinWidth(60));
                    _sizeA.y = _sizeA.x;
                    forceSquareButton.boolValue = EditorGUILayout.ToggleLeft("Square", forceSquareButton.boolValue, GUILayout.MaxWidth(60));

                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Size Horizontal");
                    _sizeA.x = EditorGUILayout.Slider(_sizeA.x, 0f, 1f);
                    forceSquareButton.boolValue = EditorGUILayout.ToggleLeft("Square", forceSquareButton.boolValue, GUILayout.MaxWidth(60));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Size Vertical");
                    _sizeA.y = EditorGUILayout.Slider(_sizeA.y, 0f, 1f, GUILayout.MinWidth(60));
                    EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(60));
                }
                EditorGUILayout.EndHorizontal();
            }
            sizeA.vector2Value = _sizeA;

            GUILayout.Space(8);

            SerializedProperty centerDefault = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("centerDefault");
            SerializedProperty centerA = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("centerA");
            SerializedProperty centerAnew = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("centerAnew");
            Vector2 _centerDefault = centerDefault.vector2Value;
            EditorGUILayout.LabelField("Center");
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("   - X");
                _centerDefault.x = EditorGUILayout.Slider(_centerDefault.x, 0f, 1f);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("   - Y");
                _centerDefault.y = EditorGUILayout.Slider(_centerDefault.y, 0f, 1f);
            }
            EditorGUILayout.EndHorizontal();
            centerDefault.vector2Value = _centerDefault;
            if (!Application.isPlaying)
            {
                centerA.vector2Value = _centerDefault;
                centerAnew.vector2Value = _centerDefault;
            }

            GUILayout.Space(8);

            SerializedProperty moveWithTouch = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("moveWithTouch");
            SerializedProperty smoothingTime = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("smoothingTime");
            EditorGUILayout.PropertyField(moveWithTouch);
            if (moveWithTouch.boolValue)
            {
                EditorGUILayout.PropertyField(smoothingTime, new GUIContent("   - Smoothing Time"));
            }

            SerializedProperty useAsSwitch = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("useAsSwitch");
            EditorGUILayout.PropertyField(useAsSwitch);

            GUILayout.Space(8);

            SerializedProperty editorSimulationButtonKey = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("editorSimulationButtonKey");
            EditorGUILayout.PropertyField(editorSimulationButtonKey);
        }
    }

    //====================================================================

    private void TextureSettings()
    {
        SerializedProperty enabled = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("enabled");
        EditorGUILayout.PropertyField(enabled);

        SerializedProperty nameProp = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("name");
        EditorGUILayout.PropertyField(nameProp);

        GUILayout.Space(4);

        GUILayout.Space(4);

        SerializedProperty textureA = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("textureA");
        SerializedProperty alwaysVisible = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("alwaysVisible");
        float textureSize = Mathf.Clamp(EditorGUIUtility.currentViewWidth * 0.25f, 0f, 80f);

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Texture");
            textureA.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField(textureA.objectReferenceValue, typeof(Texture2D), true, GUILayout.Height(textureSize), GUILayout.Width(textureSize));
        }
        EditorGUILayout.EndHorizontal();

        if (textureA.objectReferenceValue != null)
        {
            GUILayout.Space(8);

            SerializedProperty forceSquareButton = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("forceSquareButton");
            SerializedProperty sizeA = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("sizeA");
            Vector2 _sizeA = sizeA.vector2Value;
            if (forceSquareButton.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Size");
                    _sizeA.x = EditorGUILayout.Slider(_sizeA.x, 0f, 1f, GUILayout.MinWidth(60));
                    _sizeA.y = _sizeA.x;
                    forceSquareButton.boolValue = EditorGUILayout.ToggleLeft("Square", forceSquareButton.boolValue, GUILayout.MaxWidth(60));

                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Size Horizontal");
                    _sizeA.x = EditorGUILayout.Slider(_sizeA.x, 0f, 1f);
                    forceSquareButton.boolValue = EditorGUILayout.ToggleLeft("Square", forceSquareButton.boolValue, GUILayout.MaxWidth(60));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Size Vertical");
                    _sizeA.y = EditorGUILayout.Slider(_sizeA.y, 0f, 1f, GUILayout.MinWidth(60));
                    EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(60));
                }
                EditorGUILayout.EndHorizontal();
            }
            sizeA.vector2Value = _sizeA;

            GUILayout.Space(8);

            SerializedProperty centerDefault = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("centerDefault");
            SerializedProperty centerA = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("centerA");
            SerializedProperty centerAnew = uiList.GetArrayElementAtIndex(index).FindPropertyRelative("centerAnew");
            Vector2 _centerDefault = centerDefault.vector2Value;
            EditorGUILayout.LabelField("Center");
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("   - X");
                _centerDefault.x = EditorGUILayout.Slider(_centerDefault.x, 0f, 1f);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("   - Y");
                _centerDefault.y = EditorGUILayout.Slider(_centerDefault.y, 0f, 1f);
            }
            EditorGUILayout.EndHorizontal();
            centerDefault.vector2Value = _centerDefault;
            if (!Application.isPlaying)
            {
                centerA.vector2Value = _centerDefault;
                centerAnew.vector2Value = _centerDefault;
            }
        }
    }

    //====================================================================

    private void ShowRuntimeValues()
    {
        GUILayout.Space(8);
        EditorGUILayout.BeginVertical("Box");
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("--== RUNTIME VALUES ==--", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            for (int i = 0; i < uiList.arraySize; i++)
            {
                SerializedProperty type = uiList.GetArrayElementAtIndex(i).FindPropertyRelative("type");
                switch (type.enumDisplayNames[type.enumValueIndex])
                {
                    case touchpadStr:
                        EditorGUILayout.BeginVertical("Box");
                        {
                            GUILayout.Space(4);
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("enabled"), GUIContent.none);
                            GUI.enabled = false;
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("name"));
                            EditorGUILayout.PropertyField(type);
                            GUILayout.Space(4);
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("axis"));
                            GUILayout.Space(4);
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("doubleTap"));
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("doubleTapHold"));
                            GUILayout.Space(4);
                            GUI.enabled = true;
                        }
                        EditorGUILayout.EndVertical();
                        break;
                    case joystickStr:
                        EditorGUILayout.BeginVertical("Box");
                        {
                            GUILayout.Space(4);
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("enabled"), GUIContent.none);
                            GUI.enabled = false;
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("name"));
                            EditorGUILayout.PropertyField(type);
                            GUILayout.Space(4);
                            EditorGUILayout.PrefixLabel(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("name").ToString());
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("axis"));
                            GUILayout.Space(4);
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("angle"));
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("magnitude"));
                            GUILayout.Space(4);
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("doubleTapHold"), new GUIContent("Double Tapped"));
                            GUILayout.Space(4);
                            GUI.enabled = true;
                        }
                        EditorGUILayout.EndVertical();
                        break;
                    case buttonStr:
                        EditorGUILayout.BeginVertical("Box");
                        {
                            GUILayout.Space(4);
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("enabled"), GUIContent.none);
                            GUI.enabled = false;
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("name"));
                            EditorGUILayout.PropertyField(type);
                            GUILayout.Space(4);
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("status"), new GUIContent("Button Status = "));
                            GUILayout.Space(4);
                            GUI.enabled = true;
                        }
                        EditorGUILayout.EndVertical();
                        break;
                    case textureStr:
                        EditorGUILayout.BeginVertical("Box");
                        {
                            GUILayout.Space(4);
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("enabled"), GUIContent.none);
                            GUI.enabled = false;
                            EditorGUILayout.PropertyField(uiList.GetArrayElementAtIndex(i).FindPropertyRelative("name"));
                            EditorGUILayout.PropertyField(type);
                            GUILayout.Space(4);
                            GUI.enabled = true;
                        }
                        EditorGUILayout.EndVertical();
                        break;
                    default:
                        break;
                }
            }

            EditorGUILayout.Space();
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();

    }

    //====================================================================
}