using UnityEngine;

namespace MSP_Input
{
    public partial class TouchInterface : MonoBehaviour
    {
        //====================================================================

        void UpdateTexture(int i)
        {
            if (i < 0 || i >= uiList.Count)
            {
                return;
            }

            if (!uiList[i].enabled)
            {
                return;
            }

            uiList[i].touchID = -1;
            uiList[i].isActive = false;
            uiList[i].status = ButtonStatus.Up;

            // Calculate Screen Rects
            //uiList[i].screenRectA.width = (int)(uiList[i].sizeA.x * sw);
            //uiList[i].screenRectA.height = uiList[i].forceSquareButton ? uiList[i].screenRectA.width : (int)(uiList[i].sizeA.y * sh);
            //uiList[i].screenRectA.center = new Vector2((int)(uiList[i].centerA.x * sw), (int)(uiList[i].centerA.y * sh));
        }

        //====================================================================

    }
}