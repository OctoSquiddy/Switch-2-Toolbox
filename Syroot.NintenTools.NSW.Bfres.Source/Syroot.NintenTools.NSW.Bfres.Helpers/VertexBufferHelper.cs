using System;
using System.Collections.Generic;
using System.IO;
using Syroot.BinaryData;
using Syroot.Maths;
using Syroot.NintenTools.NSW.Bfres.Core;
using Syroot.NintenTools.NSW.Bfres.GFX;

namespace Syroot.NintenTools.NSW.Bfres.Helpers;

/// <summary>
/// Represents a helper class for working with <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexBuffer" /> instances.
/// </summary>
public class VertexBufferHelper
{
	/// <summary>
	/// Gets or sets the <see cref="P:Syroot.NintenTools.NSW.Bfres.Helpers.VertexBufferHelper.ByteOrder" /> in which vertex data will be stored when calling
	/// <see cref="M:Syroot.NintenTools.NSW.Bfres.Helpers.VertexBufferHelper.ToVertexBuffer" />. This should be the same as the remainder of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> in
	/// which it will be stored.
	/// </summary>
	public ByteOrder ByteOrder { get; set; }

	/// <summary>
	/// Gets or sets the number of bones influencing the vertices stored in the buffer. 0 influences equal
	/// rigidbodies (no skinning), 1 equal rigid skinning and 2 or more smooth skinning.
	/// </summary>
	public uint VertexSkinCount { get; set; }

	/// <summary>
	/// Gets or sets the list of <see cref="T:Syroot.NintenTools.NSW.Bfres.Helpers.VertexBufferHelperAttrib" /> instances which store the data.
	/// </summary>
	public IList<VertexBufferHelperAttrib> Attributes { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.Helpers.VertexBufferHelperAttrib" /> instance at the given <paramref name="index" />.
	/// </summary>
	/// <param name="index">The index of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Helpers.VertexBufferHelperAttrib" /> instance.</param>
	/// <returns>The <see cref="T:Syroot.NintenTools.NSW.Bfres.Helpers.VertexBufferHelperAttrib" /> instance at the given index.</returns>
	public VertexBufferHelperAttrib this[int index]
	{
		get
		{
			return Attributes[index];
		}
		set
		{
			Attributes[index] = value;
		}
	}

	/// <summary>
	/// Gets or sets the first <see cref="T:Syroot.NintenTools.NSW.Bfres.Helpers.VertexBufferHelperAttrib" /> instance with the given
	/// <paramref name="attribName" />.
	/// </summary>
	/// <param name="attribName">The name of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Helpers.VertexBufferHelperAttrib" /> instance.</param>
	/// <returns>The <see cref="T:Syroot.NintenTools.NSW.Bfres.Helpers.VertexBufferHelperAttrib" /> instance with the given name.</returns>
	public VertexBufferHelperAttrib this[string attribName]
	{
		get
		{
			foreach (VertexBufferHelperAttrib attribute in Attributes)
			{
				if (attribute.Name == attribName)
				{
					return attribute;
				}
			}
			throw new ArgumentException("No attribute with name " + attribName + " was found.", "attribName");
		}
		set
		{
			int num = 0;
			foreach (VertexBufferHelperAttrib attribute in Attributes)
			{
				if (attribute.Name == attribName)
				{
					Attributes[num] = value;
					return;
				}
				num++;
			}
			throw new ArgumentException("No attribute wFormat_16_16_16_16_Singleith name " + attribName + " was found.", "attribName");
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Helpers.VertexBufferHelper" /> class.
	/// </summary>
	public VertexBufferHelper()
	{
		ByteOrder = ByteOrderHelper.SystemByteOrder;
		Attributes = new List<VertexBufferHelperAttrib>();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Helpers.VertexBufferHelper" /> class with data read from the given
	/// <paramref name="vertexBuffer" />. The data is available in the <paramref name="byteOrder" />, which defaults
	/// to system byte order.
	/// </summary>
	/// <param name="vertexBuffer">The <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexBuffer" /> to initially read data from.</param>
	/// <param name="byteOrder">The <see cref="P:Syroot.NintenTools.NSW.Bfres.Helpers.VertexBufferHelper.ByteOrder" /> in which vertex data is available. <c>null</c> to use
	/// system byte order.</param>
	public VertexBufferHelper(VertexBuffer vertexBuffer, ByteOrder? byteOrder = null)
	{
		ByteOrder = byteOrder ?? ByteOrderHelper.SystemByteOrder;
		VertexSkinCount = vertexBuffer.VertexSkinCount;
		vertexBuffer.Attributes = vertexBuffer.Attributes ?? new List<VertexAttrib>();
		Attributes = new List<VertexBufferHelperAttrib>(vertexBuffer.Attributes.Count);
		foreach (VertexAttrib attribute in vertexBuffer.Attributes)
		{
			Attributes.Add(new VertexBufferHelperAttrib
			{
				Name = attribute.Name,
				Format = attribute.Format,
				Data = FromRawData(vertexBuffer, attribute)
			});
		}
	}

	/// <summary>
	/// Returns a <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexBuffer" /> instance out of the stored helper data.
	/// </summary>
	/// <returns>A new <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexBuffer" />.</returns>
	public VertexBuffer ToVertexBuffer()
	{
		VertexBuffer vertexBuffer = new VertexBuffer();
		vertexBuffer.VertexSkinCount = VertexSkinCount;
		vertexBuffer.MemoryPool = new MemoryPool();
		int num = Attributes[0].Data.Length;
		vertexBuffer.Attributes = new List<VertexAttrib>();
		vertexBuffer.AttributeDict = new ResDict();
		vertexBuffer.Buffers = new List<VertexBuffer.buffData>(Attributes.Count);
		vertexBuffer.StrideArray = new List<VertexBufferStride>();
		vertexBuffer.VertexBufferSizeArray = new List<VertexBufferSize>();
		vertexBuffer.VertexCount = (uint)num;
		foreach (VertexBufferHelperAttrib attribute in Attributes)
		{
			if (num != attribute.Data.Length)
			{
				throw new InvalidDataException("Attribute data arrays have different sizes.");
			}
			vertexBuffer.Attributes.Add(new VertexAttrib
			{
				Name = attribute.Name,
				Format = attribute.Format,
				BufferIndex = (byte)vertexBuffer.Buffers.Count,
				Offset = 0
			});
			vertexBuffer.AttributeDict.Add(attribute.Name);
			VertexBufferSize vertexBufferSize = new VertexBufferSize();
			vertexBufferSize.Size = (uint)ToRawData(attribute).Length;
			VertexBufferStride vertexBufferStride = new VertexBufferStride();
			vertexBufferStride.Stride = (uint)attribute.FormatSize;
			vertexBuffer.StrideArray.Add(vertexBufferStride);
			vertexBuffer.VertexBufferSizeArray.Add(vertexBufferSize);
			vertexBuffer.Buffers.Add(new VertexBuffer.buffData
			{
				Stride = (ushort)attribute.FormatSize,
				Data = ToRawData(attribute),
				buffSize = ToRawData(attribute).Length,
				DataOffset = 0
			});
		}
		return vertexBuffer;
	}

	private Vector4F[] FromRawData(VertexBuffer vertexBuffer, VertexAttrib attrib)
	{
		VertexBuffer.buffData buffData = vertexBuffer.Buffers[attrib.BufferIndex];
		using BinaryDataReader binaryDataReader = new BinaryDataReader(new MemoryStream(buffData.Data));
		binaryDataReader.ByteOrder = ByteOrder.LittleEndian;
		Func<BinaryDataReader, Vector4F> attribCallback = binaryDataReader.GetAttribCallback(attrib.Format);
		Vector4F[] array = new Vector4F[vertexBuffer.VertexCount];
		for (int i = 0; i < vertexBuffer.VertexCount; i++)
		{
			binaryDataReader.Position = attrib.Offset + i * vertexBuffer.StrideArray[attrib.BufferIndex].Stride;
			array[i] = attribCallback(binaryDataReader);
			if (attrib.Name.Contains("_i"))
			{
				AttribFormat format = attrib.Format;
				AttribFormat attribFormat = format;
				if (attribFormat == AttribFormat.Format_16_SNorm)
				{
					array[i].X = array[i].X * 32767f;
				}
			}
		}
		return array;
	}

	private byte[] ToRawData(VertexBufferHelperAttrib helperAttrib)
	{
		byte[] array = new byte[helperAttrib.Data.Length * helperAttrib.FormatSize];
		using (BinaryDataWriter binaryDataWriter = new BinaryDataWriter(new MemoryStream(array, writable: true)))
		{
			binaryDataWriter.ByteOrder = ByteOrder;
			Action<BinaryDataWriter, Vector4F> attribCallback = binaryDataWriter.GetAttribCallback(helperAttrib.Format);
			Vector4F[] data = helperAttrib.Data;
			foreach (Vector4F arg in data)
			{
				attribCallback(binaryDataWriter, arg);
			}
		}
		return array;
	}
}
