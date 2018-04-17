using UnityEngine;
using UnityEditor;

namespace JdemLib.Attribute
{
	[CustomPropertyDrawer (typeof (DisableWithBoolAttribute))]
	public class DisableWithBoolDrawer : PropertyDrawer
	{
		SerializedProperty boolProperty;

		private void CatchProperty (DisableWithBoolAttribute disableAttribute, SerializedProperty property)
		{
			int propertyBasePathFinalIndex = property.propertyPath.LastIndexOf (".") + 1;
			string propertyPath = property.propertyPath.Substring (0, propertyBasePathFinalIndex) + disableAttribute.BoolProperty;
			boolProperty = property.serializedObject.FindProperty (propertyPath);

			if (boolProperty != null)
				return;

			boolProperty = property.serializedObject.FindProperty (disableAttribute.BoolProperty);
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight (property, label, true);
		}
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			var disableAttribute = attribute as DisableWithBoolAttribute;
			CatchProperty (disableAttribute, property);
			if (boolProperty != null)
			{
				if (boolProperty.propertyType == SerializedPropertyType.Boolean)
				{
					bool guienable = GUI.enabled;

					if (boolProperty.boolValue == disableAttribute.InverseProperty)
						GUI.enabled = false;

					// Get the attributes
					RangeAttribute[] attribs = this.fieldInfo.GetCustomAttributes (
							typeof (RangeAttribute), false) as RangeAttribute[];
					if (attribs.Length > 0)
					{
						if (property.propertyType == SerializedPropertyType.Integer)
							property.intValue = EditorGUI.IntSlider (position, label, property.intValue, (int)attribs[0].min, (int)attribs[0].max);
						else
							property.floatValue = EditorGUI.Slider (position, label, property.floatValue, attribs[0].min, attribs[0].max);
					}
					else
					{
						EditorGUI.PropertyField (position, property, label, true);
					}
					GUI.enabled = guienable;
				}
				else
				{
					EditorGUI.HelpBox (position, string.Format ("Property {0} is not boolean", disableAttribute.BoolProperty), MessageType.Error);
				}
			}
			else
			{
				EditorGUI.HelpBox (position, string.Format ("Couldn't find property {0}", disableAttribute.BoolProperty), MessageType.Error);
			}
		}
	}
}