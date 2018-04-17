using UnityEngine;
using UnityEditor;

namespace JdemLib.Attribute
{
	[CustomPropertyDrawer (typeof (HideWithBoolAttribute))]
	public class HideWithBoolDrawer : PropertyDrawer
	{
		SerializedProperty boolProperty;

		private void CatchProperty (HideWithBoolAttribute hideAttribute, SerializedProperty property)
		{
			int propertyBasePathFinalIndex = property.propertyPath.LastIndexOf (".") + 1;
			string propertyPath = property.propertyPath.Substring (0, propertyBasePathFinalIndex) + hideAttribute.BoolProperty;
			boolProperty = property.serializedObject.FindProperty (propertyPath);

			if (boolProperty != null)
				return;

			boolProperty = property.serializedObject.FindProperty (hideAttribute.BoolProperty);
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			var hideAttribute = attribute as HideWithBoolAttribute;
			CatchProperty (hideAttribute, property);
			if (boolProperty != null && boolProperty.propertyType == SerializedPropertyType.Boolean && (boolProperty.boolValue == hideAttribute.InverseProperty))
			{
				return 0;
			}
			else
				return EditorGUI.GetPropertyHeight (property, label, true);
		}
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			var hideAttribute = attribute as HideWithBoolAttribute;
			CatchProperty (hideAttribute, property);
			if (boolProperty != null)
			{
				if (boolProperty.propertyType == SerializedPropertyType.Boolean)
				{
					if (boolProperty.boolValue != hideAttribute.InverseProperty)
					{
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
					}
				}
				else
				{
					EditorGUI.HelpBox (position, string.Format ("Property {0} is not boolean", hideAttribute.BoolProperty), MessageType.Error);
				}
			}
			else
			{
				EditorGUI.HelpBox (position, string.Format ("Couldn't find property {0}", hideAttribute.BoolProperty), MessageType.Error);
			}
		}
	}
}