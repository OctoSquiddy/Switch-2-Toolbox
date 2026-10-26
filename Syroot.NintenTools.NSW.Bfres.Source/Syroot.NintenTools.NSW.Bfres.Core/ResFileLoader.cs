using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Syroot.BinaryData;

namespace Syroot.NintenTools.NSW.Bfres.Core;

/// <summary>
/// Loads the hierachy and data of a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" />.
/// </summary>
public class ResFileLoader : BinaryDataReader
{
	private IDictionary<uint, IResData> _dataMap;

	internal List<long> RelocatedPointers = new List<long>();

	/// <summary>
	/// Gets the loaded <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> instance.
	/// </summary>
	internal ResFile ResFile { get; }

	/// <summary>
	/// Gets the loaded <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> instance.
	/// </summary>
	internal Model Model { get; }

	/// <summary>
	/// Gets the saved <see cref="T:Syroot.NintenTools.NSW.Bfres.MaterialAnim" /> instance.
	/// </summary>
	internal MaterialAnim MaterialAnim { get; }

	/// <summary>
	/// Gets the saved <see cref="T:Syroot.NintenTools.NSW.Bfres.SceneAnim" /> instance.
	/// </summary>
	internal SceneAnim SceneAnim { get; }

	/// <summary>
	/// Gets the saved <see cref="T:Syroot.NintenTools.NSW.Bfres.ShapeAnim" /> instance.
	/// </summary>
	internal ShapeAnim ShapeAnim { get; }

	/// <summary>
	/// Gets the saved <see cref="T:Syroot.NintenTools.NSW.Bfres.VisibilityAnim" /> instance.
	/// </summary>
	internal VisibilityAnim VisibilityAnim { get; }

	/// <summary>
	/// Gets the saved <see cref="T:Syroot.NintenTools.NSW.Bfres.SkeletalAnim" /> instance.
	/// </summary>
	internal SkeletalAnim SkeletalAnim { get; }

	/// <summary>
	/// Gets the loaded <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" /> instance.
	/// </summary>
	internal Material Material { get; }

	/// <summary>
	/// Gets the saved <see cref="T:Syroot.NintenTools.NSW.Bfres.Skeleton" /> instance.
	/// </summary>
	internal Skeleton Skeleton { get; }

	/// <summary>
	/// Gets the saved <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" /> instance.
	/// </summary>
	internal Bone Bone { get; }

	/// <summary>
	/// Gets the loaded <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" /> instance.
	/// </summary>
	internal Shape Shape { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileLoader" /> class loading data into the given
	/// <paramref name="resFile" /> from the specified <paramref name="stream" /> which is optionally left open.
	/// </summary>
	/// <param name="resFile">The <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> instance to load data into.</param>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to read data from.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after reading, otherwise <c>false</c>.</param>
	internal ResFileLoader(ResFile resFile, Stream stream, bool leaveOpen = false)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		ResFile = resFile;
		_dataMap = new Dictionary<uint, IResData>();
	}

	internal ResFileLoader(Material material, Stream stream, bool leaveOpen = false)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ResFile = new ResFile();
		ByteOrder = ByteOrder.LittleEndian;
		Material = material;
		_dataMap = new Dictionary<uint, IResData>();
	}

	internal ResFileLoader(Shape shape, Stream stream, bool leaveOpen = false)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ResFile = new ResFile();
		ByteOrder = ByteOrder.LittleEndian;
		Shape = shape;
		_dataMap = new Dictionary<uint, IResData>();
	}

	internal ResFileLoader(Model model, ResFile resFile, Stream stream, bool leaveOpen = false)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		Model = model;
		ResFile = resFile;
		_dataMap = new Dictionary<uint, IResData>();
	}

	internal ResFileLoader(SkeletalAnim skeletalAnim, Stream stream, bool leaveOpen = false)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ResFile = new ResFile();
		ByteOrder = ByteOrder.LittleEndian;
		SkeletalAnim = skeletalAnim;
		_dataMap = new Dictionary<uint, IResData>();
	}

	internal ResFileLoader(MaterialAnim materialAnim, Stream stream, bool leaveOpen = false)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ResFile = new ResFile();
		ByteOrder = ByteOrder.LittleEndian;
		MaterialAnim = materialAnim;
		_dataMap = new Dictionary<uint, IResData>();
	}

	internal ResFileLoader(VisibilityAnim visibilityAnim, Stream stream, bool leaveOpen = false)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ResFile = new ResFile();
		ByteOrder = ByteOrder.LittleEndian;
		VisibilityAnim = visibilityAnim;
		_dataMap = new Dictionary<uint, IResData>();
	}

	internal ResFileLoader(ShapeAnim shapeAnim, Stream stream, bool leaveOpen = false)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ResFile = new ResFile();
		ByteOrder = ByteOrder.LittleEndian;
		ShapeAnim = shapeAnim;
		_dataMap = new Dictionary<uint, IResData>();
	}

	internal ResFileLoader(SceneAnim sceneAnim, Stream stream, bool leaveOpen = false)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ResFile = new ResFile();
		ByteOrder = ByteOrder.LittleEndian;
		SceneAnim = sceneAnim;
		_dataMap = new Dictionary<uint, IResData>();
	}

	internal ResFileLoader(Skeleton skeleton, Stream stream, bool leaveOpen = false)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ResFile = new ResFile();
		ByteOrder = ByteOrder.LittleEndian;
		Skeleton = skeleton;
		_dataMap = new Dictionary<uint, IResData>();
	}

	internal ResFileLoader(Bone bone, Stream stream, bool leaveOpen = false)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ResFile = new ResFile();
		ByteOrder = ByteOrder.LittleEndian;
		Bone = bone;
		_dataMap = new Dictionary<uint, IResData>();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileLoader" /> class from the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="resFile">The <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> instance to load data into.</param>
	/// <param name="fileName">The name of the file to load the data from.</param>
	internal ResFileLoader(ResFile resFile, string fileName)
		: this(resFile, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	{
	}

	internal ResFileLoader(Model model, ResFile resFile, string fileName)
		: this(model, resFile, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	{
	}

	internal ResFileLoader(SkeletalAnim skeletonAnim, string fileName)
		: this(skeletonAnim, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	{
	}

	internal ResFileLoader(MaterialAnim materialAnim, string fileName)
		: this(materialAnim, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	{
	}

	internal ResFileLoader(VisibilityAnim visibilityAnim, string fileName)
		: this(visibilityAnim, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	{
	}

	internal ResFileLoader(SceneAnim sceneAnim, string fileName)
		: this(sceneAnim, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	{
	}

	internal ResFileLoader(ShapeAnim shapeAnim, string fileName)
		: this(shapeAnim, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	{
	}

	internal ResFileLoader(Skeleton skeleton, string fileName)
		: this(skeleton, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	{
	}

	internal ResFileLoader(Bone bone, string fileName)
		: this(bone, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	{
	}

	internal ResFileLoader(Material material, string fileName)
		: this(material, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	{
	}

	internal ResFileLoader(Shape shape, string fileName)
		: this(shape, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	{
	}

	private void ReadImportedFileHeader()
	{
		ByteOrder = ByteOrder.BigEndian;
		Seek(8L, SeekOrigin.Begin);
		uint num = ReadUInt32();
		if (num != 0)
		{
			ResFile.SetVersionInfo(num);
		}
		ByteOrder = ByteOrder.LittleEndian;
		Seek(48L, SeekOrigin.Begin);
	}

	internal void ImportMaterials()
	{
		ReadImportedFileHeader();
		((IResData)Material).Load(this);
	}

	internal void ImportSkeleton()
	{
		ReadImportedFileHeader();
		((IResData)Skeleton).Load(this);
	}

	internal void ImportBone()
	{
		ReadImportedFileHeader();
		Seek(4L, SeekOrigin.Current);
		((IResData)Bone).Load(this);
	}

	internal void ImportSkeletonAnimations()
	{
		ReadImportedFileHeader();
		((IResData)SkeletalAnim).Load(this);
	}

	internal void ImportMaterialAnims()
	{
		ReadImportedFileHeader();
		((IResData)MaterialAnim).Load(this);
	}

	internal void ImportBoneVisualAnimations()
	{
		ReadImportedFileHeader();
		((IResData)VisibilityAnim).Load(this);
	}

	internal void ImportShapeAnimations()
	{
		ReadImportedFileHeader();
		((IResData)ShapeAnim).Load(this);
	}

	internal void ImportSceneAnimations()
	{
		ReadImportedFileHeader();
		((IResData)SceneAnim).Load(this);
	}

	internal void ImportModel()
	{
		ReadImportedFileHeader();
		long bufferOffset = BufferInfo.BufferOffset;
		long offset = ReadInt64();
		using (TemporarySeek(offset, SeekOrigin.Begin))
		{
			Seek(8L);
			BufferInfo.BufferOffset = ReadInt64();
		}
		((IResData)Model).Load(this);
		BufferInfo.BufferOffset = bufferOffset;
	}

	internal void ImportShapes(VertexBuffer vtx)
	{
		ReadImportedFileHeader();
		Seek(48L, SeekOrigin.Begin);
		long bufferOffset = BufferInfo.BufferOffset;
		long offset = ReadInt64();
		long offset2 = ReadInt64();
		using (TemporarySeek(offset2, SeekOrigin.Begin))
		{
			Seek(8L);
			BufferInfo.BufferOffset = ReadInt64();
		}
		using (TemporarySeek(offset, SeekOrigin.Begin))
		{
			((IResData)vtx).Load(this);
		}
		Seek(64L, SeekOrigin.Begin);
		((IResData)Shape).Load(this);
		BufferInfo.BufferOffset = bufferOffset;
	}

	/// <summary>
	/// Starts deserializing the data from the <see cref="P:Syroot.NintenTools.NSW.Bfres.Core.ResFileLoader.ResFile" /> root.
	/// </summary>
	internal void Execute()
	{
		((IResData)ResFile).Load(this);
	}

	/// <summary>
	/// Reads and returns an <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.IResData" /> instance of type <typeparamref name="T" /> from the following
	/// offset or returns <c>null</c> if the read offset is 0.
	/// </summary>
	/// <typeparam name="T">The type of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.IResData" /> to read.</typeparam>
	/// <returns>The <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.IResData" /> instance or <c>null</c>.</returns>
	[DebuggerStepThrough]
	internal T Load<T>(bool Relocated = false) where T : IResData, new()
	{
		long num = ReadOffset();
		if (num == 0)
		{
			return default;
		}
		if (Relocated)
		{
		}
		using (TemporarySeek(num, SeekOrigin.Begin))
		{
			return ReadResData<T>();
		}
	}

	internal T Load<T>(uint offset) where T : IResData, new()
	{
		if (offset == 0)
		{
			return default;
		}
		using (TemporarySeek(offset, SeekOrigin.Begin))
		{
			return ReadResData<T>();
		}
	}

	/// <summary>
	/// Reads and returns an instance of arbitrary type <typeparamref name="T" /> from the following offset with the
	/// given <paramref name="callback" /> or returns <c>null</c> if the read offset is 0.
	/// </summary>
	/// <typeparam name="T">The type of the data to read.</typeparam>
	/// <param name="callback">The callback to read the instance data with.</param>
	/// <param name="offset">The optional offset to use instead of reading a following one.</param>
	/// <returns>The data instance or <c>null</c>.</returns>
	/// <remarks>Offset required for ExtFile header (offset specified before size).</remarks>
	[DebuggerStepThrough]
	internal T LoadCustom<T>(Func<T> callback, long? offset = null)
	{
		offset = offset ?? ReadOffset();
		if (offset == 0)
		{
			return default;
		}
		using (TemporarySeek(offset.Value, SeekOrigin.Begin))
		{
			return callback();
		}
	}

	/// <summary>
	/// Reads and returns an <see cref="!:ResDict&lt;T&gt;" /> instance with elements of type <typeparamref name="T" /> from
	/// the following offset or returns an empty instance if the read offset is 0.
	/// </summary>
	/// <typeparam name="T">The type of the <see cref="!:IRes Data" /> elements.</typeparam>
	/// <returns>The <see cref="!:ResDict&lt;T&gt;" /> instance.</returns>
	[DebuggerStepThrough]
	internal ResDict LoadDict()
	{
		long num = ReadOffset();
		if (num == 0)
		{
			return new ResDict();
		}
		using (TemporarySeek(num, SeekOrigin.Begin))
		{
			ResDict resDict = new ResDict();
			((IResData)resDict).Load(this);
			return resDict;
		}
	}

	/// <summary>
	/// Reads and returns an <see cref="T:System.Collections.Generic.IList`1" /> instance with <paramref name="count" /> elements of type
	/// <typeparamref name="T" /> from the following offset or returns <c>null</c> if the read offset is 0.
	/// </summary>
	/// <typeparam name="T">The type of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.IResData" /> elements.</typeparam>
	/// <param name="count">The number of elements to expect for the list.</param>
	/// <param name="offset">The optional offset to use instead of reading a following one.</param>
	/// <returns>The <see cref="T:System.Collections.Generic.IList`1" /> instance or <c>null</c>.</returns>
	/// <remarks>Offset required for FMDL FVTX lists (offset specified before count).</remarks>
	[DebuggerStepThrough]
	internal IList<T> LoadList<T>(int count, long? offset = null) where T : IResData, new()
	{
		List<T> list = new List<T>(count);
		offset = offset ?? ReadOffset();
		if (offset == 0 || count == 0)
		{
			return list;
		}
		using (TemporarySeek(offset.Value, SeekOrigin.Begin))
		{
			while (count > 0)
			{
				list.Add(ReadResData<T>());
				count--;
			}
			return list;
		}
	}

	/// <summary>
	/// Reads and returns a <see cref="T:System.String" /> instance from the following offset or <c>null</c> if the read
	/// offset is 0.
	/// </summary>
	/// <param name="encoding">The optional encoding of the text.</param>
	/// <returns>The read text.</returns>
	[DebuggerStepThrough]
	internal string LoadString(Encoding encoding = null)
	{
		long num = ReadOffset();
		if (num == 0)
		{
			return null;
		}
		if (StringCache.Strings.ContainsKey(num))
		{
			return StringCache.Strings[num];
		}
		encoding = encoding ?? Encoding;
		using (TemporarySeek(num, SeekOrigin.Begin))
		{
			short num2 = ReadInt16();
			return ReadString(BinaryStringFormat.ZeroTerminated, encoding);
		}
	}

	internal void LoadHeaderBlock()
	{
		uint offset = ReadUInt32();
		long size = ReadInt64();
		SetHeaderBlock(offset, size);
	}

	internal byte[] SetHeaderBlock(uint Offset, long Size)
	{
		using (TemporarySeek(Offset, SeekOrigin.Begin))
		{
			return ReadBytes((int)Size);
		}
	}

	/// <summary>
	/// Reads and returns <paramref name="count" /> <see cref="T:System.String" /> instances from the following offsets.
	/// </summary>
	/// <param name="count">The number of instances to read.</param>
	/// <param name="encoding">The optional encoding of the texts.</param>
	/// <returns>The read texts.</returns>
	[DebuggerStepThrough]
	internal IList<string> LoadStrings(int count, Encoding encoding = null)
	{
		long[] array = ReadOffsets(count);
		encoding = encoding ?? Encoding;
		string[] array2 = new string[array.Length];
		using (TemporarySeek())
		{
			for (int i = 0; i < array.Length; i++)
			{
				long num = array[i];
				if (num != 0)
				{
					if (StringCache.Strings.ContainsKey(num))
					{
						array2[i] = StringCache.Strings[num];
						continue;
					}
					Position = num;
					short num2 = ReadInt16();
					array2[i] = ReadString(BinaryStringFormat.ZeroTerminated, encoding);
				}
			}
			return array2;
		}
	}

	/// <summary>
	/// Reads a BFRES signature consisting of 4 ASCII characters encoded as an <see cref="T:System.UInt32" /> and checks for
	/// validity.
	/// </summary>
	/// <param name="validSignature">A valid signature.</param>
	internal void CheckSignature(string validSignature)
	{
		string text = ReadString(4, Encoding.ASCII);
		if (text != validSignature)
		{
			throw new ResException("Invalid signature, expected '" + validSignature + "' but got '" + text + "'.");
		}
	}

	/// <summary>
	/// Reads a BFRES offset which is the absolute address.
	/// </summary>
	/// <returns>The absolute address of the offset.</returns>
	internal long ReadOffset(bool Relocated = false)
	{
		long position = Position;
		long num = ReadInt64();
		return (num == 0L) ? 0 : num;
	}

	/// <summary>
	/// Reads BFRES offsets which is the absolute addresses.+
	/// </summary>
	/// <param name="count">The number of offsets to read.</param>
	/// <returns>The absolute addresses of the offsets.</returns>
	internal long[] ReadOffsets(int count)
	{
		long[] array = new long[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = ReadOffset();
		}
		return array;
	}

	[DebuggerStepThrough]
	private T ReadResData<T>() where T : IResData, new()
	{
		uint key = (uint)Position;
		T val = new T();
		val.Load(this);
		if (_dataMap.TryGetValue(key, out var value))
		{
			return (T)value;
		}
		_dataMap.Add(key, val);
		return val;
	}
}
