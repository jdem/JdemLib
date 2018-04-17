using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace JdemLib.Attribute
{
	using EditorExtensions;
	[CustomPropertyDrawer (typeof (FileSelectorAttribute))]
	public class FileSelectorDrawer : PropertyDrawer
	{
		bool isInit = false;
		bool foldout = false;

		string prePath;
		string path;

		Action onChanged;

		public static bool isExistedFileSelectorGUIDDirty = false;
		static Dictionary<string, Dictionary<string, object>> existedFileSelectorProperty;
		static Dictionary<string, List<string>> existedFileSelectorGUID;

		static void ReadMemoFromSettings ()
		{
			existedFileSelectorProperty = CustomEditorSetting.ReadAllSettings ("FileSeletorMemo");

			existedFileSelectorGUID = new Dictionary<string, List<string>> ();
			foreach (KeyValuePair<string, Dictionary<string, object>> kv in existedFileSelectorProperty)
			{
				Dictionary<string, object> guidMap = kv.Value;
				if (guidMap == null)
				{
					continue;
				}

				foreach (KeyValuePair<string, object> kv2 in guidMap)
				{
					string guid = kv2.Key;

					if (!existedFileSelectorGUID.ContainsKey (guid))
					{
						existedFileSelectorGUID.Add (guid, new List<string> ());
					}

					existedFileSelectorGUID[guid].Add (kv.Key);
				}
			}
		}

		public static Dictionary<string, Dictionary<string, object>> ExistedFileSelectorProperty
		{
			get
			{
				if (existedFileSelectorProperty == null)
				{
					ReadMemoFromSettings ();
				}

				return existedFileSelectorProperty;
			}
		}

		public static Dictionary<string, List<string>> ExistedFileSelectorGUID
		{
			get
			{
				if (existedFileSelectorGUID == null)
				{
					ReadMemoFromSettings ();
				}

				return existedFileSelectorGUID;
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight (property, label);
		}

		void OnInit (SerializedProperty property)
		{
			if (property.type != "string")
			{
				return;
			}

			prePath = path = AssetDatabase.GUIDToAssetPath (property.stringValue);

			FileSelectorAttribute fileSelectorAttribute = (FileSelectorAttribute)attribute;
			if (!string.IsNullOrEmpty (fileSelectorAttribute.onChanged))
			{
				try
				{
					onChanged = (Action)Delegate.CreateDelegate (typeof (Action), property.serializedObject.targetObject, fileSelectorAttribute.onChanged);
				}
				catch
				{
					onChanged = null;
				}
			}
		}

		void AddGUIDToMemo (string guid, UnityEngine.Object propertyObject)
		{
			string propertyGUID = AssetDatabase.AssetPathToGUID (AssetDatabase.GetAssetPath (propertyObject));

			if (!ExistedFileSelectorProperty.ContainsKey (propertyGUID))
			{
				Dictionary<string, object> newGUIDMap = new Dictionary<string, object> ();
				newGUIDMap.Add (guid, (int)1);
				ExistedFileSelectorProperty.Add (propertyGUID, newGUIDMap);
			}
			else
			{
				Dictionary<string, object> guidMap = ExistedFileSelectorProperty[propertyGUID];
				if (!guidMap.ContainsKey (guid))
				{
					guidMap.Add (guid, (int)1);
				}
				else
				{
					guidMap[guid] = Convert.ToInt32 (guidMap[guid].ToString ()) + 1;
				}
			}

			if (!ExistedFileSelectorGUID.ContainsKey (guid))
			{
				ExistedFileSelectorGUID.Add (guid, new List<string> ());
			}
			ExistedFileSelectorGUID[guid].Add (propertyGUID);

			isExistedFileSelectorGUIDDirty = true;
		}

		void RemoveGUIDFromMemo (string guid, UnityEngine.Object propertyObject)
		{
			if (!ExistedFileSelectorGUID.ContainsKey (guid))
			{
				return;
			}

			string propertyGUID = AssetDatabase.AssetPathToGUID (AssetDatabase.GetAssetPath (propertyObject));

			Dictionary<string, object> guidMap = ExistedFileSelectorProperty[propertyGUID];
			int guidNum = Convert.ToInt32 (guidMap[guid].ToString ()) - 1;
			if (guidNum > 0)
			{
				guidMap[guid] = guidNum;
			}
			else
			{
				guidMap.Remove (guid);
				if (guidMap.Count == 0)
				{
					ExistedFileSelectorProperty.Remove (propertyGUID);
				}
			}

			ExistedFileSelectorGUID[guid].Remove (propertyGUID);
			if (ExistedFileSelectorGUID[guid].Count <= 0)
			{
				ExistedFileSelectorGUID.Remove (guid);
			}

			isExistedFileSelectorGUIDDirty = true;
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			if (!isInit)
			{
				OnInit (property);
				isInit = true;
			}

			if (property.type != "string")
			{
				EditorGUIExtended.AnchorLabelField (position, label.text.ToUnityStyle () + ": File Selector Attribute can only be used to string field!", TextAnchor.UpperLeft, Color.red);
				return;
			}

			FileSelectorAttribute fileSelectorAttribute = (FileSelectorAttribute)attribute;

			path = EditorGUIExtended.FileField (position, label.text.ToUnityStyle (), ref foldout, path, string.IsNullOrEmpty (path) ? fileSelectorAttribute.defaultPath : Path.GetDirectoryName (path), fileSelectorAttribute.extension);

			string relativedPath = path == null ? "" : "Assets/" + path.TrimStart (Application.dataPath.ToCharArray ());
			bool skipRelativedPathCheck = false;
			if (GUILayout.Button ("Refresh"))
			{
				skipRelativedPathCheck = true;
			}
			if (skipRelativedPathCheck == false && relativedPath == prePath)
			{
				return;
			}

			UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath (relativedPath, typeof (UnityEngine.Object));
			// Select a valid prefab
			if (obj != null)
			{
				property.stringValue = AssetDatabase.AssetPathToGUID (relativedPath);
				property.serializedObject.ApplyModifiedProperties ();

				// Modify GUID memo
				if (!string.IsNullOrEmpty (prePath))
				{
					RemoveGUIDFromMemo (AssetDatabase.AssetPathToGUID (prePath), property.serializedObject.targetObject);
				}
				AddGUIDToMemo (AssetDatabase.AssetPathToGUID (relativedPath), property.serializedObject.targetObject);

				prePath = path = relativedPath;

				// Notify variable is changed
				if (onChanged != null)
				{
					onChanged ();
				}
			}
			// Select an invalid prefab
			else if (path != null)
			{
				if (path != "")
				{
					EditorUtility.DisplayDialog ("Error!", "Selected file [" + path + "] is invalid!", "OK");
				}
				path = prePath;
			}
			// Clear the path
			else if (path == null && !string.IsNullOrEmpty (prePath))
			{
				property.stringValue = "";
				property.serializedObject.ApplyModifiedProperties ();

				RemoveGUIDFromMemo (AssetDatabase.AssetPathToGUID (prePath), property.serializedObject.targetObject);
				prePath = path;

				// Notify variable is changed
				if (onChanged != null)
				{
					onChanged ();
				}
			}
		}
	}

	public class FileSelectorOnChanged : UnityEditor.AssetPostprocessor
	{
		static void NotifyPropertyChanged (Type propertyType, object host, string deletedGUID = null)
		{
			MemberInfo[] members = propertyType.GetMembers (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (MemberInfo member in members.Where (member => member.GetCustomAttributes (typeof (FileSelectorAttribute), true).Length != 0))
			{
				if (member.MemberType != MemberTypes.Field)
				{
					continue;
				}

				foreach (FileSelectorAttribute fileAttribute in member.GetCustomAttributes (typeof (FileSelectorAttribute), true) as FileSelectorAttribute[])
				{
					if (string.IsNullOrEmpty (fileAttribute.onChanged))
					{
						continue;
					}

					if (!string.IsNullOrEmpty (deletedGUID))
					{
						FieldInfo field = (FieldInfo)member;
						string guid = (string)field.GetValue (host);
						if (guid == deletedGUID)
						{
							field.SetValue (host, "");
						}
					}

					MethodInfo method = propertyType.GetMethod (fileAttribute.onChanged, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					if (method != null)
					{
						method.Invoke (host, null);
					}
				}
			}
		}

		static void NotifyPropertyChanged (UnityEngine.Object propertyAsset, string deletedGUID = null)
		{
			if (propertyAsset == null)
			{
				return;
			}

			Type assetType = propertyAsset.GetType ();
			if (assetType == typeof (AnimatorController))
			{
				AnimatorController controller = propertyAsset as AnimatorController;
				StateMachineBehaviour[] behaviours = controller.GetBehaviours<StateMachineBehaviour> ();

				foreach (StateMachineBehaviour behaviour in behaviours)
				{
					NotifyPropertyChanged (behaviour.GetType (), behaviour, deletedGUID);
				}
			}
			else if (assetType == typeof (GameObject))
			{
				GameObject gobj = propertyAsset as GameObject;
				Component[] comps = gobj.GetComponentsInChildren<Component> (true);

				foreach (Component comp in comps)
				{
					NotifyPropertyChanged (comp.GetType (), comp, deletedGUID);
				}
			}
		}

		public static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			Dictionary<string, Dictionary<string, object>> existedFileSelectorProperty = FileSelectorDrawer.ExistedFileSelectorProperty;
			Dictionary<string, List<string>> existedFileSelectorGUID = FileSelectorDrawer.ExistedFileSelectorGUID;

			// Remove deleted properties
			foreach (string deletedProperty in deletedAssets)
			{
				string deletedPropertyGUID = AssetDatabase.AssetPathToGUID (deletedProperty);
				if (existedFileSelectorProperty.ContainsKey (deletedPropertyGUID))
				{
					existedFileSelectorProperty.Remove (deletedPropertyGUID);
					existedFileSelectorGUID[deletedPropertyGUID].RemoveAll (guid => guid == deletedPropertyGUID);
				}
			}

			// Remove deleted GUID
			foreach (string deletedPath in deletedAssets)
			{
				string deletedGUID = AssetDatabase.AssetPathToGUID (deletedPath);
				if (existedFileSelectorGUID.ContainsKey (deletedGUID))
				{
					foreach (string property in existedFileSelectorGUID[deletedGUID])
					{
						existedFileSelectorProperty[property].Remove (deletedGUID);

						UnityEngine.Object propertyAsset = AssetDatabase.LoadAssetAtPath (AssetDatabase.GUIDToAssetPath (property), typeof (UnityEngine.Object));
						NotifyPropertyChanged (propertyAsset, deletedGUID);
					}
					existedFileSelectorGUID.Remove (deletedGUID);
				}
			}

			// Notify the properties are changed
			foreach (string propertyPath in movedAssets.Where (path => existedFileSelectorGUID.ContainsKey (AssetDatabase.AssetPathToGUID (path)))
													  .Select (path => existedFileSelectorGUID[AssetDatabase.AssetPathToGUID (path)])
													  .ToList ()
													  .SelectMany (propertyGUID => propertyGUID.OfType<string> ())
													  .Select (propertyGUID => AssetDatabase.GUIDToAssetPath (propertyGUID))
													  )
			{
				UnityEngine.Object propertyAsset = AssetDatabase.LoadAssetAtPath (propertyPath, typeof (UnityEngine.Object));
				NotifyPropertyChanged (propertyAsset);
			}
		}
	}

	public class FileSelectorOnSaved : UnityEditor.AssetModificationProcessor
	{
		static void CheckPropertyConsistency (Type propertyType, object host, Dictionary<string, object> checkedGUIDList)
		{
			MemberInfo[] members = propertyType.GetMembers (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (MemberInfo member in members.Where (member => member.GetCustomAttributes (typeof (FileSelectorAttribute), true).Length != 0))
			{
				if (member.MemberType != MemberTypes.Field)
				{
					continue;
				}

				FileSelectorAttribute[] fileAttributes = member.GetCustomAttributes (typeof (FileSelectorAttribute), true) as FileSelectorAttribute[];
				for (int i = 0; i < fileAttributes.Length; ++i)
				{
					FieldInfo field = (FieldInfo)member;
					string guid = (string)field.GetValue (host);
					if (string.IsNullOrEmpty (guid))
					{
						continue;
					}

					if (!checkedGUIDList.ContainsKey (guid))
					{
						checkedGUIDList.Add (guid, (int)1);
					}
					else
					{
						checkedGUIDList[guid] = Convert.ToInt32 (checkedGUIDList[guid].ToString ()) + 1;
					}
				}
			}
		}

		static void CheckPropertyConsistency (string changedProperty, Dictionary<string, object> guidList)
		{
			UnityEngine.Object propertyAsset = AssetDatabase.LoadAssetAtPath (changedProperty, typeof (UnityEngine.Object));
			if (propertyAsset == null)
			{
				return;
			}

			Dictionary<string, object> checkedGUIDList = new Dictionary<string, object> ();
			Type assetType = propertyAsset.GetType ();
			if (assetType == typeof (AnimatorController))
			{
				AnimatorController controller = propertyAsset as AnimatorController;
				StateMachineBehaviour[] behaviours = controller.GetBehaviours<StateMachineBehaviour> ();

				foreach (StateMachineBehaviour behaviour in behaviours)
				{
					CheckPropertyConsistency (behaviour.GetType (), behaviour, checkedGUIDList);
				}
			}
			else if (assetType == typeof (GameObject))
			{
				GameObject gobj = propertyAsset as GameObject;
				Component[] comps = gobj.GetComponentsInChildren<Component> (true);

				foreach (Component comp in comps)
				{
					CheckPropertyConsistency (comp.GetType (), comp, checkedGUIDList);
				}
			}

			List<string> deletedKeys = new List<string> ();
			foreach (KeyValuePair<string, object> kv in guidList)
			{
				if (!checkedGUIDList.ContainsKey (kv.Key))
				{
					deletedKeys.Add (kv.Key);
				}
				else if (Convert.ToInt32 (checkedGUIDList[kv.Key].ToString ()) != Convert.ToInt32 (kv.Value.ToString ()))
				{
					guidList[kv.Key] = Convert.ToInt32 (checkedGUIDList[kv.Key].ToString ());
				}
			}
			foreach (string deletedKey in deletedKeys)
			{
				guidList.Remove (deletedKey);
			}
		}

		public static string[] OnWillSaveAssets (string[] paths)
		{
			// Sync GUID memo
			Dictionary<string, Dictionary<string, object>> existedFileSelectorProperty = FileSelectorDrawer.ExistedFileSelectorProperty;
			foreach (KeyValuePair<string, Dictionary<string, object>> kv in existedFileSelectorProperty)
			{
				string guidPath = AssetDatabase.GUIDToAssetPath (kv.Key);
				if (paths.Contains (guidPath))
				{
					CheckPropertyConsistency (guidPath, kv.Value);
				}
			}

			if (FileSelectorDrawer.isExistedFileSelectorGUIDDirty)
			{
				CustomEditorSetting.ClearSettings ("FileSeletorMemo", true, null, existedFileSelectorProperty.Select (kv => kv.Key).ToList ());
				foreach (KeyValuePair<string, Dictionary<string, object>> kv in existedFileSelectorProperty)
				{
					string settingPath = "FileSeletorMemo/" + kv.Key;
					CustomEditorSetting.WriteSetting (settingPath, kv.Value);
				}
				AssetDatabase.Refresh ();
				FileSelectorDrawer.isExistedFileSelectorGUIDDirty = false;
			}

			return paths;
		}
	}
}