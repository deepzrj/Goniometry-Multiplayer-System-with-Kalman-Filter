using UnityEngine;
using UnityEditor;

namespace MSP_MobileDeviceInput
{
	[CustomPropertyDrawer(typeof(MSP_Input.FloatMinMax))]
	public class FloatMinMaxDrawer : PropertyDrawer
	{
		private const float FieldWidth = 50.0f;
		private const float SliderPadding = 4.0f;
		private const float numberOfLines = 2f;
	
		//------
	
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float propertyHeigth = base.GetPropertyHeight(property, label) * numberOfLines;
			return propertyHeigth;
		}
	
		// Draw the property inside the given rect
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty value = property.FindPropertyRelative("value");
			SerializedProperty minValue = property.FindPropertyRelative("minValue");
			SerializedProperty maxValue = property.FindPropertyRelative("maxValue");
			SerializedProperty minValueLimit = property.FindPropertyRelative("minValueLimit");
			SerializedProperty maxValueLimit = property.FindPropertyRelative("maxValueLimit");
	
			// set property height
			position.height = EditorGUIUtility.singleLineHeight * numberOfLines;
	
			EditorGUI.BeginProperty(position, label, property);
			{
				// Label
				position = EditorGUI.PrefixLabel(position, label);
	
				// Child objects shouldn't be indented
				int indent = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;
	
				// Temp variables
				float _value = value.floatValue;
				float _minValue = minValue.floatValue;
				float _maxValue = maxValue.floatValue;
				float _minValueLimit = minValueLimit.floatValue;
				float _maxValueLimit = maxValueLimit.floatValue;
	
				float singleLineHeight = position.height / numberOfLines;
	
				// Value slider            
				Rect sliderRect = new Rect(position.x + SliderPadding + FieldWidth, position.y, Mathf.Max(0.0f, position.width - FieldWidth - SliderPadding), singleLineHeight);
				_value = EditorGUI.Slider(sliderRect, _value, _minValueLimit, _maxValueLimit);
	
				// MinMax Slider
				Rect minRect = new Rect(position.x, position.y + singleLineHeight, FieldWidth, singleLineHeight);
				Rect sliderMinMaxRect = new Rect(position.x + SliderPadding + FieldWidth, position.y + singleLineHeight, Mathf.Max(0.0f, position.width - FieldWidth * 2.0f - SliderPadding * 2.0f), singleLineHeight);
				Rect maxRect = new Rect(position.x + position.width - FieldWidth, position.y + singleLineHeight, FieldWidth, singleLineHeight);
				EditorGUI.MinMaxSlider(sliderMinMaxRect, ref _minValue, ref _maxValue, _minValueLimit, _maxValueLimit);
				_minValue = EditorGUI.FloatField(minRect, _minValue);
				_maxValue = EditorGUI.FloatField(maxRect, _maxValue);
	
	
				_minValue = Mathf.Clamp(_minValue, _minValueLimit, _maxValue);
				_maxValue = Mathf.Clamp(_maxValue, _minValue, _maxValueLimit);
				_value = Mathf.Clamp(_value, _minValue, _maxValue);
	
				value.floatValue = _value;
				minValue.floatValue = _minValue;
				maxValue.floatValue = _maxValue;
	
				// Reset indenting
				EditorGUI.indentLevel = indent;
			}
			EditorGUI.EndProperty();
		}
	}
}