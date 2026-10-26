using Syroot.Maths;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a 2D transformation.
/// </summary>
public struct Srt2D
{
	/// <summary>
	/// The size of this structure.
	/// </summary>
	public const int SizeInBytes = 20;

	/// <summary>
	/// The scaling amount of the transformation.
	/// </summary>
	public Vector2F Scaling;

	/// <summary>
	/// The rotation angle of the transformation.
	/// </summary>
	public float Rotation;

	/// <summary>
	/// The translation amount of the transformation.
	/// </summary>
	public Vector2F Translation;
}
