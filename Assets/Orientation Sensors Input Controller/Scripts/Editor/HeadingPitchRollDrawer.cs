using UnityEditor;
using UnityEngine;

namespace MSP_Input
{
	[CustomPropertyDrawer(typeof(MSP_Input.HeadingPitchRoll))]
	public class OrientationDrawer : PropertyDrawer
	{
	    // Draw the property inside the given rect
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	    {
	        EditorGUI.BeginProperty(position, label, property);
	        {
	            //bool GUIenabledCache = GUI.enabled;
	            //GUI.enabled = false;
	
	            // Draw label
	            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
	
	            // Don't make child fields be indented
	            int indentCache = EditorGUI.indentLevel;
	            EditorGUI.indentLevel = 0;
	
	            // Calculate rects
	
	            float width = position.width/3;
	            Rect rect;
	
	            rect = new Rect(position.x, position.y, 16, position.height);
	            EditorGUI.LabelField(rect, new GUIContent("H: ", "Heading"));
	   
	            rect = new Rect(position.x+ 16, position.y, width-16, position.height);
	            EditorGUI.PropertyField(rect, property.FindPropertyRelative("heading"), GUIContent.none);
	
	            rect = new Rect(position.x+width, position.y, 16, position.height);
	            EditorGUI.LabelField(rect, new GUIContent("P: ", "Pitch"));
	
	            rect = new Rect(position.x + width + 16, position.y, width - 16, position.height);
	            EditorGUI.PropertyField(rect, property.FindPropertyRelative("pitch"), GUIContent.none);
	
	            rect = new Rect(position.x + 2*width, position.y, 16, position.height);
	            EditorGUI.LabelField(rect, new GUIContent("R: ", "Roll"));
	
	            rect = new Rect(position.x + 2*width + 16, position.y, width - 16, position.height);
	            EditorGUI.PropertyField(rect, property.FindPropertyRelative("roll"), GUIContent.none);
	
	            // Set indent back to what it was
	            EditorGUI.indentLevel = indentCache;
	
	            //GUI.enabled = GUIenabledCache;
	        }
	        EditorGUI.EndProperty();
	    }
	}
}