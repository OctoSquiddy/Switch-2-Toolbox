namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the possible transformations for bones to handle them as billboards.
/// </summary>
public enum BoneFlagsBillboard : uint
{
	/// <summary>
	/// No transformation is applied.
	/// </summary>
	None = 0u,
	/// <summary>
	/// Transforms of the child are applied.
	/// </summary>
	Child = 65536u,
	/// <summary>
	/// Transforms the Z axis parallel to the camera.
	/// </summary>
	WorldViewVector = 131072u,
	/// <summary>
	/// Transforms the Z axis parallel to the direction of the camera.
	/// </summary>
	WorldViewPoint = 196608u,
	/// <summary>
	/// Transforms the Y axis parallel to the camera up vector, and the Z parallel to the camera up-vector.
	/// </summary>
	ScreenViewVector = 262144u,
	/// <summary>
	/// Transforms the Y axis parallel to the camera up vector, and the Z axis parallel to the direction of the
	/// camera.
	/// </summary>
	ScreenViewPoint = 327680u,
	/// <summary>
	/// Transforms the Z axis parallel to the camera by rotating only the Y axis.
	/// </summary>
	YAxisViewVector = 393216u,
	/// <summary>
	/// Transforms the Z axis parallel to the direction of the camera by rotating only the Y axis.
	/// </summary>
	YAxisViewPoint = 458752u
}
