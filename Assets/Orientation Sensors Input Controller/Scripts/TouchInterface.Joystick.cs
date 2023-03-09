using UnityEngine;

namespace MSP_Input
{
    public partial class TouchInterface : MonoBehaviour
    {
        //====================================================================

        void UpdateJoystick(int i)
        {
            if (i < 0 || i >= uiList.Count)
            {
                return;
            }

            if (!uiList[i].enabled)
            {
                uiList[i].touchID = -1;
                uiList[i].isActive = false;
                uiList[i].axis = Vector2.zero;
                uiList[i].angle = 0f;
                uiList[i].magnitude = 0f;
                uiList[i].doubleTap = false;
                uiList[i].doubleTapHold = false;
                return;
            }

            uiList[i].screenRectA.width = (int)(uiList[i].sizeA.x * sw);
            uiList[i].screenRectA.height = uiList[i].screenRectA.width;
            uiList[i].screenRectA.center = new Vector2((int)(uiList[i].centerA.x * sw), (int)(uiList[i].centerA.y * sh));

            uiList[i].screenRectB.width = (int)(uiList[i].sizeA.x * uiList[i].sizeB.x * sw);
            uiList[i].screenRectB.height = uiList[i].screenRectB.width;
            uiList[i].screenRectB.center = new Vector2((int)(uiList[i].centerB.x * sw), (int)(uiList[i].centerB.y * sh));

            uiList[i].doubleTap = false;
            uiList[i].sensitivity = Mathf.Clamp(uiList[i].sensitivity, 0f, 1f);
            uiList[i].sensitivityCurve = new AnimationCurve(new Keyframe(-1f, 1f), new Keyframe(0f, uiList[i].sensitivity), new Keyframe(1f, 1f));

#if UNITY_EDITOR
            if (Input.touches.Length == 0)
            {
                // Mouse input in editor (GameWindow)
                Vector2 centerTouch = new Vector2(Input.mousePosition.x / sw, Input.mousePosition.y / sh);
                if (uiList[i].touchID == -1)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (uiList[i].screenRectA.Contains(Input.mousePosition))
                        {
                            uiList[i].isActive = true;
                            uiList[i].touchID = 100;
                            uiList[i].centerAnew = centerTouch;
                            uiList[i].centerBnew = centerTouch;
                            uiList[i].axis = Vector2.zero;
                            uiList[i].angle = 0f;
                            uiList[i].magnitude = 0f;
                            if ((Time.time - uiList[i].previousTime) < maxTimeForDoubleTap)
                            {
                                uiList[i].doubleTap = true;
                                uiList[i].doubleTapHold = true;
                            }
                            else
                            {
                                uiList[i].doubleTap = false;
                                uiList[i].doubleTapHold = false;
                            }
                            uiList[i].previousTime = Time.time;
                        }
                    }
                }
                if (uiList[i].touchID == 100)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        uiList[i].isActive = false;
                        uiList[i].touchID = -1;
                        uiList[i].centerAnew = uiList[i].centerDefault;
                        uiList[i].centerBnew = uiList[i].centerDefault;
                        uiList[i].axis = Vector2.zero;
                        uiList[i].angle = 0f;
                        uiList[i].magnitude = 0f;
                        uiList[i].doubleTap = false;
                        uiList[i].doubleTapHold = false;
                    }
                    else
                    {
                        float maxJoyMagnitude = 0.5f * uiList[i].sizeA.x * (1f - uiList[i].sizeB.x);
                        Vector2 joyDir = centerTouch - uiList[i].centerAnew;
                        joyDir.y *= sh / sw;
                        uiList[i].axis = joyDir / maxJoyMagnitude;
                        if (uiList[i].axis.sqrMagnitude > 1f) uiList[i].axis.Normalize();
                        uiList[i].centerBnew.x = uiList[i].centerAnew.x + uiList[i].axis.x * maxJoyMagnitude;
                        uiList[i].centerBnew.y = uiList[i].centerAnew.y + uiList[i].axis.y * maxJoyMagnitude * sw / sh;

                        uiList[i].angle = Vector2.Angle(Vector2.up, uiList[i].axis) * Mathf.Sign(joyDir.x);

                        uiList[i].axis.x *= uiList[i].sensitivityCurve.Evaluate(uiList[i].magnitude) * uiList[i].axisMultiplier.x;
                        uiList[i].axis.y *= uiList[i].sensitivityCurve.Evaluate(uiList[i].magnitude) * uiList[i].axisMultiplier.y;

                        uiList[i].magnitude = uiList[i].axis.magnitude;
                    }
                }

                // Editor: Simulation keys
                if (uiList[i].editorSimulationAxisKeys.simulationMode != EditorSimulationAxisKeysMode.None)
                {
                    switch (uiList[i].editorSimulationAxisKeys.simulationMode)
                    {
                        case EditorSimulationAxisKeysMode.Keyboard_WASD:
                            uiList[i].editorSimulationAxisKeys.keyCode_Up = KeyCode.W;
                            uiList[i].editorSimulationAxisKeys.keyCode_Left = KeyCode.A;
                            uiList[i].editorSimulationAxisKeys.keyCode_Down = KeyCode.S;
                            uiList[i].editorSimulationAxisKeys.keyCode_Right = KeyCode.D;
                            uiList[i].editorSimulationAxisKeys.keyCode_DoubleTap = KeyCode.LeftShift;
                            break;
                        case EditorSimulationAxisKeysMode.Keyboard_CursorKeys:
                            uiList[i].editorSimulationAxisKeys.keyCode_Up = KeyCode.UpArrow;
                            uiList[i].editorSimulationAxisKeys.keyCode_Left = KeyCode.LeftArrow;
                            uiList[i].editorSimulationAxisKeys.keyCode_Down = KeyCode.DownArrow;
                            uiList[i].editorSimulationAxisKeys.keyCode_Right = KeyCode.RightArrow;
                            uiList[i].editorSimulationAxisKeys.keyCode_DoubleTap = KeyCode.RightShift;
                            break;
                        default:
                            break;
                    }

                    if (Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_Right) ||
                        Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_Left) ||
                        Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_Up) ||
                        Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_Down))
                    {
                        uiList[i].isActive = true;
                        uiList[i].centerAnew = uiList[i].centerDefault;
                        uiList[i].centerBnew = uiList[i].centerDefault;
                        uiList[i].axis = Vector2.zero;
                        uiList[i].angle = 0f;
                        uiList[i].magnitude = 0f;
                    }
                    if (Input.GetKeyUp(uiList[i].editorSimulationAxisKeys.keyCode_Right) ||
                        Input.GetKeyUp(uiList[i].editorSimulationAxisKeys.keyCode_Left) ||
                        Input.GetKeyUp(uiList[i].editorSimulationAxisKeys.keyCode_Up) ||
                        Input.GetKeyUp(uiList[i].editorSimulationAxisKeys.keyCode_Down))
                    {
                        uiList[i].isActive = false;
                        uiList[i].centerAnew = uiList[i].centerDefault;
                        uiList[i].centerBnew = uiList[i].centerDefault;
                        uiList[i].axis = Vector2.zero;
                        uiList[i].angle = 0f;
                        uiList[i].magnitude = 0f;
                    }

                    Vector2 dj = Vector2.zero;
                    if (Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_Up))
                    {
                        dj.y += 1;
                    }
                    if (Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_Down))
                    {
                        dj.y -= 1;
                    }
                    if (Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_Right))
                    {
                        dj.x += 1;
                    }
                    if (Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_Left))
                    {
                        dj.x -= 1;
                    }

                    if (dj != Vector2.zero)
                    {
                        dj.Normalize();

                        float maxJoyMagnitude = 0.5f * uiList[i].sizeA.x * (1f - uiList[i].sizeB.x);
                        Vector2 joyDir = dj;
                        joyDir.y *= sh / sw;
                        uiList[i].axis = joyDir / maxJoyMagnitude;
                        if (uiList[i].axis.sqrMagnitude > 1f) uiList[i].axis.Normalize();
                        uiList[i].centerBnew.x = uiList[i].centerAnew.x + uiList[i].axis.x * maxJoyMagnitude;
                        uiList[i].centerBnew.y = uiList[i].centerAnew.y + uiList[i].axis.y * maxJoyMagnitude * sw / sh;

                        uiList[i].angle = Vector2.Angle(Vector2.up, uiList[i].axis) * Mathf.Sign(joyDir.x);

                        uiList[i].axis.x = Mathf.Clamp(dj.x * uiList[i].axisMultiplier.x, -uiList[i].axisMultiplier.x, uiList[i].axisMultiplier.x);
                        uiList[i].axis.y = Mathf.Clamp(dj.y * uiList[i].axisMultiplier.y, -uiList[i].axisMultiplier.y, uiList[i].axisMultiplier.y);

                        uiList[i].magnitude = uiList[i].axis.magnitude;
                    }
                }
                if (Input.GetKeyDown(uiList[i].editorSimulationAxisKeys.keyCode_DoubleTap))
                {
                    uiList[i].doubleTap = true;
                }
                if (Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_DoubleTap))
                {
                    uiList[i].doubleTapHold = true;
                }
                if (Input.GetKeyUp(uiList[i].editorSimulationAxisKeys.keyCode_DoubleTap))
                {
                    uiList[i].doubleTap = false;
                    uiList[i].doubleTapHold = false;
                }
            }
#endif

            // Touch input
            foreach (Touch touch in Input.touches)
            {
                Vector2 centerTouch = new Vector2(touch.position.x / sw, touch.position.y / sh);

                if (touch.phase == TouchPhase.Began)
                {
                    if (uiList[i].screenRectA.Contains(touch.position))
                    {
                        uiList[i].isActive = true;
                        uiList[i].touchID = touch.fingerId;
                        uiList[i].centerAnew = centerTouch;
                        uiList[i].centerBnew = centerTouch;
                        uiList[i].axis = Vector2.zero;
                        uiList[i].angle = 0f;
                        uiList[i].magnitude = 0f;
                        if ((Time.time - uiList[i].previousTime) < maxTimeForDoubleTap)
                        {
                            uiList[i].doubleTap = true;
                            uiList[i].doubleTapHold = true;
                        }
                        else
                        {
                            uiList[i].doubleTap = false;
                            uiList[i].doubleTapHold = false;
                        }
                        uiList[i].previousTime = Time.time;
                    }
                }

                if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && touch.fingerId == uiList[i].touchID)
                {
                    uiList[i].isActive = false;
                    uiList[i].touchID = -1;
                    uiList[i].centerAnew = uiList[i].centerDefault;
                    uiList[i].centerBnew = uiList[i].centerDefault;
                    uiList[i].axis = Vector2.zero;
                    uiList[i].angle = 0f;
                    uiList[i].magnitude = 0f;
                    uiList[i].doubleTap = false;
                    uiList[i].doubleTapHold = false;
                }

                if (uiList[i].isActive && touch.fingerId == uiList[i].touchID)
                {
                    float maxJoyMagnitude = 0.5f * uiList[i].sizeA.x * (1f - uiList[i].sizeB.x);
                    Vector2 joyDir = centerTouch - uiList[i].centerAnew;
                    joyDir.y *= sh / sw;
                    uiList[i].axis = joyDir / maxJoyMagnitude;
                    if (uiList[i].axis.sqrMagnitude > 1f) uiList[i].axis.Normalize();
                    uiList[i].centerBnew.x = uiList[i].centerAnew.x + uiList[i].axis.x * maxJoyMagnitude;
                    uiList[i].centerBnew.y = uiList[i].centerAnew.y + uiList[i].axis.y * maxJoyMagnitude * sw / sh;

                    uiList[i].angle = Vector2.Angle(Vector2.up, uiList[i].axis) * Mathf.Sign(joyDir.x);

                    uiList[i].axis.x *= uiList[i].sensitivityCurve.Evaluate(uiList[i].magnitude) * uiList[i].axisMultiplier.x;
                    uiList[i].axis.y *= uiList[i].sensitivityCurve.Evaluate(uiList[i].magnitude) * uiList[i].axisMultiplier.y;

                    uiList[i].magnitude = uiList[i].axis.magnitude;
                }
            }

            float smoothFactor = (uiList[i].smoothingTime > Time.deltaTime) ? Time.deltaTime / uiList[i].smoothingTime : 1f;
            uiList[i].centerA = Vector2.Lerp(uiList[i].centerA, uiList[i].centerAnew, smoothFactor);
            uiList[i].centerB = Vector2.Lerp(uiList[i].centerB, uiList[i].centerBnew, smoothFactor);

            if (uiList[i].updateOrientationSensorsScript)
            {
                OrientationSensors.AddFloatToHeadingOffset(uiList[i].axis.x);
                OrientationSensors.AddFloatToPitchOffset(uiList[i].axis.y);
            }
        }

        //====================================================================

    }
}