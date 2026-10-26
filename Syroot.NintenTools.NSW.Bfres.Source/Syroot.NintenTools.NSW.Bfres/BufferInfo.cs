using System.Collections.Generic;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an buffer info section in a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> subfile. References vertex and index buffers
/// </summary>
public class BufferInfo : IResData
{
	/// <summary>
	/// Gets or sets the buffer instance that stores face data first, then vertex buffer after.
	/// </summary>
	public static long BufferOffset { get; set; }

	/// <summary>
	/// Gets or sets the buffer instance that stores face data
	/// </summary>
	public byte[][] VertexBufferData { get; set; }

	/// <summary>
	/// Gets or sets the buffer instance that stores vertex data
	/// </summary>
	public byte[][] IndexBufferData { get; set; }

	/// <summary>
	///
	/// </summary>
	public int[] VertexAlignments { get; set; }

	/// <summary>
	/// Gets or sets an unkown value
	/// </summary>
	public uint unk { get; set; } = 34u;

	public uint getIndexBufferSize(ResFileSaver saver)
	{
		uint num = 0u;
		foreach (Model model in saver.ResFile.Models)
		{
			foreach (Shape shape in model.Shapes)
			{
				uint gPUBufferAlignent = model.VertexBuffers[shape.VertexBufferIndex].GPUBufferAlignent;
				foreach (Mesh mesh in shape.Meshes)
				{
					num += (uint)mesh.Data.Length;
					if (num % gPUBufferAlignent != 0)
					{
						num += gPUBufferAlignent - num % gPUBufferAlignent;
					}
				}
			}
		}
		return num;
	}

	public uint GetVertexBufferSize(ResFileSaver saver)
	{
		uint num = 0u;
		foreach (Model model in saver.ResFile.Models)
		{
			foreach (VertexBuffer vertexBuffer in model.VertexBuffers)
			{
				uint gPUBufferAlignent = vertexBuffer.GPUBufferAlignent;
				foreach (VertexBuffer.buffData buffer in vertexBuffer.Buffers)
				{
					num += (uint)buffer.Data.Length;
					if (num % gPUBufferAlignent != 0)
					{
						num += gPUBufferAlignent - num % gPUBufferAlignent;
					}
				}
			}
		}
		return num;
	}

	private void SetIndexBufferData(ResFileSaver saver)
	{
		List<byte[]> list = new List<byte[]>();
		foreach (Model model in saver.ResFile.Models)
		{
			foreach (Shape shape in model.Shapes)
			{
				foreach (Mesh mesh in shape.Meshes)
				{
					list.Add(mesh.Data);
				}
			}
		}
		IndexBufferData = new byte[list.Count][];
		for (int i = 0; i < list.Count; i++)
		{
			IndexBufferData[i] = list[i];
		}
		list.Clear();
	}

	private void SetVertexBufferData(ResFileSaver saver)
	{
		List<byte[]> list = new List<byte[]>();
		List<int> list2 = new List<int>();
		foreach (Model model in saver.ResFile.Models)
		{
			foreach (VertexBuffer vertexBuffer in model.VertexBuffers)
			{
				foreach (VertexBuffer.buffData buffer in vertexBuffer.Buffers)
				{
					list.Add(buffer.Data);
					list2.Add((int)vertexBuffer.GPUBufferAlignent);
				}
			}
		}
		VertexAlignments = list2.ToArray();
		VertexBufferData = list.ToArray();
		list2.Clear();
		list.Clear();
	}

	void IResData.Load(ResFileLoader loader)
	{
		unk = loader.ReadUInt32();
		uint num = loader.ReadUInt32();
		BufferOffset = loader.ReadInt64();
		byte[] array = loader.ReadBytes(16);
	}

	void IResData.Save(ResFileSaver saver)
	{
		SetIndexBufferData(saver);
		SetVertexBufferData(saver);
		saver.Write(unk);
		uint num = 0u;
		for (int i = 0; i < IndexBufferData.Length; i++)
		{
			num += (uint)IndexBufferData[i].Length;
		}
		for (int j = 0; j < VertexBufferData.Length; j++)
		{
			num += (uint)VertexBufferData[j].Length;
		}
		if (num % saver.ResFile.Alignment != 0)
		{
			num += saver.ResFile.Alignment - num % saver.ResFile.Alignment;
		}
		saver.SaveBufferTotalSize();
		saver.SaveRelocateEntryToSection(saver.Position, 1u, 1u, 0u, 2, "Buffer Data");
		saver.SaveIndexBufferPointer();
		saver.Seek(16L);
	}
}
