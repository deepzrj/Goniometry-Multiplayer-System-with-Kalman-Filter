using System;
using UnityEngine;

namespace MSP_Input
{
    [Serializable]
    public struct HeadingPitchRoll
    {
        public float heading;
        public float pitch;
        public float roll;

        private const float sqrthalf = 0.707106781186548f;

        //=====================================================================

        // overload operator +
        public static HeadingPitchRoll operator +(HeadingPitchRoll a, HeadingPitchRoll b)
        {
            return new HeadingPitchRoll()
            {
                heading = a.heading + b.heading,
                pitch = a.pitch + b.pitch,
                roll = a.roll + b.roll
            };
        }

        //=====================================================================

        // overload operator -
        public static HeadingPitchRoll operator -(HeadingPitchRoll a, HeadingPitchRoll b)
        {
            return new HeadingPitchRoll()
            {
                heading = a.heading - b.heading,
                pitch = a.pitch - b.pitch,
                roll = a.roll - b.roll
            };
        }

        //=====================================================================

        // overload operator *
        public static HeadingPitchRoll operator *(HeadingPitchRoll a, float b)
        {
            return new HeadingPitchRoll()
            {
                heading = a.heading * b,
                pitch = a.pitch * b,
                roll = a.roll * b
            };
        }

        //=====================================================================

        // overload operator *
        public static HeadingPitchRoll operator *(float a, HeadingPitchRoll b)
        {
            return new HeadingPitchRoll()
            {
                heading = a * b.heading,
                pitch = a * b.pitch,
                roll = a * b.roll
            };
        }

        //=====================================================================

        // overload operator *
        public static HeadingPitchRoll operator *(HeadingPitchRoll a, Vector3 b)
        {
            return new HeadingPitchRoll()
            {
                heading = a.heading * b.x,
                pitch = a.pitch * b.y,
                roll = a.roll * b.z
            };
        }

        //=====================================================================

        // overload operator *
        public static HeadingPitchRoll operator *(Vector3 a, HeadingPitchRoll b)
        {
            return new HeadingPitchRoll()
            {
                heading = a.x * b.heading,
                pitch = a.y * b.pitch,
                roll = a.z * b.roll
            };
        }

        //=====================================================================

        // overload operator *
        public static HeadingPitchRoll operator *(HeadingPitchRoll a, HeadingPitchRoll b)
        {
            return new HeadingPitchRoll()
            {
                heading = a.heading * b.heading,
                pitch = a.pitch * b.pitch,
                roll = a.roll * b.roll
            };
        }

        //=====================================================================

        // overload operator /
        public static HeadingPitchRoll operator /(HeadingPitchRoll a, float b)
        {
            return new HeadingPitchRoll()
            {
                heading = a.heading / b,
                pitch = a.pitch / b,
                roll = a.roll / b
            };
        }

        //=====================================================================

        // overload operator /
        public static HeadingPitchRoll operator /(float a, HeadingPitchRoll b)
        {
            return new HeadingPitchRoll()
            {
                heading = a / b.heading,
                pitch = a / b.pitch,
                roll = a / b.roll
            };
        }

        //=====================================================================

        // overload operator /
        public static HeadingPitchRoll operator /(HeadingPitchRoll a, Vector3 b)
        {
            return new HeadingPitchRoll()
            {
                heading = a.heading / b.x,
                pitch = a.pitch / b.y,
                roll = a.roll / b.z
            };
        }

        //=====================================================================

        // overload operator /
        public static HeadingPitchRoll operator /(Vector3 a, HeadingPitchRoll b)
        {
            return new HeadingPitchRoll()
            {
                heading = a.x / b.heading,
                pitch = a.y / b.pitch,
                roll = a.z / b.roll
            };
        }

        //=====================================================================

        public static HeadingPitchRoll Square(HeadingPitchRoll a)
        {
            return new HeadingPitchRoll()
            {
                heading = a.heading * a.heading,
                pitch = a.pitch * a.pitch,
                roll = a.roll * a.roll
            };
        }

        //=====================================================================

        public static HeadingPitchRoll Sqrt(HeadingPitchRoll a)
        {
            return new HeadingPitchRoll()
            {
                heading = Mathf.Sqrt(a.heading),
                pitch = Mathf.Sqrt(a.pitch),
                roll = Mathf.Sqrt(a.roll)
            };
        }

        //=====================================================================

        public static HeadingPitchRoll Zero()
        {
            return new HeadingPitchRoll()
            {
                heading = 0f,
                pitch = 0f,
                roll = 0f
            };
        }

        //=====================================================================

        public static HeadingPitchRoll Lerp(HeadingPitchRoll a, HeadingPitchRoll b, float t)
        {
            return new HeadingPitchRoll()
            {
                heading = Mathf.LerpAngle(a.heading, b.heading, t),
                pitch = Mathf.LerpAngle(a.pitch, b.pitch, t),
                roll = Mathf.LerpAngle(a.roll, b.roll, t)
            };
        }

        //=====================================================================

        public static HeadingPitchRoll Lerp(HeadingPitchRoll a, HeadingPitchRoll b, HeadingPitchRoll t)
        {
            return new HeadingPitchRoll()
            {
                heading = Mathf.LerpAngle(a.heading, b.heading, t.heading),
                pitch = Mathf.LerpAngle(a.pitch, b.pitch, t.pitch),
                roll = Mathf.LerpAngle(a.roll, b.roll, t.roll)
            };
        }

        //=====================================================================

        public void Clamp()
        {
            this = Clamp(this);
        }

        //=====================================================================

        public static HeadingPitchRoll Clamp(HeadingPitchRoll orientation)
        {
            while (orientation.heading > 180f) orientation.heading -= 360f;
            while (orientation.heading < -180f) orientation.heading += 360f;

            while (orientation.pitch > 90f) orientation.pitch = 90f;
            while (orientation.pitch < -90f) orientation.pitch = -90f;

            while (orientation.roll > 180f) orientation.roll -= 360f;
            while (orientation.roll < -180f) orientation.roll += 360f;

            return orientation;
        }

        //=====================================================================

        public Quaternion ToQuaternion()
        {
            return ToQuaternion(this);
        }

        //=====================================================================

        public static Quaternion ToQuaternion(HeadingPitchRoll orientation)
        {
            return Quaternion.Euler(0f, orientation.heading, 0f) * Quaternion.Euler(orientation.pitch, 0f, 0f) * Quaternion.Euler(0f, 0f, orientation.roll);
        }

        //=====================================================================

        public static Quaternion ToQuaternion(float heading, float pitch, float roll)
        {
            return Quaternion.Euler(0f, heading, 0f) * Quaternion.Euler(pitch, 0f, 0f) * Quaternion.Euler(0f, 0f, roll);
        }

        //=====================================================================

        public Quaternion ToQuaternionTopDownView()
        {
            return ToQuaternionTopDownView(this);
        }

        //=====================================================================

        public static Quaternion ToQuaternionTopDownView(HeadingPitchRoll orientation)
        {
	        return Quaternion.Euler(0f, orientation.heading, 0f) * Quaternion.Euler(orientation.pitch, 0f, 0f) * Quaternion.Euler(0f, 0f, orientation.roll) * new Quaternion(sqrthalf, 0f, 0f, sqrthalf);
        }

        //=====================================================================

        public static Quaternion ToQuaternionTopDownView(float heading, float pitch, float roll)
        {
	        return Quaternion.Euler(0f, heading, 0f) * Quaternion.Euler(pitch, 0f, 0f) * Quaternion.Euler(0f, 0f, roll) * new Quaternion(sqrthalf, 0f, 0f, sqrthalf);
        }

        //=====================================================================

        public static HeadingPitchRoll FromQuaternion(Quaternion quaternion, bool preventGimbalLock = false)
        {
            Vector3 quaternionForward = quaternion * Vector3.forward;
            float pitch = 90f - Vector3.Angle(Vector3.down, quaternionForward);
            float heading = Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(quaternionForward, Vector3.up), Vector3.up);
            Quaternion tmpQuat = Quaternion.AngleAxis(-pitch, Vector3.right) * Quaternion.AngleAxis(-heading, Vector3.up) * quaternion;
            float roll = Vector3.SignedAngle(Vector3.up, (tmpQuat * Vector3.up), Vector3.forward);
            if (preventGimbalLock)
            {
                AnimationCurve rollAdjustmentCurve = new AnimationCurve(new Keyframe(-90f, 0f), new Keyframe(-80f, 1f), new Keyframe(80f, 1f), new Keyframe(90f, 0f));
                roll = roll * rollAdjustmentCurve.Evaluate(pitch);
            }
            return new HeadingPitchRoll()
            {
                heading = heading,
                pitch = pitch,
                roll = roll
            };
        }

        //=====================================================================

        public static HeadingPitchRoll FromQuaternionTopDownView(Quaternion quaternion, bool preventGimbalLock = false)
        {
            Vector3 quaternionForward = quaternion * Vector3.forward;
            Vector3 quaternionUp = quaternion * Vector3.up;
	        float pitch = 90f - Vector3.Angle(Vector3.down, quaternionUp);
            float heading = Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(quaternionUp, Vector3.up), Vector3.up);
	        //Quaternion tmpQuat = Quaternion.AngleAxis(-pitch, Vector3.right) * Quaternion.AngleAxis(-heading, Vector3.up) * quaternion;
	        Quaternion tmpQuat = Quaternion.AngleAxis(-pitch, Vector3.right) * Quaternion.AngleAxis(-heading, Vector3.up) * quaternion;
	        float roll = Vector3.SignedAngle(Vector3.down, (tmpQuat * Vector3.forward), Vector3.forward);
	        if (preventGimbalLock)
	        {
                AnimationCurve rollAdjustmentCurve = new AnimationCurve(new Keyframe(-90f, 0f), new Keyframe(-80f, 1f), new Keyframe(80f, 1f), new Keyframe(90f, 0f));
                roll = roll * rollAdjustmentCurve.Evaluate(pitch);
            }
            return new HeadingPitchRoll()
            {
                heading = heading,
                pitch = pitch,
                roll = roll
            };
        }

	    //public static HeadingPitchRoll FromQuaternionTopDownView(Quaternion quaternion, bool preventGimbalLock = false)
	    //{
		//    Vector3 quaternionForward = quaternion * Vector3.forward;
		//    Vector3 quaternionUp = quaternion * Vector3.up;
		//    float pitch = 90f - Vector3.Angle(Vector3.down, quaternionForward);
		//    float heading = Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(quaternionUp, Vector3.up), Vector3.up);
		//    Quaternion tmpQuat = Quaternion.AngleAxis(-pitch, Vector3.right) * Quaternion.AngleAxis(-heading, Vector3.up) * quaternion;
		//    float roll = Vector3.SignedAngle(Vector3.up, (tmpQuat * Vector3.up), Vector3.forward);
		//    {
		//	    AnimationCurve rollAdjustmentCurve = new AnimationCurve(new Keyframe(-90f, 0f), new Keyframe(-80f, 1f), new Keyframe(80f, 1f), new Keyframe(90f, 0f));
		//	    roll = roll * rollAdjustmentCurve.Evaluate(pitch);
		//    }
		//    return new HeadingPitchRoll()
		//    {
		//	    heading = heading,
		//	    pitch = pitch,
		//	    roll = roll
        //    };
	    //}

        //=====================================================================
    }
}