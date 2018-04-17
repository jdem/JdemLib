using UnityEditor;
using UnityEngine;

namespace JdemLib.Attribute
{
	[CustomPropertyDrawer (typeof (LayerAttribute))]
	public class LayerDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginChangeCheck ();
			int value = EditorGUILayout.LayerField (label, prop.intValue);
			if (EditorGUI.EndChangeCheck ())
			{
				prop.intValue = value;
			}
		}
	}
}