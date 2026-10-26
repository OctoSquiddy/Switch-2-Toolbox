namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Gets the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.AnimDataOffset" /> for <see cref="T:Syroot.NintenTools.NSW.Bfres.LightAnimData" /> instances.
/// </summary>
public enum LightAnimDataOffset : uint
{
	/// <summary>
	/// Animates <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Enable" />.
	/// </summary>
	Enable = 0u,
	/// <summary>
	/// Animates the X component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Position" />.
	/// </summary>
	PositionX = 4u,
	/// <summary>
	/// Animates the Y component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Position" />.
	/// </summary>
	PositionY = 8u,
	/// <summary>
	/// Animates the Z component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Position" />.
	/// </summary>
	PositionZ = 12u,
	/// <summary>
	/// Animates the X component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Rotation" />.
	/// </summary>
	RotationX = 16u,
	/// <summary>
	/// Animates the Y component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Rotation" />.
	/// </summary>
	RotationY = 20u,
	/// <summary>
	/// Animates the Z component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Rotation" />.
	/// </summary>
	RotationZ = 24u,
	/// <summary>
	/// Animates the X component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.DistanceAttn" />.
	/// </summary>
	DistanceAttnX = 28u,
	/// <summary>
	/// Animates the Y component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.DistanceAttn" />.
	/// </summary>
	DistanceAttnY = 32u,
	/// <summary>
	/// Animates the X component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.AngleAttn" />.
	/// </summary>
	AngleAttnX = 36u,
	/// <summary>
	/// Animates the Y component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.AngleAttn" />.
	/// </summary>
	AngleAttnY = 40u,
	/// <summary>
	/// Animates the X (red) component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Color0" />.
	/// </summary>
	Color0R = 44u,
	/// <summary>
	/// Animates the Y (green) component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Color0" />.
	/// </summary>
	Color0G = 48u,
	/// <summary>
	/// Animates the Z (blue) component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Color0" />.
	/// </summary>
	Color0B = 52u,
	/// <summary>
	/// Animates the X (red) component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Color1" />.
	/// </summary>
	Color1R = 56u,
	/// <summary>
	/// Animates the Y (green) component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Color1" />.
	/// </summary>
	Color1G = 60u,
	/// <summary>
	/// Animates the Z (blue) component of <see cref="F:Syroot.NintenTools.NSW.Bfres.LightAnimData.Color1" />.
	/// </summary>
	Color1B = 64u
}
