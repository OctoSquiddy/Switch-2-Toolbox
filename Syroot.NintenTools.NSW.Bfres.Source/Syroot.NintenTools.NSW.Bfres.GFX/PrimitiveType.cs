namespace Syroot.NintenTools.NSW.Bfres.GFX;

/// <summary>
/// Represents the type of primitives to draw.
/// </summary>
public enum PrimitiveType : uint
{
	Points = 0u,
	Lines = 1u,
	LineStrip = 4u,
	Triangles = 3u,
	TriangleStrip = LineStrip,
	LinesAdjacency = 5u,
	LineStripAdjacency = 6u,
	TrianglesAdjacency = 7u,
	TriangleStripAdjacency = 8u,
	Patches = 9u
}
