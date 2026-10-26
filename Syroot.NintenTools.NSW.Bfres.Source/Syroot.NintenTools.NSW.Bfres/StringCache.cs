using System.Collections.Generic;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// A cache of strings that are saved through raw IDs in place of the pointer field of a name offset.
/// This is used and required for TOTK models.
/// </summary>
public class StringCache
{
	public static Dictionary<long, string> Strings = new Dictionary<long, string>();
}
