using System;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents flags specifying which fields are animated.
/// </summary>
[Flags]
public enum LightAnimField : ushort
{
	/// <summary>
	/// Enabled state is animated.
	/// </summary>
	Enable = 0x200,
	/// <summary>
	/// Position is animated.
	/// </summary>
	Position = 0x400,
	/// <summary>
	/// Rotation is animated.
	/// </summary>
	Rotation = 0x800,
	/// <summary>
	/// Distance attenuation is animated.
	/// </summary>
	DistanceAttn = 0x1000,
	/// <summary>
	/// Angle attenuation is animated in degrees.
	/// </summary>
	AngleAttn = 0x2000,
	/// <summary>
	/// Color 0 is animated.
	/// </summary>
	Color0 = 0x4000,
	/// <summary>
	/// Color 1 is animated.
	/// </summary>
	Color1 = 0x8000
}
