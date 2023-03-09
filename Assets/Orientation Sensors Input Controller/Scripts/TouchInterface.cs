using UnityEngine;
using System.Collections.Generic;

namespace MSP_Input
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("MSP_Input/Touch Interface")]
    public partial class TouchInterface : MonoBehaviour
    {
        public enum Type { None, Touchpad, Joystick, Button, Texture };

        private static TouchInterface Singleton;
        public enum ButtonStatus { Up, Down, GoingUp, GoingDown };

        public enum EditorSimulationAxisKeysMode { None, Keyboard_WASD, Keyboard_CursorKeys, Keyboard_Custom };
        [System.Serializable]
        public class EditorSimulationAxisKeys
        {
            public EditorSimulationAxisKeysMode simulationMode = EditorSimulationAxisKeysMode.None;
            public KeyCode keyCode_Left = KeyCode.None;
            public KeyCode keyCode_Right = KeyCode.None;
            public KeyCode keyCode_Up = KeyCode.None;
            public KeyCode keyCode_Down = KeyCode.None;
            public KeyCode keyCode_DoubleTap = KeyCode.None;
        }

        [System.Serializable]
        public class Properties
        {
            public Type type = Type.None;
            public string name = "";
            public bool enabled = true;
            [HideInInspector]
            public bool isActive = false;

            public Vector2 centerDefault = new Vector2(0.5f, 0.5f);

            public Texture2D textureA;
            public Vector2 sizeA = new Vector2(0.1f, 0.1f);
            [HideInInspector]
            public Vector2 centerA = new Vector2(0.5f, 0.5f);
            [HideInInspector]
            public Vector2 centerAnew = new Vector2(0.5f, 0.5f);
            [HideInInspector]
            public Rect screenRectA = new Rect(0, 0, 0, 0);
            public Rect rectA = new Rect(0.4f, 0.4f, 0.2f, 0.2f);

            public Texture2D textureB;
            public Vector2 sizeB = new Vector2(0.25f, 0.25f);
            [HideInInspector]
            public Vector2 centerB = new Vector2(0.5f, 0.5f);
            [HideInInspector]
            public Vector2 centerBnew = new Vector2(0.5f, 0.5f);
            [HideInInspector]
            public Rect screenRectB = new Rect(0, 0, 0, 0);

            public bool forceSquareButton = true;
            public bool useAsSwitch = false;
            public bool moveWithTouch = false;

            [HideInInspector]
            public ButtonStatus status = ButtonStatus.Up;

            [HideInInspector]
            public int touchID = -1;

            [Range(0.0f, 1.0f)]
            public float smoothingTime = 0.1f;
            [HideInInspector]
            public float previousTime = 0f;

            [Range(0.0f, 1.0f)]
            public float sensitivity = 1f;
            [HideInInspector]
            public AnimationCurve sensitivityCurve = new AnimationCurve(new Keyframe(-1f, 1f), new Keyframe(0f, 1f), new Keyframe(1f, 1f));

            public Vector2 axisMultiplier = new Vector2(1f, 1f);
            public bool compensateForDeviceRoll = true;
            public bool alwaysVisible = true;
            public bool updateOrientationSensorsScript = false;

            [HideInInspector]
            public Vector2 axis = Vector2.zero;
            [HideInInspector]
            public float angle = 0f;
            [HideInInspector]
            public float magnitude = 0f;
            [HideInInspector]
            public bool doubleTap = false;
            [HideInInspector]
            public bool doubleTapHold = false;

            public KeyCode editorSimulationButtonKey = KeyCode.None;
            public EditorSimulationAxisKeys editorSimulationAxisKeys;
        }

        public List<Properties> uiList = new List<Properties>();

        private float sw;
        private float sh;

        [HideInInspector]
        [SerializeField]
        private int inspIndex = -1;

        private Vector2 mouseScreenPointOld;

        const float maxTimeForDoubleTap = 0.5f;

#if UNITY_EDITOR
        [SerializeField]
        private bool showInEditMode = true;
#endif

        private Camera cam;
        [SerializeField]
        private Material material;
        private Rect drawRrect = new Rect(0f, 0f, 1f, 1f);

        //====================================================================

        private void Awake()
        {
            Singleton = this;
            ResetCameraSettings();

            for (int i = 0; i < uiList.Count; i++)
            {
                uiList[i].isActive = false;
                uiList[i].centerA = uiList[i].centerDefault;
                uiList[i].centerAnew = uiList[i].centerDefault;
                uiList[i].centerB = uiList[i].centerDefault;
                uiList[i].centerBnew = uiList[i].centerDefault;
                uiList[i].touchID = -1;
                uiList[i].previousTime = -1000f;
                uiList[i].status = ButtonStatus.Up;
            }

            mouseScreenPointOld = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
        }

        //====================================================================

        private void OnValidate()
        {
            Singleton = this;
            ResetCameraSettings();
        }

        //====================================================================

        private void Update()
        {
            Singleton = this;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            sw = (float)Screen.width;
            sh = (float)Screen.height;

            for (int index = 0; index < uiList.Count; index++)
            {
                switch (uiList[index].type)
                {
                    case Type.Touchpad:
                        UpdateTouchpad(index);
                        break;
                    case Type.Joystick:
                        UpdateJoystick(index);
                        break;
                    case Type.Button:
                        UpdateButton(index);
                        break;
                    case Type.Texture:
                        UpdateTexture(index);
                        break;
                    default:
                        break;
                }
            }
        }

        //====================================================================

        public void Add()
        {
            uiList.Add(new Properties());
            inspIndex = uiList.Count - 1;
        }

        //====================================================================

        public void Add(int index)
        {
            if (uiList.Count == 0)
            {
                uiList.Add(new Properties());
                inspIndex = 0;
            }
            else
            {
                index = Mathf.Clamp(index, 0, uiList.Count - 1);
                uiList.Insert(index, new Properties());
                inspIndex = index;
            }
        }

        //====================================================================

        public void Delete()
        {
            if (uiList.Count > 0)
            {
                uiList.RemoveAt(uiList.Count - 1);
                if (inspIndex > uiList.Count - 1)
                {
                    inspIndex = uiList.Count - 1;
                }
            }
        }

        //====================================================================

        public void Delete(int index)
        {
            if (index < uiList.Count && index >= 0)
            {
                uiList.RemoveAt(index);
                inspIndex = index - 1;
                if (inspIndex < 0 && uiList.Count > 0)
                {
                    inspIndex = 0;
                }
            }
        }

        //====================================================================

        public void MoveDown(int index)
        {
            if (index > 0 && uiList.Count > 1)
            {
                Properties temp = uiList[index - 1];
                uiList[index - 1] = uiList[index];
                uiList[index] = temp;
                inspIndex = index - 1;
            }
        }

        //====================================================================

        public void MoveUp(int index)
        {
            if (index >= 0 && index < uiList.Count - 1)
            {
                Properties temp = uiList[index + 1];
                uiList[index + 1] = uiList[index];
                uiList[index] = temp;
                inspIndex = index + 1;
            }
        }

        //====================================================================

        public string[] GetNamesList()
        {
            string[] names = new string[uiList.Count];
            for (int j = 0; j < uiList.Count; j++)
            {
                names[j] = j.ToString() + " [" + uiList[j].type.ToString() + "]" + " - \"" + uiList[j].name + "\"";
                if (!uiList[j].enabled)
                {
                    names[j] += " {disabled}";
                }
            }
            return names;
        }

        //====================================================================

    }
}