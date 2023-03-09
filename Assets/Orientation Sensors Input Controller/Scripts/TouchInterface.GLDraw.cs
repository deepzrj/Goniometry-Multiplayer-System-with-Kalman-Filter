using UnityEngine;

namespace MSP_Input
{
    public partial class TouchInterface : MonoBehaviour
    {
        //====================================================================

        void OnPostRender()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !showInEditMode)
            {
                return;
            }
#endif

            if (material == null)
            {
                //material = new Material(Shader.Find("Unlit/Transparent"));
                material = new Material(Shader.Find("Hidden/MSP_GUI"));
                material.color = Color.white;
                material.mainTextureScale = Vector2.one;
                material.mainTextureOffset = Vector2.zero;
                material.enableInstancing = true;
            }

            GL.PushMatrix();
            GL.LoadOrtho();

            foreach (TouchInterface.Properties ui in uiList)
            {
                if (ui.enabled)
                {
                    switch (ui.type)
                    {
                        case TouchInterface.Type.Touchpad:
#if UNITY_EDITOR
                            if (!Application.isPlaying || ui.alwaysVisible)
#else
                            if (ui.alwaysVisible)
#endif
                            {

                                material.mainTexture = ui.textureA;
                                if (material.mainTexture)
                                {
                                    material.SetPass(0);
                                    drawRrect = ui.rectA;
                                    GL_DrawTexture();
                                }
                            }
                            break;

                        case TouchInterface.Type.Joystick:
#if UNITY_EDITOR
                            if (!Application.isPlaying || ui.alwaysVisible || (!ui.alwaysVisible && ui.isActive))
#else
                            if (ui.alwaysVisible || (!ui.alwaysVisible && ui.isActive))
#endif
                            {
                                material.mainTexture = ui.textureA;
                                if (material.mainTexture)
                                {
                                    material.SetPass(0);
                                    drawRrect.width = ui.sizeA.x;
                                    drawRrect.height = drawRrect.width * cam.aspect;
                                    drawRrect.center = ui.centerA;
                                    GL_DrawTexture();
                                }
                                material.mainTexture = ui.textureB;
                                if (material.mainTexture)
                                {
                                    material.SetPass(0);
                                    drawRrect.width = ui.sizeA.x * ui.sizeB.x;
                                    drawRrect.height = drawRrect.width * cam.aspect;
                                    drawRrect.center = ui.centerB;
                                    GL_DrawTexture();
                                }
                            }
                            break;

                        case TouchInterface.Type.Button:
                            material.mainTexture = (ui.status == TouchInterface.ButtonStatus.Down || ui.status == TouchInterface.ButtonStatus.GoingDown) ? ui.textureB : ui.textureA;
                            if (material.mainTexture)
                            {
                                material.SetPass(0);
                                drawRrect.width = ui.sizeA.x;
                                drawRrect.height = ui.forceSquareButton ? drawRrect.width * cam.aspect : ui.sizeA.y;
                                drawRrect.center = ui.centerA;
                                GL_DrawTexture();
                            }
                            break;

                        case TouchInterface.Type.Texture:
                            material.mainTexture = ui.textureA;
                            if (material.mainTexture)
                            {
                                material.SetPass(0);
                                drawRrect.width = ui.sizeA.x;
                                drawRrect.height = ui.forceSquareButton ? drawRrect.width * cam.aspect : ui.sizeA.y;
                                drawRrect.center = ui.centerA;
                                GL_DrawTexture();
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            GL.PopMatrix();
        }

        //====================================================================

        public void GL_DrawTexture()
        {
            //GL.PushMatrix();
            //GL.LoadOrtho();
            GL.Begin(GL.QUADS);
            GL.TexCoord2(0f, 0f);
            GL.Vertex3(drawRrect.xMin, drawRrect.yMin, 0f);
            GL.TexCoord2(0f, 1f);
            GL.Vertex3(drawRrect.xMin, drawRrect.yMax, 0f);
            GL.TexCoord2(1f, 1f);
            GL.Vertex3(drawRrect.xMax, drawRrect.yMax, 0f);
            GL.TexCoord2(1f, 0f);
            GL.Vertex3(drawRrect.xMax, drawRrect.yMin, 0f);
            GL.End();
            //GL.PopMatrix();
        }

        //====================================================================

        private void ResetCameraSettings()
        {
            cam = gameObject.GetComponent<Camera>() as Camera;
            if (cam == null)
            {
                return;
            }
            cam.clearFlags = CameraClearFlags.Depth;
            cam.cullingMask = 0;
            cam.orthographic = true;
            cam.fieldOfView = 60f;
            cam.rect = new Rect(0, 0, 1, 1);
            cam.usePhysicalProperties = false;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 1f;
            cam.depth = 90;
            cam.renderingPath = RenderingPath.Forward;
            cam.useOcclusionCulling = false;
            cam.allowHDR = false;
            cam.allowMSAA = false;
            cam.allowDynamicResolution = false;
            cam.enabled = true;
        }

        //====================================================================

    }
}