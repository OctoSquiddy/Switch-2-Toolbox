using System;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents flags specifying how animation data is stored.
/// </summary>
[Flags]
public enum LightAnimFlags : ushort
{
	/// <summary>
	/// The stored curve data has been baked.
	/// </summary>
	BakedCurve = 1,
	/// <summary>
	/// The animation repeats from the start after the last frame has been played.
	/// </summary>
	Looping = 4,
	EnableCurve = 0x100,
	BaseEnable = 0x200,
	BasePos = 0x400,
	BaseDir = 0x800,
	BaseDistAttn = 0x1000,
	BaseAngleAttn = 0x2000,
	BaseColor0 = 0x4000,
	BaseColor1 = 0x8000
}
