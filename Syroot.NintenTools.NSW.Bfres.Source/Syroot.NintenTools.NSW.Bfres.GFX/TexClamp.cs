namespace Syroot.NintenTools.NSW.Bfres.GFX;

/// <summary>
/// Represents how to treat texture coordinates outside of the normalized coordinate texture range.
/// </summary>
public enum TexClamp : sbyte
{
	Repeat,
	Mirror,
	Clamp,
	ClampToEdge,
	MirrorOnce,
	MirrorOnceClampToEdge
}
