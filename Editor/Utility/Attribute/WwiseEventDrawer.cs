using UnityEditor;
using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

namespace JdemLib.Attribute
{
	[CustomPropertyDrawer (typeof (WwiseEventAttribute))]
	public class WwiseEventDrawer : PropertyDrawer
	{
		public class WwiseEventInfo
		{
			public SerializedProperty eventIDProperty;
			public SerializedProperty[] guidProperty;

			protected int defaultEventID = 0;
			public int DefaultEventID
			{
				set
				{
					if (eventIDProperty == null)
						return;

					defaultEventID = value;

					if (eventIDProperty.intValue == -1)
					{
						SetEventGuid (eventIDProperty, guidProperty, defaultEventID);
					}
					else
					{
						SetEventGuid (eventIDProperty, guidProperty, eventIDProperty.intValue);
					}
				}
				get
				{
					return defaultEventID;
				}
			}

			public bool allowEmpty;

			public bool buttonWasPressed = false;
			public Rect pickerPos = new Rect ();

			public Rect eventIDGUIPosition = new Rect ();
		}

		protected string typeName = "Event";

		//protected List<SerializedProperty[]> guidPropertys = new List<SerializedProperty[]> ();

		//private bool buttonWasPressed = false;
		//private Rect pickerPos = new Rect ();

		protected List<WwiseEventInfo> wwiseEventInfo = new List<WwiseEventInfo> ();

		private List<SerializedProperty> eventIDs = new List<SerializedProperty> ();

		void OnInit (SerializedProperty property)
		{
			if (property.type != "int")
			{
				return;
			}
			WwiseEventAttribute wwiseEventAttribute = attribute as WwiseEventAttribute;

			eventIDs.Add (property);
			WwiseEventInfo eventInfo = new WwiseEventInfo ();
			eventInfo.eventIDProperty = property;
			eventInfo.allowEmpty = wwiseEventAttribute.AllowEmpty;

			SerializedProperty[] tempProperty = new SerializedProperty[1];

			int propertyBasePathFinalIndex = property.propertyPath.LastIndexOf (".") + 1;
			string propertyPath = property.propertyPath.Substring (0, propertyBasePathFinalIndex) + "valueGuid.Array";
			tempProperty[0] = property.serializedObject.FindProperty (propertyPath);

			if (tempProperty[0].arraySize != 16)
			{
				tempProperty[0].arraySize = 16;
			}

			if (tempProperty[0] != null)
			{
				eventInfo.guidProperty = tempProperty;
				eventInfo.DefaultEventID = wwiseEventAttribute.DefaultEventID;

				wwiseEventInfo.Add (eventInfo);
				return;
			}

			tempProperty[0] = property.serializedObject.FindProperty ("valueGuid.Array");
			eventInfo.guidProperty = tempProperty;
			eventInfo.DefaultEventID = wwiseEventAttribute.DefaultEventID;

			wwiseEventInfo.Add (eventInfo);
		}

		int GetEventIDIndex (SerializedProperty eventID)
		{
			for (int i = 0; i < eventIDs.Count; ++i)
			{
				if (eventID.propertyPath == eventIDs[i].propertyPath)
					return i;
			}
			return -1;
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			float height = base.GetPropertyHeight (property, label);
			//return height;
			return height * 2.4f + 6f;
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			int eventIDIndex = GetEventIDIndex (property);
			if (eventIDIndex == -1)
			{
				OnInit (property);
			}

			Rect newPosition = new Rect (position);
			newPosition.y = position.y + 3f;

			property.serializedObject.ApplyModifiedProperties ();

			/********************************************Draw GUI***************************************************************/
			eventIDIndex = (eventIDIndex == -1) ? 0 : eventIDIndex;

			WwiseEventInfo eventInfo;
			eventInfo = wwiseEventInfo[eventIDIndex];
			string componentName = UpdateIds (eventInfo.eventIDProperty, GenEventGuid (eventInfo.guidProperty));

			string labelText = label.text;
			GUI.Label (newPosition, labelText, EditorStyles.boldLabel);

			// draw picker button
			float resetButtonWidth = 50f;
			GUIStyle pickerButtonStyle = new GUIStyle (EditorStyles.toolbarButton);
			pickerButtonStyle.alignment = TextAnchor.MiddleLeft;
			if (componentName.Equals (string.Empty))
			{
				componentName = "No Event is currently selected";
				pickerButtonStyle.normal.textColor = Color.red;
			}

			Rect boxRect = new Rect (newPosition.x - 5f, newPosition.y - 2f, newPosition.width + 10f, base.GetPropertyHeight (property, label) * 2.4f + 2f);
			GUI.Box (boxRect, "", "box");
			Rect pickerButtonPos = new Rect (newPosition.x, newPosition.y + base.GetPropertyHeight (property, label), newPosition.width - resetButtonWidth - 1f, base.GetPropertyHeight (property, label));
			if (GUI.Button (pickerButtonPos, componentName, pickerButtonStyle))
			{
				eventInfo.buttonWasPressed = true;

				// We don't want to set object as dirty only because we clicked the button.
				// It will be set as dirty if the wwise object has been changed by the tree view.
				GUI.changed = false;
			}

			// draw reset button
			bool guienable = GUI.enabled;
			if (componentName.Equals (string.Empty))
			{
				GUI.enabled = false;
			}
			GUIStyle resetButtonStyle = new GUIStyle (EditorStyles.toolbarButton);
			pickerButtonStyle.alignment = TextAnchor.MiddleCenter;
			Rect resetButtonPos = new Rect (pickerButtonPos.x + pickerButtonPos.width + 1f, pickerButtonPos.y, resetButtonWidth, pickerButtonPos.height);
			if (GUI.Button (resetButtonPos, "Reset", resetButtonStyle))
			{
				SetEventGuid (eventInfo.eventIDProperty, eventInfo.guidProperty, (eventInfo.allowEmpty == true) ? 0 : eventInfo.DefaultEventID);
			}

			GUI.enabled = guienable;
			/***********************************************************************************************************************/

			eventInfo.eventIDGUIPosition = pickerButtonPos;

			//GUILayoutUtility.GetLastRect and AkUtilities.GetLastRectAbsolute must be called in repaint mode 
			if (Event.current.type == EventType.Repaint)
			{
				GUILayoutUtility.GetLastRect ();
				AkUtilities.GetLastRectAbsolute ();

				for (int i = 0; i < wwiseEventInfo.Count; ++i)
				{
					eventInfo = wwiseEventInfo[i];
					if (eventInfo.buttonWasPressed)
					{
						Vector2 pickerPos = EditorGUIUtility.GUIToScreenPoint (eventInfo.eventIDGUIPosition.position);
						pickerPos.y = Math.Abs (pickerPos.y);
						pickerPos.y = pickerPos.y % Screen.height;
						eventInfo.pickerPos = new Rect (pickerPos, newPosition.size);
						//EditorApplication.delayCall += () => DelayCreateCall (eventInfo.eventID, i);
						EditorApplication.delayCall += () => AkWwiseComponentPicker.Create (AkWwiseProjectData.WwiseObjectType.EVENT, eventInfo.guidProperty, property.serializedObject, eventInfo.pickerPos);
						eventInfo.buttonWasPressed = false;
						break;
					}
				}
			}

			if (GUI.changed)
			{
				EditorUtility.SetDirty (property.serializedObject.targetObject);
			}
		}

		public static void ResetEventGuid (SerializedProperty[] guidProperty)
		{
			if (guidProperty == null)
				guidProperty = new SerializedProperty[1];

			for (int i = 0; i < guidProperty.Length; i++)
			{
				AkUtilities.SetByteArrayProperty (guidProperty[i], new byte[guidProperty[i].arraySize]);
			}
		}

		public static void SetEventGuid (SerializedProperty eventIDProperty, SerializedProperty[] guidProperty, int eventID)
		{
			bool result = false;
			for (int i = 0; i < AkWwiseProjectInfo.GetData ().EventWwu.Count; i++)
			{
				AkWwiseProjectData.Event e = AkWwiseProjectInfo.GetData ().EventWwu[i].List.Find (x => (x.ID).Equals (eventID));

				if (e != null)
				{
					AkUtilities.SetByteArrayProperty (guidProperty[0], e.Guid);
					eventIDProperty.intValue = e.ID;
					result = true;
					break;
				}
			}
			if (result == false)
			{
				ResetEventGuid (guidProperty);
				eventIDProperty.intValue = 0;
			}
		}

		public static Guid[] GenEventGuid (SerializedProperty[] guidProperty)
		{
			Guid[] componentGuid = new Guid[guidProperty.Length];
			for (int i = 0; i < componentGuid.Length; i++)
			{
				byte[] guidBytes = AkUtilities.GetByteArrayProperty (guidProperty[i]);
				componentGuid[i] = guidBytes == null ? Guid.Empty : new Guid (guidBytes);
			}

			return componentGuid;
		}

		public static string UpdateIds (SerializedProperty property, Guid[] in_guid)
		{
			SerializedProperty eventID = property;
			for (int i = 0; i < AkWwiseProjectInfo.GetData ().EventWwu.Count; i++)
			{
				AkWwiseProjectData.Event e = AkWwiseProjectInfo.GetData ().EventWwu[i].List.Find (x => new Guid (x.Guid).Equals (in_guid[0]));

				if (e != null)
				{
					eventID.intValue = e.ID;
					return e.Name;
				}
			}

			return string.Empty;
		}
	}
}