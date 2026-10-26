using System.Collections.Generic;
using System.IO;
using Syroot.BinaryData;
using Syroot.Maths;
using Syroot.NintenTools.NSW.Bfres.Core;
using Syroot.NintenTools.NSW.Bfres.GFX;
using Syroot.NintenTools.NSW.Bfres.Helpers;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a data buffer holding vertices for a <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> subfile.
/// </summary>
public class VertexBuffer : IResData
{
	public class buffData
	{
		public int buffSize;

		public int Stride;

		public int DataOffset;

		public byte[] Data;
	}

	private const string _signature = "FVTX";

	/// <summary>
	/// Gets the number of vertices stored by the <see cref="P:Syroot.NintenTools.NSW.Bfres.VertexBuffer.Buffers" />. It is calculated from the size of the first
	/// <see cref="!:Buffer" /> in bytes divided by the <see cref="P:Syroot.NintenTools.NSW.Bfres.VertexBuffer.StrideArray" />.
	/// </summary>
	public uint VertexCount;

	private uint UnknownFlag;

	internal long AttributeOffset;

	internal long AttributeDictOffset;

	internal long UnkBufferOffset;

	internal long UnkBuffer2Offset;

	internal long BufferSizeArrayOffset;

	internal long StideArrayOffset;

	internal long Position;

	/// <summary>
	/// Gets or sets the number of bones influencing the vertices stored in this buffer. 0 influences equal
	/// rigidbodies (no skinning), 1 equal rigid skinning and 2 or more smooth skinning.
	/// </summary>
	public uint VertexSkinCount { get; set; }

	/// <summary>
	///
	/// </summary>
	public uint GPUBufferAlignent { get; set; } = 8u;

	/// <summary>
	/// Gets or sets the <see cref="P:Syroot.NintenTools.NSW.Bfres.VertexBuffer.MemoryPool" /> instance storing buffer data at runtime
	/// </summary>
	public MemoryPool MemoryPool { get; set; }

	/// <summary>
	/// The size of a full vertex in bytes.
	/// </summary>
	public IList<VertexBufferStride> StrideArray { get; set; }

	public IList<VertexBufferSize> VertexBufferSizeArray { get; set; }

	/// <summary>
	/// Gets or sets the dictionary of <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexAttrib" /> instances describing how to interprete data in the
	/// <see cref="P:Syroot.NintenTools.NSW.Bfres.VertexBuffer.Buffers" />.
	/// </summary>
	public ResDict AttributeDict { get; set; }

	public IList<VertexAttrib> Attributes { get; set; }

	/// <summary>
	/// Gets or sets the list of <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexBuffer.buffData" /> instances storing raw unformatted vertex data.
	/// </summary>
	public IList<buffData> Buffers { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Mesh" /> class.
	/// </summary>
	public VertexBuffer()
	{
		VertexSkinCount = 0u;
		MemoryPool = new MemoryPool();
		StrideArray = new List<VertexBufferStride>();
		VertexBufferSizeArray = new List<VertexBufferSize>();
		AttributeDict = new ResDict();
		Attributes = new List<VertexAttrib>();
		Buffers = new List<buffData>();
	}

	public void CreateEmptyVertexBuffer()
	{
		Vector4F[] data = new Vector4F[4]
		{
			new Vector4F(-1f, 0f, 1f, 0f),
			new Vector4F(-1f, 0f, 1f, 0f),
			new Vector4F(-1f, 0f, 1f, 0f),
			new Vector4F(-1f, 0f, 1f, 0f)
		};
		Vector4F[] data2 = new Vector4F[4]
		{
			new Vector4F(0f, -1f, 0f, 0f),
			new Vector4F(0f, -1f, 0f, 0f),
			new Vector4F(0f, -1f, 0f, 0f),
			new Vector4F(0f, -1f, 0f, 0f)
		};
		VertexBufferHelper vertexBufferHelper = new VertexBufferHelper(new VertexBuffer(), ByteOrder.LittleEndian);
		List<VertexBufferHelperAttrib> list = new List<VertexBufferHelperAttrib>();
		VertexBufferHelperAttrib vertexBufferHelperAttrib = new VertexBufferHelperAttrib();
		vertexBufferHelperAttrib.Name = "_p0";
		vertexBufferHelperAttrib.Data = data;
		vertexBufferHelperAttrib.Format = AttribFormat.Format_32_32_32_Single;
		list.Add(vertexBufferHelperAttrib);
		VertexBufferHelperAttrib vertexBufferHelperAttrib2 = new VertexBufferHelperAttrib();
		vertexBufferHelperAttrib2.Name = "_n0";
		vertexBufferHelperAttrib2.Data = data2;
		vertexBufferHelperAttrib2.Format = AttribFormat.Format_10_10_10_2_SNorm;
		list.Add(vertexBufferHelperAttrib2);
		vertexBufferHelper.Attributes = list;
		VertexBuffer vertexBuffer = vertexBufferHelper.ToVertexBuffer();
		VertexSkinCount = vertexBuffer.VertexSkinCount;
		AttributeDict = vertexBuffer.AttributeDict;
		Attributes = vertexBuffer.Attributes;
		Buffers = vertexBuffer.Buffers;
		VertexBufferSizeArray = vertexBuffer.VertexBufferSizeArray;
		StrideArray = vertexBuffer.StrideArray;
	}

	public uint GetTotalBufferSize()
	{
		uint num = 0u;
		foreach (buffData buffer in Buffers)
		{
			num += (uint)buffer.buffSize;
		}
		return num;
	}

	public uint SetVertexBufferArrayOffset(ResFileSaver saver)
	{
		uint num = (uint)BufferInfo.BufferOffset;
		uint num2 = num;
		if (saver.Shape != null)
		{
			foreach (Mesh mesh in saver.Shape.Meshes)
			{
				if (num2 % 8 != 0)
				{
					num2 += 8 - num2 % 8;
				}
				num2 += (uint)mesh.Data.Length;
			}
			return num2 - num;
		}
		foreach (Model model in saver.ResFile.Models)
		{
			foreach (Shape shape in model.Shapes)
			{
				foreach (Mesh mesh2 in shape.Meshes)
				{
					if (num2 % 8 != 0)
					{
						num2 += 8 - num2 % 8;
					}
					num2 += (uint)mesh2.Data.Length;
				}
			}
		}
		foreach (Model model2 in saver.ResFile.Models)
		{
			foreach (VertexBuffer vertexBuffer in model2.VertexBuffers)
			{
				// Must match ResFileSaver.WriteVertexBuffer, which pads each buffer by the GPU alignment (0x100 in v10)
				uint alignment = ((vertexBuffer.GPUBufferAlignent == 0) ? 8u : vertexBuffer.GPUBufferAlignent);
				foreach (buffData buffer in vertexBuffer.Buffers)
				{
					if (num2 % alignment != 0)
					{
						num2 += alignment - num2 % alignment;
					}
					if (vertexBuffer == this)
					{
						return num2 - num;
					}
					num2 += (uint)buffer.Data.Length;
				}
			}
		}
		return num2 - num;
	}

	private void SaveBuffers()
	{
		VertexBufferSizeArray.Clear();
		StrideArray.Clear();
		for (int i = 0; i < Buffers.Count; i++)
		{
			VertexBufferSize vertexBufferSize = new VertexBufferSize();
			vertexBufferSize.Size = (uint)Buffers[i].buffSize;
			VertexBufferSizeArray.Add(vertexBufferSize);
			VertexBufferStride vertexBufferStride = new VertexBufferStride();
			vertexBufferStride.Stride = (uint)Buffers[i].Stride;
			StrideArray.Add(vertexBufferStride);
		}
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FVTX");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			UnknownFlag = loader.ReadUInt32();
		}
		else
		{
			loader.LoadHeaderBlock();
		}
		long value = loader.ReadOffset();
		AttributeDict = loader.LoadDict();
		MemoryPool = loader.Load<MemoryPool>();
		long num = loader.ReadOffset();
		if (loader.ResFile.VersionMajor2 > 2 || loader.ResFile.VersionMajor != 0)
		{
			loader.ReadOffset();
		}
		long value2 = loader.ReadOffset();
		long value3 = loader.ReadOffset();
		long num2 = loader.ReadInt64();
		int num3 = loader.ReadInt32();
		byte count = loader.ReadByte();
		byte b = loader.ReadByte();
		ushort num4 = loader.ReadUInt16();
		VertexCount = loader.ReadUInt32();
		VertexSkinCount = loader.ReadUInt16();
		GPUBufferAlignent = loader.ReadUInt16();
		StrideArray = loader.LoadList<VertexBufferStride>(b, value3);
		VertexBufferSizeArray = loader.LoadList<VertexBufferSize>(b, value2);
		Attributes = loader.LoadList<VertexAttrib>(count, value);
		Buffers = new List<buffData>();
		using (loader.TemporarySeek(BufferInfo.BufferOffset + num3, SeekOrigin.Begin))
		{
			for (int i = 0; i < b; i++)
			{
				buffData buffData2 = new buffData();
				buffData2.buffSize = (int)VertexBufferSizeArray[i].Size;
				buffData2.Stride = (int)StrideArray[i].Stride;
				loader.Align((int)GPUBufferAlignent);
				buffData2.Data = loader.ReadBytes(buffData2.buffSize);
				Buffers.Add(buffData2);
			}
		}
	}

	void IResData.Save(ResFileSaver saver)
	{
		Position = saver.Position;
		SaveBuffers();
		saver.WriteSignature("FVTX");
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(UnknownFlag);
		}
		else
		{
			saver.Seek(12L);
		}
		saver.SaveRelocateEntryToSection(saver.Position, 2u, 1u, 0u, 1, "FVTX");
		AttributeOffset = saver.SaveOffset();
		AttributeDictOffset = saver.SaveOffset();
		if (MemoryPool != null)
		{
			saver.SaveRelocateEntryToSection(saver.Position, 1u, 1u, 0u, 4, "Memory pool");
		}
		saver.SaveMemoryPoolPointer();
		saver.SaveRelocateEntryToSection(saver.Position, 4u, 1u, 0u, 1, "Vertex buffer info");
		UnkBufferOffset = saver.SaveOffset();
		UnkBuffer2Offset = saver.SaveOffset();
		BufferSizeArrayOffset = saver.SaveOffset();
		StideArrayOffset = saver.SaveOffset();
		saver.Write(0L);
		saver.Write(SetVertexBufferArrayOffset(saver));
		saver.Write((byte)Attributes.Count);
		saver.Write((byte)Buffers.Count);
		saver.Write((ushort)saver.CurrentIndex);
		saver.Write(VertexCount);
		saver.Write((ushort)VertexSkinCount);
		saver.Write((ushort)GPUBufferAlignent);
	}
}
