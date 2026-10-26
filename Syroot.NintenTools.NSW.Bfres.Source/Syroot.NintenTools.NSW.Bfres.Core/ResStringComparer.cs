using System;

namespace Syroot.NintenTools.NSW.Bfres.Core;

/// <summary>
/// Represents a <see cref="P:System.StringComparer.Ordinal" /> sorting empty strings to the end of lists.
/// </summary>
internal class ResStringComparer : StringComparer
{
	private static ResStringComparer _instance;

	internal static ResStringComparer Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ResStringComparer();
			}
			return _instance;
		}
	}

	private ResStringComparer()
	{
	}

	public override int Compare(string x, string y)
	{
		if ((object)x == y)
		{
			return 0;
		}
		if (string.IsNullOrEmpty(x))
		{
			return 1;
		}
		if (string.IsNullOrEmpty(y))
		{
			return -1;
		}
		return string.CompareOrdinal(x, y);
	}

	public override bool Equals(string x, string y)
	{
		return StringComparer.Ordinal.Equals(x, y);
	}

	public override int GetHashCode(string obj)
	{
		return StringComparer.Ordinal.GetHashCode(obj);
	}
}
