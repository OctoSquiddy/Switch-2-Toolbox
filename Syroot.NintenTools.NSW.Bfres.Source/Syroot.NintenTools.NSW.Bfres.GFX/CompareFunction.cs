namespace Syroot.NintenTools.NSW.Bfres.GFX;

/// <summary>
/// Represents compare functions used for depth and stencil tests.
/// </summary>
public enum CompareFunction : byte
{
	Never,
	Less,
	Equal,
	LessOrEqual,
	Greater,
	NotEqual,
	GreaterOrEqual,
	Always
}
