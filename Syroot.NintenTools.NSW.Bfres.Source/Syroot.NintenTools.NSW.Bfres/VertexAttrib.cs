using System.Diagnostics;
using Syroot.BinaryData;
using Syroot.NintenTools.NSW.Bfres.Core;
using Syroot.NintenTools.NSW.Bfres.GFX;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an attribute of a <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexBuffer" /> describing the data format, type and layout of a
/// specific data subset in the buffer.
/// </summary>
[DebuggerDisplay("VertexAttrib {Name}")]
public class VertexAttrib : IResData
{
	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in
	/// <see cref="!:ResDict&lt;VertexAttrib&gt;" /> instances.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the index of the buffer storing the data in the <see cref="P:Syroot.NintenTools.NSW.Bfres.VertexBuffer.Buffers" /> list.
	/// </summary>
	public ushort BufferIndex { get; set; }

	/// <summary>
	/// Gets or sets the offset in bytes to the attribute in each vertex.
	/// </summary>
	public ushort Offset { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat" /> determining the type in which attribute data is available.
	/// </summary>
	public AttribFormat Format { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexAttrib" /> class.
	/// </summary>
	public VertexAttrib()
	{
		Name = "";
		BufferIndex = 0;
		Offset = 0;
		Format = AttribFormat.Format_32_32_32_32_Single;
	}

	void IResData.Load(ResFileLoader loader)
	{
		Name = loader.LoadString();
		loader.ByteOrder = ByteOrder.BigEndian;
		Format = loader.ReadEnum<AttribFormat>(strict: true);
		loader.ByteOrder = ByteOrder.LittleEndian;
		loader.Seek(2L);
		Offset = loader.ReadUInt16();
		BufferIndex = loader.ReadUInt16();
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.SaveString(Name);
		saver.ByteOrder = ByteOrder.BigEndian;
		saver.Write(Format, strict: true);
		saver.ByteOrder = ByteOrder.LittleEndian;
		saver.Write((short)0);
		saver.Write(Offset);
		saver.Write(BufferIndex);
	}
}
