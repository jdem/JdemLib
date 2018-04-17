using UnityEngine;
using UnityEditor;

namespace JdemLib.Attribute
{
	[CustomPropertyDrawer (typeof (MinMaxSliderAttribute))]
	class MinMaxSliderDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.Vector2)
			{
				Vector2 range = property.vector2Value;
				float min = range.x;
				float max = range.y;
				MinMaxSliderAttribute attr = attribute as MinMaxSliderAttribute;

				float componentHeight = 18.0f;
				float verticalPadding = 1.0f;

				label = EditorGUI.BeginProperty (position, label, property);

				Rect sliderRect = new Rect (position.x,
										   position.y,
										   position.width,
										   componentHeight);

				Rect minimumValueRect = EditorGUI.PrefixLabel (sliderRect, label);
				minimumValueRect.y += componentHeight + verticalPadding;

				Rect maximumValueRect = new Rect (minimumValueRect.x,
									  minimumValueRect.y + componentHeight + verticalPadding,
									  minimumValueRect.width,
									  componentHeight);

				EditorGUI.BeginChangeCheck ();
				EditorGUI.MinMaxSlider (label, sliderRect, ref min, ref max, attr.min, attr.max);

				min = EditorGUI.FloatField (minimumValueRect, "Minimum", min);
				max = EditorGUI.FloatField (maximumValueRect, "Maximum", max);

				if (EditorGUI.EndChangeCheck ())
				{
					range.x = min;
					range.y = max;
					property.vector2Value = range;
				}

				EditorGUI.EndProperty ();
			}
			else
			{
				EditorGUI.LabelField (position, label, "Use only with Vector2");
			}
		}

		override public float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight (property, label) + 18.0f + 18.0f;
		}
	}
}