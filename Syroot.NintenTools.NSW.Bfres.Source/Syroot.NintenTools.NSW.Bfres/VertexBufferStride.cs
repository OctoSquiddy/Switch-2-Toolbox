using System.Diagnostics;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents stride and size in a <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexBuffer" />
/// specific data subset in the buffer.
/// </summary>
[DebuggerDisplay("VertexBufferStride {Stride}")]
public class VertexBufferStride : IResData
{
	/// <summary>
	/// The size of a full vertex in bytes.
	/// </summary>
	public uint Stride { get; set; }

	void IResData.Load(ResFileLoader loader)
	{
		Stride = loader.ReadUInt32();
		loader.Seek(12L);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.Write(Stride);
		saver.Seek(12L);
	}
}
