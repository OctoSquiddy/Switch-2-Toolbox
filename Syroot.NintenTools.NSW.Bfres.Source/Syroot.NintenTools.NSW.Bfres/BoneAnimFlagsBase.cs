using System;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents if initial values exist for the corresponding transformation in the base animation data.
/// </summary>
[Flags]
public enum BoneAnimFlagsBase : uint
{
	/// <summary>
	/// Initial scaling values exist.
	/// </summary>
	Scale = 8u,
	/// <summary>
	/// Initial rotation values exist.
	/// </summary>
	Rotate = 0x10u,
	/// <summary>
	/// Initial translation values exist.
	/// </summary>
	Translate = 0x20u
}
