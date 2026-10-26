using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an FMAT subsection of a <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> subfile, storing information on with which textures and
/// how technically a surface is drawn.
/// </summary>
[DebuggerDisplay("Material {Name}")]
public class Material : IResData
{
	private const string _signature = "FMAT";

	internal MaterialParserV10.ShaderInfo ShaderInfoV10 = new MaterialParserV10.ShaderInfo();

	public long SourceParamOffset;

	public int RenderInfoSize;

	internal long PosRenderInfoOffset;

	internal long PosRenderInfoDictOffset;

	internal long PosShaderAssignOffset;

	internal long PosTextureUnk1Offset;

	internal long PosTextureRefsOffset;

	internal long PosTextureUnk2Offset;

	internal long PosSamplersOffset;

	internal long PosSamplerDictOffset;

	internal long PosShaderParamsOffset;

	internal long PosShaderParamDictOffset;

	internal long PosShaderParamDataOffset;

	internal long PosUserDataMaterialOffset;

	internal long PosUserDataDictMaterialOffset;

	internal long PosVolatileFlagsOffset;

	internal long PosSamplerSlotArrayOffset;

	internal long PosTextureSlotArrayOffset;

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;Material&gt;" />
	/// instances.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets flags specifying how a <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" /> is rendered.
	/// </summary>
	public MaterialFlags Flags { get; set; }

	public ResDict RenderInfoDict { get; set; }

	public List<RenderInfo> RenderInfos { get; set; }

	public ShaderAssign ShaderAssign { get; set; }

	/// <summary>
	/// Gets or sets the list of <see cref="T:System.String" /> instances referencing the <see cref="!:Texture" /> instances
	/// required to draw the material.
	/// </summary>
	public IList<string> TextureRefs { get; set; }

	/// <summary>
	/// Gets or sets a dictionary of <see cref="T:Syroot.NintenTools.NSW.Bfres.Sampler" /> instances which configure how to draw
	/// <see cref="!:Texture" /> instances referenced by the <see cref="P:Syroot.NintenTools.NSW.Bfres.Material.TextureRefs" /> list.
	/// </summary>
	public ResDict SamplerDict { get; set; }

	public IList<Sampler> Samplers { get; set; }

	public ResDict ShaderParamDict { get; set; }

	public IList<ShaderParam> ShaderParams { get; set; }

	/// <summary>
	/// Gets or sets the raw data block which stores <see cref="T:Syroot.NintenTools.NSW.Bfres.ShaderParam" /> values.
	/// </summary>
	public byte[] ShaderParamData { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="T:Syroot.NintenTools.NSW.Bfres.UserData" /> instances.
	/// </summary>
	public ResDict UserDataDict { get; set; }

	public IList<UserData> UserDatas { get; set; }

	/// <summary>
	/// Gets or sets a set of bits determining whether <see cref="T:Syroot.NintenTools.NSW.Bfres.ShaderParam" /> instances are volatile.
	/// </summary>
	public byte[] VolatileFlags { get; set; }

	public long[] TextureSlotArray { get; set; }

	public long[] SamplerSlotArray { get; set; }

	public int[] ParamIndices { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" /> class.
	/// </summary>
	public Material()
	{
		Name = "";
		RenderInfoDict = new ResDict();
		ShaderAssign = new ShaderAssign();
		SamplerDict = new ResDict();
		ShaderParamDict = new ResDict();
		UserDataDict = new ResDict();
		Flags = MaterialFlags.Visible;
		SourceParamOffset = 0L;
		RenderInfos = new List<RenderInfo>();
		TextureRefs = new List<string>();
		Samplers = new List<Sampler>();
		UserDatas = new List<UserData>();
		ShaderParams = new List<ShaderParam>();
		ShaderParamData = new byte[0];
		VolatileFlags = new byte[0];
		TextureSlotArray = new long[0];
		SamplerSlotArray = new long[0];
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" /> class from the given <paramref name="stream" /> which
	/// is optionally left open.
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to load the data from.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after reading, otherwise <c>false</c>.</param>
	public void Import(Stream stream, bool leaveOpen = false)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, stream, leaveOpen);
		resFileLoader.ImportMaterials();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class from the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to load the data from.</param>
	public void Import(string fileName)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, fileName);
		resFileLoader.ImportMaterials();
	}

	/// <summary>
	/// Saves the contents in the given <paramref name="stream" /> and optionally leaves it open
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to save the contents into.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
	public void Export(Stream stream, ResFile resFile, bool leaveOpen = false)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, stream, leaveOpen);
		resFileSaver.ExportMaterials();
	}

	/// <summary>
	/// Saves the contents in the file with the given <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to save the contents into.</param>
	public void Export(string fileName, ResFile resFile)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, fileName);
		resFileSaver.ExportMaterials();
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FMAT");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			Flags = loader.ReadEnum<MaterialFlags>(strict: true);
		}
		else
		{
			loader.LoadHeaderBlock();
		}
		Name = loader.LoadString();
		if (loader.ResFile.VersionMajor2 >= 10)
		{
			MaterialParserV10.Load(loader, this);
			return;
		}
		long value = loader.ReadOffset();
		RenderInfoDict = loader.LoadDict();
		ShaderAssign = loader.Load<ShaderAssign>();
		long num = loader.ReadOffset();
		long value2 = loader.ReadOffset();
		long num2 = loader.ReadOffset();
		long value3 = loader.ReadOffset();
		SamplerDict = loader.LoadDict();
		long value4 = loader.ReadOffset();
		ShaderParamDict = loader.LoadDict();
		SourceParamOffset = loader.ReadOffset();
		long value5 = loader.ReadOffset();
		UserDataDict = loader.LoadDict();
		long value6 = loader.ReadOffset();
		long num3 = loader.ReadOffset();
		long value7 = loader.ReadOffset();
		long value8 = loader.ReadOffset();
		if (loader.ResFile.VersionMajor2 != 9)
		{
			Flags = loader.ReadEnum<MaterialFlags>(strict: true);
		}
		ushort num4 = loader.ReadUInt16();
		ushort count = loader.ReadUInt16();
		byte numTextureRef = loader.ReadByte();
		byte numSampler = loader.ReadByte();
		ushort numShaderParam = loader.ReadUInt16();
		ushort num5 = loader.ReadUInt16();
		ushort sizParamSource = loader.ReadUInt16();
		ushort num6 = loader.ReadUInt16();
		ushort count2 = loader.ReadUInt16();
		if (loader.ResFile.VersionMajor2 != 9)
		{
			loader.ReadUInt32();
		}
		RenderInfos = loader.LoadList<RenderInfo>(count, value).ToList();
		TextureRefs = loader.LoadCustom(() => loader.LoadStrings(numTextureRef), value2);
		Samplers = loader.LoadList<Sampler>(numSampler, value3);
		UserDatas = loader.LoadList<UserData>(count2, value5);
		ShaderParams = loader.LoadList<ShaderParam>(numShaderParam, value4);
		ShaderParamData = loader.LoadCustom(() => loader.ReadBytes(sizParamSource), SourceParamOffset);
		VolatileFlags = loader.LoadCustom(() => loader.ReadBytes((int)Math.Ceiling((float)(int)numShaderParam / 8f)), value6);
		TextureSlotArray = loader.LoadCustom(() => loader.ReadInt64s(numTextureRef), value7);
		SamplerSlotArray = loader.LoadCustom(() => loader.ReadInt64s(numSampler), value8);
	}

	void IResData.Save(ResFileSaver saver)
	{
		if (VolatileFlags == null)
		{
			VolatileFlags = new byte[0];
		}
		if (TextureRefs == null)
		{
			TextureRefs = new List<string>();
		}
		if (ShaderParamData == null)
		{
			ShaderParamData = new byte[0];
		}
		long[] array = new long[TextureRefs.Count];
		long[] array2 = new long[TextureRefs.Count * 15];
		saver.WriteSignature("FMAT");
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(Flags, strict: true);
		}
		else
		{
			saver.Seek(12L);
		}
		if (saver.ResFile.VersionMajor2 >= 10)
		{
			MaterialParserV10.Save(saver, this);
			return;
		}
		saver.SaveRelocateEntryToSection(saver.Position, 15u, 1u, 0u, 1, "FMAT");
		saver.SaveString(Name);
		PosRenderInfoOffset = saver.SaveOffset();
		PosRenderInfoDictOffset = saver.SaveOffset();
		PosShaderAssignOffset = saver.SaveOffset();
		PosTextureUnk1Offset = saver.SaveOffset();
		PosTextureRefsOffset = saver.SaveOffset();
		PosTextureUnk2Offset = saver.SaveOffset();
		PosSamplersOffset = saver.SaveOffset();
		PosSamplerDictOffset = saver.SaveOffset();
		PosShaderParamsOffset = saver.SaveOffset();
		PosShaderParamDictOffset = saver.SaveOffset();
		PosShaderParamDataOffset = saver.SaveOffset();
		PosUserDataMaterialOffset = saver.SaveOffset();
		PosUserDataDictMaterialOffset = saver.SaveOffset();
		PosVolatileFlagsOffset = saver.SaveOffset();
		saver.Write(0L);
		saver.SaveRelocateEntryToSection(saver.Position, 2u, 1u, 0u, 1, "Material texture slots");
		PosSamplerSlotArrayOffset = saver.SaveOffset();
		PosTextureSlotArrayOffset = saver.SaveOffset();
		if (saver.ResFile.VersionMajor2 != 9)
		{
			saver.Write(Flags, strict: true);
		}
		saver.Write((ushort)saver.CurrentIndex);
		saver.Write((ushort)RenderInfoDict.Count);
		saver.Write((byte)Samplers.Count);
		saver.Write((byte)TextureRefs.Count);
		saver.Write((ushort)ShaderParams.Count);
		saver.Write((ushort)0);
		saver.Write((ushort)ShaderParamData.Length);
		saver.Write((ushort)0);
		saver.Write((ushort)UserDataDict.Count);
		if (saver.ResFile.VersionMajor2 != 9)
		{
			saver.Write(0);
		}
	}
}
