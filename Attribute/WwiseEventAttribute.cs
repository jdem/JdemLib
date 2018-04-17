using UnityEngine;
using System;

namespace JdemLib.Attribute
{
	public class WwiseEventAttribute : PropertyAttribute
	{
		public int DefaultEventID { get; private set; }
		public bool AllowEmpty { get; private set; }

		public WwiseEventAttribute (uint defaultEventID = 0, bool allowEmpty = true)
		{
			DefaultEventID = (int)defaultEventID;
			AllowEmpty = allowEmpty;
		}
	}
}