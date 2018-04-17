using UnityEngine;
using System;
using System.Collections;

namespace JdemLib.Utility
{
	public class ApplicationStatusSensor : MonoBehaviour
	{
		private Callback onResume;

		public void RegisterResumeCb (Callback cb)
		{
			onResume -= cb;
			onResume += cb;
		}

		public void UnregisterResumeCb (Callback cb)
		{
			onResume -= cb;
		}

		public void Resume ()
		{
			DelegateExtended.CallbackSmartInvoke (onResume);
		}

		private static bool isPause = false;
		public static bool IsPause
		{ get { return isPause; } }

		private static double pauseSecond = 0;
		public static double pauseSecond
		{ get { return pauseSecond; } }

		private DateTime? pauseTime = null;
		void OnApplicationPause (bool pauseStatus)
		{
			isPause = pauseStatus;
			
			if (pauseStatus)
			{
				pauseTime = DateTime.Now;
			}
			else
			{
				if (pauseTime != null)
				{
					TimeSpan interval = DateTime.Now - pauseTime.Value;
					pauseSecond = interval.TotalSeconds;
					if (interval.TotalMinutes > 1.5f)
					{
						Resume;
					}
					StartCoroutine (ResetPauseSecond ());
				}
			}
		}

		private IEnumerator ResetPauseSecond ()
		{
			yield return null;
			pauseSecond = 0;
		}

		private static bool isQuitting = false;
		public static bool IsQuitting
		{ get { return isQuitting; } }

		void OnApplicationQuit ()
		{
			isQuitting = true;
		}
	}
}