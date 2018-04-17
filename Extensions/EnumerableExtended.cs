using System.Collections.Generic;

namespace JdemLib.Extensions
{
	public interface ICustomPair<T, V>
	{
		T Key { get; }
		V Value { get; }
	}

	public static class EnumerableExtended
	{
		public static Dictionary<T, V> ToDictionary<T, V> (this ICustomPair<T, V>[] pairs, bool overrideSameKey)
		{
			if (pairs == null)
				return null;

			Dictionary<T, V> dict = new Dictionary<T, V> ();
			for (int i = 0; i < pairs.Length; ++i)
			{
				ICustomPair<T, V> pair = pairs[i];
				if (dict.ContainsKey (pair.Key))
				{
					if (overrideSameKey)
					{
						dict[pair.Key] = pair.Value;
					}
				}
				else
				{
					dict.Add (pair.Key, pair.Value);
				}
			}
			return dict;
		}
	}
}