using UnityEditor;
using UnityEngine;

namespace JdemLib.Attribute
{
	[CustomPropertyDrawer (typeof (TagsAttribute))]
	public class TagsDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginChangeCheck ();
			string value = EditorGUILayout.TagField (label, prop.stringValue);
			if (EditorGUI.EndChangeCheck ())
			{
				prop.stringValue = value;
			}
		}
	}
}