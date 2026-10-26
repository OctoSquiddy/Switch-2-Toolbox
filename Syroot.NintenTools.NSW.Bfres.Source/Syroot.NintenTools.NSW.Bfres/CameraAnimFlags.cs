using System;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents flags specifying how animation data is stored or should be played.
/// </summary>
[Flags]
public enum CameraAnimFlags : ushort
{
	/// <summary>
	/// The stored curve data has been baked.
	/// </summary>
	BakedCurve = 1,
	/// <summary>
	/// The animation repeats from the start after the last frame has been played.
	/// </summary>
	Looping = 4,
	/// <summary>
	/// The rotation mode stores ZXY angles rather than look-at points in combination with a twist.
	/// </summary>
	EulerZXY = 0x100,
	/// <summary>
	/// The projection mode is perspective rather than ortographic.
	/// </summary>
	Perspective = 0x400
}
