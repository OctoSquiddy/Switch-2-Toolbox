using Syroot.Maths;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a 3D transformation.
/// </summary>
public struct Srt3D
{
	/// <summary>
	/// The size of this structure.
	/// </summary>
	public const int SizeInBytes = 36;

	/// <summary>
	/// The scaling amount of the transformation.
	/// </summary>
	public Vector3F Scaling;

	/// <summary>
	/// The rotation amount of the transformation.
	/// </summary>
	public Vector3F Rotation;

	/// <summary>
	/// The translation amount of the transformation.
	/// </summary>
	public Vector3F Translation;
}
