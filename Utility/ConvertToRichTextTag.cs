using UnityEngine;
using System.Text;
using System.Collections;

namespace JdemLib.Utility
{
	public enum RichTextTagType
	{
		BOLDFACE,
		ITALICS,
		SIZE,
		COLOR,
	}

	public class RichTextTagColor
	{
		public const string AQUA = "00ffffff";          //aqua
		public const string BLACK = "000000ff";         //black
		public const string BLUE = "0000ffff";          //blue
		public const string BROWN = "a52a2aff";         // brown
		public const string CYAN = "00ffffff";          // cyan
		public const string DARKBLUE = "0000a0ff";      // darkblue
		public const string FUCHSIA = "ff00ffff";       // fuchsia
		public const string GREEN = "008000ff";         // green
		public const string GREY = "808080ff";          // grey
		public const string LIGHTBLUE = "add8e6ff";     // lightblue
		public const string LIME = "00ff00ff";          // lime
		public const string MAGENTA = "ff00ffff";       // magenta
		public const string MAROON = "800000ff";        // maroon
		public const string NAVY = "000080ff";          // navy
		public const string OLIVE = "808000ff";         // olive
		public const string ORANGE = "ffa500ff";        // orange
		public const string PURPLE = "800080ff";        // purple
		public const string RED = "ff0000ff";           // red
		public const string SILVER = "c0c0c0ff";        // silver
		public const string TEAL = "008080ff";          // teal
		public const string WHITE = "ffffffff";         // white
		public const string YELLOW = "ffff00ff";        // yellow
	}

	public class ConvertToRichTextTag
	{
		private const string BOLDFACE_TAG_FRONT = "<b>";
		private const string BOLDFACE_TAG_REAR = "</b>";

		private const string ITALICS_TAG_FRONT = "<i>";
		private const string ITALICS_TAG_REAR = "</i>";

		private const string SIZE_TAG_FRONT_1 = "<size=";
		private const string SIZE_TAG_FRONT_2 = ">";
		private const string SIZE_TAG_REAR = "</size>";

		private const string COLOR_TAG_FRONT_1 = "<color=#";
		private const string COLOR_TAG_FRONT_2 = ">";
		private const string COLOR_TAG_REAR = "</color>";

		/// <summary>
		/// Adds the color tag.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="color">Color.</param>
		public static void AddColorTag (ref string content, Color color)
		{
			AddTag (ref content, RichTextTagType.COLOR, ColorConvertor.ToHexColor (color));
		}

		/// <summary>
		/// Adds the size tag.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="size">Size.</param>
		public static void AddSizeTag (ref string content, int size)
		{
			AddTag (ref content, RichTextTagType.SIZE, size.ToString ());
		}

		/// <summary>
		/// Adds the tag.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="type">Type.</param>
		/// <param name="value">Value.</param>
		public static void AddTag (ref string text, RichTextTagType type, string value = "")
		{
			int length = 0;
			string tag_front_1 = "";
			string tag_front_2 = "";
			string tag_rear = "";
			switch (type)
			{
				case RichTextTagType.BOLDFACE:
					length = 7;
					tag_front_1 = BOLDFACE_TAG_FRONT;
					tag_rear = BOLDFACE_TAG_REAR;
					break;
				case RichTextTagType.ITALICS:
					tag_front_1 = ITALICS_TAG_FRONT;
					tag_rear = ITALICS_TAG_REAR;
					length = 7;
					break;
				case RichTextTagType.SIZE:
					tag_front_1 = SIZE_TAG_FRONT_1;
					tag_front_2 = SIZE_TAG_FRONT_2;
					tag_rear = SIZE_TAG_REAR;
					length = 14;
					break;
				case RichTextTagType.COLOR:
					tag_front_1 = COLOR_TAG_FRONT_1;
					tag_front_2 = COLOR_TAG_FRONT_2;
					tag_rear = COLOR_TAG_REAR;
					if (value.IndexOf ("0x") != -1)
						value.Replace ("0x", "");
					if (value.IndexOf ("0X") != -1)
						value.Replace ("0X", "");
					if (value.IndexOf ("#") != -1)
						value.Replace ("#", "");
					length = 17;
					break;
			}
			length = length + text.Length + (string.IsNullOrEmpty (value) ? 0 : value.Length);
			StringBuilder builder = new StringBuilder (length);
			builder.Append (tag_front_1).Append (value).Append (tag_front_2).Append (text).Append (tag_rear);

			text = builder.ToString ();
		}
	}
}