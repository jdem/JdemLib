using UnityEditor;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace JdemLib.EditorExtensions
{
	using Utility;
	public static class EditorGUIExtended
	{
		static GUIStyle defaultAnchorLabelStyle = new GUIStyle(GUI.skin.label);
		static GUIContent textTrial = new GUIContent();

		public static void AnchorLabelField(Rect position, string text, TextAnchor anchor = TextAnchor.UpperLeft, Color? color = null, GUIStyle style = null)
		{
			if(style == null)
			{
				style = defaultAnchorLabelStyle;
			}
			style.alignment = anchor;
			style.richText = true;

			if(color != null)
			{
				ConvertToRichTextTag.AddColorTag (ref text, color.Value);
				EditorGUI.LabelField(position, text, style);
			}
			else
			{
				EditorGUI.LabelField(position, text, style);
			}
		}

		public static string FileField(Rect position, string label, ref bool foldout, string path = null, string defaultPath = null, string extension = null)
		{
			foldout = EditorGUI.Foldout(position, foldout, label);
			if(foldout)
			{
				textTrial.text = "...";
				Vector2 fileButtonSize = GUI.skin.button.CalcSize(textTrial);

				GUILayout.BeginHorizontal();
				GUILayout.TextArea(path == null ? "" : path, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - fileButtonSize.x));
				if(GUILayout.Button("...", GUILayout.Width(fileButtonSize.x)))
				{
					path = EditorUtility.OpenFilePanel(label, 
					                                   defaultPath == null ? Application.dataPath : defaultPath,
					                                   extension == null ? "" : extension);
				}
				GUILayout.EndHorizontal();

                if(GUILayout.Button("Clear"))
                {
                    path = null;
                }
            }
			return path;
		}

		public static void RulerField(Rect position, int graduation = 100, Color? lineColor = null, float current = 0f, Color? currentColor = null)
		{
			if(Event.current.type != EventType.Repaint)
			{
				return;
			}
			
			HandleUtilityWrapper.handleWireMaterial.SetPass(0);
			GL.Color(lineColor == null ? Color.white : lineColor.Value);
			
			GL.Begin(GL.LINES);
			{
				// Top line
				GL.Vertex3(position.x, position.y, 0f);
				GL.Vertex3(position.x + position.width, position.y, 0f);

				// Buttom line
				GL.Vertex3(position.x, position.y + position.height, 0f);
				GL.Vertex3(position.x + position.width, position.y + position.height, 0f);

				// Graduation
				for(int i = 0; i <= graduation; ++i) 
				{
					if(i % 10 == 0) 
					{
						GL.Vertex3(position.x + position.width*i/graduation, position.y, 0f);
						GL.Vertex3(position.x + position.width*i/graduation, position.y + position.height, 0f);
					}
					else if(i % 5 == 0)
					{
						GL.Vertex3(position.x + position.width*i/graduation, position.y, 0f);
						GL.Vertex3(position.x + position.width*i/graduation, position.y + position.height*0.6f, 0f);
					}
					else 
					{
						GL.Vertex3(position.x + position.width*i/graduation, position.y, 0f);
						GL.Vertex3(position.x + position.width*i/graduation, position.y + position.height*0.3f, 0f);
					}
				}

				// Current line
				if(currentColor != null)
				{
					current = Mathf.Clamp(current, 0f, (float)graduation);
					GL.Color(currentColor.Value);
					GL.Vertex3(position.x + position.width*current/graduation, position.y, 0f);
					GL.Vertex3(position.x + position.width*current/graduation, position.y + position.height, 0f);
				}
			}
			GL.End();
		}
	}

	public static class EditorGUILayoutExtended
	{
		static GUIStyle defaultTextStyle = new GUIStyle(GUI.skin.label);
		static GUIContent textTrial = new GUIContent();
		static SerializedObject serializedObj;

		public static void LabelField(string text, Color? color = null, GUIStyle style = null)
		{ 
			if(style == null)
			{
				style = defaultTextStyle;
			}
			style.richText = true;

			textTrial.text = text;
			float trialWidth = GUI.skin.label.CalcSize(textTrial).x;

			if(color != null)
			{
				ConvertToRichTextTag.AddColorTag (ref text, color.Value);
				EditorGUILayout.LabelField(text, style, GUILayout.Width(trialWidth));
			}
			else
			{
				EditorGUILayout.LabelField(text, style, GUILayout.Width(trialWidth));
			}
		}

		public static string FileField(string label, ref bool foldout, string path = null, string defaultPath = null, string extension = null)
		{
			foldout = EditorGUILayout.Foldout(foldout, label);
			if(foldout)
			{
				textTrial.text = "...";
				Vector2 fileButtonSize = GUI.skin.button.CalcSize(textTrial);
				
				GUILayout.BeginHorizontal();
				GUILayout.TextField(path == null ? "" : path, GUILayout.MaxWidth(Screen.width - fileButtonSize.x));
				if(GUILayout.Button("...", GUILayout.MaxWidth(fileButtonSize.x)))
				{
					path = EditorUtility.OpenFilePanel(label, 
					                                   defaultPath == null ? "" : defaultPath,
					                                   extension == null ? "" : extension);
				}
				GUILayout.EndHorizontal();
			}
			return path;
		}

		static void ReadWritePropertyField(FieldInfo field, object fieldHost)
		{
			UnityEngine.Object fieldHostObj = fieldHost as UnityEngine.Object;
			if(serializedObj == null || serializedObj.targetObject != fieldHostObj)
			{
				serializedObj = new SerializedObject(fieldHostObj);
			}
			
			if(serializedObj != null)
			{
				SerializedProperty prop = serializedObj.FindProperty(field.Name);
				EditorGUILayout.PropertyField(prop, true);
				serializedObj.ApplyModifiedProperties();
			}
		}

		public static void ArrayField(FieldInfo field, object fieldHost)
		{
			ReadWritePropertyField(field, fieldHost);
		}

		public static void CustomPropertyField(FieldInfo field, object fieldHost)
		{
			Attribute[] attribs = field.GetCustomAttributes(true) as Attribute[];
			if(attribs.Any(attrib => attrib is PropertyAttribute))
			{
				ReadWritePropertyField(field, fieldHost);
			}
		}

		static void ReadWriteField<T>(FieldInfo field, object fieldHost, Func<string, T, T> action)
		{
			T originalValue = (T)field.GetValue(fieldHost);
			T newValue = action(field.Name.ToUnityStyle(), originalValue);
			if(originalValue == null && newValue != null || originalValue != null && !originalValue.Equals(newValue))
			{
				field.SetValue(fieldHost, newValue);
				EditorUtility.SetDirty(fieldHost as UnityEngine.Object);
			}
		}

		public static void AutoField(FieldInfo field, object fieldHost)
		{
			// Array
			if(field.FieldType.IsArray)
			{
				ArrayField(field, fieldHost);
				return;
			}

			// Range
			RangeAttribute[] ranges = field.GetCustomAttributes(typeof(RangeAttribute), true) as RangeAttribute[];
			if(ranges.Length != 0)
			{
				RangeAttribute range = ranges[0];

				if(field.FieldType.Name == typeof(int).Name)
				{
					ReadWriteField<int>(field, fieldHost, (labelName, value) => { return EditorGUILayout.IntSlider(labelName, value, (int)range.min, (int)range.max); });
				}
				else if(field.FieldType.Name == typeof(float).Name || field.FieldType.Name == typeof(double).Name)
				{
					ReadWriteField<float>(field, fieldHost, (labelName, value) => { return EditorGUILayout.Slider(labelName, value, range.min, range.max); });
				}

				return;
			}

            // FileSelector
            FileSelectorAttribute[] fileSelectors = field.GetCustomAttributes(typeof(FileSelectorAttribute), true) as FileSelectorAttribute[];
            if(fileSelectors.Length != 0)
            {
                ReadWritePropertyField(field, fieldHost);
                return;
            }

            // Button
            ButtonAttribute[] buttons = field.GetCustomAttributes(typeof(ButtonAttribute), true) as ButtonAttribute[];
            if(buttons.Length != 0)
            {
                ReadWritePropertyField(field, fieldHost);
                return;
            }

            // Label
            LabelAttribute[] labels = field.GetCustomAttributes(typeof(LabelAttribute), true) as LabelAttribute[];
            if(labels.Length != 0)
            {
                ReadWriteField<string>(field, fieldHost, (labelName, value) => { EditorGUILayout.LabelField(labelName, value); return value; });
                return;
            }

            // Enum
            if(field.FieldType.IsEnum)
			{
				ReadWriteField<System.Enum>(field, fieldHost, (labelName, value) => { return EditorGUILayout.EnumPopup(labelName, value); });
			}
			// Bool
			if(field.FieldType.Name == typeof(bool).Name)
			{
				ReadWriteField<bool>(field, fieldHost, (labelName, value) => { return EditorGUILayout.Toggle(labelName, value); });
			}
			// Int
			else if(field.FieldType.Name == typeof(int).Name)
			{
				ReadWriteField<int>(field, fieldHost, (labelName, value) => { return EditorGUILayout.IntField(labelName, value); });
			}
			// Float
			else if(field.FieldType.Name == typeof(float).Name)
			{
				ReadWriteField<float>(field, fieldHost, (labelName, value) => { return EditorGUILayout.FloatField(labelName, value); });
			}
			// Double
			else if(field.FieldType.Name == typeof(double).Name)
			{
				ReadWriteField<double>(field, fieldHost, (labelName, value) => { return EditorGUILayout.DoubleField(labelName, value); });
			}
			// String
			else if(field.FieldType.Name == typeof(string).Name)
			{
				ReadWriteField<string>(field, fieldHost, (labelName, value) => { return EditorGUILayout.TextField(labelName, value); });
			}
			// Vector2
			else if(field.FieldType.Name == typeof(Vector2).Name)
			{
				ReadWriteField<Vector2>(field, fieldHost, (labelName, value) => { return EditorGUILayout.Vector2Field(labelName, value); });
			}
			// Vector3
			else if(field.FieldType.Name == typeof(Vector3).Name)
			{
				ReadWriteField<Vector3>(field, fieldHost, (labelName, value) => { return EditorGUILayout.Vector3Field(labelName, value); });
			}
			// Vector4
			else if(field.FieldType.Name == typeof(Vector4).Name)
			{
				ReadWriteField<Vector4>(field, fieldHost, (labelName, value) => { return EditorGUILayout.Vector4Field(labelName, value); });
			}
			// Color
			else if(field.FieldType.Name == typeof(Color).Name || field.FieldType.Name == typeof(Color32).Name)
			{
				ReadWriteField<Color>(field, fieldHost, (labelName, value) => { return EditorGUILayout.ColorField(labelName, value); });
			}
			// Curve
			else if(field.FieldType.Name == typeof(AnimationCurve).Name)
			{
				ReadWritePropertyField(field, fieldHost);
			}
			// GameObject
			else if(field.FieldType.Name == typeof(GameObject).Name)
			{
				ReadWritePropertyField(field, fieldHost);
			}
			// Component
			else if(field.FieldType.IsAssignableFrom(typeof(Component)))
			{
				ReadWritePropertyField(field, fieldHost);
			}
			// Custom Property Drawer
			else
			{
				CustomPropertyField(field, fieldHost);
			}
		}

		public static void BottomLayout()
		{
			// Use an empty scroll view to bottom rest gui
			EditorGUILayout.BeginScrollView(Vector2.zero);
			EditorGUILayout.EndScrollView();
		}
	}
}