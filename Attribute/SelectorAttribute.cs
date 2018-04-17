using UnityEngine;

namespace JdemLib.Attribute
{
	public class FileSelectorAttribute : PropertyAttribute
	{
		public readonly string defaultPath;
		public readonly string extension;
		public readonly string onChanged;

		public FileSelectorAttribute (string defaultPath = null, string extension = null, string onChanged = null)
		{
			this.defaultPath = defaultPath;
			this.extension = extension;
			this.onChanged = onChanged;
		}
	}
}