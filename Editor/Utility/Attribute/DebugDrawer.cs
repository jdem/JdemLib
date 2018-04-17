using UnityEngine;
using UnityEditor;

namespace JdemLib.Attribute
{
	[CustomPropertyDrawer (typeof (DebugAttribute))]
	public class DebugDrawer : PropertyDrawer
	{
		// Necessary since some properties tend to collapse smaller than their content
		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return 0f;
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			//EditorGUI.PropertyField(position, property, label, true);
		}
	}
}