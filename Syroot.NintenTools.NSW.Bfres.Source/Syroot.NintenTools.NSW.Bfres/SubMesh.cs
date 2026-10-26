using System.Diagnostics;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a subarray of a <see cref="T:Syroot.NintenTools.NSW.Bfres.Mesh" /> section, storing a slice of indices to draw from the index buffer
/// referenced in the mesh, mostly used for hiding parts of a model when not visible.
/// </summary>
[DebuggerDisplay("SubMesh [{Offset},{Count})")]
public class SubMesh : IResData
{
	/// <summary>
	/// Gets the offset into the index buffer in bytes.
	/// </summary>
	public uint Offset { get; set; }

	/// <summary>
	/// Gets the number of indices to reference.
	/// </summary>
	public uint Count { get; set; }

	void IResData.Load(ResFileLoader loader)
	{
		Offset = loader.ReadUInt32();
		Count = loader.ReadUInt32();
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.Write(Offset);
		saver.Write(Count);
	}
}
