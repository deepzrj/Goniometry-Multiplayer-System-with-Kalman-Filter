using UnityEngine;
using System.Collections.Generic;

namespace MSP_Input
{
    public partial class OrientationSensors : MonoBehaviour
    {
        //====================================================================

        [System.Serializable]
        public struct TransformUpdateData
        {
            public bool enabled;
            public Transform targetTransform;
            [Range(0f, 2f)]
            public float smoothingTime;
            public bool copyHeading;
            public FloatMinMax heading;
            public bool copyPitch;
            public FloatMinMax pitch;
            public bool copyRoll;
            public FloatMinMax roll;
            public bool canPushEdge;

            public TransformUpdateData(Transform targetTransform)
            {
                this.enabled = true;
                this.targetTransform = targetTransform;
                this.smoothingTime = 0f;
                this.copyHeading = true;
                this.heading = new FloatMinMax(0f, -180f, 180f, -180f, 180f);
                this.copyPitch = true;
                this.pitch = new FloatMinMax(0f, -90f, 90f, -90f, 90f);
                this.copyRoll = true;
                this.roll = new FloatMinMax(0f, -180f, 180f, -180f, 180f);
                this.canPushEdge = false;
            }
        }

        [SerializeField]
        private List<TransformUpdateData> transformUpdateList = new List<TransformUpdateData>();

        [HideInInspector]
        [SerializeField]
        private int inspTransformIndex = -1;

        //====================================================================

        public void AddTransform()
        {
            transformUpdateList.Add(new TransformUpdateData(null));
            inspTransformIndex = transformUpdateList.Count - 1;
        }

        //====================================================================

        public void AddTransform(int index)
        {
            if (transformUpdateList.Count == 0)
            {
                transformUpdateList.Add(new TransformUpdateData(null));
                inspTransformIndex = 0;
            }
            else
            {
                index = Mathf.Clamp(index, 0, transformUpdateList.Count - 1);
                transformUpdateList.Insert(index, new TransformUpdateData(null));
                inspTransformIndex = index;
            }
        }

        //====================================================================

        public void DeleteTransform()
        {
            if (transformUpdateList.Count > 0)
            {
                transformUpdateList.RemoveAt(transformUpdateList.Count - 1);
                if (inspTransformIndex > transformUpdateList.Count - 1)
                {
                    inspTransformIndex = transformUpdateList.Count - 1;
                }
            }
        }

        //====================================================================

        public void DeleteTransform(int index)
        {
            if (index < transformUpdateList.Count && index >= 0)
            {
                transformUpdateList.RemoveAt(index);
                inspTransformIndex = index - 1;
                if (inspTransformIndex < 0 && transformUpdateList.Count > 0)
                {
                    inspTransformIndex = 0;
                }
            }
        }

        //====================================================================

        public void MoveDownTransform(int index)
        {
            if (index > 0 && transformUpdateList.Count > 1)
            {
                TransformUpdateData tempTUD = transformUpdateList[index - 1];
                transformUpdateList[index - 1] = transformUpdateList[index];
                transformUpdateList[index] = tempTUD;
                inspTransformIndex = index - 1;
            }
        }

        //====================================================================

        public void MoveUpTransform(int index)
        {
            if (index >= 0 && index < transformUpdateList.Count - 1)
            {
                TransformUpdateData tempTUD = transformUpdateList[index + 1];
                transformUpdateList[index + 1] = transformUpdateList[index];
                transformUpdateList[index] = tempTUD;
                inspTransformIndex = index + 1;
            }
        }

        //====================================================================

        public string[] GetTransformNames()
        {
            string[] transformNames = new string[transformUpdateList.Count];
            for (int j = 0; j < transformUpdateList.Count; j++)
            {
                string name = (transformUpdateList[j].targetTransform) ? transformUpdateList[j].targetTransform.name : "null";
                transformNames[j] = (transformUpdateList[j].enabled) ? "#" + j + " - " + name : "#" + j + " - " + name + " [disabled]";
            }
            return transformNames;
        }

        //====================================================================

        public void SetParentAsTargetTransform(int index)
        {
            TransformUpdateData tempTUD = transformUpdateList[index];
            tempTUD.targetTransform = this.transform;
            transformUpdateList[index] = tempTUD;
        }

        //====================================================================

        public void UpdateTransforms()
        {
            for (int i = 0; i < transformUpdateList.Count; i++)
            {
                TransformUpdateData tud = transformUpdateList[i];

                if (tud.enabled)
                {
                    if (tud.targetTransform == null)
                    {
                        continue;
                    }

                    float smoothing = (tud.smoothingTime > Time.deltaTime) ? Time.deltaTime / tud.smoothingTime : 1f;
                    Quaternion rotNew = Quaternion.identity;
                    Quaternion rotOld = tud.targetTransform.rotation;

                    // Full rotation copy?
                    if (tud.copyHeading && tud.copyPitch && tud.copyRoll)
                    {
                        if (tud.heading.minValue <= -180f &&
                            tud.heading.maxValue >= 180f &&
                            tud.pitch.minValue <= -90f &&
                            tud.pitch.maxValue >= 90f &&
                            tud.roll.minValue <= -180f &&
                            tud.roll.maxValue >= 180f)
                        {
                            rotNew = OrientationSensors.GetRotation();
                            tud.targetTransform.rotation = Quaternion.Lerp(rotOld, rotNew, smoothing);
                            continue;
                        }
                    }

                    // Partial rotation copy
                    OrientationSensors.GetHeadingPitchRoll(out float heading, out float pitch, out float roll);

                    HeadingPitchRoll newOrientation = new HeadingPitchRoll()
                    {
                        heading = (tud.copyHeading) ? Mathf.Clamp(heading, tud.heading.minValue, tud.heading.maxValue) : tud.heading.value,
                        pitch = (tud.copyPitch) ? Mathf.Clamp(pitch, tud.pitch.minValue, tud.pitch.maxValue) : tud.pitch.value,
                        roll = (tud.copyRoll) ? Mathf.Clamp(roll, tud.roll.minValue, tud.roll.maxValue) : tud.roll.value
                    };

                    if (tud.canPushEdge)
                    {
                        if (Mathf.Abs(heading - newOrientation.heading) > 0.0001)
                        {
                            OrientationSensors.SetHeading(newOrientation.heading);
                        }
                        if (Mathf.Abs(pitch - newOrientation.pitch) > 0.0001)
                        {
                            OrientationSensors.SetPitch(newOrientation.pitch);
                        }
                    }

                    rotNew = newOrientation.ToQuaternion();
                    tud.targetTransform.rotation = Quaternion.Lerp(rotOld, rotNew, smoothing);

                    tud.heading = new FloatMinMax(newOrientation.heading, tud.heading.minValue, tud.heading.maxValue, -180f, 180f);
                    tud.pitch = new FloatMinMax(newOrientation.pitch, tud.pitch.minValue, tud.pitch.maxValue, -90f, 90f);
                    tud.roll = new FloatMinMax(newOrientation.roll, tud.roll.minValue, tud.roll.maxValue, -180f, 180f);

                    transformUpdateList[i] = tud;
                }
            }
        }

        //====================================================================
    }
}