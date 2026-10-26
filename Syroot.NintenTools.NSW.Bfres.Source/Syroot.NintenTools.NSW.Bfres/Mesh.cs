using System;
using System.Collections.Generic;
using System.IO;
using Syroot.BinaryData;
using Syroot.NintenTools.NSW.Bfres.Core;
using Syroot.NintenTools.NSW.Bfres.GFX;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the surface net of a <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" /> section, storing information on which
/// index <see cref="T:System.Buffer" /> to use for referencing vertices of the shape, mostly used for different levels of
/// detail (LoD) models.
/// </summary>
public class Mesh : IResData
{
	public byte[] Data;

	public long DataOffset;

	internal long PosSubMeshesOffset;

	internal long PosBufferUnkOffset;

	internal long PosBufferSizeOffset;

	/// <summary>
	/// Gets or sets the <see cref="P:Syroot.NintenTools.NSW.Bfres.Mesh.MemoryPool" /> instance storing buffer data at runtime
	/// </summary>
	public MemoryPool MemoryPool { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.BufferSize" /> instance storing buffer size
	/// </summary>
	public BufferSize bufferSize { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="P:Syroot.NintenTools.NSW.Bfres.Mesh.PrimitiveType" /> which determines how indices are used to form polygons.
	/// </summary>
	public PrimitiveType PrimitiveType { get; set; }

	/// <summary>
	/// Gets the <see cref="P:Syroot.NintenTools.NSW.Bfres.Mesh.IndexFormat" /> determining the data type of the indices in the
	/// <see cref="!:IndexBuffer" />.
	/// </summary>
	public IndexFormat IndexFormat { get; private set; }

	/// <summary>
	/// Gets the number of indices stored in the <see cref="!:IndexBuffer" />.
	/// </summary>
	public uint IndexCount
	{
		get
		{
			if (Data == null)
			{
				return 0u;
			}
			int num = 0;
			int formatSize = FormatSize;
			int num2 = Data.Length;
			if (num2 % formatSize != 0)
			{
				throw new InvalidDataException("Cannot form complete indices.");
			}
			return (uint)(num + num2 / formatSize);
		}
	}

	/// <summary>
	/// Gets or sets the list of <see cref="T:Syroot.NintenTools.NSW.Bfres.SubMesh" /> instances which split up a mesh into parts which can be
	/// hidden if they are not visible to optimize rendering performance.
	/// </summary>
	public IList<SubMesh> SubMeshes { get; set; }

	/// <summary>
	/// Gets or sets the offset to the first vertex element of a <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexBuffer" /> to reference by indices.
	/// </summary>
	public uint FirstVertex { get; set; }

	internal int FormatSize => IndexFormat switch
	{
		IndexFormat.UnsignedByte => 1, 
		IndexFormat.UInt16 => 2, 
		IndexFormat.UInt32 => 4, 
		_ => throw new ArgumentException(string.Format("Invalid {0} {1}.", "IndexFormat", IndexFormat), "IndexFormat"), 
	};

	internal ByteOrder FormatByteOrder
	{
		get
		{
			IndexFormat indexFormat = IndexFormat;
			IndexFormat indexFormat2 = indexFormat;
			if (indexFormat2 - 1 <= IndexFormat.UInt16)
			{
				return ByteOrder.LittleEndian;
			}
			throw new ArgumentException(string.Format("Invalid {0} {1}.", "IndexFormat", IndexFormat), "IndexFormat");
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Mesh" /> class.
	/// </summary>
	public Mesh()
	{
		MemoryPool = new MemoryPool();
		bufferSize = new BufferSize();
		PrimitiveType = PrimitiveType.Triangles;
		IndexFormat = IndexFormat.UInt16;
		SubMeshes = new List<SubMesh>();
	}

	/// <summary>
	/// Returns the indices stored in the <see cref="!:IndexBuffer" /> as <see cref="T:System.UInt32" /> instances.
	/// </summary>
	/// <returns>The indices stored in the <see cref="!:IndexBuffer" />.</returns>
	public IEnumerable<uint> GetIndices()
	{
		using BinaryDataReader reader = new BinaryDataReader(new MemoryStream(Data));
		reader.ByteOrder = ByteOrder.LittleEndian;
		uint elementCount = IndexCount;
		IndexFormat indexFormat = IndexFormat;
		IndexFormat indexFormat2 = indexFormat;
		if (indexFormat2 == IndexFormat.UInt16)
		{
			while (elementCount != 0)
			{
				yield return reader.ReadUInt16();
				elementCount--;
			}
		}
		else
		{
			while (elementCount != 0)
			{
				yield return reader.ReadUInt32();
				elementCount--;
			}
		}
	}

	/// <summary>
	/// Stores the given <paramref name="indices" /> in the <see cref="!:IndexBuffer" /> in the provided
	/// <paramref name="format" />, or the current <see cref="P:Syroot.NintenTools.NSW.Bfres.Mesh.IndexFormat" /> if none was specified.
	/// </summary>
	/// <param name="indices">The indices to store in the <see cref="!:IndexBuffer" />.</param>
	/// <param name="format">The <see cref="P:Syroot.NintenTools.NSW.Bfres.Mesh.IndexFormat" /> to use or <c>null</c> to use the current format.
	/// </param>
	public void SetIndices(IList<uint> indices, IndexFormat? format = null)
	{
		IndexFormat = format ?? IndexFormat;
		Data = new byte[indices.Count * FormatSize];
		using BinaryDataWriter binaryDataWriter = new BinaryDataWriter(new MemoryStream(Data, writable: true));
		binaryDataWriter.ByteOrder = FormatByteOrder;
		IndexFormat indexFormat = IndexFormat;
		IndexFormat indexFormat2 = indexFormat;
		if (indexFormat2 == IndexFormat.UInt16)
		{
			{
				foreach (uint index in indices)
				{
					binaryDataWriter.Write((ushort)index);
				}
				return;
			}
		}
		binaryDataWriter.Write(indices);
	}

	public uint SetFaceBufferOffset(ResFileSaver saver)
	{
		uint num = 0u;
		if (saver.Shape != null)
		{
			foreach (Mesh mesh in saver.Shape.Meshes)
			{
				if (mesh == this)
				{
					return num;
				}
				num += (uint)mesh.Data.Length;
				if (num % 8 != 0)
				{
					num += 8 - num % 8;
				}
			}
			return num;
		}
		foreach (Model model in saver.ResFile.Models)
		{
			foreach (Shape shape in model.Shapes)
			{
				foreach (Mesh mesh2 in shape.Meshes)
				{
					if (mesh2 == this)
					{
						return num;
					}
					num += (uint)mesh2.Data.Length;
					if (num % 8 != 0)
					{
						num += 8 - num % 8;
					}
				}
			}
		}
		return num;
	}

	void IResData.Load(ResFileLoader loader)
	{
		long value = loader.ReadOffset();
		MemoryPool = loader.Load<MemoryPool>();
		long num = loader.ReadOffset();
		bufferSize = loader.Load<BufferSize>();
		uint num2 = loader.ReadUInt32();
		PrimitiveType = loader.ReadEnum<PrimitiveType>(strict: true);
		IndexFormat = loader.ReadEnum<IndexFormat>(strict: true);
		uint num3 = loader.ReadUInt32();
		FirstVertex = loader.ReadUInt32();
		ushort count = loader.ReadUInt16();
		ushort num4 = loader.ReadUInt16();
		SubMeshes = loader.LoadList<SubMesh>(count, value);
		DataOffset = (uint)((int)BufferInfo.BufferOffset + (int)num2);
		Data = loader.LoadCustom(() => loader.ReadBytes((int)bufferSize.Size), DataOffset);
	}

	void IResData.Save(ResFileSaver saver)
	{
		bufferSize = new BufferSize();
		bufferSize.Size = (uint)Data.Length;
		saver.SaveRelocateEntryToSection(saver.Position, 1u, 1u, 0u, 1, "Mesh");
		PosSubMeshesOffset = saver.SaveOffset();
		if (MemoryPool != null)
		{
			saver.SaveRelocateEntryToSection(saver.Position, 1u, 1u, 0u, 4, "Memory pool");
		}
		saver.SaveMemoryPoolPointer();
		saver.SaveRelocateEntryToSection(saver.Position, 2u, 1u, 0u, 1, "Mesh buffer info");
		PosBufferUnkOffset = saver.SaveOffset();
		PosBufferSizeOffset = saver.SaveOffset();
		saver.Write(SetFaceBufferOffset(saver));
		saver.Write(PrimitiveType, strict: true);
		saver.Write(IndexFormat, strict: true);
		saver.Write(IndexCount);
		saver.Write(FirstVertex);
		saver.Write((ushort)SubMeshes.Count);
		saver.Seek(2L);
	}
}
