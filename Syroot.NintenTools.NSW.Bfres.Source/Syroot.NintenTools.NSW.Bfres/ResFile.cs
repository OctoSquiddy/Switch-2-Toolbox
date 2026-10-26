using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Syroot.BinaryData;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a NintendoWare for Cafe (NW4F) graphics data archive file.
/// </summary>
[DebuggerDisplay("ResFile {Name}")]
public class ResFile : IResData
{
	public enum ExternalFlags : byte
	{
		IsExternalModelUninitalized = 1,
		HasExternalString = 2,
		HoldsExternalStrings = 4,
		HasExternalGPU = 8,
		MeshCodecResave = 0x80
	}

	private const string _signature = "FRES";

	public uint DataAlignmentOverride = 0u;

	public static bool UseExternalGPU;

	internal long ModelOffset = 0L;

	internal long SkeletonAnimationOffset = 0L;

	internal long MaterialAnimationOffset = 0L;

	internal long ShapeAnimationOffset = 0L;

	internal long BoneVisAnimationOffset = 0L;

	internal long SceneAnimationOffset = 0L;

	internal long ExternalFileOffset = 0L;

	internal long ModelDictOffset = 0L;

	internal long SkeletonAnimationDictOffset = 0L;

	internal long MaterialAnimationnDictOffset = 0L;

	internal long ShapeAnimationDictOffset = 0L;

	internal long BoneVisAnimationDictOffset = 0L;

	internal long SceneAnimationDictOffset = 0L;

	internal long ExternalFileDictOffset = 0L;

	internal long BufferInfoOffset = 0L;

	/// <summary>
	/// Gets or sets a name describing the contents.
	/// </summary>
	[Browsable(true)]
	[Category("Binary Info")]
	[DisplayName("Name")]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the alignment to use for raw data blocks in the file.
	/// </summary>
	[Browsable(true)]
	[ReadOnly(true)]
	[Category("Binary Info")]
	[DisplayName("Alignment")]
	public int DataAlignment => 1 << (int)Alignment;

	/// <summary>
	/// Gets or sets the major revision of the BFRES structure formats.
	/// </summary>
	[Browsable(true)]
	[ReadOnly(true)]
	[Category("Version")]
	[DisplayName("Version Full")]
	public string VersionFull => $"{VersionMajor},{VersionMajor2},{VersionMinor},{VersionMinor2}";

	internal uint Version { get; set; }

	/// <summary>
	/// Gets or sets the major revision of the BFRES structure formats.
	/// </summary>
	[Browsable(true)]
	[Category("Version")]
	[DisplayName("Version Major")]
	public uint VersionMajor { get; set; }

	/// <summary>
	/// Gets or sets the second major revision of the BFRES structure formats.
	/// </summary>
	[Browsable(true)]
	[Category("Version")]
	[DisplayName("Version Major 2")]
	public uint VersionMajor2 { get; set; }

	/// <summary>
	/// Gets or sets the minor revision of the BFRES structure formats.
	/// </summary>
	[Browsable(true)]
	[Category("Version")]
	[DisplayName("Version Minor")]
	public uint VersionMinor { get; set; }

	/// <summary>
	/// Gets or sets the second minor revision of the BFRES structure formats.
	/// </summary>
	[Browsable(true)]
	[Category("Version")]
	[DisplayName("Version Minor 2")]
	public uint VersionMinor2 { get; set; }

	/// <summary>
	/// Gets the byte order in which data is stored. Must be the endianness of the target platform.
	/// </summary>
	[Browsable(false)]
	public ByteOrder ByteOrder { get; private set; } = ByteOrder.LittleEndian;

	/// <summary>
	/// Gets or sets the alignment value.
	/// </summary>
	[Browsable(false)]
	public uint Alignment { get; set; }

	/// <summary>
	/// Gets or sets the target adress size to use for raw data blocks in the file.
	/// </summary>
	[Browsable(false)]
	public uint TargetAddressSize { get; set; }

	/// <summary>
	/// Gets or sets the flag. Unknown purpose.
	/// </summary>
	[Browsable(false)]
	public uint Flag { get; set; }

	/// <summary>
	/// Gets or sets the BlockOffset. 
	/// </summary>
	[Browsable(false)]
	public uint BlockOffset { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> (FMDL) names.
	/// </summary>
	[Browsable(false)]
	public ResDict ModelDict { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> (FMDL) instances.
	/// </summary>
	[Browsable(false)]
	public IList<Model> Models { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="T:Syroot.NintenTools.NSW.Bfres.SkeletalAnim" /> (FSKA) instances.
	/// </summary>
	[Browsable(false)]
	public ResDict SkeletalAnimDict { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="T:Syroot.NintenTools.NSW.Bfres.SkeletalAnim" /> (FSKA)  instances for many types of skeletal animations.
	/// </summary>
	[Browsable(false)]
	public IList<SkeletalAnim> SkeletalAnims { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="T:Syroot.NintenTools.NSW.Bfres.MaterialAnim" /> (FMAA) names.
	/// </summary>
	[Browsable(false)]
	public ResDict MaterialAnimDict { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="T:Syroot.NintenTools.NSW.Bfres.MaterialAnim" /> (FMAA) instances for many types of material animations.
	/// </summary>
	[Browsable(false)]
	public IList<MaterialAnim> MaterialAnims { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="T:Syroot.NintenTools.NSW.Bfres.VisibilityAnim" /> (FVIS) names
	/// </summary>
	[Browsable(false)]
	public ResDict BoneVisibilityAnimDict { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="T:Syroot.NintenTools.NSW.Bfres.VisibilityAnim" /> (FVIS) instances for bone visibility animations.
	/// </summary>
	[Browsable(false)]
	public IList<VisibilityAnim> BoneVisibilityAnims { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="T:Syroot.NintenTools.NSW.Bfres.ShapeAnim" /> (FSHA) names.
	/// </summary>
	[Browsable(false)]
	public ResDict ShapeAnimDict { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="T:Syroot.NintenTools.NSW.Bfres.ShapeAnim" /> (FSHA) instances.
	/// </summary>
	[Browsable(false)]
	public IList<ShapeAnim> ShapeAnims { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="T:Syroot.NintenTools.NSW.Bfres.SceneAnim" /> (FSCN) names.
	/// </summary>
	[Browsable(false)]
	public ResDict SceneAnimDict { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="T:Syroot.NintenTools.NSW.Bfres.SceneAnim" /> (FSCN) instances.
	/// </summary>
	[Browsable(false)]
	public IList<SceneAnim> SceneAnims { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="P:Syroot.NintenTools.NSW.Bfres.ResFile.MemoryPool" /> instances. 
	/// </summary>
	[Browsable(false)]
	public MemoryPool MemoryPool { get; set; }

	/// <summary>
	/// Gets or sets the stored <see cref="P:Syroot.NintenTools.NSW.Bfres.ResFile.BufferInfo" /> instances.
	/// </summary>
	[Browsable(false)]
	public BufferInfo BufferInfo { get; set; }

	/// <summary>
	/// Gets or sets attached <see cref="T:Syroot.NintenTools.NSW.Bfres.ExternalFile" /> names
	/// </summary>
	[Browsable(false)]
	public ResDict ExternalFileDict { get; set; }

	/// <summary>
	/// Gets or sets attached <see cref="T:Syroot.NintenTools.NSW.Bfres.ExternalFile" /> instances. The key of the dictionary typically represents
	/// the name of the file they were originally created from.
	/// </summary>
	[Browsable(false)]
	public IList<ExternalFile> ExternalFiles { get; set; }

	[Browsable(false)]
	public StringTable StringTable { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class.
	/// </summary>
	public ResFile()
	{
		Name = "";
		Alignment = 12u;
		TargetAddressSize = 0u;
		Flag = 0u;
		VersionMajor = 0u;
		VersionMajor2 = 5u;
		VersionMinor = 0u;
		VersionMinor2 = 3u;
		MemoryPool = new MemoryPool();
		BufferInfo = new BufferInfo();
		StringTable = new StringTable();
		Models = new List<Model>();
		SkeletalAnims = new List<SkeletalAnim>();
		MaterialAnims = new List<MaterialAnim>();
		BoneVisibilityAnims = new List<VisibilityAnim>();
		ShapeAnims = new List<ShapeAnim>();
		SceneAnims = new List<SceneAnim>();
		ExternalFiles = new List<ExternalFile>();
		ModelDict = new ResDict();
		SkeletalAnimDict = new ResDict();
		MaterialAnimDict = new ResDict();
		BoneVisibilityAnimDict = new ResDict();
		ShapeAnimDict = new ResDict();
		SceneAnimDict = new ResDict();
		ExternalFileDict = new ResDict();
		BlockOffset = 208u;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class from the given <paramref name="stream" /> which
	/// is optionally left open.
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to load the data from.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after reading, otherwise <c>false</c>.</param>
	public ResFile(Stream stream, bool leaveOpen = false)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, stream, leaveOpen);
		resFileLoader.Execute();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class from the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to load the data from.</param>
	public ResFile(string fileName)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, fileName);
		resFileLoader.Execute();
	}

	/// <summary>
	/// Saves the contents in the given <paramref name="stream" /> and optionally leaves it open
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to save the contents into.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
	public void Save(Stream stream, bool leaveOpen = false)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, stream, leaveOpen);
		resFileSaver.Execute();
	}

	/// <summary>
	/// Saves the contents in the file with the given <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to save the contents into.</param>
	public void Save(string fileName)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, fileName);
		resFileSaver.Execute();
	}

	internal uint SaveVersion()
	{
		return (VersionMajor << 24) | (VersionMajor2 << 16) | (VersionMinor << 8) | VersionMinor2;
	}

	internal void SetVersionInfo(uint Version)
	{
		VersionMajor = Version >> 24;
		VersionMajor2 = (Version >> 16) & 0xFF;
		VersionMinor = (Version >> 8) & 0xFF;
		VersionMinor2 = Version & 0xFF;
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FRES");
		uint num = loader.ReadUInt32();
		Version = loader.ReadUInt32();
		SetVersionInfo(Version);
		ByteOrder = loader.ReadEnum<ByteOrder>(strict: false);
		Alignment = loader.ReadByte();
		TargetAddressSize = loader.ReadByte();
		uint num2 = loader.ReadUInt32();
		Flag = loader.ReadUInt16();
		BlockOffset = loader.ReadUInt16();
		uint num3 = loader.ReadUInt32();
		uint num4 = loader.ReadUInt32();
		Name = loader.LoadString();
		long num5 = loader.ReadOffset();
		ModelDict = loader.LoadDict();
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			long num6 = loader.ReadOffset();
			long num7 = loader.ReadOffset();
			long num8 = loader.ReadOffset();
			long num9 = loader.ReadOffset();
		}
		long value = loader.ReadOffset();
		SkeletalAnimDict = loader.LoadDict();
		long value2 = loader.ReadOffset();
		MaterialAnimDict = loader.LoadDict();
		long value3 = loader.ReadOffset();
		BoneVisibilityAnimDict = loader.LoadDict();
		long value4 = loader.ReadOffset();
		ShapeAnimDict = loader.LoadDict();
		long value5 = loader.ReadOffset();
		SceneAnimDict = loader.LoadDict();
		MemoryPool = loader.Load<MemoryPool>(Relocated: true);
		BufferInfo = loader.Load<BufferInfo>(Relocated: true);
		if (loader.ResFile.VersionMajor2 >= 10)
		{
			ExternalFlags externalFlags = (ExternalFlags)PeekFlags();
			if (externalFlags.HasFlag(ExternalFlags.HoldsExternalStrings))
			{
				long offset = loader.ReadOffset();
				ResDict resDict = loader.LoadDict();
				using (loader.TemporarySeek(offset, SeekOrigin.Begin))
				{
					StringCache.Strings.Clear();
					foreach (string item in (IEnumerable)resDict)
					{
						long key = loader.ReadInt64();
						StringCache.Strings.Add(key, item);
					}
					return;
				}
			}
			if (externalFlags.HasFlag(ExternalFlags.HasExternalGPU))
			{
				DataAlignmentOverride = 4096u;
				using (loader.TemporarySeek(num4, SeekOrigin.Begin))
				{
					uint num10 = loader.ReadUInt32();
					uint num11 = loader.ReadUInt32();
					BufferInfo = new BufferInfo();
					BufferInfo.BufferOffset = num4 + 288;
				}
			}
		}
		long value7 = loader.ReadOffset();
		ExternalFileDict = loader.LoadDict();
		long num12 = loader.ReadInt64();
		StringTable = loader.Load<StringTable>(Relocated: true);
		uint num13 = loader.ReadUInt32();
		ushort num14 = loader.ReadUInt16();
		Console.WriteLine($"ModelArrayOffset {num5}");
		Console.WriteLine($"numModel {num14}");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			ushort num15 = loader.ReadUInt16();
			ushort num16 = loader.ReadUInt16();
			if (num15 != 0)
			{
				throw new Exception("unk1 has section!");
			}
			if (num16 != 0)
			{
				throw new Exception("unk2 has section!");
			}
		}
		ushort count = loader.ReadUInt16();
		ushort count2 = loader.ReadUInt16();
		ushort count3 = loader.ReadUInt16();
		ushort count4 = loader.ReadUInt16();
		ushort count5 = loader.ReadUInt16();
		ushort count6 = loader.ReadUInt16();
		byte b = loader.ReadByte();
		byte b2 = loader.ReadByte();
		if (b2 == 1)
		{
			DataAlignmentOverride = 4096u;
		}
		Models = loader.LoadList<Model>(num14, num5);
		SkeletalAnims = loader.LoadList<SkeletalAnim>(count, value);
		MaterialAnims = loader.LoadList<MaterialAnim>(count2, value2);
		BoneVisibilityAnims = loader.LoadList<VisibilityAnim>(count3, value3);
		ShapeAnims = loader.LoadList<ShapeAnim>(count4, value4);
		SceneAnims = loader.LoadList<SceneAnim>(count5, value5);
		ExternalFiles = loader.LoadList<ExternalFile>(count6, value7);
		byte PeekFlags()
		{
			using (loader.TemporarySeek(238L, SeekOrigin.Begin))
			{
				return loader.ReadByte();
			}
		}
	}

	void IResData.Save(ResFileSaver saver)
	{
		PreSave();
		saver.WriteSignature("FRES");
		saver.Write(538976288);
		saver.Write(SaveVersion());
		saver.Write(ByteOrder, strict: true);
		saver.Write((byte)Alignment);
		saver.Write((byte)TargetAddressSize);
		saver.SaveFileNameString(Name);
		saver.Write((ushort)Flag);
		saver.SaveHeaderBlock(IsBinaryHeader: true);
		saver.SaveRelocationTablePointerPointer();
		saver.SaveFieldFileSize();
		if (saver.ResFile.VersionMajor2 == 9)
		{
			saver.SaveRelocateEntryToSection(saver.Position, 15u, 1u, 0u, 1, "ResFile");
		}
		else
		{
			saver.SaveRelocateEntryToSection(saver.Position, 13u, 1u, 0u, 1, "ResFile");
		}
		saver.SaveString(Name);
		ModelOffset = saver.SaveOffset();
		ModelDictOffset = saver.SaveOffset();
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(0L);
			saver.Write(0L);
			saver.Write(0L);
			saver.Write(0L);
		}
		SkeletonAnimationOffset = saver.SaveOffset();
		SkeletonAnimationDictOffset = saver.SaveOffset();
		MaterialAnimationOffset = saver.SaveOffset();
		MaterialAnimationnDictOffset = saver.SaveOffset();
		BoneVisAnimationOffset = saver.SaveOffset();
		BoneVisAnimationDictOffset = saver.SaveOffset();
		ShapeAnimationOffset = saver.SaveOffset();
		ShapeAnimationDictOffset = saver.SaveOffset();
		SceneAnimationOffset = saver.SaveOffset();
		SceneAnimationDictOffset = saver.SaveOffset();
		if (MemoryPool != null)
		{
			saver.SaveRelocateEntryToSection(saver.Position, 1u, 1u, 0u, 4, "Memory pool");
		}
		saver.SaveMemoryPoolPointer();
		if (BufferInfo != null)
		{
			saver.SaveRelocateEntryToSection(saver.Position, 1u, 1u, 0u, 1, "Buffer info");
		}
		BufferInfoOffset = saver.SaveOffset();
		if (ExternalFiles.Count > 0)
		{
			saver.SaveRelocateEntryToSection(saver.Position, 2u, 1u, 0u, 1, "External Files");
		}
		ExternalFileOffset = saver.SaveOffset();
		ExternalFileDictOffset = saver.SaveOffset();
		saver.Write(0L);
		saver.SaveRelocateEntryToSection(saver.Position, 1u, 1u, 0u, 1, "String pool");
		saver.SaveFieldStringPool();
		saver.Write((ushort)ModelDict.Count);
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write((ushort)0);
			saver.Write((ushort)0);
		}
		saver.Write((ushort)SkeletalAnimDict.Count);
		saver.Write((ushort)MaterialAnimDict.Count);
		saver.Write((ushort)BoneVisibilityAnimDict.Count);
		saver.Write((ushort)ShapeAnimDict.Count);
		saver.Write((ushort)SceneAnimDict.Count);
		saver.Write((ushort)ExternalFileDict.Count);
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write((byte)0);
			saver.Write((byte)1);
		}
		else
		{
			saver.Seek(6L);
		}
	}

	private void SaveDataBlocks()
	{
	}

	private void PreSave()
	{
		ModelDict.Clear();
		SkeletalAnimDict.Clear();
		MaterialAnimDict.Clear();
		BoneVisibilityAnimDict.Clear();
		ShapeAnimDict.Clear();
		SceneAnimDict.Clear();
		if (Models.Count > 0)
		{
			MemoryPool = new MemoryPool();
		}
		foreach (Model model in Models)
		{
			model.ShapeDict.Clear();
			model.MaterialDict.Clear();
			model.UserDataDict.Clear();
			model.Skeleton.BoneDict.Clear();
			ModelDict.Add(model.Name);
			foreach (VertexBuffer vertexBuffer in model.VertexBuffers)
			{
				vertexBuffer.MemoryPool = MemoryPool;
				vertexBuffer.AttributeDict.Clear();
				foreach (VertexAttrib attribute in vertexBuffer.Attributes)
				{
					vertexBuffer.AttributeDict.Add(attribute.Name);
				}
			}
			foreach (Shape shape in model.Shapes)
			{
				foreach (Mesh mesh in shape.Meshes)
				{
					mesh.MemoryPool = MemoryPool;
				}
				model.ShapeDict.Add(shape.Name);
				shape.VertexBuffer = model.VertexBuffers[shape.VertexBufferIndex];
			}
			foreach (Material material in model.Materials)
			{
				material.RenderInfoDict.Clear();
				material.ShaderParamDict.Clear();
				material.UserDataDict.Clear();
				model.MaterialDict.Add(material.Name);
				foreach (RenderInfo renderInfo in material.RenderInfos)
				{
					material.RenderInfoDict.Add(renderInfo.Name);
				}
				foreach (ShaderParam shaderParam in material.ShaderParams)
				{
					material.ShaderParamDict.Add(shaderParam.Name);
				}
				foreach (UserData userData in material.UserDatas)
				{
					material.UserDataDict.Add(userData.Name);
				}
			}
			foreach (Bone bone in model.Skeleton.Bones)
			{
				model.Skeleton.BoneDict.Add(bone.Name);
				bone.UserDataDict.Clear();
				foreach (UserData userDatum in bone.UserData)
				{
					bone.UserDataDict.Add(userDatum.Name);
				}
			}
			foreach (UserData userDatum2 in model.UserData)
			{
				model.UserDataDict.Add(userDatum2.Name);
			}
		}
		foreach (SkeletalAnim skeletalAnim in SkeletalAnims)
		{
			SkeletalAnimDict.Add(skeletalAnim.Name);
			skeletalAnim.UserDataDict.Clear();
			foreach (UserData userData2 in skeletalAnim.UserDatas)
			{
				skeletalAnim.UserDataDict.Add(userData2.Name);
			}
			int num = 0;
			foreach (BoneAnim boneAnim in skeletalAnim.BoneAnims)
			{
				boneAnim.BeginCurve = num;
				num += boneAnim.Curves.Count;
			}
		}
		foreach (MaterialAnim materialAnim in MaterialAnims)
		{
			MaterialAnimDict.Add(materialAnim.Name);
			materialAnim.UserDataDict.Clear();
			foreach (UserData userDatum3 in materialAnim.UserData)
			{
				materialAnim.UserDataDict.Add(userDatum3.Name);
			}
			int num2 = 0;
			foreach (MaterialAnimData materialAnimData in materialAnim.MaterialAnimDataList)
			{
				num2 += materialAnimData.Curves.Count;
			}
		}
		foreach (VisibilityAnim boneVisibilityAnim in BoneVisibilityAnims)
		{
			BoneVisibilityAnimDict.Add(boneVisibilityAnim.Name);
			boneVisibilityAnim.UserDataDict.Clear();
			foreach (UserData userDatum4 in boneVisibilityAnim.UserData)
			{
				boneVisibilityAnim.UserDataDict.Add(userDatum4.Name);
			}
		}
		foreach (SceneAnim sceneAnim in SceneAnims)
		{
			sceneAnim.FogAnimDict.Clear();
			sceneAnim.LightAnimDict.Clear();
			sceneAnim.CameraAnimDict.Clear();
			SceneAnimDict.Add(sceneAnim.Name);
			sceneAnim.UserDataDict.Clear();
			foreach (UserData userDatum5 in sceneAnim.UserData)
			{
				sceneAnim.UserDataDict.Add(userDatum5.Name);
			}
			foreach (FogAnim fogAnim in sceneAnim.FogAnims)
			{
				sceneAnim.FogAnimDict.Add(fogAnim.Name);
				fogAnim.UserDataDict.Clear();
				foreach (UserData userDatum6 in fogAnim.UserData)
				{
					fogAnim.UserDataDict.Add(userDatum6.Name);
				}
			}
			foreach (LightAnim lightAnim in sceneAnim.LightAnims)
			{
				sceneAnim.LightAnimDict.Add(lightAnim.Name);
				lightAnim.UserDataDict.Clear();
				foreach (UserData userDatum7 in lightAnim.UserData)
				{
					lightAnim.UserDataDict.Add(userDatum7.Name);
				}
			}
			foreach (CameraAnim cameraAnim in sceneAnim.CameraAnims)
			{
				sceneAnim.CameraAnimDict.Add(cameraAnim.Name);
				cameraAnim.UserDataDict.Clear();
				foreach (UserData userDatum8 in cameraAnim.UserData)
				{
					cameraAnim.UserDataDict.Add(userDatum8.Name);
				}
			}
		}
		foreach (ShapeAnim shapeAnim in ShapeAnims)
		{
			ShapeAnimDict.Add(shapeAnim.Name);
			int num3 = 0;
			int num4 = 0;
			foreach (VertexShapeAnim vertexShapeAnim in shapeAnim.VertexShapeAnims)
			{
				vertexShapeAnim.BeginCurve = num3;
				vertexShapeAnim.BeginKeyShapeAnim = num4;
				num3 += vertexShapeAnim.Curves.Count;
				num4 += vertexShapeAnim.KeyShapeAnimInfos.Count;
			}
		}
	}
}
