using UnityEngine;

namespace MSP_Input
{
    public partial class TouchInterface : MonoBehaviour
    {
        //====================================================================

        void UpdateButton(int i)
        {
            if (i < 0 || i >= uiList.Count)
            {
                return;
            }

            if (!uiList[i].enabled)
            {
                uiList[i].touchID = -1;
                uiList[i].isActive = false;
                uiList[i].status = ButtonStatus.Up;
                return;
            }

            // Calculate Screen Rects
            uiList[i].screenRectA.width = (int)(uiList[i].sizeA.x * sw);
            uiList[i].screenRectA.height = uiList[i].forceSquareButton ? uiList[i].screenRectA.width : (int)(uiList[i].sizeA.y * sh);
            uiList[i].screenRectA.center = new Vector2((int)(uiList[i].centerA.x * sw), (int)(uiList[i].centerA.y * sh));

#if UNITY_EDITOR
            if (Input.touches.Length == 0)
            {
                // Mouse input in editor
                Vector2 centerTouch = new Vector2(Input.mousePosition.x / sw, Input.mousePosition.y / sh);
                if (uiList[i].touchID == -1)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (uiList[i].screenRectA.Contains(Input.mousePosition))
                        {
                            uiList[i].touchID = 100;
                            uiList[i].isActive = true;
                            uiList[i].centerAnew = uiList[i].moveWithTouch ? centerTouch : uiList[i].centerDefault;
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
                    }
                    else
                    {
                        uiList[i].centerAnew = uiList[i].moveWithTouch ? centerTouch : uiList[i].centerDefault;
                    }
                }

                // Editor Simulation Key
                if (!uiList[i].useAsSwitch)
                {
                    if (Input.GetKeyDown(uiList[i].editorSimulationButtonKey))
                    {
                        uiList[i].isActive = true;
                    }
                    if (Input.GetKeyUp(uiList[i].editorSimulationButtonKey))
                    {
                        uiList[i].isActive = false;
                    }
                }
                else
                {
                    if (Input.GetKeyDown(uiList[i].editorSimulationButtonKey))
                    {
                        uiList[i].isActive = !uiList[i].isActive;
                    }
                }
            }
#endif

            // Touch input
            foreach (Touch touch in Input.touches)
            {
                Vector2 centerTouch = new Vector2(touch.position.x / sw, touch.position.y / sh);
                //
                if (touch.phase == TouchPhase.Began)
                {
                    if (uiList[i].screenRectA.Contains(touch.position))
                    {
                        uiList[i].touchID = touch.fingerId;
                        uiList[i].isActive = true;
                        uiList[i].centerAnew = uiList[i].moveWithTouch ? centerTouch : uiList[i].centerDefault;
                    }
                }
                //
                if (uiList[i].touchID == touch.fingerId)
                {
                    if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                    {
                        uiList[i].isActive = false;
                        uiList[i].touchID = -1;
                        uiList[i].centerAnew = uiList[i].centerDefault;
                    }
                    else
                    {
                        uiList[i].centerAnew = uiList[i].moveWithTouch ? centerTouch : uiList[i].centerDefault;
                    }
                }
            }

            float smoothFactor = (uiList[i].smoothingTime > Time.deltaTime) ? Time.deltaTime / uiList[i].smoothingTime : 1f;
            uiList[i].centerA = Vector2.Lerp(uiList[i].centerA, uiList[i].centerAnew, smoothFactor);

            uiList[i].centerBnew = uiList[i].centerAnew;
            uiList[i].centerB = uiList[i].centerA;

            // handle button status
            if (uiList[i].status == ButtonStatus.GoingUp)
            {
                uiList[i].status = ButtonStatus.Up;
            }
            if (uiList[i].status == ButtonStatus.GoingDown)
            {
                uiList[i].status = ButtonStatus.Down;
            }

            if (!uiList[i].useAsSwitch)
            {
                if (uiList[i].isActive)
                {
                    if (uiList[i].status == ButtonStatus.Up)
                    {
                        uiList[i].status = ButtonStatus.GoingDown;
                    }
                }
                else
                {
                    if (uiList[i].status == ButtonStatus.Down)
                    {
                        uiList[i].status = ButtonStatus.GoingUp;
                    }
                }
            }
            else
            {
                if (uiList[i].isActive)
                {
                    if (uiList[i].status == ButtonStatus.Up)
                    {
                        uiList[i].status = ButtonStatus.GoingDown;
                    }
                    if (uiList[i].status == ButtonStatus.Down)
                    {
                        uiList[i].status = ButtonStatus.GoingUp;
                    }
                    uiList[i].isActive = false;
                }
            }

        }

        //====================================================================

    }
}