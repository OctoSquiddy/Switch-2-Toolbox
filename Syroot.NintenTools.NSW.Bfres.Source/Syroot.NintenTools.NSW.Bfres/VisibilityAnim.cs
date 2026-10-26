using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an FVIS subfile in a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" />, storing visibility animations of <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" /> or
/// <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" /> instances.
/// </summary>
[DebuggerDisplay("VisibilityAnim {Name}")]
public class VisibilityAnim : IResData
{
	private const string _signature = "FBVS";

	private const ushort _flagsMask = 7;

	private const ushort _flagsMaskType = 256;

	internal ushort _flags;

	private ushort Unknown;

	internal long PosBindModelOffset;

	internal long PosBindIndicesOffset;

	internal long PosCurvesOffset;

	internal long PosBaseDataOffset;

	internal long PosNamesOffset;

	internal long PosUserDataOffset;

	internal long PosUserDataDictOffset;

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in
	/// <see cref="!:ResDict&lt;VisibilityAnim&gt;" /> instances.
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

	/// <summary>
	/// Gets or sets flags controlling how animation data is stored or how the animation should be played.
	/// </summary>
	[Browsable(false)]
	public VisibilityAnimFlags Flags
	{
		get
		{
			return (VisibilityAnimFlags)(_flags & 7);
		}
		set
		{
			_flags = (ushort)((uint)(_flags & -8) | (uint)value);
		}
	}

	[Browsable(true)]
	[Category("Animation")]
	public bool Loop
	{
		get
		{
			return Flags.HasFlag(VisibilityAnimFlags.Looping);
		}
		set
		{
			if (value)
			{
				Flags |= VisibilityAnimFlags.Looping;
			}
			else
			{
				Flags &= ~VisibilityAnimFlags.Looping;
			}
		}
	}

	[Browsable(true)]
	[Category("Animation")]
	public bool Baked
	{
		get
		{
			return Flags.HasFlag(VisibilityAnimFlags.BakedCurve);
		}
		set
		{
			if (value)
			{
				Flags |= VisibilityAnimFlags.BakedCurve;
			}
			else
			{
				Flags &= ~VisibilityAnimFlags.BakedCurve;
			}
		}
	}

	/// <summary>
	/// Gets or sets the number of bytes required to bake all <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> instances of all
	/// <see cref="!:BoneAnims" />.
	/// </summary>
	[Browsable(true)]
	[ReadOnly(true)]
	[Category("Animation")]
	[DisplayName("Baked Size")]
	public uint BakedSize { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> instance affected by this animation.
	/// </summary>
	[Browsable(false)]
	public Model BindModel { get; set; }

	/// <summary>
	/// Gets or sets the indices of entries in the <see cref="P:Syroot.NintenTools.NSW.Bfres.Skeleton.Bones" /> or <see cref="P:Syroot.NintenTools.NSW.Bfres.Model.Materials" />
	/// dictionaries to bind to for each animation. <see cref="F:System.UInt16.MaxValue" /> specifies no binding.
	/// </summary>
	[Browsable(false)]
	public ushort[] BindIndices { get; set; }

	/// <summary>
	/// Gets or sets the names of entries in the <see cref="P:Syroot.NintenTools.NSW.Bfres.Skeleton.Bones" /> or <see cref="P:Syroot.NintenTools.NSW.Bfres.Model.Materials" />
	/// dictionaries to bind to for each animation.
	/// </summary>
	[Browsable(false)]
	public IList<string> Names { get; set; }

	/// <summary>
	/// Gets or sets <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> instances animating properties of objects stored in this section.
	/// </summary>
	[Browsable(false)]
	public IList<AnimCurve> Curves { get; set; }

	/// <summary>
	/// Gets or sets boolean values storing the initial visibility for each <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" /> or
	/// <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" />.
	/// </summary>
	[Browsable(false)]
	public bool[] BaseDataList { get; set; }

	[Browsable(false)]
	public byte[] BaseSDataList { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.VisibilityAnim.UserData" /> instances.
	/// </summary>
	[Browsable(false)]
	public ResDict UserDataDict { get; set; }

	[Browsable(false)]
	public IList<UserData> UserData { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.VisibilityAnim" /> class.
	/// </summary>
	public VisibilityAnim()
	{
		Name = "";
		Path = "";
		Flags = (VisibilityAnimFlags)0;
		FrameCount = 1;
		BakedSize = 0u;
		Curves = new List<AnimCurve>();
		BindIndices = new ushort[0];
		Names = new List<string>();
		BaseDataList = new bool[0];
		BaseSDataList = new byte[0];
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
		resFileLoader.ImportBoneVisualAnimations();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class from the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to load the data from.</param>
	public void Import(string fileName)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, fileName);
		resFileLoader.ImportBoneVisualAnimations();
	}

	/// <summary>
	/// Saves the contents in the given <paramref name="stream" /> and optionally leaves it open
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to save the contents into.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
	public void Export(Stream stream, ResFile ResFile, bool leaveOpen = false)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, ResFile, stream, leaveOpen);
		resFileSaver.ExportBoneVisualAnimation();
	}

	/// <summary>
	/// Saves the contents in the file with the given <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to save the contents into.</param>
	public void Export(string fileName, ResFile ResFile)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, ResFile, fileName);
		resFileSaver.ExportBoneVisualAnimation();
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FBVS");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			_flags = loader.ReadUInt16();
			loader.ReadUInt16();
		}
		else
		{
			loader.LoadHeaderBlock();
		}
		Name = loader.LoadString();
		Path = loader.LoadString();
		BindModel = loader.Load<Model>();
		long value = loader.ReadOffset();
		long value2 = loader.ReadOffset();
		long value3 = loader.ReadOffset();
		long value4 = loader.ReadOffset();
		long value5 = loader.ReadOffset();
		UserDataDict = loader.LoadDict();
		ushort num = 0;
		ushort numAnim = 0;
		ushort num2 = 0;
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			FrameCount = loader.ReadInt32();
			BakedSize = loader.ReadUInt32();
			numAnim = loader.ReadUInt16();
			num2 = loader.ReadUInt16();
			num = loader.ReadUInt16();
			loader.ReadUInt16();
		}
		else
		{
			_flags = loader.ReadUInt16();
			num = loader.ReadUInt16();
			FrameCount = loader.ReadInt32();
			numAnim = loader.ReadUInt16();
			num2 = loader.ReadUInt16();
			BakedSize = loader.ReadUInt32();
		}
		BindIndices = loader.LoadCustom(() => loader.ReadUInt16s(numAnim), value);
		Names = loader.LoadCustom(() => loader.LoadStrings(numAnim), value4);
		Curves = loader.LoadList<AnimCurve>(num2, value2);
		UserData = loader.LoadList<UserData>(num, value5);
		BaseDataList = new bool[numAnim];
		BaseSDataList = loader.LoadCustom(() =>
		{
			List<byte> list = new List<byte>();
			int num3 = 0;
			while (num3 < numAnim)
			{
				byte b = loader.ReadByte();
				list.Add(b);
				for (int i = 0; i < 8; i++)
				{
					if (num3 >= numAnim)
					{
						break;
					}
					BaseDataList[num3] = b.GetBit(i);
					num3++;
				}
			}
			return list.ToArray();
		}, value3);
	}

	internal void WriteBaseData(ResFileSaver saver)
	{
		Console.WriteLine(Name + " " + BaseSDataList.Length);
		saver.Write(BaseSDataList);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.WriteSignature("FBVS");
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(_flags);
			saver.Write((ushort)0);
		}
		else
		{
			saver.SaveHeaderBlock();
		}
		saver.SaveString(Name);
		saver.SaveString(Path);
		saver.Write(0L);
		PosBindIndicesOffset = saver.SaveOffset();
		PosCurvesOffset = saver.SaveOffset();
		PosBaseDataOffset = saver.SaveOffset();
		PosNamesOffset = saver.SaveOffset();
		PosUserDataOffset = saver.SaveOffset();
		PosUserDataDictOffset = saver.SaveOffset();
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(FrameCount);
			saver.Write(BakedSize);
			if (Names != null)
			{
				saver.Write((ushort)Names.Count);
			}
			else
			{
				saver.Write((ushort)0);
			}
			saver.Write((ushort)Curves.Count);
			saver.Write((ushort)UserDataDict.Count);
			saver.Write((ushort)0);
		}
		else
		{
			saver.Write(_flags);
			saver.Write((ushort)UserDataDict.Count);
			saver.Write(FrameCount);
			if (Names != null)
			{
				saver.Write((ushort)Names.Count);
			}
			else
			{
				saver.Write((ushort)0);
			}
			saver.Write((ushort)Curves.Count);
			saver.Write(BakedSize);
		}
	}
}
