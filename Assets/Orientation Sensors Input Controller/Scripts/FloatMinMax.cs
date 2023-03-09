using UnityEngine;

namespace MSP_Input
{
    [System.Serializable]
    public struct FloatMinMax
    {
        public float value;
        public float minValue;
        public float maxValue;
        public float minValueLimit;
        public float maxValueLimit;

        //=====================================================================

        public FloatMinMax(float value, float minValue, float maxValue, float minValueLimit, float maxValueLimit)
        {
            this.minValueLimit = Mathf.Min(minValueLimit, minValue, maxValue, maxValueLimit);
            this.maxValueLimit = Mathf.Max(minValueLimit, minValue, maxValue, maxValueLimit);
            this.minValue = Mathf.Max(minValueLimit, Mathf.Min(minValue, maxValue));
            this.maxValue = Mathf.Min(maxValueLimit, Mathf.Max(minValue, maxValue));
            this.value = Mathf.Clamp(value, this.minValue, this.maxValue);
        }

        //=====================================================================

        public FloatMinMax(float value, float minValue, float maxValue)
        {
            this.minValueLimit = Mathf.Min(minValue, maxValue);
            this.maxValueLimit = Mathf.Max(minValue, maxValue);
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.value = Mathf.Clamp(value, this.minValue, this.maxValue);
        }

        //=====================================================================

        public void ValidateData()
        {
            float _value = this.value;
            float _minValue = this.minValue;
            float _maxValue = this.maxValue;
            float _minValueLimit = this.minValueLimit;
            float _maxValueLimit = this.maxValueLimit;

            this.minValueLimit = Mathf.Min(_minValueLimit, _minValue, _maxValue, _maxValueLimit);
            this.maxValueLimit = Mathf.Max(_minValueLimit, _minValue, _maxValue, _maxValueLimit);
            this.minValue = Mathf.Max(_minValueLimit, Mathf.Min(_minValue, _maxValue));
            this.maxValue = Mathf.Min(_maxValueLimit, Mathf.Max(_minValue, _maxValue));
            this.value = Mathf.Clamp(_value, this.minValue, this.maxValue);
        }

        //=====================================================================

        public void SetValue(float value)
        {
            this.value = Mathf.Clamp(value, this.minValue, this.maxValue);
        }

        //=====================================================================

        public float GetClampedValue(float value)
        {
            return Mathf.Clamp(value, this.minValue, this.maxValue);
        }

        //=====================================================================

    }
}