using UnityEngine;

namespace MSP_Input
{
    public partial class TouchInterface : MonoBehaviour
    {
        //====================================================================

        void UpdateTouchpad(int i)
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
                uiList[i].doubleTap = false;
                uiList[i].doubleTapHold = false;
                return;
            }

            uiList[i].screenRectA.width = (int)(uiList[i].rectA.width * sw);
            uiList[i].screenRectA.height = (int)(uiList[i].rectA.height * sh);
            uiList[i].screenRectA.x = (int)(uiList[i].rectA.x * sw);
            uiList[i].screenRectA.y = (int)(uiList[i].rectA.y * sh);

            uiList[i].isActive = false;
            uiList[i].doubleTap = false;
            Vector2 delta = Vector2.zero;

#if UNITY_EDITOR
            if (Input.touches.Length == 0)
            {
                // Editor: Mouse 
                Vector2 mouseScreenPoint = Input.mousePosition;
                if (Input.GetMouseButtonDown(0))
                {
                    if (uiList[i].screenRectA.Contains(mouseScreenPoint))
                    {
                        uiList[i].isActive = true;
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
                        mouseScreenPointOld = mouseScreenPoint;
                    }
                }
                else
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (uiList[i].screenRectA.Contains(mouseScreenPoint))
                        {
                            uiList[i].isActive = true;
                            Vector2 mouseDelta = mouseScreenPoint - mouseScreenPointOld;
                            mouseDelta.x /= sw;
                            mouseDelta.y /= sh;
                            delta += mouseDelta;
                        }
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        uiList[i].doubleTap = false;
                        uiList[i].doubleTapHold = false;
                    }
                }
                mouseScreenPointOld = mouseScreenPoint;

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
                    }
                    Vector2 keyDelta = Vector2.zero;
                    if (Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_Up))
                    {
                        keyDelta.y += Time.deltaTime * 0.25f;
                    }
                    if (Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_Down))
                    {
                        keyDelta.y -= Time.deltaTime * 0.25f;
                    }
                    if (Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_Right))
                    {
                        keyDelta.x += Time.deltaTime * 0.25f;
                    }
                    if (Input.GetKey(uiList[i].editorSimulationAxisKeys.keyCode_Left))
                    {
                        keyDelta.x -= Time.deltaTime * 0.25f;
                    }
                    delta += keyDelta;
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

            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (uiList[i].screenRectA.Contains(touch.position))
                    {
                        uiList[i].isActive = true;
                        uiList[i].touchID = touch.fingerId;
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

            if (uiList[i].touchID < 0 || uiList[i].touchID >= Input.touches.Length)
            {
                uiList[i].touchID = -1;
                return;
            }

            if (uiList[i].touchID != -1)
            {
                Touch touch = Input.touches[uiList[i].touchID];
                if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
                {
                    uiList[i].isActive = false;
                    uiList[i].touchID = -1;
                    uiList[i].doubleTap = false;
                    uiList[i].doubleTapHold = false;
                }
                else
                {
                    uiList[i].isActive = true;
                    if (uiList[i].screenRectA.Contains(touch.position))
                    {
                        if (uiList[i].compensateForDeviceRoll)
                        {
                            float alpha = Mathf.Deg2Rad * MSP_Input.OrientationSensors.GetRoll();
                            float cosin = Mathf.Cos(alpha);
                            float sinus = Mathf.Sin(alpha);
                            delta.x = -(touch.deltaPosition.y / sh) * sinus + (touch.deltaPosition.x / sw) * cosin;
                            delta.y = (touch.deltaPosition.x / sw) * sinus + (touch.deltaPosition.y / sh) * cosin;
                        }
                        else
                        {
                            delta = touch.deltaPosition;
                            delta.x /= sw;
                            delta.y /= sh;
                        }
                    }
                }
            }

            delta.x *= uiList[i].axisMultiplier.x * uiList[i].rectA.width * 100f;
            delta.y *= uiList[i].axisMultiplier.y * uiList[i].rectA.height * 100f;

            uiList[i].axis = delta;

            if (uiList[i].updateOrientationSensorsScript)
            {
                OrientationSensors.AddFloatToHeadingOffset(uiList[i].axis.x);
                OrientationSensors.AddFloatToPitchOffset(uiList[i].axis.y);
            }
        }

        //====================================================================

    }
}