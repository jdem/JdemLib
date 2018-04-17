using UnityEngine;
using System.Text;
using System.Collections;

namespace JdemLib.Utility
{
	public class ColorConvertor
	{
		private const string defaultPrefixHexColor = "#{0}";

		/// <summary>
		/// Tos the color of the hex.
		/// </summary>
		/// <returns>The hex color(RRGGBBAA).</returns>
		/// <param name="c">C.</param>
		/// <param name="prefix">Prefix.</param>
		/// <param name="includeAlpha">If set to <c>true</c> include alpha.</param>
		public static string ToHexColor (Color c, string prefix = "", bool includeAlpha = true)
		{
			StringBuilder builder = new StringBuilder (9);
			if (!string.IsNullOrEmpty (prefix))
				builder.Append (prefix);

			if (includeAlpha)
			{
				builder.Append (ColorUtility.ToHtmlStringRGBA (c));
			}
			else
			{
				builder.Append (ColorUtility.ToHtmlStringRGB (c));
			}

			return builder.ToString ();
		}

		/// <summary>
		/// Covert Hex String to Color
		/// <param name="hexRgba">color hex string format</param>
		/// <returns>Color</returns>
		/// </summary>
		public static Color ToColor (string hexRgba)
		{
			if (string.IsNullOrEmpty (hexRgba))
				return Color.clear;

			Color retColor = Color.black;
			ColorUtility.TryParseHtmlString (string.Format (defaultPrefixHexColor, hexRgba), out retColor);
			return retColor;
		}
	}
}