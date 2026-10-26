namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Gets the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.AnimDataOffset" /> for <see cref="T:Syroot.NintenTools.NSW.Bfres.BoneAnimData" /> instances.
/// </summary>
public enum BoneAnimDataOffset : uint
{
	/// <summary>
	/// Animates <see cref="F:Syroot.NintenTools.NSW.Bfres.BoneAnimData.Flags" /> (never seen in files).
	/// </summary>
	Flags = 0u,
	/// <summary>
	/// Animates the X component of <see cref="F:Syroot.NintenTools.NSW.Bfres.BoneAnimData.Scale" />.
	/// </summary>
	ScaleX = 4u,
	/// <summary>
	/// Animates the Y component of <see cref="F:Syroot.NintenTools.NSW.Bfres.BoneAnimData.Scale" />.
	/// </summary>
	ScaleY = 8u,
	/// <summary>
	/// Animates the Z component of <see cref="F:Syroot.NintenTools.NSW.Bfres.BoneAnimData.Scale" />.
	/// </summary>
	ScaleZ = 12u,
	/// <summary>
	/// Animates the X component of <see cref="F:Syroot.NintenTools.NSW.Bfres.BoneAnimData.Translate" />.
	/// </summary>
	TranslateX = 16u,
	/// <summary>
	/// Animates the Y component of <see cref="F:Syroot.NintenTools.NSW.Bfres.BoneAnimData.Translate" />.
	/// </summary>
	TranslateY = 20u,
	/// <summary>
	/// Animates the Z component of <see cref="F:Syroot.NintenTools.NSW.Bfres.BoneAnimData.Translate" />.
	/// </summary>
	TranslateZ = 24u,
	/// <summary>
	/// Animates the X component of <see cref="F:Syroot.NintenTools.NSW.Bfres.BoneAnimData.Rotate" />.
	/// </summary>
	RotateX = 32u,
	/// <summary>
	/// Animates the Y component of <see cref="F:Syroot.NintenTools.NSW.Bfres.BoneAnimData.Rotate" />.
	/// </summary>
	RotateY = 36u,
	/// <summary>
	/// Animates the Z component of <see cref="F:Syroot.NintenTools.NSW.Bfres.BoneAnimData.Rotate" />.
	/// </summary>
	RotateZ = 40u,
	/// <summary>
	/// Animates the W component of <see cref="F:Syroot.NintenTools.NSW.Bfres.BoneAnimData.Rotate" />.
	/// </summary>
	RotateW = 44u
}
