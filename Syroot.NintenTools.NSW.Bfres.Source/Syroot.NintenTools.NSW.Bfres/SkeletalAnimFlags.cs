using System;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents flags specifying how animation data is stored or should be played.
/// </summary>
[Flags]
public enum SkeletalAnimFlags : uint
{
	/// <summary>
	/// The stored curve data has been baked.
	/// </summary>
	BakedCurve = 1u,
	/// <summary>
	/// The animation repeats from the start after the last frame has been played.
	/// </summary>
	Looping = 4u
}
