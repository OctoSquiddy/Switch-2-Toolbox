namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the data types in which <see cref="T:Syroot.NintenTools.NSW.Bfres.ShaderParam" /> instances can store their value.
/// </summary>
public enum ShaderParamType : byte
{
	/// <summary>
	/// The value is a single <see cref="T:System.Boolean" />.
	/// </summary>
	Bool,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Vector2Bool" />.
	/// </summary>
	Bool2,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Vector3Bool" />.
	/// </summary>
	Bool3,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Vector4Bool" />.
	/// </summary>
	Bool4,
	/// <summary>
	/// The value is a single <see cref="T:System.Int32" />.
	/// </summary>
	Int,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Vector2" />.
	/// </summary>
	Int2,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Vector3" />.
	/// </summary>
	Int3,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Vector4" />.
	/// </summary>
	Int4,
	/// <summary>
	/// The value is a single <see cref="T:System.UInt32" />.
	/// </summary>
	UInt,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Vector2U" />.
	/// </summary>
	UInt2,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Vector3U" />.
	/// </summary>
	UInt3,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Vector4U" />.
	/// </summary>
	UInt4,
	/// <summary>
	/// The value is a single <see cref="T:System.Single" />.
	/// </summary>
	Float,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Vector2F" />.
	/// </summary>
	Float2,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Vector3F" />.
	/// </summary>
	Float3,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Vector4F" />.
	/// </summary>
	Float4,
	/// <summary>
	/// An invalid type for <see cref="T:Syroot.NintenTools.NSW.Bfres.ShaderParam" /> values, only used for internal computations.
	/// </summary>
	Reserved2,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Matrix2" />.
	/// </summary>
	Float2x2,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Matrix2x3" />.
	/// </summary>
	Float2x3,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Matrix2x4" />.
	/// </summary>
	Float2x4,
	/// <summary>
	/// An invalid type for <see cref="T:Syroot.NintenTools.NSW.Bfres.ShaderParam" /> values, only used for internal computations.
	/// </summary>
	Reserved3,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Matrix3x2" />.
	/// </summary>
	Float3x2,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Matrix3" />.
	/// </summary>
	Float3x3,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Matrix3x4" />.
	/// </summary>
	Float3x4,
	/// <summary>
	/// An invalid type for <see cref="T:Syroot.NintenTools.NSW.Bfres.ShaderParam" /> values, only used for internal computations.
	/// </summary>
	Reserved4,
	/// <summary>
	/// The value is a <see cref="T:System.Single" />.
	/// </summary>
	Float4x2,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Matrix4x3" />.
	/// </summary>
	Float4x3,
	/// <summary>
	/// The value is a <see cref="T:Syroot.Maths.Matrix4" />.
	/// </summary>
	Float4x4,
	/// <summary>
	/// The value is a <see cref="F:Syroot.NintenTools.NSW.Bfres.ShaderParamType.Srt2D" />.
	/// </summary>
	Srt2D,
	/// <summary>
	/// The value is a <see cref="F:Syroot.NintenTools.NSW.Bfres.ShaderParamType.Srt3D" />.
	/// </summary>
	Srt3D,
	/// <summary>
	/// The value is a <see cref="F:Syroot.NintenTools.NSW.Bfres.ShaderParamType.TexSrt" />.
	/// </summary>
	TexSrt,
	/// <summary>
	/// The value is a <see cref="F:Syroot.NintenTools.NSW.Bfres.ShaderParamType.TexSrtEx" />.
	/// </summary>
	TexSrtEx
}
