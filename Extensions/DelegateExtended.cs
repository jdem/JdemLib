using System;
using UnityEngine;

namespace JdemLib.Extensions
{
	using Utility;
	public static class DelegateExtended
	{
		/// <summary>
		/// Checks if a Delegate.Target is null or gameobject has been destroyed.
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static bool Validity (this Delegate callback)
		{
			return CheckValidity (callback);
		}

		/// <summary>
		/// Checks if a Delegate.Target is null or gameobject has been destroyed.
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static bool Contains(this Delegate callback, Delegate other)
		{
			Delegate[] dArray = callback.GetInvocationList();

			for (int i = 0; i < dArray.Length; i++)
			{
				if (dArray[i] == null)
				{
					continue;
				}
				else if (dArray[i].GetType() == other.GetType() && dArray[i].Method == other.Method)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Checks if the Delegate last Target is null or gameobject has been destroyed.
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static bool CheckValidity (Delegate callback)
		{
			if (callback == null)
				return false;

			if (ApplicationStatusSensor.IsQuitting)
				return false;

			object target = callback.Target;

			// Delegate.Target will be null if the callback function is static function
			if (target == null && (callback.Method.Attributes & System.Reflection.MethodAttributes.Static) == System.Reflection.MethodAttributes.Static)
				return true;

			if (target == null)
				return false;

			// UnityEngine overloads the Equals and == opeator for the UnityEngine.Object type
			// and returns null when the object has been destroyed.
			if ((target is UnityEngine.Object) && target.Equals (null))
				return false;

			return true;
		}

		public static bool CheckMethodValidity (Callback callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckMethodValidity<T1> (Callback<T1> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckMethodValidity<T1, T2>(Callback<T1, T2> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckMethodValidity<T1, T2, T3>(Callback<T1, T2, T3> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckMethodValidity<T1, T2, T3, T4>(Callback<T1, T2, T3, T4> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckMethodValidity<T1, T2, T3, T4, T5>(Callback<T1, T2, T3, T4, T5> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckMethodValidity<T1, T2, T3, T4, T5, T6>(Callback<T1, T2, T3, T4, T5, T6> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckMethodValidity<T1, T2, T3, T4, T5, T6, T7>(Callback<T1, T2, T3, T4, T5, T6, T7> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckMethodValidity<T1, T2, T3, T4, T5, T6, T7, T8>(Callback<T1, T2, T3, T4, T5, T6, T7, T8> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckMethodValidity<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckMethodValidity<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckActionValidity (Action callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckActionValidity<T1>(Action<T1> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckActionValidity<T1, T2>(Action<T1, T2> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckActionValidity<T1, T2, T3>(Action<T1, T2, T3> callback)
		{
			return CheckValidity (callback);
		}

		public static bool CheckActionValidity<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
		{
			return CheckValidity (callback);
		}

		/// <summary>
		/// Checks if the Delegate all Target is null or gameobject has been destroyed.
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static bool CheckAllValidity (Delegate callback)
		{
			if (callback == null)
				return false;

			if (ApplicationStatusSensor.IsQuitting)
				return false;

			Delegate[] delegates = callback.GetInvocationList ();
			int length = delegates.Length;
			for (int i = 0; i < length; ++i)
			{
				object target = delegates[i].Target;

				// Delegate.Target will be null if the callback function is static function
				if (target == null && (callback.Method.Attributes & System.Reflection.MethodAttributes.Static) == System.Reflection.MethodAttributes.Static)
					continue;

				if (target == null)
					return false;

				// UnityEngine overloads the Equals and == opeator for the UnityEngine.Object type
				// and returns null when the object has been destroyed.
				if ((target is UnityEngine.Object) && target.Equals (null))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Checks if the TriggerEvent all Target is null or gameObject has been destroyed.
		/// </summary>
		/// <param name="triggerEvent"></param>
		/// <returns></returns>
		public static bool CheckAllValidity (UnityEngine.EventSystems.EventTrigger.TriggerEvent triggerEvent, params object[] args)
		{
			if (triggerEvent == null)
				return false;

			if (ApplicationStatusSensor.IsQuitting)
				return false;

			int count = triggerEvent.GetPersistentEventCount ();
			for (int i = 0; i < count; ++i)
			{
				UnityEngine.Object target = triggerEvent.GetPersistentTarget (i);

				Type t = target.GetType ();
				string methodName = triggerEvent.GetPersistentMethodName (i);
				System.Reflection.MethodInfo method = t.GetMethod (methodName);
				if (method == null)
					continue;

				if (target == null && (method.Attributes & System.Reflection.MethodAttributes.Static) == System.Reflection.MethodAttributes.Static && CheckParameters (method.GetParameters (), args))
					continue;

				if (target == null || target.Equals (null))
					return false;

				if (CheckParameters (method.GetParameters (), args) == false)
					return false;
			}

			return true;
		}

		/// <summary>
		/// Safety but Slowly Invoke Method
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="args"></param>
		public static void SafetyInvoke (this Delegate callback, params object[] args)
		{
			if (callback == null)
				return;

			if (ApplicationStatusSensor.IsQuitting)
				return;

			Delegate[] delegates = callback.GetInvocationList ();
			int length = delegates.Length;
			for (int i = 0; i < length; ++i)
			{
				object target = delegates[i].Target;
				Delegate d = delegates[i];

				// Delegate.Target will be null if the callback function is static function
				if (target == null && (callback.Method.Attributes & System.Reflection.MethodAttributes.Static) == System.Reflection.MethodAttributes.Static)
				{
					d.Method.Invoke (null, args);
					continue;
				}

				if (target == null)
					continue;

				// UnityEngine overloads the Equals and == opeator for the UnityEngine.Object type
				// and returns null when the object has been destroyed.
				if ((target is UnityEngine.Object) && target.Equals (null))
					continue;

				// very slow ...
				d.Method.Invoke (d.Target, args);
			}
		}

		/// <summary>
		/// Safety but Slowly Invoke Method
		/// </summary>
		/// <param name="triggerEvent"></param>
		/// <param name="args"></param>
		public static void SafetyInvoke (this UnityEngine.EventSystems.EventTrigger.TriggerEvent triggerEvent, params object[] args)
		{
			if (triggerEvent == null)
				return;

			if (ApplicationStatusSensor.IsQuitting)
				return;

			int count = triggerEvent.GetPersistentEventCount ();
			for (int i = 0; i < count; ++i)
			{
				UnityEngine.Object target = triggerEvent.GetPersistentTarget (i);

				Type t = target.GetType ();
				string methodName = triggerEvent.GetPersistentMethodName (i);
				System.Reflection.MethodInfo method = t.GetMethod (methodName);
				if (method == null)
					continue;

				if (target == null && (method.Attributes & System.Reflection.MethodAttributes.Static) == System.Reflection.MethodAttributes.Static)
				{
					if (CheckParameters (method.GetParameters (), args))
						method.Invoke (null, args);
					continue;
				}

				if (target == null || target.Equals (null))
					continue;

				// very slow ...
				if (CheckParameters (method.GetParameters (), args))
					method.Invoke (null, args);
			}
		}

		public static int GetLength (this Delegate callback)
		{
			if (callback == null)
				return 0;

			Delegate[] delegates = callback.GetInvocationList ();
			return delegates != null ? delegates.Length : 0;
		}

		public static int GetLength (this UnityEngine.EventSystems.EventTrigger.TriggerEvent triggerEvent)
		{
			if (triggerEvent == null)
				return 0;

			return triggerEvent.GetPersistentEventCount ();
		}

		/// <summary>
		/// Check Method All Parameters Type.
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		private static bool CheckParameters (System.Reflection.ParameterInfo[] parameters, params object[] args)
		{
			for (int i = 0; i < parameters.Length; ++i)
			{
				if (i >= args.Length && parameters[i].IsOptional == false)
					return false;

				if (i >= args.Length && parameters[i].IsOptional)
					return true;

				Type t1 = parameters[i].ParameterType;
				Type t2 = args[i].GetType ();

				if (t1 != t2)
					return false;
			}

			return true;
		}

		public static void SmartInvoke (this Callback callback)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback ();
				else
					callback.SafetyInvoke ();
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback ();
			}
		}

		public static void SmartInvoke<T1>(this Callback<T1> callback, T1 arg1)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1);
				else
					callback.SafetyInvoke (arg1);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1);
			}
		}

		public static void SmartInvoke<T1, T2>(this Callback<T1, T2> callback, T1 arg1, T2 arg2)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1, arg2);
				else
					callback.SafetyInvoke (arg1, arg2);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1, arg2);
			}
		}

		public static void SmartInvoke<T1, T2, T3>(this Callback<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1, arg2, arg3);
				else
					callback.SafetyInvoke (arg1, arg2, arg3);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1, arg2, arg3);
			}
		}

		public static void SmartInvoke<T1, T2, T3, T4>(this Callback<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1, arg2, arg3, arg4);
				else
					callback.SafetyInvoke (arg1, arg2, arg3, arg4);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1, arg2, arg3, arg4);
			}
		}

		public static void SmartInvoke<T1, T2, T3, T4, T5>(this Callback<T1, T2, T3, T4, T5> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1, arg2, arg3, arg4, arg5);
				else
					callback.SafetyInvoke (arg1, arg2, arg3, arg4, arg5);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1, arg2, arg3, arg4, arg5);
			}
		}

		public static void SmartInvoke<T1, T2, T3, T4, T5, T6>(this Callback<T1, T2, T3, T4, T5, T6> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1, arg2, arg3, arg4, arg5, arg6);
				else
					callback.SafetyInvoke (arg1, arg2, arg3, arg4, arg5, arg6);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1, arg2, arg3, arg4, arg5, arg6);
			}
		}

		public static void SmartInvoke<T1, T2, T3, T4, T5, T6, T7>(this Callback<T1, T2, T3, T4, T5, T6, T7> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1, arg2, arg3, arg4, arg5, arg6, arg7);
				else
					callback.SafetyInvoke (arg1, arg2, arg3, arg4, arg5, arg6, arg7);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1, arg2, arg3, arg4, arg5, arg6, arg7);
			}
		}

		public static void SmartInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(this Callback<T1, T2, T3, T4, T5, T6, T7, T8> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
				else
					callback.SafetyInvoke (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
			}
		}

		public static void SmartInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
				else
					callback.SafetyInvoke (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
			}
		}

		public static void SmartInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
				else
					callback.SafetyInvoke (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
			}
		}

		public static void SmartInvoke (this Action callback)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback ();
				else
					callback.SafetyInvoke ();
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback ();
			}
		}

		public static void SmartInvoke<T1>(this Action<T1> callback, T1 arg1)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1);
				else
					callback.SafetyInvoke (arg1);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1);
			}
		}

		public static void SmartInvoke<T1, T2>(this Action<T1, T2> callback, T1 arg1, T2 arg2)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1, arg2);
				else
					callback.SafetyInvoke (arg1, arg2);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1, arg2);
			}
		}

		public static void SmartInvoke<T1, T2, T3>(this Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1, arg2, arg3);
				else
					callback.SafetyInvoke (arg1, arg2, arg3);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1, arg2, arg3);
			}
		}

		public static void SmartInvoke<T1, T2, T3, T4> (this Action<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (callback == null)
				return;

			int delegateLength = callback.GetLength ();
			if (delegateLength > 1)
			{
				if (CheckAllValidity (callback))
					callback (arg1, arg2, arg3, arg4);
				else
					callback.SafetyInvoke (arg1, arg2, arg3, arg4);
			}
			else if (delegateLength > 0 && callback.Validity ())
			{
				callback (arg1, arg2, arg3, arg4);
			}
		}

		public static void CallbackSmartInvoke (Callback callback)
		{
			if (callback != null)
				callback.SmartInvoke ();
		}

		public static void CallbackSmartInvoke<T1>(Callback<T1> callback, T1 arg1)
		{
			if (callback != null)
				callback.SmartInvoke (arg1);
		}

		public static void CallbackSmartInvoke<T1, T2>(Callback<T1, T2> callback, T1 arg1, T2 arg2)
		{
			if (callback != null)
				callback.SmartInvoke (arg1, arg2);
		}

		public static void CallbackSmartInvoke<T1, T2, T3>(Callback<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
		{
			if (callback != null)
				callback.SmartInvoke (arg1, arg2, arg3);
		}

		public static void CallbackSmartInvoke<T1, T2, T3, T4>(Callback<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (callback != null)
				callback.SmartInvoke (arg1, arg2, arg3, arg4);
		}

		public static void CallbackSmartInvoke<T1, T2, T3, T4, T5>(Callback<T1, T2, T3, T4, T5> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			if (callback != null)
				callback.SmartInvoke (arg1, arg2, arg3, arg4, arg5);
		}

		public static void CallbackSmartInvoke<T1, T2, T3, T4, T5, T6>(Callback<T1, T2, T3, T4, T5, T6> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			if (callback != null)
				callback.SmartInvoke (arg1, arg2, arg3, arg4, arg5, arg6);
		}

		public static void CallbackSmartInvoke<T1, T2, T3, T4, T5, T6, T7>(Callback<T1, T2, T3, T4, T5, T6, T7> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			if (callback != null)
				callback.SmartInvoke (arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}

		public static void CallbackSmartInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(Callback<T1, T2, T3, T4, T5, T6, T7, T8> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			if (callback != null)
				callback.SmartInvoke (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}

		public static void CallbackSmartInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			if (callback != null)
				callback.SmartInvoke (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}

		public static void CallbackSmartInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			if (callback != null)
				callback.SmartInvoke (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
		}

		public static void ActionSmartInvoke (Action callback)
		{
			if (callback != null)
				callback.SmartInvoke ();
		}

		public static void ActionSmartInvoke<T1> (Action<T1> callback, T1 arg1)
		{
			if (callback != null)
				callback.SmartInvoke (arg1);
		}

		public static void ActionSmartInvoke<T1, T2> (Action<T1, T2> callback, T1 arg1, T2 arg2)
		{
			if (callback != null)
				callback.SmartInvoke (arg1, arg2);
		}

		public static void ActionSmartInvoke<T1, T2, T3> (Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
		{
			if (callback != null)
				callback.SmartInvoke (arg1, arg2, arg3);
		}

		public static void ActionSmartInvoke<T1, T2, T3, T4> (Action<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (callback != null)
				callback.SmartInvoke (arg1, arg2, arg3, arg4);
		}
	}
}
