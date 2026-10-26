using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Syroot.BinaryData;
using Syroot.Maths;

namespace Syroot.NintenTools.NSW.Bfres.Core;

/// <summary>
/// Saves the hierachy and data of a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" />.
/// </summary>
public class ResFileSaver : BinaryDataWriter
{
	[DebuggerDisplay("{Type} {Data}")]
	private class ItemEntry
	{
		internal object Data;

		internal ItemEntryType Type;

		internal List<uint> Offsets;

		internal uint? Target;

		internal Action Callback;

		internal int Index;

		internal ItemEntry(object data, ItemEntryType type, uint? offset = null, uint? target = null, Action callback = null, int index = -1)
		{
			Data = data;
			Type = type;
			Offsets = new List<uint>();
			if (offset.HasValue)
			{
				Offsets.Add(offset.Value);
			}
			Callback = callback;
			Target = target;
			Index = index;
		}
	}

	private enum ItemEntryType
	{
		List,
		Dict,
		ResData,
		Custom
	}

	private class RelocationSection
	{
		internal List<RelocationEntry> Entries;

		internal int EntryIndex;

		internal uint Size;

		internal uint Position;

		internal RelocationSection(uint position, int entryIndex, uint size, List<RelocationEntry> entries)
		{
			Position = position;
			EntryIndex = entryIndex;
			Size = size;
			Entries = entries;
		}
	}

	private class RelocationEntry
	{
		internal uint Position;

		internal uint PadingCount;

		internal uint StructCount;

		internal uint OffsetCount;

		internal string Hint;

		internal RelocationEntry(uint position, uint offsetCount, uint structCount, uint padingCount, string hint)
		{
			Position = position;
			StructCount = structCount;
			OffsetCount = offsetCount;
			PadingCount = padingCount;
			Hint = hint;
		}
	}

	private class StringEntry
	{
		internal List<uint> Offsets;

		internal Encoding Encoding;

		internal StringEntry(uint offset, Encoding encoding = null)
		{
			Offsets = new List<uint>(new uint[1] { offset });
			Encoding = encoding;
		}
	}

	private class BlockEntry
	{
		internal List<uint> Offsets;

		internal int Alignment;

		internal Action Callback;

		internal BlockEntry(uint offset, int alignment, Action callback)
		{
			Offsets = new List<uint> { offset };
			Alignment = alignment;
			Callback = callback;
		}
	}

	/// <summary>
	/// Gets or sets a data block alignment typically seen with <see cref="!:Buffer.Data" />.
	/// </summary>
	internal const int AlignmentVertexBuffer = 8;

	internal const int Section1 = 1;

	internal const int Section2 = 2;

	internal const int Section3 = 3;

	internal const int Section4 = 4;

	internal const int Section5 = 5;

	public bool SaveIndexBufferRuntimeData = false;

	public bool SaveVertexBufferRuntimeData = false;

	private uint _ofsFileName;

	private uint _ofsFileNameString;

	private uint _ofsFileSize;

	private uint _ofsStringPool;

	private uint _ofsIndexBuffer;

	private uint _ofsRelocationTable;

	private uint _ofsEndOfBlock;

	private uint _ofsEndOfStringTable;

	private long bufferInfoOffset;

	private uint _ofsVertexBuffer;

	private long memoryPoolOffset;

	private long _ofsTotalBufferSize;

	private uint _ofsMemoryPool;

	private uint _MemoryPoolSize;

	private uint _ofsExternalFileBlock;

	private List<long> _savedMemoryPoolPointers;

	private List<RelocationEntry> _savedSection1Entries;

	private List<RelocationEntry> _savedSection2Entries;

	private List<RelocationEntry> _savedSection3Entries;

	private List<RelocationEntry> _savedSection4Entries;

	private List<RelocationEntry> _savedSection5Entries;

	private string _fileName;

	private List<ItemEntry> _savedItems;

	private List<long> _savedHeaderBlockPositions;

	private IDictionary<string, StringEntry> _savedStrings;

	private IDictionary<object, BlockEntry> _savedBlocks;

	private List<RelocationSection> _savedRelocatedSections;

	private uint SectionCount = 5u;

	/// <summary>
	/// Gets the saved <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> instance.
	/// </summary>
	internal ResFile ResFile { get; }

	/// <summary>
	/// Gets the saved <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> instance.
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
	/// Gets the saved <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" /> instance.
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
	/// Gets the saved <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" /> instance.
	/// </summary>
	internal Shape Shape { get; }

	/// <summary>
	/// Gets the current index when writing lists or dicts.
	/// </summary>
	internal int CurrentIndex { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> class saving data from the given
	/// <paramref name="resFile" /> into the specified <paramref name="stream" /> which is optionally left open.
	/// </summary>
	/// <param name="resFile">The <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> instance to save data from.</param>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to save data into.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
	internal ResFileSaver(ResFile resFile, Stream stream, bool leaveOpen)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		ResFile = resFile;
	}

	internal ResFileSaver(Model model, ResFile resFile, Stream stream, bool leaveOpen)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		Model = model;
		ResFile = resFile;
	}

	internal ResFileSaver(MaterialAnim materialAnim, ResFile resFile, Stream stream, bool leaveOpen)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		MaterialAnim = materialAnim;
		ResFile = resFile;
	}

	internal ResFileSaver(ShapeAnim shapeAnim, ResFile resFile, Stream stream, bool leaveOpen)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		ShapeAnim = shapeAnim;
		ResFile = resFile;
	}

	internal ResFileSaver(SceneAnim sceneAnim, ResFile resFile, Stream stream, bool leaveOpen)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		SceneAnim = sceneAnim;
		ResFile = resFile;
	}

	internal ResFileSaver(VisibilityAnim visibilityAnim, ResFile resFile, Stream stream, bool leaveOpen)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		VisibilityAnim = visibilityAnim;
		ResFile = resFile;
	}

	internal ResFileSaver(Material material, ResFile resFile, Stream stream, bool leaveOpen)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		Material = material;
		ResFile = resFile;
	}

	internal ResFileSaver(Shape shape, ResFile resFile, Stream stream, bool leaveOpen)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		Shape = shape;
		ResFile = resFile;
	}

	internal ResFileSaver(Skeleton skeleton, ResFile resFile, Stream stream, bool leaveOpen)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		Skeleton = skeleton;
		ResFile = resFile;
	}

	internal ResFileSaver(Bone bone, ResFile resFile, Stream stream, bool leaveOpen)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		Bone = bone;
		ResFile = resFile;
	}

	internal ResFileSaver(SkeletalAnim skeletonAnim, ResFile resFile, Stream stream, bool leaveOpen)
		: base(stream, Encoding.ASCII, leaveOpen)
	{
		ByteOrder = ByteOrder.LittleEndian;
		SkeletalAnim = skeletonAnim;
		ResFile = resFile;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> class for the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="resFile">The <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> instance to save.</param>
	/// <param name="fileName">The name of the file to save the data into.</param>
	internal ResFileSaver(ResFile resFile, string fileName)
		: this(resFile, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), leaveOpen: false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> class for the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="model">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> instance to save.</param>
	/// <param name="fileName">The name of the file to save the data into.</param>
	internal ResFileSaver(Model model, ResFile resFile, string fileName)
		: this(model, resFile, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), leaveOpen: false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> class for the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="skeletonAnim">The <see cref="T:Syroot.NintenTools.NSW.Bfres.SkeletalAnim" /> instance to save.</param>
	/// <param name="fileName">The name of the file to save the data into.</param>
	internal ResFileSaver(SkeletalAnim skeletonAnim, ResFile resFile, string fileName)
		: this(skeletonAnim, resFile, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), leaveOpen: false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> class for the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="materialAnim">The <see cref="T:Syroot.NintenTools.NSW.Bfres.MaterialAnim" /> instance to save.</param>
	/// <param name="fileName">The name of the file to save the data into.</param>
	internal ResFileSaver(MaterialAnim materialAnim, ResFile resFile, string fileName)
		: this(materialAnim, resFile, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), leaveOpen: false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> class for the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="materialAnim">The <see cref="T:Syroot.NintenTools.NSW.Bfres.ShapeAnim" /> instance to save.</param>
	/// <param name="fileName">The name of the file to save the data into.</param>
	internal ResFileSaver(ShapeAnim shapeAnim, ResFile resFile, string fileName)
		: this(shapeAnim, resFile, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), leaveOpen: false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> class for the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="sceneAnim">The <see cref="T:Syroot.NintenTools.NSW.Bfres.SceneAnim" /> instance to save.</param>
	/// <param name="fileName">The name of the file to save the data into.</param>
	internal ResFileSaver(SceneAnim sceneAnim, ResFile resFile, string fileName)
		: this(sceneAnim, resFile, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), leaveOpen: false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> class for the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="visibilityAnim">The <see cref="T:Syroot.NintenTools.NSW.Bfres.VisibilityAnim" /> instance to save.</param>
	/// <param name="fileName">The name of the file to save the data into.</param>
	internal ResFileSaver(VisibilityAnim visibilityAnim, ResFile resFile, string fileName)
		: this(visibilityAnim, resFile, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), leaveOpen: false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> class for the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="material">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" /> instance to save.</param>
	/// <param name="fileName">The name of the file to save the data into.</param>
	internal ResFileSaver(Material material, ResFile resFile, string fileName)
		: this(material, resFile, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), leaveOpen: false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> class for the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="shape">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" /> instance to save.</param>
	/// <param name="fileName">The name of the file to save the data into.</param>
	internal ResFileSaver(Shape shape, ResFile resFile, string fileName)
		: this(shape, resFile, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), leaveOpen: false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> class for the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="bone">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" /> instance to save.</param>
	/// <param name="fileName">The name of the file to save the data into.</param>
	internal ResFileSaver(Bone bone, ResFile resFile, string fileName)
		: this(bone, resFile, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), leaveOpen: false)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> class for the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="skeleton">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Skeleton" /> instance to save.</param>
	/// <param name="fileName">The name of the file to save the data into.</param>
	internal ResFileSaver(Skeleton skeleton, ResFile resFile, string fileName)
		: this(skeleton, resFile, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), leaveOpen: false)
	{
	}

	internal void ExportSkeletonAnimation()
	{
		SetupLists();
		WriteHeader("fresSUB", "FSKA\0\0\0\0");
		((IResData)SkeletalAnim).Save(this);
		WriteSkeletonAnimations(SkeletalAnim);
		WriteStrings();
	}

	internal void ExportMaterialAnimation()
	{
		SetupLists();
		WriteHeader("fresSUB", "FMAA\0\0\0\0");
		((IResData)MaterialAnim).Save(this);
		WriteMaterialAnimations(MaterialAnim);
		WriteStrings();
	}

	internal void ExportBoneVisualAnimation()
	{
		SetupLists();
		WriteHeader("fresSUB", "FVBN\0\0\0\0");
		((IResData)VisibilityAnim).Save(this);
		WriteBoneVisabiltyAnimations(VisibilityAnim);
		WriteStrings();
	}

	internal void ExportShapeAnimation()
	{
		SetupLists();
		WriteHeader("fresSUB", "FSHA\0\0\0\0");
		((IResData)ShapeAnim).Save(this);
		WriteShapeAnimations(ShapeAnim);
		WriteStrings();
	}

	internal void ExportSceneAnimation()
	{
		SetupLists();
		WriteHeader("fresSUB", "FSCN\0\0\0\0");
		((IResData)SceneAnim).Save(this);
		WriteSceneAnimations(SceneAnim);
		WriteStrings();
	}

	internal void ExportBone()
	{
		SetupLists();
		WriteHeader("fmdlSUB", "FSKL\0\0\0\0");
		WriteSignature("BONE");
		((IResData)Bone).Save(this);
		WriteStrings();
	}

	internal void ExportSkeleton()
	{
		SetupLists();
		WriteHeader("fmdlSUB", "FSKL\0\0\0\0");
		((IResData)Skeleton).Save(this);
		WriteSkeleton(Skeleton);
		WriteStrings();
	}

	internal void ExportMaterials()
	{
		SetupLists();
		WriteHeader("fmdlSUB", "FMAT\0\0\0\0");
		((IResData)Material).Save(this);
		WriteMaterials(Material);
		WriteStrings();
		Flush();
	}

	internal void WriteHeader(string SubSection, string Magic, int Offset = 32)
	{
		ByteOrder = ByteOrder.BigEndian;
		_savedItems = new List<ItemEntry>();
		_savedStrings = new SortedDictionary<string, StringEntry>(ResStringComparer.Instance);
		_savedBlocks = new Dictionary<object, BlockEntry>();
		Write((byte)127);
		Write(Encoding.ASCII.GetBytes(SubSection));
		Write(ResFile.Version);
		WriteSignature(Magic);
		Write((int)(Offset - Position));
		Write(1);
		Write(0);
		Seek(48L, SeekOrigin.Begin);
		ByteOrder = ByteOrder.LittleEndian;
	}

	internal void ExportModel()
	{
		SetupLists();
		WriteHeader("fresSUB", "FMDL\0\0\0\0", 48);
		long offset = SaveOffset();
		foreach (Shape shape in Model.Shapes)
		{
			shape.VertexBuffer = Model.VertexBuffers[shape.VertexBufferIndex];
		}
		((IResData)Model).Save(this);
		WriteModel(Model);
		WriteModelBlock(Model);
		WriteStrings();
		WriteOffset(offset);
		((IResData)ResFile.BufferInfo).Save(this);
		WriteIndexBuffer();
		WriteVertexBuffer();
		WriteMemoryPool();
	}

	internal void ExportShape()
	{
		SetupLists();
		WriteHeader("fmdlSUB", "FSHP\0\0\0\0");
		long offset = SaveOffset();
		long offset2 = SaveOffset();
		((IResData)Shape).Save(this);
		WriteShapes(Shape);
		WriteOffset(offset);
		((IResData)Shape.VertexBuffer).Save(this);
		WriteVertexBuffers(Shape.VertexBuffer);
		WriteStrings();
		WriteOffset(offset2);
		((IResData)ResFile.BufferInfo).Save(this);
		WriteIndexBuffer();
		WriteVertexBuffer();
		WriteMemoryPool();
		Flush();
	}

	internal void SetupLists()
	{
		_savedItems = new List<ItemEntry>();
		_savedStrings = new SortedDictionary<string, StringEntry>(ResStringComparer.Instance);
		_savedBlocks = new Dictionary<object, BlockEntry>();
		_savedHeaderBlockPositions = new List<long>();
		_savedMemoryPoolPointers = new List<long>();
		_savedRelocatedSections = new List<RelocationSection>();
		_savedSection1Entries = new List<RelocationEntry>();
		_savedSection2Entries = new List<RelocationEntry>();
		_savedSection3Entries = new List<RelocationEntry>();
		_savedSection4Entries = new List<RelocationEntry>();
		_savedSection5Entries = new List<RelocationEntry>();
	}

	/// <summary>
	/// Starts serializing the data from the <see cref="P:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver.ResFile" /> root.
	/// </summary>
	internal void Execute()
	{
		SetupLists();
		((IResData)ResFile).Save(this);
		if (ResFile.Models.Count > 0)
		{
			WriteOffset(ResFile.ModelOffset);
			foreach (Model model in ResFile.Models)
			{
				((IResData)model).Save(this);
			}
		}
		if (ResFile.SkeletalAnims.Count > 0)
		{
			if (ResFile.VersionMajor2 >= 9)
			{
				SaveRelocateEntryToSection(Position + 8, 2u, (uint)ResFile.SkeletalAnims.Count, 8u, 1, "Skeleton Animation");
				SaveRelocateEntryToSection(Position + 32, 4u, (uint)ResFile.SkeletalAnims.Count, 6u, 1, "Skeleton Animation");
			}
			else
			{
				SaveRelocateEntryToSection(Position + 16, 2u, (uint)ResFile.SkeletalAnims.Count, 10u, 1, "Skeleton Animation");
				SaveRelocateEntryToSection(Position + 40, 4u, (uint)ResFile.SkeletalAnims.Count, 8u, 1, "Skeleton Animation");
			}
			WriteOffset(ResFile.SkeletonAnimationOffset);
			foreach (SkeletalAnim skeletalAnim in ResFile.SkeletalAnims)
			{
				((IResData)skeletalAnim).Save(this);
			}
		}
		if (ResFile.MaterialAnims.Count > 0)
		{
			WriteOffset(ResFile.MaterialAnimationOffset);
			foreach (MaterialAnim materialAnim in ResFile.MaterialAnims)
			{
				((IResData)materialAnim).Save(this);
			}
		}
		if (ResFile.BoneVisibilityAnims.Count > 0)
		{
			if (ResFile.VersionMajor2 >= 9)
			{
				SaveRelocateEntryToSection(Position + 8, 2u, (uint)ResFile.BoneVisibilityAnims.Count, 10u, 1, "Bone Vis Animation");
				SaveRelocateEntryToSection(Position + 32, 6u, (uint)ResFile.BoneVisibilityAnims.Count, 6u, 1, "Bone Vis Animation");
			}
			else
			{
				SaveRelocateEntryToSection(Position + 16, 2u, (uint)ResFile.BoneVisibilityAnims.Count, 11u, 1, "Bone Vis Animation");
				SaveRelocateEntryToSection(Position + 40, 6u, (uint)ResFile.BoneVisibilityAnims.Count, 7u, 1, "Bone Vis Animation");
			}
			WriteOffset(ResFile.BoneVisAnimationOffset);
			foreach (VisibilityAnim boneVisibilityAnim in ResFile.BoneVisibilityAnims)
			{
				((IResData)boneVisibilityAnim).Save(this);
			}
		}
		if (ResFile.ShapeAnims.Count > 0)
		{
			if (ResFile.VersionMajor2 >= 9)
			{
				SaveRelocateEntryToSection(Position + 8, 2u, (uint)ResFile.ShapeAnims.Count, 10u, 1, "Shape Animation");
				SaveRelocateEntryToSection(Position + 32, 6u, (uint)ResFile.ShapeAnims.Count, 6u, 1, "Shape Animation");
			}
			else
			{
				SaveRelocateEntryToSection(Position + 16, 2u, (uint)ResFile.ShapeAnims.Count, 11u, 1, "Shape Animation");
				SaveRelocateEntryToSection(Position + 40, 6u, (uint)ResFile.ShapeAnims.Count, 7u, 1, "Shape Animation");
			}
			WriteOffset(ResFile.ShapeAnimationOffset);
			foreach (ShapeAnim shapeAnim in ResFile.ShapeAnims)
			{
				((IResData)shapeAnim).Save(this);
			}
		}
		if (ResFile.SceneAnims.Count > 0)
		{
			if (ResFile.VersionMajor2 >= 9)
			{
				SaveRelocateEntryToSection(Position + 8, 2u, (uint)ResFile.SceneAnims.Count, 10u, 1, "Scene Animation");
				SaveRelocateEntryToSection(Position + 32, 6u, (uint)ResFile.SceneAnims.Count, 6u, 1, "Scene Animation");
			}
			else
			{
				SaveRelocateEntryToSection(Position + 16, 10u, (uint)ResFile.SceneAnims.Count, 3u, 1, "Scene Animation");
			}
			WriteOffset(ResFile.SceneAnimationOffset);
			foreach (SceneAnim sceneAnim in ResFile.SceneAnims)
			{
				((IResData)sceneAnim).Save(this);
			}
		}
		if (ResFile.BufferInfo != null)
		{
			WriteOffset(ResFile.BufferInfoOffset);
			((IResData)ResFile.BufferInfo).Save(this);
		}
		else if (ResFile.VersionMajor2 != 9)
		{
			Write(34);
			Seek(28L);
		}
		if (ResFile.ExternalFiles.Count > 0)
		{
			WriteOffset(ResFile.ExternalFileOffset);
			foreach (ExternalFile externalFile in ResFile.ExternalFiles)
			{
				((IResData)externalFile).Save(this);
			}
		}
		if (ResFile.Models.Count > 0)
		{
			WriteOffset(ResFile.ModelDictOffset);
			((IResData)ResFile.ModelDict).Save(this);
		}
		if (ResFile.SkeletalAnims.Count > 0)
		{
			WriteOffset(ResFile.SkeletonAnimationDictOffset);
			((IResData)ResFile.SkeletalAnimDict).Save(this);
		}
		if (ResFile.MaterialAnimDict.Count > 0)
		{
			WriteOffset(ResFile.MaterialAnimationnDictOffset);
			((IResData)ResFile.MaterialAnimDict).Save(this);
		}
		if (ResFile.BoneVisibilityAnimDict.Count > 0)
		{
			WriteOffset(ResFile.BoneVisAnimationDictOffset);
			((IResData)ResFile.BoneVisibilityAnimDict).Save(this);
		}
		if (ResFile.ShapeAnimDict.Count > 0)
		{
			WriteOffset(ResFile.ShapeAnimationDictOffset);
			((IResData)ResFile.ShapeAnimDict).Save(this);
		}
		if (ResFile.SceneAnimDict.Count > 0)
		{
			WriteOffset(ResFile.SceneAnimationDictOffset);
			((IResData)ResFile.SceneAnimDict).Save(this);
		}
		if (ResFile.ExternalFiles.Count > 0)
		{
			WriteOffset(ResFile.ExternalFileDictOffset);
			((IResData)ResFile.ExternalFileDict).Save(this);
		}
		foreach (Model model2 in ResFile.Models)
		{
			WriteModel(model2);
		}
		foreach (Model model3 in ResFile.Models)
		{
			WriteModelBlock(model3);
		}
		foreach (SkeletalAnim skeletalAnim2 in ResFile.SkeletalAnims)
		{
			WriteSkeletonAnimations(skeletalAnim2);
		}
		foreach (MaterialAnim materialAnim2 in ResFile.MaterialAnims)
		{
			WriteMaterialAnimations(materialAnim2);
		}
		foreach (VisibilityAnim boneVisibilityAnim2 in ResFile.BoneVisibilityAnims)
		{
			WriteBoneVisabiltyAnimations(boneVisibilityAnim2);
		}
		foreach (ShapeAnim shapeAnim2 in ResFile.ShapeAnims)
		{
			WriteShapeAnimations(shapeAnim2);
		}
		foreach (SceneAnim sceneAnim2 in ResFile.SceneAnims)
		{
			WriteSceneAnimations(sceneAnim2);
		}
		WriteStrings();
		if (ResFile.BufferInfo != null)
		{
			WriteIndexBuffer();
			WriteVertexBuffer();
		}
		if (ResFile.MemoryPool != null)
		{
			WriteMemoryPool();
		}
		WriteBlocks();
		SetupRelocationTable();
		Align(8);
		WriteRelocationTable();
		for (int i = 0; i < _savedHeaderBlockPositions.Count; i++)
		{
			Position = _savedHeaderBlockPositions[i];
			if (i == 0)
			{
				Write((ushort)(_savedHeaderBlockPositions[1] - 4));
			}
			else if (i == _savedHeaderBlockPositions.Count - 1)
			{
				Write(0);
				Write(_ofsEndOfBlock - _savedHeaderBlockPositions[i]);
			}
			else if (i < _savedHeaderBlockPositions.Count - 1)
			{
				uint num = (uint)(_savedHeaderBlockPositions[i + 1] - _savedHeaderBlockPositions[i]);
				WriteHeaderBlock(num, num);
			}
		}
		Position = _ofsFileName;
		Write(_ofsFileNameString + 2);
		Position = _ofsFileSize;
		Write((uint)BaseStream.Length);
		Flush();
	}

	private void WriteModel(Model mdl)
	{
		if (mdl.VertexBuffers.Count > 0)
		{
			int num = 0;
			WriteOffset(mdl.VertexBufferOffset);
			foreach (VertexBuffer vertexBuffer in mdl.VertexBuffers)
			{
				CurrentIndex = num++;
				((IResData)vertexBuffer).Save(this);
			}
		}
		if (mdl.Materials.Count > 0)
		{
			int num2 = 0;
			WriteOffset(mdl.MaterialsOffset);
			foreach (Material material in mdl.Materials)
			{
				CurrentIndex = num2++;
				((IResData)material).Save(this);
			}
		}
		if (mdl.Shapes.Count > 0)
		{
			int num3 = 0;
			WriteOffset(mdl.ShapeOffset);
			foreach (Shape shape in mdl.Shapes)
			{
				CurrentIndex = num3++;
				((IResData)shape).Save(this);
			}
		}
		if (mdl.UserData.Count > 0)
		{
			SaveUserData(mdl.UserData, mdl.PosUserDataOffset);
		}
		if (mdl.Skeleton != null)
		{
			WriteOffset(mdl.SkeletonOffset);
			if (mdl.Skeleton != null)
			{
				((IResData)mdl.Skeleton).Save(this);
			}
		}
	}

	private void WriteModelBlock(Model mdl)
	{
		if (mdl.Skeleton.Bones.Count > 0)
		{
			if (ResFile.VersionMajor2 >= 10)
			{
				SaveRelocateEntryToSection(Position, 3u, (uint)mdl.Skeleton.Bones.Count, 8u, 1, "Bone array");
			}
			else if (ResFile.VersionMajor2 == 8 || ResFile.VersionMajor2 == 9)
			{
				SaveRelocateEntryToSection(Position, 3u, (uint)mdl.Skeleton.Bones.Count, 9u, 1, "Bone array");
			}
			else
			{
				SaveRelocateEntryToSection(Position, 3u, (uint)mdl.Skeleton.Bones.Count, 7u, 1, "Bone array");
			}
		}
		WriteSkeleton(mdl.Skeleton);
		if (mdl.ShapeDict.Count > 0)
		{
			WriteOffset(mdl.ShapeDictOffset);
			((IResData)mdl.ShapeDict).Save(this);
		}
		if (mdl.MaterialDict.Count > 0)
		{
			WriteOffset(mdl.MaterialsDictOffset);
			((IResData)mdl.MaterialDict).Save(this);
		}
		if (mdl.UserData.Count > 0)
		{
			WriteOffset(mdl.PosUserDataDictOffset);
			((IResData)mdl.UserDataDict).Save(this);
			SaveUserDataData(mdl.UserData);
			Align(8);
		}
		foreach (Shape shape in mdl.Shapes)
		{
			WriteShapes(shape);
		}
		foreach (VertexBuffer vertexBuffer in mdl.VertexBuffers)
		{
			WriteVertexBuffers(vertexBuffer);
		}
		foreach (Material material in mdl.Materials)
		{
			WriteMaterials(material);
		}
	}

	private void WriteSkeleton(Skeleton skl)
	{
		if (skl.Bones.Count > 0)
		{
			int num = 0;
			WriteOffset(skl.PosBoneArrayOffset);
			foreach (Bone bone in skl.Bones)
			{
				CurrentIndex = num++;
				((IResData)bone).Save(this);
			}
		}
		if (skl.MatrixToBoneList.Count > 0)
		{
			Align(8);
			WriteOffset(skl.PosMatrixToBoneListOffset);
			Write(skl.MatrixToBoneList);
		}
		if (skl.InverseModelMatrices.Count > 0)
		{
			Align(8);
			WriteOffset(skl.PosInverseModelMatricesOffset);
			skl.WriteMatrices(this);
		}
		ushort[] userIndices = skl.userIndices;
		if (userIndices != null && userIndices.Length != 0)
		{
			Align(8);
			WriteOffset(skl.PosUserPointer);
			Write(skl.userIndices);
		}
		if (skl.BoneDict.Count > 0)
		{
			Align(8);
			WriteOffset(skl.PosBoneDictOffset);
			((IResData)skl.BoneDict).Save(this);
		}
		if (skl.Bones.Count <= 0)
		{
			return;
		}
		foreach (Bone bone2 in skl.Bones)
		{
			if (bone2.UserData.Count > 0)
			{
				SaveUserData(bone2.UserData, bone2.PosUserDataOffset);
			}
			if (bone2.UserData.Count > 0)
			{
				WriteOffset(bone2.PosUserDataDictOffset);
				((IResData)bone2.UserDataDict).Save(this);
				SaveUserDataData(bone2.UserData);
				Align(8);
			}
		}
	}

	private void WriteShapes(Shape shp)
	{
		WriteOffset(shp.PosMeshArrayOffset);
		foreach (Mesh mesh in shp.Meshes)
		{
			((IResData)mesh).Save(this);
		}
		if (shp.SkinBoneIndices.Count > 0)
		{
			WriteOffset(shp.PosSkinBoneIndicesOffset);
			Write(shp.SkinBoneIndices);
		}
		if (shp.SubMeshBoundings.Count > 0)
		{
			Align(8);
			WriteOffset(shp.PosSubMeshBoundingsOffset);
			shp.WriteBoudnings(this);
		}
		if (shp.KeyShapes.Count > 0)
		{
			WriteOffset(shp.PosKeyShapesOffset);
			foreach (KeyShape keyShape in shp.KeyShapes)
			{
				((IResData)keyShape).Save(this);
			}
		}
		if (shp.RadiusArray.Count > 0)
		{
			WriteOffset(shp.PosRadiusArrayOffset);
			if (ResFile.VersionMajor2 >= 10)
			{
				foreach (Vector4F boundingRadius in shp.BoundingRadiusList)
				{
					Write(boundingRadius.X);
					Write(boundingRadius.Y);
					Write(boundingRadius.Z);
					Write(boundingRadius.W);
				}
			}
			else
			{
				Write(shp.RadiusArray);
			}
		}
		foreach (Mesh mesh2 in shp.Meshes)
		{
			Align(8);
			WriteOffset(mesh2.PosSubMeshesOffset);
			foreach (SubMesh subMesh in mesh2.SubMeshes)
			{
				((IResData)subMesh).Save(this);
			}
			WriteOffset(mesh2.PosBufferUnkOffset);
			Write(new long[9]);
			WriteOffset(mesh2.PosBufferSizeOffset);
			((IResData)mesh2.bufferSize).Save(this);
		}
	}

	private void WriteVertexBuffers(VertexBuffer vtx)
	{
		long[] values = new long[vtx.Buffers.Count];
		long[] values2 = new long[vtx.Buffers.Count * 9];
		if (vtx.Attributes.Count > 0)
		{
			SaveRelocateEntryToSection(Position, 1u, (uint)vtx.Attributes.Count, 1u, 1, "Vertex Attributes");
			WriteOffset(vtx.AttributeOffset);
			foreach (VertexAttrib attribute in vtx.Attributes)
			{
				((IResData)attribute).Save(this);
			}
		}
		if (vtx.Buffers.Count > 0)
		{
			WriteOffset(vtx.UnkBufferOffset);
			Write(values2);
			WriteOffset(vtx.BufferSizeArrayOffset);
			foreach (VertexBufferSize item in vtx.VertexBufferSizeArray)
			{
				((IResData)item).Save(this);
			}
			WriteOffset(vtx.StideArrayOffset);
			foreach (VertexBufferStride item2 in vtx.StrideArray)
			{
				((IResData)item2).Save(this);
			}
			if (SaveVertexBufferRuntimeData)
			{
				Seek(32L);
			}
			WriteOffset(vtx.UnkBuffer2Offset);
			Write(values);
		}
		if (vtx.AttributeDict.Count > 0)
		{
			WriteOffset(vtx.AttributeDictOffset);
			((IResData)vtx.AttributeDict).Save(this);
		}
	}

	private void WriteSkeletonAnimations(SkeletalAnim ska)
	{
		if (ska.BindIndices.Length != 0)
		{
			Align(8);
			WriteOffset(ska.BindIndicesOffset);
			Write(ska.BindIndices);
		}
		if (ska.BoneAnims.Count > 0)
		{
			Align(8);
			if (ResFile.VersionMajor2 >= 9)
			{
				SaveRelocateEntryToSection(Position, 3u, (uint)ska.BoneAnims.Count, 4u, 1, "Bone Animation");
			}
			else
			{
				SaveRelocateEntryToSection(Position, 3u, (uint)ska.BoneAnims.Count, 2u, 1, "Bone Animation");
			}
			WriteOffset(ska.BoneAnimsOffset);
			foreach (BoneAnim boneAnim in ska.BoneAnims)
			{
				((IResData)boneAnim).Save(this);
			}
		}
		foreach (BoneAnim boneAnim2 in ska.BoneAnims)
		{
			object obj = boneAnim2.BaseData;
			if (obj != null)
			{
				Align(8);
				WriteOffset(boneAnim2.PosBaseDataOffset);
				boneAnim2.BaseData.Save(this, boneAnim2.FlagsBase);
			}
			if (boneAnim2.Curves.Count <= 0)
			{
				continue;
			}
			Align(8);
			SaveRelocateEntryToSection(Position, 2u, (uint)boneAnim2.Curves.Count, 4u, 1, "Animation Curve");
			WriteOffset(boneAnim2.PosCurvesOffset);
			foreach (AnimCurve curf in boneAnim2.Curves)
			{
				((IResData)curf).Save(this);
			}
			foreach (AnimCurve curf2 in boneAnim2.Curves)
			{
				WriteOffset(curf2.PosFrameOffset);
				curf2.SaveFrames(this);
				Align(8);
				WriteOffset(curf2.PosKeyDataOffset);
				curf2.SaveKeyData(this);
				Align(8);
			}
		}
		if (ska.UserDatas.Count > 0)
		{
			SaveUserData(ska.UserDatas, ska.UserDataOffset);
		}
		if (ska.UserDataDict.Count > 0)
		{
			WriteOffset(ska.UserDataDictOffset);
			((IResData)ska.UserDataDict).Save(this);
			SaveUserDataData(ska.UserDatas);
			Align(8);
		}
	}

	private void WriteMaterialAnimations(MaterialAnim matanim)
	{
		if (matanim.BindIndices != null && matanim.BindIndices.Length != 0)
		{
			WriteOffset(matanim.PosBindIndicesOffset);
			Write(matanim.BindIndices);
			Align(8);
		}
		if (matanim.MaterialAnimDataList.Count > 0)
		{
			WriteOffset(matanim.PosMatAnimDataOffset);
			foreach (MaterialAnimData materialAnimData in matanim.MaterialAnimDataList)
			{
				((IResData)materialAnimData).Save(this);
			}
		}
		foreach (MaterialAnimData materialAnimData2 in matanim.MaterialAnimDataList)
		{
			if (materialAnimData2.ParamAnimInfos.Count > 0)
			{
				SaveRelocateEntryToSection(Position, 1u, (uint)materialAnimData2.ParamAnimInfos.Count, 2u, 1, "Param Anim Info");
				WriteOffset(materialAnimData2.PosParamInfoOffset);
				foreach (ParamAnimInfo paramAnimInfo in materialAnimData2.ParamAnimInfos)
				{
					((IResData)paramAnimInfo).Save(this);
				}
			}
			if (materialAnimData2.TexturePatternAnimInfos.Count > 0)
			{
				SaveRelocateEntryToSection(Position, 1u, (uint)materialAnimData2.TexturePatternAnimInfos.Count, 1u, 1, "Tex Pat Anim Info");
				WriteOffset(materialAnimData2.PosTexPatInfoOffset);
				foreach (TexturePatternAnimInfo texturePatternAnimInfo in materialAnimData2.TexturePatternAnimInfos)
				{
					((IResData)texturePatternAnimInfo).Save(this);
				}
			}
			if (materialAnimData2.Constants != null && materialAnimData2.Constants.Count > 0)
			{
				WriteOffset(materialAnimData2.PosConstansOffset);
				materialAnimData2.WriteConstansts(this);
			}
			if (materialAnimData2.Curves.Count <= 0)
			{
				continue;
			}
			SaveRelocateEntryToSection(Position, 2u, (uint)materialAnimData2.Curves.Count, 4u, 1, "Animation Curve");
			WriteOffset(materialAnimData2.PosCurvesOffset);
			foreach (AnimCurve curf in materialAnimData2.Curves)
			{
				((IResData)curf).Save(this);
			}
			foreach (AnimCurve curf2 in materialAnimData2.Curves)
			{
				WriteOffset(curf2.PosFrameOffset);
				curf2.SaveFrames(this);
				Align(8);
				WriteOffset(curf2.PosKeyDataOffset);
				curf2.SaveKeyData(this);
				Align(8);
			}
		}
		if (matanim.TextureNames != null && matanim.TextureNames.Count > 0)
		{
			WriteOffset(matanim.PosTextureNamesUnkOffset);
			Write(new long[matanim.TextureNames.Count]);
			SaveRelocateEntryToSection(Position, (uint)matanim.TextureNames.Count, 1u, 0u, 1, "Texture Pat List");
			WriteOffset(matanim.PosTextureNamesOffset);
			SaveStrings(matanim.TextureNames);
			WriteOffset(matanim.PosTextureBindArrayOffset);
			Write(matanim.TextureBindArray);
		}
		if (matanim.UserData.Count > 0)
		{
			SaveUserData(matanim.UserData, matanim.PosUserDataOffset);
		}
		if (matanim.UserDataDict.Count > 0)
		{
			WriteOffset(matanim.PosUserDataDictOffset);
			((IResData)matanim.UserDataDict).Save(this);
			SaveUserDataData(matanim.UserData);
			Align(8);
		}
	}

	private void WriteBoneVisabiltyAnimations(VisibilityAnim bnanim)
	{
		if (bnanim.BindIndices != null)
		{
			WriteOffset(bnanim.PosBindIndicesOffset);
			Write(bnanim.BindIndices);
			Align(8);
		}
		if (bnanim.Names != null && bnanim.Names.Count > 0)
		{
			SaveRelocateEntryToSection(Position, (uint)bnanim.Names.Count, 1u, 0u, 1, "Bone Vis List");
			WriteOffset(bnanim.PosNamesOffset);
			SaveStrings(bnanim.Names);
			Align(8);
		}
		if (bnanim.Curves.Count > 0)
		{
			SaveRelocateEntryToSection(Position, 2u, (uint)bnanim.Curves.Count, 4u, 1, "Animation Curve");
			WriteOffset(bnanim.PosCurvesOffset);
			foreach (AnimCurve curf in bnanim.Curves)
			{
				((IResData)curf).Save(this);
			}
			foreach (AnimCurve curf2 in bnanim.Curves)
			{
				WriteOffset(curf2.PosFrameOffset);
				curf2.SaveFrames(this);
				Align(8);
				WriteOffset(curf2.PosKeyDataOffset);
				curf2.SaveKeyData(this);
				Align(8);
			}
		}
		if (bnanim.BaseSDataList != null)
		{
			WriteOffset(bnanim.PosBaseDataOffset);
			bnanim.WriteBaseData(this);
			Align(8);
		}
	}

	private void WriteShapeAnimations(ShapeAnim shpanim)
	{
		if (shpanim.BindIndices.Length != 0)
		{
			WriteOffset(shpanim.PosBindIndicesOffset);
			Write(shpanim.BindIndices);
		}
		if (shpanim.VertexShapeAnims.Count > 0)
		{
			WriteOffset(shpanim.PosVertexShapeAnimsOffset);
			foreach (VertexShapeAnim vertexShapeAnim in shpanim.VertexShapeAnims)
			{
				((IResData)vertexShapeAnim).Save(this);
			}
			foreach (VertexShapeAnim vertexShapeAnim2 in shpanim.VertexShapeAnims)
			{
				float[] baseDataList = vertexShapeAnim2.BaseDataList;
				if (baseDataList != null && baseDataList.Length != 0)
				{
					WriteOffset(vertexShapeAnim2.PosBaseDataOffset);
					Write(vertexShapeAnim2.BaseDataList);
					Align(8);
				}
				IList<KeyShapeAnimInfo> keyShapeAnimInfos = vertexShapeAnim2.KeyShapeAnimInfos;
				if (keyShapeAnimInfos != null && keyShapeAnimInfos.Count > 0)
				{
					WriteOffset(vertexShapeAnim2.PosKeyShapeAnimInfosOffset);
					foreach (KeyShapeAnimInfo keyShapeAnimInfo in vertexShapeAnim2.KeyShapeAnimInfos)
					{
						((IResData)keyShapeAnimInfo).Save(this);
					}
				}
				if (vertexShapeAnim2.Curves.Count <= 0)
				{
					continue;
				}
				Align(8);
				SaveRelocateEntryToSection(Position, 2u, (uint)vertexShapeAnim2.Curves.Count, 4u, 1, "Animation Curve");
				WriteOffset(vertexShapeAnim2.PosCurvesOffset);
				foreach (AnimCurve curf in vertexShapeAnim2.Curves)
				{
					((IResData)curf).Save(this);
				}
				foreach (AnimCurve curf2 in vertexShapeAnim2.Curves)
				{
					WriteOffset(curf2.PosFrameOffset);
					curf2.SaveFrames(this);
					Align(8);
					WriteOffset(curf2.PosKeyDataOffset);
					curf2.SaveKeyData(this);
					Align(8);
				}
			}
		}
		if (shpanim.UserData.Count > 0)
		{
			WriteOffset(shpanim.PosUserDataOffset);
			foreach (UserData userDatum in shpanim.UserData)
			{
				((IResData)userDatum).Save(this);
			}
		}
		if (shpanim.UserDataDict.Count > 0)
		{
			WriteOffset(shpanim.PosUserDataDictOffset);
			((IResData)shpanim.UserDataDict).Save(this);
		}
	}

	private void WriteSceneAnimations(SceneAnim scnanim)
	{
		Seek(104L);
		if (scnanim.CameraAnims.Count > 0)
		{
			int num = 0;
			foreach (CameraAnim cameraAnim in scnanim.CameraAnims)
			{
				if (num == 0)
				{
					WriteOffset(scnanim.PosCameraAnimArrayOffset);
				}
				((IResData)cameraAnim).Save(this);
				num++;
			}
		}
		if (scnanim.CameraAnims.Count > 0)
		{
			WriteOffset(scnanim.PosCameraAnimDictOffset);
			((IResData)scnanim.CameraAnimDict).Save(this);
		}
		foreach (CameraAnim cameraAnim2 in scnanim.CameraAnims)
		{
			if (cameraAnim2.Curves.Count > 0)
			{
				Align(8);
				SaveRelocateEntryToSection(Position, 2u, (uint)cameraAnim2.Curves.Count, 4u, 1, "Animation Curve");
				WriteOffset(cameraAnim2.PosCurveArrayOffset);
				foreach (AnimCurve curf in cameraAnim2.Curves)
				{
					((IResData)curf).Save(this);
				}
			}
			WriteOffset(cameraAnim2.PosBaseDataOffset);
			cameraAnim2.BaseData.Save(this);
			Align(8);
			foreach (AnimCurve curf2 in cameraAnim2.Curves)
			{
				WriteOffset(curf2.PosFrameOffset);
				curf2.SaveFrames(this);
				Align(8);
				WriteOffset(curf2.PosKeyDataOffset);
				curf2.SaveKeyData(this);
				Align(8);
			}
			if (cameraAnim2.UserData.Count > 0)
			{
				SaveUserData(cameraAnim2.UserData, cameraAnim2.PosUserDataOffset);
				WriteOffset(cameraAnim2.PosUserDataDictOffset);
				((IResData)cameraAnim2.UserDataDict).Save(this);
				SaveUserDataData(cameraAnim2.UserData);
				Align(8);
			}
		}
		foreach (LightAnim lightAnim in scnanim.LightAnims)
		{
			WriteOffset(lightAnim.PosBaseDataOffset);
			lightAnim.BaseData.Save(this, lightAnim.AnimatedFields);
			if (lightAnim.Curves.Count > 0)
			{
				SaveRelocateEntryToSection(Position, 2u, (uint)lightAnim.Curves.Count, 4u, 1, "Animation Curve");
				WriteOffset(lightAnim.PosCurveArrayOffset);
				foreach (AnimCurve curf3 in lightAnim.Curves)
				{
					((IResData)curf3).Save(this);
				}
				foreach (AnimCurve curf4 in lightAnim.Curves)
				{
					WriteOffset(curf4.PosFrameOffset);
					curf4.SaveFrames(this);
					Align(8);
					WriteOffset(curf4.PosKeyDataOffset);
					curf4.SaveKeyData(this);
					Align(8);
				}
			}
			if (lightAnim.UserData.Count > 0)
			{
				SaveUserData(lightAnim.UserData, lightAnim.PosUserDataOffset);
				WriteOffset(lightAnim.PosUserDataDictOffset);
				((IResData)lightAnim.UserDataDict).Save(this);
				SaveUserDataData(lightAnim.UserData);
				Align(8);
			}
		}
		foreach (FogAnim fogAnim in scnanim.FogAnims)
		{
			WriteOffset(fogAnim.PosBaseDataOffset);
			fogAnim.BaseData.Save(this);
			if (fogAnim.Curves.Count > 0)
			{
				SaveRelocateEntryToSection(Position, 2u, (uint)fogAnim.Curves.Count, 4u, 1, "Animation Curve");
				WriteOffset(fogAnim.PosCurveArrayOffset);
				foreach (AnimCurve curf5 in fogAnim.Curves)
				{
					((IResData)curf5).Save(this);
				}
				foreach (AnimCurve curf6 in fogAnim.Curves)
				{
					WriteOffset(curf6.PosFrameOffset);
					curf6.SaveFrames(this);
					Align(8);
					WriteOffset(curf6.PosKeyDataOffset);
					curf6.SaveKeyData(this);
					Align(8);
				}
			}
			if (fogAnim.UserData.Count > 0)
			{
				SaveUserData(fogAnim.UserData, fogAnim.PosUserDataOffset);
				WriteOffset(fogAnim.PosUserDataDictOffset);
				((IResData)fogAnim.UserDataDict).Save(this);
				SaveUserDataData(fogAnim.UserData);
				Align(8);
			}
		}
		if (scnanim.LightAnims.Count > 0)
		{
			WriteOffset(scnanim.PosLightAnimDictOffset);
			((IResData)scnanim.LightAnimDict).Save(this);
		}
		if (scnanim.FogAnims.Count > 0)
		{
			WriteOffset(scnanim.PosFogAnimDictOffset);
			((IResData)scnanim.FogAnimDict).Save(this);
		}
		if (scnanim.UserData.Count > 0)
		{
			WriteOffset(scnanim.PosUserDataDictOffset);
			((IResData)scnanim.UserDataDict).Save(this);
		}
	}

	private void WriteMaterials(Material mat)
	{
		if (ResFile.VersionMajor2 >= 10)
		{
			SaveEntries();
			if (mat.UserDatas.Count > 0)
			{
				SaveUserData(mat.UserDatas, mat.PosUserDataMaterialOffset);
			}
			if (mat.UserDataDict.Count > 0)
			{
				WriteOffset(mat.PosUserDataDictMaterialOffset);
				((IResData)mat.UserDataDict).Save(this);
				SaveUserDataData(mat.UserDatas);
				Align(8);
			}
			return;
		}
		if (mat.RenderInfos == null)
		{
			mat.RenderInfos = new List<RenderInfo>();
		}
		if (mat.ShaderParams == null)
		{
			mat.ShaderParams = new List<ShaderParam>();
		}
		if (mat.UserDatas == null)
		{
			mat.UserDatas = new List<UserData>();
		}
		if (mat.ShaderAssign == null)
		{
			mat.ShaderAssign = new ShaderAssign();
		}
		if (mat.TextureRefs == null)
		{
			mat.TextureRefs = new List<string>();
		}
		if (mat.VolatileFlags == null)
		{
			mat.VolatileFlags = new byte[0];
		}
		if (mat.RenderInfos.Count > 0)
		{
			SaveRelocateEntryToSection(Position, 2u, (uint)mat.RenderInfos.Count, 1u, 1, "Render infos");
			WriteOffset(mat.PosRenderInfoOffset);
			foreach (RenderInfo renderInfo in mat.RenderInfos)
			{
				((IResData)renderInfo).Save(this);
			}
			int num = mat.RenderInfos.Sum((RenderInfo item) => item.GetValueStrings().Length);
			if (num != 0)
			{
				SaveRelocateEntryToSection(Position, (uint)num, 1u, 0u, 1, "Render info strings");
			}
			foreach (RenderInfo renderInfo2 in mat.RenderInfos)
			{
				if (renderInfo2.Type == RenderInfoType.String && renderInfo2.GetValueStrings().Length != 0)
				{
					WriteOffset(renderInfo2.DataOffset);
					renderInfo2.SaveStrings(this);
				}
			}
			foreach (RenderInfo renderInfo3 in mat.RenderInfos)
			{
				if (renderInfo3.Type == RenderInfoType.Int32 && renderInfo3.GetValueInt32s().Length != 0)
				{
					WriteOffset(renderInfo3.DataOffset);
					renderInfo3.SaveInts(this);
				}
			}
			foreach (RenderInfo renderInfo4 in mat.RenderInfos)
			{
				if (renderInfo4.Type == RenderInfoType.Single && renderInfo4.GetValueSingles().Length != 0)
				{
					WriteOffset(renderInfo4.DataOffset);
					renderInfo4.SaveFloats(this);
				}
			}
		}
		if (mat.TextureRefs.Count > 0)
		{
			long[] values = new long[mat.TextureRefs.Count];
			Align(8);
			WriteOffset(mat.PosTextureUnk1Offset);
			Write(values);
		}
		if (mat.Samplers.Count > 0)
		{
			long[] values2 = new long[mat.TextureRefs.Count * 15];
			Align(8);
			WriteOffset(mat.PosSamplersOffset);
			foreach (Sampler sampler in mat.Samplers)
			{
				((IResData)sampler).Save(this);
			}
			WriteOffset(mat.PosTextureUnk2Offset);
			Write(values2);
		}
		if (mat.ShaderParams.Count > 0)
		{
			SaveRelocateEntryToSection(Position + 8, 1u, (uint)mat.ShaderParams.Count, 3u, 1, "Shader Param");
			WriteOffset(mat.PosShaderParamsOffset);
			foreach (ShaderParam shaderParam in mat.ShaderParams)
			{
				((IResData)shaderParam).Save(this);
			}
			WriteOffset(mat.PosShaderParamDataOffset);
			Write(mat.ShaderParamData);
		}
		if (mat.VolatileFlags.Length != 0)
		{
			Align(8);
			WriteOffset(mat.PosVolatileFlagsOffset);
			Write(mat.VolatileFlags);
			Align(8);
		}
		if (mat.UserDatas.Count > 0)
		{
			SaveUserData(mat.UserDatas, mat.PosUserDataMaterialOffset);
		}
		if (mat.Samplers.Count > 0)
		{
			WriteOffset(mat.PosSamplerSlotArrayOffset);
			Write(mat.SamplerSlotArray);
		}
		if (mat.TextureRefs.Count > 0)
		{
			WriteOffset(mat.PosTextureSlotArrayOffset);
			Write(mat.TextureSlotArray);
		}
		if (mat.TextureRefs.Count > 0)
		{
			SaveRelocateEntryToSection(Position, (uint)mat.TextureRefs.Count, 1u, 0u, 1, "Texture Refs");
			WriteOffset(mat.PosTextureRefsOffset);
			SaveStrings(mat.TextureRefs);
		}
		if (mat.ShaderAssign != null)
		{
			ShaderAssign shaderAssign = mat.ShaderAssign;
			if (shaderAssign.ShaderOptions == null)
			{
				shaderAssign.ShaderOptions = new List<string>();
			}
			SaveRelocateEntryToSection(Position, 8u, 1u, 0u, 1, "Shader assign");
			WriteOffset(mat.PosShaderAssignOffset);
			((IResData)shaderAssign).Save(this);
			uint offsetCount = (uint)(mat.ShaderAssign.AttribAssignDict.Count + mat.ShaderAssign.SamplerAssignDict.Count + mat.ShaderAssign.ShaderOptionDict.Count);
			SaveRelocateEntryToSection(Position, offsetCount, 1u, 0u, 1, "Shader assign data");
			if (shaderAssign.AttribAssigns.Count > 0)
			{
				WriteOffset(shaderAssign.PosAttribAssigns);
				SaveStrings(shaderAssign.AttribAssigns);
			}
			if (shaderAssign.SamplerAssigns.Count > 0)
			{
				WriteOffset(shaderAssign.PosSamplerAssigns);
				SaveStrings(shaderAssign.SamplerAssigns);
			}
			if (shaderAssign.ShaderOptions.Count > 0)
			{
				WriteOffset(shaderAssign.PosShaderOptions);
				SaveStrings(shaderAssign.ShaderOptions);
			}
			if (shaderAssign.AttribAssigns.Count > 0)
			{
				WriteOffset(shaderAssign.PosAttribAssignDict);
				((IResData)shaderAssign.AttribAssignDict).Save(this);
			}
			if (shaderAssign.SamplerAssigns.Count > 0)
			{
				WriteOffset(shaderAssign.PosSamplerAssignDict);
				((IResData)shaderAssign.SamplerAssignDict).Save(this);
			}
			if (shaderAssign.ShaderOptions.Count > 0)
			{
				WriteOffset(shaderAssign.PosShaderOptionsDict);
				((IResData)shaderAssign.ShaderOptionDict).Save(this);
			}
		}
		if (mat.RenderInfoDict.Count > 0)
		{
			WriteOffset(mat.PosRenderInfoDictOffset);
			((IResData)mat.RenderInfoDict).Save(this);
		}
		if (mat.SamplerDict.Count > 0)
		{
			WriteOffset(mat.PosSamplerDictOffset);
			((IResData)mat.SamplerDict).Save(this);
		}
		if (mat.ShaderParamDict.Count > 0)
		{
			WriteOffset(mat.PosShaderParamDictOffset);
			((IResData)mat.ShaderParamDict).Save(this);
		}
		if (mat.UserDataDict.Count > 0)
		{
			WriteOffset(mat.PosUserDataDictMaterialOffset);
			((IResData)mat.UserDataDict).Save(this);
			SaveUserDataData(mat.UserDatas);
			Align(8);
		}
	}

	private void SaveUserData(IList<UserData> userData, long Target)
	{
		Align(8);
		SaveRelocateEntryToSection(Position, 2u, (uint)userData.Count, 6u, 1, "user Data");
		WriteOffset(Target);
		foreach (UserData userDatum in userData)
		{
			((IResData)userDatum).Save(this);
		}
	}

	private void SaveUserDataData(IList<UserData> userData)
	{
		int num = userData.Sum((UserData item) => item.GetValueStringArray().Length);
		if (num != 0)
		{
			SaveRelocateEntryToSection(Position, (uint)num, 1u, 0u, 1, "user Data info strings");
		}
		foreach (UserData userDatum in userData)
		{
			if (userDatum.Type == UserDataType.String && userDatum.GetValueStringArray().Length != 0)
			{
				WriteOffset(userDatum.DataOffset);
				userDatum.SaveData(this);
				Align(8);
			}
		}
		foreach (UserData userDatum2 in userData)
		{
			if (userDatum2.Type == UserDataType.WString && userDatum2.GetValueStringArray().Length != 0)
			{
				WriteOffset(userDatum2.DataOffset);
				userDatum2.SaveData(this);
				Align(8);
			}
		}
		foreach (UserData userDatum3 in userData)
		{
			if (userDatum3.Type == UserDataType.Int32 && userDatum3.GetValueInt32Array().Length != 0)
			{
				WriteOffset(userDatum3.DataOffset);
				userDatum3.SaveData(this);
				Align(8);
			}
		}
		foreach (UserData userDatum4 in userData)
		{
			if (userDatum4.Type == UserDataType.Single && userDatum4.GetValueSingleArray().Length != 0)
			{
				WriteOffset(userDatum4.DataOffset);
				userDatum4.SaveData(this);
				Align(8);
			}
		}
		foreach (UserData userDatum5 in userData)
		{
			if (userDatum5.Type == UserDataType.Byte && userDatum5.GetValueByteArray().Length != 0)
			{
				WriteOffset(userDatum5.DataOffset);
				userDatum5.SaveData(this);
			}
		}
	}

	internal void WriteOffset(long offset)
	{
		long position = Position;
		using (TemporarySeek(offset, SeekOrigin.Begin))
		{
			Write(position);
		}
	}

	internal long SaveOffset()
	{
		long position = Position;
		Write(0L);
		return position;
	}

	internal void Write(sbyte[] value)
	{
		for (int i = 0; i < value.Length; i++)
		{
			Write(value[i]);
		}
	}

	internal void SetupRelocationTable()
	{
		if (ResFile.ExternalFiles.Count > 0)
		{
			byte[] data = ResFile.ExternalFiles[0].Data;
			if (data != null && data.Length <= 3)
			{
				Align(256);
			}
			else
			{
				Align(ResFile.DataAlignment);
			}
		}
		long position = Position;
		int num = 0;
		uint position2 = 0u;
		uint ofsEndOfStringTable = _ofsEndOfStringTable;
		int num2 = 0;
		int num3 = 0;
		_savedSection2Entries = _savedSection2Entries.OrderBy((RelocationEntry o) => o.Position).ToList();
		_savedSection3Entries = _savedSection3Entries.OrderBy((RelocationEntry o) => o.Position).ToList();
		_savedSection4Entries = _savedSection4Entries.OrderBy((RelocationEntry o) => o.Position).ToList();
		_savedSection5Entries = _savedSection5Entries.OrderBy((RelocationEntry o) => o.Position).ToList();
		if (true)
		{
			foreach (RelocationEntry savedSection1Entry in _savedSection1Entries)
			{
				Console.WriteLine("Pos = " + savedSection1Entry.Position + " " + savedSection1Entry.StructCount + " " + savedSection1Entry.OffsetCount + " " + savedSection1Entry.PadingCount + " " + savedSection1Entry.Hint);
			}
			foreach (RelocationEntry savedSection2Entry in _savedSection2Entries)
			{
				Console.WriteLine("Pos = " + savedSection2Entry.Position + " " + savedSection2Entry.StructCount + " " + savedSection2Entry.OffsetCount + " " + savedSection2Entry.PadingCount + " " + savedSection2Entry.Hint);
			}
			foreach (RelocationEntry savedSection4Entry in _savedSection4Entries)
			{
				Console.WriteLine("Pos = " + savedSection4Entry.Position + " " + savedSection4Entry.StructCount + " " + savedSection4Entry.OffsetCount + " " + savedSection4Entry.PadingCount + " " + savedSection4Entry.Hint);
			}
			foreach (RelocationEntry savedSection5Entry in _savedSection5Entries)
			{
				Console.WriteLine("Pos = " + savedSection5Entry.Position + " " + savedSection5Entry.StructCount + " " + savedSection5Entry.OffsetCount + " " + savedSection5Entry.PadingCount + " " + savedSection5Entry.Hint);
			}
		}
		RelocationSection item = new RelocationSection(position2, num, ofsEndOfStringTable, _savedSection1Entries);
		num += _savedSection1Entries.Count;
		RelocationSection item2;
		RelocationSection item3;
		if (ResFile.BufferInfo != null)
		{
			position2 = (uint)bufferInfoOffset;
			item2 = new RelocationSection(position2, num, ResFile.BufferInfo.getIndexBufferSize(this), _savedSection2Entries);
			num += _savedSection2Entries.Count;
			position2 = _ofsVertexBuffer;
			item3 = new RelocationSection(position2, num, ResFile.BufferInfo.GetVertexBufferSize(this), _savedSection3Entries);
			num += _savedSection3Entries.Count;
		}
		else
		{
			position2 = ofsEndOfStringTable;
			item2 = new RelocationSection(position2, num, 0u, _savedSection2Entries);
			item3 = new RelocationSection(position2, num, 0u, _savedSection3Entries);
		}
		RelocationSection item4;
		if (ResFile.MemoryPool != null)
		{
			position2 = (uint)memoryPoolOffset;
			item4 = new RelocationSection(position2, num, 288u, _savedSection4Entries);
			num += _savedSection4Entries.Count;
		}
		else
		{
			position2 = ofsEndOfStringTable;
			item4 = new RelocationSection(position2, num, 0u, _savedSection4Entries);
		}
		num += _savedSection3Entries.Count;
		RelocationSection item5;
		if (ResFile.ExternalFiles.Count > 0)
		{
			position2 = _ofsExternalFileBlock;
			item5 = new RelocationSection(position2, num, (uint)(position - _ofsExternalFileBlock), _savedSection5Entries);
			num += _savedSection5Entries.Count;
		}
		else
		{
			item5 = new RelocationSection(position2, num, 0u, _savedSection5Entries);
		}
		num += _savedSection4Entries.Count;
		_savedRelocatedSections.Add(item);
		_savedRelocatedSections.Add(item2);
		_savedRelocatedSections.Add(item3);
		_savedRelocatedSections.Add(item4);
		_savedRelocatedSections.Add(item5);
	}

	/// <summary>
	/// Gets the block size of all external files
	/// </summary>
	internal uint GetExternalFilesBlockSize()
	{
		return 0u;
	}

	/// <summary>
	/// Reserves space for an offset to the <paramref name="resData" /> written later.
	/// </summary>
	/// <param name="resData">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.IResData" /> to save.</param>
	/// <param name="index">The index of the element, used for instances referenced by a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResDict" />.
	/// <param name="ShiftPos">The position the offset is saved to.</param>
	/// </param>
	[DebuggerStepThrough]
	internal void Save(IResData resData, int index = -1, long ShiftPos = 0L)
	{
		long num = Position;
		if (ShiftPos != 0)
		{
			num = ShiftPos;
		}
		if (resData == null && ShiftPos == 0)
		{
			Write(0L);
			return;
		}
		if (TryGetItemEntry(resData, ItemEntryType.ResData, out var entry))
		{
			entry.Offsets.Add((uint)num);
			entry.Index = index;
		}
		else
		{
			_savedItems.Add(new ItemEntry(resData, ItemEntryType.ResData, (uint)num, null, null, index));
		}
		if (ShiftPos == 0)
		{
			Write(uint.MaxValue);
			Write(0);
		}
	}

	/// <summary>
	/// Reserves space for the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> file size field which is automatically filled later.
	/// </summary>
	[DebuggerStepThrough]
	internal void SaveFieldFileSize()
	{
		_ofsFileSize = (uint)Position;
		Write(0);
	}

	/// <summary>
	/// Save pointer array to be relocated in section 1
	/// </summary>
	[DebuggerStepThrough]
	internal void SaveRelocateEntryToSection(long pos, uint OffsetCount, uint StructCount, uint PaddingCount, int SectionNumber, string Hint)
	{
		if (StructCount == 0 || OffsetCount == 0)
		{
			return;
		}
		if (StructCount == 0)
		{
			throw new Exception("Invalid struct count. Should be greater than 0! " + StructCount);
		}
		if (OffsetCount > 255)
		{
			SaveRelocateEntryToSection(pos, 255u, StructCount, PaddingCount, SectionNumber, Hint);
			long pos2 = pos + 2040;
			SaveRelocateEntryToSection(pos2, OffsetCount - 255, StructCount, PaddingCount, SectionNumber, Hint);
			return;
		}
		if (SectionNumber == 1)
		{
			_savedSection1Entries.Add(new RelocationEntry((uint)pos, OffsetCount, StructCount, PaddingCount, Hint));
		}
		if (SectionNumber == 2)
		{
			_savedSection2Entries.Add(new RelocationEntry((uint)pos, OffsetCount, StructCount, PaddingCount, Hint));
		}
		if (SectionNumber == 3)
		{
			_savedSection3Entries.Add(new RelocationEntry((uint)pos, OffsetCount, StructCount, PaddingCount, Hint));
		}
		if (SectionNumber == 4)
		{
			_savedSection4Entries.Add(new RelocationEntry((uint)pos, OffsetCount, StructCount, PaddingCount, Hint));
		}
		if (SectionNumber == 5)
		{
			_savedSection5Entries.Add(new RelocationEntry((uint)pos, OffsetCount, StructCount, PaddingCount, Hint));
		}
	}

	internal void SaveFileNameString(string Name, bool Relocate = false)
	{
		_fileName = Name;
		_ofsFileName = (uint)Position;
		Write(0);
	}

	/// <summary>
	/// Reserves space for the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> string pool size and offset fields which are automatically
	/// filled later.
	/// </summary>
	[DebuggerStepThrough]
	internal void SaveFieldStringPool(bool Relocate = false)
	{
		_ofsStringPool = (uint)Position;
		Write(0L);
		Write(0);
	}

	/// <summary>
	/// Reserves space for the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> memory pool field which is automatically filled later.
	/// </summary>
	[DebuggerStepThrough]
	internal void SaveMemoryPoolPointer()
	{
		_savedMemoryPoolPointers.Add(Position);
		Write(0L);
	}

	/// <summary>
	/// Reserves space for the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> memory pool field which is automatically filled later.
	/// </summary>
	[DebuggerStepThrough]
	internal void SaveBufferTotalSize()
	{
		_ofsTotalBufferSize = Position;
		Write(0);
	}

	/// <summary>
	/// Saves the Index buffer pointer to be used later in the relocation table
	/// </summary>
	[DebuggerStepThrough]
	internal void SaveIndexBufferPointer()
	{
		_ofsIndexBuffer = (uint)Position;
		Write(0L);
	}

	/// <summary>
	/// Saves the Vertex buffer pointer to be used later in the relocation table
	/// </summary>
	[DebuggerStepThrough]
	internal void SaveVertexBufferPointer()
	{
		_ofsVertexBuffer = (uint)Position;
	}

	/// <summary>
	/// Reserves space for the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> memory pool field which is automatically filled later.
	/// </summary>
	[DebuggerStepThrough]
	internal void SaveRelocationTablePointerPointer()
	{
		_ofsRelocationTable = (uint)Position;
		Write(0);
	}

	/// <summary>
	/// Reserves space for an offset to the <paramref name="list" /> written later.
	/// </summary>
	/// <typeparam name="T">The type of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.IResData" /> elements.</typeparam>
	/// <param name="list">The <see cref="T:System.Collections.Generic.IList`1" /> to save.</param>
	[DebuggerStepThrough]
	internal void SaveList<T>(IEnumerable<T> list, long ShiftPos = 0L) where T : IResData, new()
	{
		long num = Position;
		if (ShiftPos != 0)
		{
			num = ShiftPos;
		}
		if (list != null && list.Count() == 0 && ShiftPos != 0)
		{
			return;
		}
		if (list != null && list.Count() == 0 && ShiftPos == 0)
		{
			Write(0L);
			return;
		}
		if (TryGetItemEntry(list.First(), ItemEntryType.ResData, out var entry))
		{
			entry.Offsets.Add((uint)num);
			entry.Index = 0;
		}
		else
		{
			int num2 = 0;
			foreach (T item in list)
			{
				if (num2 == 0)
				{
					List<ItemEntry> savedItems = _savedItems;
					object data = item;
					uint? offset = (uint)num;
					int index = num2;
					savedItems.Add(new ItemEntry(data, ItemEntryType.ResData, offset, null, null, index));
				}
				else
				{
					List<ItemEntry> savedItems2 = _savedItems;
					object data2 = item;
					int index = num2;
					savedItems2.Add(new ItemEntry(data2, ItemEntryType.ResData, null, null, null, index));
				}
				num2++;
			}
		}
		if (ShiftPos == 0)
		{
			Write(uint.MaxValue);
			Write(0);
		}
	}

	/// <summary>
	/// Reserves space for an offset to the <paramref name="dict" /> written later.
	/// </summary>
	/// <typeparam name="T">The type of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.IResData" /> element values.</typeparam>
	/// <param name="dict">The <see cref="!:ResDict&lt;T&gt;" /> to save.</param>
	[DebuggerStepThrough]
	internal void SaveDict(ResDict dict, long ShiftPos = 0L)
	{
		long num = Position;
		if (ShiftPos != 0)
		{
			num = ShiftPos;
		}
		if (dict != null && dict.Count == 0 && ShiftPos == 0)
		{
			Write(0L);
			return;
		}
		if (TryGetItemEntry(dict, ItemEntryType.Dict, out var entry))
		{
			entry.Offsets.Add((uint)num);
		}
		else
		{
			_savedItems.Add(new ItemEntry(dict, ItemEntryType.Dict, (uint)num));
		}
		if (ShiftPos == 0)
		{
			Write(uint.MaxValue);
			Write(0);
		}
	}

	/// <summary>
	/// Reserves space for an offset to the <paramref name="data" /> written later with the
	/// <paramref name="callback" />.
	/// </summary>
	/// <param name="data">The data to save.</param>
	/// <param name="callback">The <see cref="T:System.Action" /> to invoke to write the data.</param>
	[DebuggerStepThrough]
	internal void SaveCustom(object data, Action callback, long ShiftOffset = 0L)
	{
		long num = Position;
		if (ShiftOffset != 0)
		{
			num = ShiftOffset;
		}
		if (data == null && ShiftOffset == 0)
		{
			Write(0L);
			return;
		}
		if (TryGetItemEntry(data, ItemEntryType.Custom, out var entry))
		{
			entry.Offsets.Add((uint)num);
		}
		else
		{
			_savedItems.Add(new ItemEntry(data, ItemEntryType.Custom, (uint)num, null, callback));
		}
		if (ShiftOffset == 0)
		{
			Write(ulong.MaxValue);
		}
	}

	/// <summary>
	/// Reserves space for an offset to the <paramref name="str" /> written later in the string pool with the
	/// specified <paramref name="encoding" />.
	/// </summary>
	/// <param name="str">The name to save.</param>
	/// <param name="encoding">The <see cref="T:System.Text.Encoding" /> in which the name will be stored.</param>
	[DebuggerStepThrough]
	internal void SaveString(string str, Encoding encoding = null, bool Relocate = false)
	{
		if (str == null)
		{
			Write(0L);
			return;
		}
		if (_savedStrings.TryGetValue(str, out var value))
		{
			value.Offsets.Add((uint)Position);
		}
		else
		{
			_savedStrings.Add(str, new StringEntry((uint)Position, encoding));
		}
		Write(uint.MaxValue);
		Write(0);
	}

	/// <summary>
	/// Reserves space for offsets to the <paramref name="strings" /> written later in the string pool with the
	/// specified <paramref name="encoding" />
	/// </summary>
	/// <param name="strings">The names to save.</param>
	/// <param name="encoding">The <see cref="T:System.Text.Encoding" /> in which the names will be stored.</param>
	[DebuggerStepThrough]
	internal void SaveStrings(IEnumerable<string> strings, bool relocate = false, Encoding encoding = null)
	{
		if (relocate)
		{
			SaveRelocateEntryToSection(Position, (uint)strings.ToList().Count, 1u, 0u, 1, "SaveStrings");
		}
		foreach (string @string in strings)
		{
			SaveString(@string, encoding);
		}
	}

	/// <summary>
	/// Reserves space for an offset and size for header block.
	/// </summary>
	[DebuggerStepThrough]
	internal void SaveHeaderBlock(bool IsBinaryHeader = false)
	{
		_savedHeaderBlockPositions.Add(Position);
		if (IsBinaryHeader)
		{
			Write((ushort)0);
		}
		else
		{
			WriteHeaderBlock(0u, 0L);
		}
	}

	/// <summary>
	/// Reserves space for an offset to the <paramref name="data" /> written later in the data block pool.
	/// </summary>
	/// <param name="data">The data to save.</param>
	/// <param name="alignment">The alignment to seek to before invoking the callback.</param>
	/// <param name="callback">The <see cref="T:System.Action" /> to invoke to write the data.</param>
	[DebuggerStepThrough]
	internal void SaveBlock(object data, int alignment, Action callback)
	{
		if (data == null)
		{
			Write(0L);
			return;
		}
		if (_savedBlocks.TryGetValue(data, out var value))
		{
			value.Offsets.Add((uint)Position);
		}
		else
		{
			_savedBlocks.Add(data, new BlockEntry((uint)Position, alignment, callback));
		}
		Write(uint.MaxValue);
		Write(0);
	}

	/// <summary>
	/// Writes a BFRES signature consisting of 4 ASCII characters encoded as an <see cref="T:System.UInt32" />.
	/// </summary>
	/// <param name="value">A valid signature.</param>
	internal void WriteSignature(string value)
	{
		Write(Encoding.ASCII.GetBytes(value));
	}

	private bool TryGetItemEntry(object data, ItemEntryType type, out ItemEntry entry)
	{
		foreach (ItemEntry savedItem in _savedItems)
		{
			if (savedItem.Data.Equals(data) && savedItem.Type == type)
			{
				entry = savedItem;
				return true;
			}
		}
		entry = null;
		return false;
	}

	public void SaveEntries()
	{
		for (int i = 0; i < _savedItems.Count; i++)
		{
			if (_savedItems[i].Target.HasValue)
			{
				continue;
			}
			ItemEntry itemEntry = _savedItems[i];
			Align(8);
			switch (itemEntry.Type)
			{
			case ItemEntryType.List:
			{
				IEnumerable<IResData> enumerable = (IEnumerable<IResData>)itemEntry.Data;
				if (TryGetItemEntry(enumerable.First(), ItemEntryType.ResData, out var entry))
				{
					itemEntry.Target = entry.Target;
					break;
				}
				itemEntry.Target = (uint)Position;
				CurrentIndex = 0;
				foreach (IResData item in enumerable)
				{
					List<ItemEntry> savedItems = _savedItems;
					uint? target = (uint)Position;
					int currentIndex = CurrentIndex;
					savedItems.Add(new ItemEntry(item, ItemEntryType.ResData, null, target, null, currentIndex));
					item.Save(this);
					CurrentIndex++;
				}
				break;
			}
			case ItemEntryType.Dict:
			case ItemEntryType.ResData:
				itemEntry.Target = (uint)Position;
				CurrentIndex = itemEntry.Index;
				((IResData)itemEntry.Data).Save(this);
				break;
			case ItemEntryType.Custom:
				itemEntry.Target = (uint)Position;
				itemEntry.Callback();
				break;
			}
		}
		WriteOffsets();
		_savedItems.Clear();
	}

	private void WriteMemoryPool()
	{
		long position = Position;
		Align((ResFile.DataAlignmentOverride != 0) ? ((int)ResFile.DataAlignmentOverride) : ResFile.DataAlignment);
		WriteTotalBufferSize(Position);
		memoryPoolOffset = Position;
		byte[] buffer = new byte[288];
		Write(buffer);
		foreach (long savedMemoryPoolPointer in _savedMemoryPoolPointers)
		{
			using (TemporarySeek(savedMemoryPoolPointer, SeekOrigin.Begin))
			{
				Write(memoryPoolOffset);
			}
		}
	}

	private void WriteTotalBufferSize(long BufferEndPos)
	{
		uint value = (uint)(BufferEndPos - bufferInfoOffset);
		using (TemporarySeek(_ofsTotalBufferSize, SeekOrigin.Begin))
		{
			Write(value);
		}
	}

	private void WriteRelocationTable()
	{
		Align(256);
		uint value = (uint)Position;
		WriteSignature("_RLT");
		_ofsEndOfBlock = (uint)Position;
		Write(value);
		Write(_savedRelocatedSections.Count);
		Write(0);
		foreach (RelocationSection savedRelocatedSection in _savedRelocatedSections)
		{
			Write(0L);
			Write(savedRelocatedSection.Position);
			Write(savedRelocatedSection.Size);
			Write(savedRelocatedSection.EntryIndex);
			Write(savedRelocatedSection.Entries.Count);
		}
		foreach (RelocationSection savedRelocatedSection2 in _savedRelocatedSections)
		{
			foreach (RelocationEntry entry in savedRelocatedSection2.Entries)
			{
				Write(entry.Position);
				Write((ushort)entry.StructCount);
				Write((byte)entry.OffsetCount);
				Write((byte)entry.PadingCount);
			}
		}
		using (TemporarySeek(_ofsRelocationTable, SeekOrigin.Begin))
		{
			Write(value);
		}
	}

	private void WriteIndexBuffer()
	{
		Align((ResFile.DataAlignmentOverride != 0) ? ((int)ResFile.DataAlignmentOverride) : ResFile.DataAlignment);
		bufferInfoOffset = Position;
		for (int i = 0; i < ResFile.BufferInfo.IndexBufferData.Length; i++)
		{
			if (Position % 8 != 0)
			{
				Position += 8 - Position % 8;
			}
			Write(ResFile.BufferInfo.IndexBufferData[i]);
		}
		using (TemporarySeek(_ofsIndexBuffer, SeekOrigin.Begin))
		{
			Write(bufferInfoOffset);
		}
	}

	private void WriteVertexBuffer()
	{
		SaveVertexBufferPointer();
		for (int i = 0; i < ResFile.BufferInfo.VertexBufferData.Length; i++)
		{
			int num = ((ResFile.BufferInfo.VertexAlignments.Length > i) ? ResFile.BufferInfo.VertexAlignments[i] : 8);
			if (Position % num != 0)
			{
				Position += num - Position % num;
			}
			Write(ResFile.BufferInfo.VertexBufferData[i]);
		}
	}

	private void WriteStrings()
	{
		Dictionary<string, StringEntry> dictionary = new Dictionary<string, StringEntry>();
		foreach (string @string in ResFile.StringTable.Strings)
		{
			if (_savedStrings.ContainsKey(@string) && !dictionary.ContainsKey(@string))
			{
				dictionary.Add(@string, _savedStrings[@string]);
			}
		}
		foreach (KeyValuePair<string, StringEntry> savedString in _savedStrings)
		{
			if (!dictionary.ContainsKey(savedString.Key))
			{
				dictionary.Add(savedString.Key, savedString.Value);
			}
		}
		Align(4);
		uint num = (uint)Position;
		WriteSignature("_STR");
		SaveHeaderBlock();
		Write(dictionary.Count);
		uint value = (uint)Position;
		foreach (KeyValuePair<string, StringEntry> item in dictionary)
		{
			if (item.Key == _fileName)
			{
				_ofsFileNameString = (uint)Position;
			}
			using (TemporarySeek())
			{
				SatisfyOffsets(item.Value.Offsets, (uint)Position);
			}
			Write((short)item.Key.Length);
			Write(item.Key, BinaryStringFormat.ZeroTerminated, item.Value.Encoding ?? Encoding);
			Align(2);
		}
		BaseStream.SetLength(Position);
		Align(2);
		_ofsEndOfStringTable = (uint)Position;
		uint value2 = _ofsEndOfStringTable - num;
		if (_ofsStringPool != 0)
		{
			using (TemporarySeek(_ofsStringPool, SeekOrigin.Begin))
			{
				Write(value);
				Write(0);
				Write(value2);
			}
		}
	}

	private void WriteHeaderBlock(uint Size, long Offset)
	{
		Write(Size);
		Write(Offset);
	}

	private void WriteBlocks()
	{
		int num = 0;
		foreach (KeyValuePair<object, BlockEntry> savedBlock in _savedBlocks)
		{
			if (savedBlock.Value.Alignment != 0)
			{
				Align(savedBlock.Value.Alignment);
			}
			if (num == 0)
			{
				_ofsExternalFileBlock = (uint)Position;
			}
			using (TemporarySeek())
			{
				SatisfyOffsets(savedBlock.Value.Offsets, (uint)Position);
			}
			savedBlock.Value.Callback();
			num++;
		}
	}

	private void WriteOffsets()
	{
		using (TemporarySeek())
		{
			foreach (ItemEntry savedItem in _savedItems)
			{
				SatisfyOffsets(savedItem.Offsets, savedItem.Target.Value);
			}
		}
	}

	private void SatisfyOffsets(IEnumerable<uint> offsets, uint target)
	{
		foreach (uint offset in offsets)
		{
			Position = offset;
			Write((ulong)target);
		}
	}
}
