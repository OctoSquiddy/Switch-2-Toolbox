using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an FMAA section in a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> subfile, storing material animation data.
/// </summary>
[DebuggerDisplay("MaterialAnim {Name}")]
public class MaterialAnim : IResData
{
	/// <summary>
	/// Represents flags specifying how animation data is stored or should be played.
	/// </summary>
	[Flags]
	public enum MaterialAnimFlags : ushort
	{
		/// <summary>
		/// The stored curve data has been baked.
		/// </summary>
		BakedCurve = 1,
		/// <summary>
		/// The animation repeats from the start after the last frame has been played.
		/// </summary>
		Looping = 4
	}

	private const string _signature = "FMAA";

	internal long PosBindModelOffset;

	internal long PosBindIndicesOffset;

	internal long PosMatAnimDataOffset;

	internal long PosTextureNamesUnkOffset;

	internal long PosTextureNamesOffset;

	internal long PosUserDataOffset;

	internal long PosUserDataDictOffset;

	internal long PosTextureBindArrayOffset;

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;MaterialAnim&gt;" />
	/// instances.
	/// </summary>
	[Browsable(true)]
	[Category("Animation")]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the path of the file which originally supplied the data of this instance.
	/// </summary>
	[Browsable(true)]
	[Category("Animation")]
	public string Path { get; set; }

	/// <summary>
	/// Gets or sets the total number of frames this animation plays.
	/// </summary>
	[Browsable(true)]
	[Category("Animation")]
	[DisplayName("Frame Count")]
	public int FrameCount { get; set; }

	[Browsable(true)]
	[Category("Animation")]
	public bool Loop
	{
		get
		{
			return Flags.HasFlag(MaterialAnimFlags.Looping);
		}
		set
		{
			if (value)
			{
				Flags |= MaterialAnimFlags.Looping;
			}
			else
			{
				Flags &= ~MaterialAnimFlags.Looping;
			}
		}
	}

	[Browsable(true)]
	[Category("Animation")]
	public bool Baked
	{
		get
		{
			return Flags.HasFlag(MaterialAnimFlags.BakedCurve);
		}
		set
		{
			if (value)
			{
				Flags |= MaterialAnimFlags.BakedCurve;
			}
			else
			{
				Flags &= ~MaterialAnimFlags.BakedCurve;
			}
		}
	}

	/// <summary>
	/// Gets or sets the number of bytes required to bake all <see cref="!:Curves" />.
	/// </summary>
	[Browsable(true)]
	[Category("Animation")]
	public uint BakedSize { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> instance affected by this animation.
	/// </summary>
	[Browsable(false)]
	public Model BindModel { get; set; }

	/// <summary>
	/// Gets the indices of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" /> instances in the <see cref="P:Syroot.NintenTools.NSW.Bfres.Model.Materials" /> dictionary to
	/// bind for each animation. <see cref="F:System.UInt16.MaxValue" /> specifies no binding.
	/// </summary>
	[Browsable(true)]
	[Category("Animation")]
	public ushort[] BindIndices { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.MaterialAnim.UserData" /> instances.
	/// </summary>
	[Browsable(false)]
	public ResDict UserDataDict { get; set; }

	[Browsable(false)]
	public IList<UserData> UserData { get; set; }

	[Browsable(false)]
	public IList<MaterialAnimData> MaterialAnimDataList { get; set; }

	[Browsable(false)]
	public IList<string> TextureNames { get; set; }

	[Browsable(false)]
	public IList<long> TextureBindArray { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.SkeletalAnimFlags" /> mode used to control looping and baked settings.
	/// </summary>
	[Browsable(false)]
	public MaterialAnimFlags Flags { get; set; }

	[Browsable(true)]
	[ReadOnly(true)]
	public ushort ShaderParamAnimCount { get; set; }

	[Browsable(true)]
	[ReadOnly(true)]
	public ushort TexturePatternAnimCount { get; set; }

	[Browsable(true)]
	[ReadOnly(true)]
	public ushort VisabiltyAnimCount { get; set; }

	[Browsable(true)]
	[ReadOnly(true)]
	public ushort TextureCount { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.MaterialAnim" /> class.
	/// </summary>
	public MaterialAnim()
	{
		Name = "";
		Path = "";
		BindModel = new Model();
		BindIndices = new ushort[0];
		TextureNames = new List<string>();
		TextureBindArray = new List<long>();
		Flags = (MaterialAnimFlags)0;
		FrameCount = 1;
		BakedSize = 0u;
		ShaderParamAnimCount = 0;
		TexturePatternAnimCount = 0;
		VisabiltyAnimCount = 0;
		TextureCount = 0;
		MaterialAnimDataList = new List<MaterialAnimData>();
		UserData = new List<UserData>();
		UserDataDict = new ResDict();
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
		resFileLoader.ImportMaterialAnims();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class from the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to load the data from.</param>
	public void Import(string fileName)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, fileName);
		resFileLoader.ImportMaterialAnims();
	}

	/// <summary>
	/// Saves the contents in the given <paramref name="stream" /> and optionally leaves it open
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to save the contents into.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
	public void Export(Stream stream, ResFile resFile, bool leaveOpen = false)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, stream, leaveOpen);
		resFileSaver.ExportMaterialAnimation();
	}

	/// <summary>
	/// Saves the contents in the file with the given <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to save the contents into.</param>
	public void Export(string fileName, ResFile resFile)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, fileName);
		resFileSaver.ExportMaterialAnimation();
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FMAA");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			Flags = loader.ReadEnum<MaterialAnimFlags>(strict: true);
			loader.ReadUInt16();
		}
		else
		{
			loader.LoadHeaderBlock();
		}
		Name = loader.LoadString();
		Path = loader.LoadString();
		BindModel = loader.Load<Model>(Relocated: true);
		long value = loader.ReadOffset();
		long value2 = loader.ReadOffset();
		long num = loader.ReadOffset();
		long value3 = loader.ReadOffset();
		long value4 = loader.ReadOffset();
		UserDataDict = loader.LoadDict();
		long value5 = loader.ReadOffset();
		if (loader.ResFile.VersionMajor2 < 9)
		{
			Flags = loader.ReadEnum<MaterialAnimFlags>(strict: true);
		}
		ushort num2 = 0;
		ushort PerMatAnimCount = 0;
		ushort num3 = 0;
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			FrameCount = loader.ReadInt32();
			BakedSize = loader.ReadUInt32();
			num2 = loader.ReadUInt16();
			PerMatAnimCount = loader.ReadUInt16();
			num3 = loader.ReadUInt16();
		}
		else
		{
			num2 = loader.ReadUInt16();
			PerMatAnimCount = loader.ReadUInt16();
			num3 = loader.ReadUInt16();
			FrameCount = loader.ReadInt32();
			BakedSize = loader.ReadUInt32();
		}
		ShaderParamAnimCount = loader.ReadUInt16();
		TexturePatternAnimCount = loader.ReadUInt16();
		VisabiltyAnimCount = loader.ReadUInt16();
		TextureCount = loader.ReadUInt16();
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			loader.ReadUInt16();
		}
		BindIndices = loader.LoadCustom(() => loader.ReadUInt16s(PerMatAnimCount), value);
		MaterialAnimDataList = loader.LoadList<MaterialAnimData>(PerMatAnimCount, value2);
		TextureNames = loader.LoadCustom(() => loader.LoadStrings(TextureCount), value3);
		TextureBindArray = loader.LoadCustom(() => loader.ReadInt64s(TextureCount), value5);
		UserData = loader.LoadList<UserData>(num2, value4);
	}

	internal void UpdateIndices()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		foreach (MaterialAnimData materialAnimData in MaterialAnimDataList)
		{
			foreach (TexturePatternAnimInfo texturePatternAnimInfo in materialAnimData.TexturePatternAnimInfos)
			{
			}
		}
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.WriteSignature("FMAA");
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(Flags, strict: true);
			saver.Seek(2L);
		}
		else
		{
			saver.Seek(12L);
		}
		saver.SaveRelocateEntryToSection(saver.Position, 2u, 1u, 0u, 1, "Material Animation");
		saver.SaveString(Name);
		saver.SaveString(Path);
		saver.Write(0L);
		saver.SaveRelocateEntryToSection(saver.Position, 7u, 1u, 0u, 1, "Material Animation");
		PosBindIndicesOffset = saver.SaveOffset();
		PosMatAnimDataOffset = saver.SaveOffset();
		PosTextureNamesUnkOffset = saver.SaveOffset();
		PosTextureNamesOffset = saver.SaveOffset();
		PosUserDataOffset = saver.SaveOffset();
		PosUserDataDictOffset = saver.SaveOffset();
		PosTextureBindArrayOffset = saver.SaveOffset();
		if (saver.ResFile.VersionMajor2 < 9)
		{
			saver.Write(Flags, strict: true);
		}
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(FrameCount);
			saver.Write(BakedSize);
			saver.Write((ushort)UserData.Count);
			saver.Write((ushort)MaterialAnimDataList.Count);
			saver.Write((ushort)MaterialAnimDataList.Sum((MaterialAnimData x) => x.Curves.Count));
		}
		else
		{
			saver.Write((ushort)UserData.Count);
			saver.Write((ushort)MaterialAnimDataList.Count);
			saver.Write((ushort)MaterialAnimDataList.Sum((MaterialAnimData x) => x.Curves.Count));
			saver.Write(FrameCount);
			saver.Write(BakedSize);
		}
		int num = MaterialAnimDataList.Sum((MaterialAnimData x) => (x.Constants != null) ? x.Constants.Count : 0);
		int num2 = 0;
		int num3 = MaterialAnimDataList.Sum((MaterialAnimData x) => x.TexturePatternAnimInfos.Count);
		for (int num4 = 0; num4 < MaterialAnimDataList.Count; num4++)
		{
			foreach (ParamAnimInfo paramAnimInfo in MaterialAnimDataList[num4].ParamAnimInfos)
			{
				num2 += paramAnimInfo.ConstantCount + paramAnimInfo.FloatCurveCount + paramAnimInfo.IntCurveCount;
			}
		}
		saver.Write((ushort)num2);
		if (num3 != 0)
		{
			saver.Write((ushort)(num3 + num));
		}
		else
		{
			saver.Write((ushort)0);
		}
		saver.Write(VisabiltyAnimCount);
		if (TextureNames != null)
		{
			saver.Write((ushort)TextureNames.Count);
		}
		else
		{
			saver.Write(TextureCount);
		}
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write((ushort)0);
		}
	}
}
