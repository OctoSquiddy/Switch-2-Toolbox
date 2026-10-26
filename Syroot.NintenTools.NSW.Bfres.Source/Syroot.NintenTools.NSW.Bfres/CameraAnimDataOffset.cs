namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Gets the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.AnimDataOffset" /> for <see cref="T:Syroot.NintenTools.NSW.Bfres.CameraAnimData" /> instances.
/// </summary>
public enum CameraAnimDataOffset : uint
{
	/// <summary>
	/// Animates <see cref="F:Syroot.NintenTools.NSW.Bfres.CameraAnimData.ClipNear" />.
	/// </summary>
	ClipNear = 0u,
	/// <summary>
	/// Animates <see cref="F:Syroot.NintenTools.NSW.Bfres.CameraAnimData.ClipFar" />.
	/// </summary>
	ClipFar = 4u,
	/// <summary>
	/// Animates <see cref="F:Syroot.NintenTools.NSW.Bfres.CameraAnimData.AspectRatio" />.
	/// </summary>
	AspectRatio = 8u,
	/// <summary>
	/// Animates <see cref="F:Syroot.NintenTools.NSW.Bfres.CameraAnimData.FieldOfView" />.
	/// </summary>
	FieldOFView = 12u,
	/// <summary>
	/// Animates the X component of <see cref="F:Syroot.NintenTools.NSW.Bfres.CameraAnimData.Position" />.
	/// </summary>
	PositionX = 16u,
	/// <summary>
	/// Animates the Y component of <see cref="F:Syroot.NintenTools.NSW.Bfres.CameraAnimData.Position" />.
	/// </summary>
	PositionY = 20u,
	/// <summary>
	/// Animates the Z component of <see cref="F:Syroot.NintenTools.NSW.Bfres.CameraAnimData.Position" />.
	/// </summary>
	PositionZ = 24u,
	/// <summary>
	/// Animates the X component of <see cref="F:Syroot.NintenTools.NSW.Bfres.CameraAnimData.Rotation" />.
	/// </summary>
	RotationX = 28u,
	/// <summary>
	/// Animates the Y component of <see cref="F:Syroot.NintenTools.NSW.Bfres.CameraAnimData.Rotation" />.
	/// </summary>
	RotationY = 32u,
	/// <summary>
	/// Animates the Z component of <see cref="F:Syroot.NintenTools.NSW.Bfres.CameraAnimData.Rotation" />.
	/// </summary>
	RotationZ = 36u,
	/// <summary>
	/// Animates <see cref="F:Syroot.NintenTools.NSW.Bfres.CameraAnimData.Twist" />.
	/// </summary>
	Twist = 40u
}
