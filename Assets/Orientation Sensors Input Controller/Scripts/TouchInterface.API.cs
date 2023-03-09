using UnityEngine;

namespace MSP_Input
{
    public partial class TouchInterface : MonoBehaviour
    {
        //====================================================================

        static private bool ValidID(int id)
        {
            if (id >= 0 && id < Singleton.uiList.Count)
            {
                return true;
            }
            return false;
        }

        //====================================================================

        // GET functions

        //====================================================================

        static public int GetID(string name)
        {
            for (int id = 0; id < Singleton.uiList.Count; id++)
            {
                if (Singleton.uiList[id].name.Equals(name))
                {
                    return id;
                }
            }
            return -1;
        }

        //====================================================================

        static public bool GetButton(int id)
        {
            if (!ValidID(id))
            {
                return false;
            }
            if (Singleton.uiList[id].type == Type.Button)
            {
                return (Singleton.uiList[id].status == ButtonStatus.Down || Singleton.uiList[id].status == ButtonStatus.GoingDown);
            }
            if (Singleton.uiList[id].type == Type.Joystick || Singleton.uiList[id].type == Type.Touchpad)
            {
                return (Singleton.uiList[id].doubleTap || Singleton.uiList[id].doubleTapHold);
            }
            return false;
        }

        //====================================================================

        static public bool GetButton(string name)
        {
            return GetButton(GetID(name));
        }

        //====================================================================

        static public bool GetButtonDown(int id)
        {
            if (!ValidID(id))
            {
                return false;
            }
            if (Singleton.uiList[id].type == Type.Button)
            {
                return (Singleton.uiList[id].status == ButtonStatus.GoingDown);
            }
            if (Singleton.uiList[id].type == Type.Joystick || Singleton.uiList[id].type == Type.Touchpad)
            {
                return (Singleton.uiList[id].doubleTap);
            }
            return false;
        }

        //====================================================================

        static public bool GetButtonDown(string name)
        {
            return GetButtonDown(GetID(name));
        }

        //====================================================================

        static public bool GetButtonUp(int id)
        {
            if (!ValidID(id))
            {
                return false;
            }
            if (Singleton.uiList[id].type == Type.Button)
            {
                return (Singleton.uiList[id].status == ButtonStatus.GoingUp);
            }
            return false;
        }

        //====================================================================

        static public bool GetButtonUp(string name)
        {
            return GetButtonUp(GetID(name));
        }

        //====================================================================

        static public Vector2 GetAxis(int id)
        {
            if (!ValidID(id))
            {
                return Vector2.zero;
            }
            if (Singleton.uiList[id].type == Type.Joystick || Singleton.uiList[id].type == Type.Touchpad)
            {
                return Singleton.uiList[id].axis;
            }
            return Vector2.zero;
        }

        //====================================================================

        static public Vector2 GetAxis(string name)
        {
            return GetAxis(GetID(name));
        }

        //====================================================================

        static public void GetAngleAndMagnitude(int id, out float angle, out float magnitude)
        {
            if (!ValidID(id))
            {
                angle = 0f;
                magnitude = 0f;
                return;
            }
            if (Singleton.uiList[id].type == Type.Joystick)
            {
                angle = Singleton.uiList[id].angle;
                magnitude = Singleton.uiList[id].magnitude;
                return;
            }
            angle = 0f;
            magnitude = 0f;
            return;
        }

        //====================================================================

        static public void GetAngleAndMagnitude(string name, out float angle, out float magnitude)
        {
            GetAngleAndMagnitude(GetID(name), out float tempAngle, out float tempMagnitude);
            angle = tempAngle;
            magnitude = tempMagnitude;
        }

        //====================================================================

        static public bool GetDoubleTap(int id)
        {
            if (!ValidID(id))
            {
                return false;
            }
            if (Singleton.uiList[id].type == Type.Joystick || Singleton.uiList[id].type == Type.Touchpad)
            {
                return Singleton.uiList[id].doubleTap;
            }
            return false;
        }

        //====================================================================

        static public bool GetDoubleTap(string name)
        {
            return GetDoubleTap(GetID(name));
        }

        //====================================================================

        static public bool GetDoubleTapHold(int id)
        {
            if (!ValidID(id))
            {
                return false;
            }
            if (Singleton.uiList[id].type == Type.Joystick || Singleton.uiList[id].type == Type.Touchpad)
            {
                return Singleton.uiList[id].doubleTapHold;
            }
            return false;
        }

        //====================================================================

        static public bool GetDoubleTapHold(string name)
        {
            return GetDoubleTapHold(GetID(name));
        }

        //====================================================================

        static public Vector2 GetAxisMultiplier(int id)
        {
            if (!ValidID(id))
            {
                return Vector2.zero;
            }
            if (Singleton.uiList[id].type == Type.Joystick || Singleton.uiList[id].type == Type.Touchpad)
            {
                return Singleton.uiList[id].axisMultiplier;
            }
            return Vector2.zero;
        }

        //====================================================================

        static public Vector2 GetAxisMultiplier(string name, Vector2 axisMultiplier)
        {
            return GetAxisMultiplier(GetID(name));
        }

        //====================================================================

        // SET functions

        //====================================================================

        static public bool Enable(int id)
        {
            if (ValidID(id))
            {
                Singleton.uiList[id].enabled = true;
                return true;
            }
            return false;
        }

        //====================================================================


        static public bool Enable(string name)
        {
            return Enable(GetID(name));
        }

        //====================================================================

        static public bool Disable(int id)
        {
            if (ValidID(id))
            {
                Singleton.uiList[id].enabled = false;
                return true;
            }
            return false;
        }

        //====================================================================

        static public bool Disable(string name)
        {
            return Disable(GetID(name));
        }

        //====================================================================

        static public bool SetAxisMultiplier(int id, Vector2 axisMultiplier)
        {
            if (!ValidID(id))
            {
                return false;
            }
            if (Singleton.uiList[id].type == Type.Joystick || Singleton.uiList[id].type == Type.Touchpad)
            {
                Singleton.uiList[id].axisMultiplier = axisMultiplier;
                return true;
            }
            return false;
        }

        //====================================================================

        static public bool SetAxisMultiplier(string name, Vector2 axisMultiplier)
        {
            return SetAxisMultiplier(GetID(name), axisMultiplier);
        }

        //====================================================================

        static public bool SetSensitivity(int id, float sensitivity)
        {
            if (!ValidID(id))
            {
                return false;
            }
            if (Singleton.uiList[id].type == Type.Joystick)
            {
                Singleton.uiList[id].sensitivity = Mathf.Clamp(sensitivity, 0f, 1f);
                return true;
            }
            return false;
        }

        //====================================================================

        static public bool SetSensitivity(string name, float sensitivity)
        {
            return SetSensitivity(GetID(name), sensitivity);
        }

        //====================================================================

    }
}