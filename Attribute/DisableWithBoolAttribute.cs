using UnityEngine;

namespace JdemLib.Attribute
{
	public class DisableWithBoolAttribute : PropertyAttribute
	{
		public string BoolProperty { get; private set; }
		public bool InverseProperty { get; private set; }

		public DisableWithBoolAttribute (string boolProperty, bool inverse = false)
		{
			BoolProperty = boolProperty;
			InverseProperty = inverse;
		}
	}
}