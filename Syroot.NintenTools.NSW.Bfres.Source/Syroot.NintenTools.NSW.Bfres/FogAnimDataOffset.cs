namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Gets the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.AnimDataOffset" /> for <see cref="T:Syroot.NintenTools.NSW.Bfres.FogAnimData" /> instances.
/// </summary>
public enum FogAnimDataOffset : uint
{
	/// <summary>
	/// Animates the X component of <see cref="F:Syroot.NintenTools.NSW.Bfres.FogAnimData.DistanceAttn" />.
	/// </summary>
	DistanceAttnX = 0u,
	/// <summary>
	/// Animates the Y component of <see cref="F:Syroot.NintenTools.NSW.Bfres.FogAnimData.DistanceAttn" />.
	/// </summary>
	DistanceAttnY = 4u,
	/// <summary>
	/// Animates the X (red) component of <see cref="F:Syroot.NintenTools.NSW.Bfres.FogAnimData.Color" />.
	/// </summary>
	ColorR = 8u,
	/// <summary>
	/// Animates the Y (green) component of <see cref="F:Syroot.NintenTools.NSW.Bfres.FogAnimData.Color" />.
	/// </summary>
	ColorG = 12u,
	/// <summary>
	/// Animates the Z (blue) component of <see cref="F:Syroot.NintenTools.NSW.Bfres.FogAnimData.Color" />.
	/// </summary>
	ColorB = 16u
}
