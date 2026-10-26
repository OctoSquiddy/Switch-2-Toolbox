using System;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents if curves exist which animate the corresponding transformation component.
/// </summary>
[Flags]
public enum BoneAnimFlagsCurve : uint
{
	/// <summary>
	/// Curve animating the X component of a bone's scale.
	/// </summary>
	ScaleX = 0x40u,
	/// <summary>
	/// Curve animating the Y component of a bone's scale.
	/// </summary>
	ScaleY = 0x80u,
	/// <summary>
	/// Curve animating the Z component of a bone's scale.
	/// </summary>
	ScaleZ = 0x100u,
	/// <summary>
	/// Curve animating the X component of a bone's rotation.
	/// </summary>
	RotateX = 0x200u,
	/// <summary>
	/// Curve animating the Y component of a bone's rotation.
	/// </summary>
	RotateY = 0x400u,
	/// <summary>
	/// Curve animating the Z component of a bone's rotation.
	/// </summary>
	RotateZ = 0x800u,
	/// <summary>
	/// Curve animating the W component of a bone's rotation.
	/// </summary>
	RotateW = 0x1000u,
	/// <summary>
	/// Curve animating the X component of a bone's translation.
	/// </summary>
	TranslateX = 0x2000u,
	/// <summary>
	/// Curve animating the Y component of a bone's translation.
	/// </summary>
	TranslateY = 0x4000u,
	/// <summary>
	/// Curve animating the Z component of a bone's translation.
	/// </summary>
	TranslateZ = 0x8000u
}
