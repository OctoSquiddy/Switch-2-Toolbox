using System;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents flags determining which data is available for <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" /> instances.
/// </summary>
[Flags]
public enum ShapeFlags : uint
{
	/// <summary>
	/// The <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" /> instance references a <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexBuffer" />.
	/// </summary>
	HasVertexBuffer = 2u,
	/// <summary>
	/// The boundings in all submeshes are consistent.
	/// </summary>
	SubMeshBoundaryConsistent = 4u
}
