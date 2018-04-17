using UnityEngine;

namespace JdemLib.Attribute
{
	public class HideWithBoolAttribute : PropertyAttribute
	{
		public string BoolProperty { get; private set; }
		public bool InverseProperty { get; private set; }

		public HideWithBoolAttribute (string boolProperty, bool inverse = false)
		{
			BoolProperty = boolProperty;
			InverseProperty = inverse;
		}
	}
}