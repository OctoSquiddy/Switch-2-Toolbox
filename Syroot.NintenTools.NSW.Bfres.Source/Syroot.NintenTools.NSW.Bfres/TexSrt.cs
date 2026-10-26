using Syroot.Maths;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a 2D texture transformation.
/// </summary>
public struct TexSrt
{
	/// <summary>
	/// The size of this structure.
	/// </summary>
	public const int SizeInBytes = 24;

	/// <summary>
	/// The <see cref="T:Syroot.NintenTools.NSW.Bfres.TexSrtMode" /> with which the transformation is applied.
	/// </summary>
	public TexSrtMode Mode;

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
