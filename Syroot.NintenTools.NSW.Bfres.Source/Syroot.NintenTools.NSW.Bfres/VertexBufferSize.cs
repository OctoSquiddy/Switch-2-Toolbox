using System.Diagnostics;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents stride and size in a <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexBuffer" />
/// specific data subset in the buffer.
/// </summary>
[DebuggerDisplay("VertexBufferSize {Size}")]
public class VertexBufferSize : IResData
{
	/// <summary>
	/// The size of a full vertex in bytes.
	/// </summary>
	public uint Size { get; set; }

	/// <summary>
	/// The gpu access flags.
	/// </summary>
	public uint GpuAccessFlags { get; set; }

	void IResData.Load(ResFileLoader loader)
	{
		Size = loader.ReadUInt32();
		GpuAccessFlags = loader.ReadUInt32();
		loader.Seek(8L);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.Write(Size);
		saver.Write(GpuAccessFlags);
		saver.Seek(8L);
	}
}
