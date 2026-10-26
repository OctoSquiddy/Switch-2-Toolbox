using Syroot.Maths;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a 2D texture transformation which is multiplied by a 3x4 matrix referenced at runtime by the
/// <see cref="F:Syroot.NintenTools.NSW.Bfres.TexSrtEx.MatrixPointer" />.
/// </summary>
public struct TexSrtEx
{
	/// <summary>
	/// The size of this structure.
	/// </summary>
	public const int SizeInBytes = 28;

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

	/// <summary>
	/// A pointer to a 3x4 matrix to multiply the transformation with. Set at runtime.
	/// </summary>
	public uint MatrixPointer;
}
