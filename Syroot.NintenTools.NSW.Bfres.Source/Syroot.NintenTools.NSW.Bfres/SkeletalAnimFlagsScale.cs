namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the data format in which scaling values are stored.
/// </summary>
public enum SkeletalAnimFlagsScale : uint
{
	/// <summary>
	/// No scaling.
	/// </summary>
	None = 0u,
	/// <summary>
	/// Default scaling.
	/// </summary>
	Standard = 256u,
	/// <summary>
	/// Autodesk Maya scaling.
	/// </summary>
	Maya = 512u,
	/// <summary>
	/// Autodesk Softimage scaling.
	/// </summary>
	Softimage = 768u
}
