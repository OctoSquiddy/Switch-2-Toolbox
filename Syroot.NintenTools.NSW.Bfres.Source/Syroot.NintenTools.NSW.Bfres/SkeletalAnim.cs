using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an FSKA subfile in a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" />, storing armature animations of <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" />
/// instances in a <see cref="T:Syroot.NintenTools.NSW.Bfres.Skeleton" />.
/// </summary>
[DebuggerDisplay("SkeletalAnim {Name}")]
public class SkeletalAnim : IResData
{
	public class CopyFilter
	{
		public bool CopySettings = true;

		public bool CopyBoneAnims = true;

		public bool CopyUserData = true;

		public bool CopyBaseData = true;
	}

	private const string _signature = "FSKA";

	private const uint _flagsMaskScale = 768u;

	private const uint _flagsMaskRotate = 28672u;

	private const uint _flagsMaskAnimSettings = 15u;

	private uint _flags;

	internal long BindSkeletonOffset = 0L;

	internal long BindIndicesOffset = 0L;

	internal long BoneAnimsOffset = 0L;

	internal long UserDataOffset = 0L;

	internal long UserDataDictOffset = 0L;

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in
	/// <see cref="!:ResDict&lt;SkeletalAnim&gt;" /> instances.
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
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.SkeletalAnimFlags" /> mode used to control looping and baked settings.
	/// </summary>
	[Browsable(false)]
	public SkeletalAnimFlags FlagsAnimSettings
	{
		get
		{
			return (SkeletalAnimFlags)(_flags & 0xF);
		}
		set
		{
			_flags = (_flags & 0xFFFFFFF0u) | (uint)value;
		}
	}

	[Browsable(true)]
	[Category("Animation")]
	public bool Loop
	{
		get
		{
			return FlagsAnimSettings.HasFlag(SkeletalAnimFlags.Looping);
		}
		set
		{
			if (value)
			{
				FlagsAnimSettings |= SkeletalAnimFlags.Looping;
			}
			else
			{
				FlagsAnimSettings &= ~SkeletalAnimFlags.Looping;
			}
		}
	}

	[Browsable(true)]
	[Category("Animation")]
	public bool Baked
	{
		get
		{
			return FlagsAnimSettings.HasFlag(SkeletalAnimFlags.BakedCurve);
		}
		set
		{
			if (value)
			{
				FlagsAnimSettings |= SkeletalAnimFlags.BakedCurve;
			}
			else
			{
				FlagsAnimSettings &= ~SkeletalAnimFlags.BakedCurve;
			}
		}
	}

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.SkeletalAnimFlagsScale" /> mode used to store scaling values.
	/// </summary>
	[Browsable(true)]
	[Category("Animation")]
	[DisplayName("Scaling")]
	public SkeletalAnimFlagsScale FlagsScale
	{
		get
		{
			return (SkeletalAnimFlagsScale)(_flags & 0x300);
		}
		set
		{
			_flags = (_flags & 0xFFFFFCFFu) | (uint)value;
		}
	}

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.SkeletalAnimFlagsRotate" /> mode used to store rotation values.
	/// </summary>
	[Browsable(true)]
	[Category("Animation")]
	[DisplayName("Rotation")]
	public SkeletalAnimFlagsRotate FlagsRotate
	{
		get
		{
			return (SkeletalAnimFlagsRotate)(_flags & 0x7000);
		}
		set
		{
			_flags = (_flags & 0xFFFF8FFFu) | (uint)value;
		}
	}

	/// <summary>
	/// Gets or sets the number of bytes required to bake all <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> instances of all
	/// <see cref="P:Syroot.NintenTools.NSW.Bfres.SkeletalAnim.BoneAnims" />.
	/// </summary>
	[Browsable(true)]
	[ReadOnly(true)]
	[Category("Animation")]
	[DisplayName("Baked Size")]
	public uint BakedSize { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.BoneAnim" /> instances creating the animation.
	/// </summary>
	[Browsable(false)]
	public IList<BoneAnim> BoneAnims { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.Skeleton" /> instance affected by this animation.
	/// </summary>
	[Browsable(false)]
	public Skeleton BindSkeleton { get; set; }

	/// <summary>
	/// Gets or sets the indices of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" /> instances in the <see cref="P:Syroot.NintenTools.NSW.Bfres.Skeleton.Bones" /> dictionary
	/// to bind for each animation. <see cref="F:System.UInt16.MaxValue" /> specifies no binding.
	/// </summary>
	[Browsable(false)]
	public ushort[] BindIndices { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="T:Syroot.NintenTools.NSW.Bfres.UserData" /> instances.
	/// </summary>
	[Browsable(false)]
	public ResDict UserDataDict { get; set; }

	[Browsable(false)]
	public IList<UserData> UserDatas { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.SkeletalAnim" /> class.
	/// </summary>
	public SkeletalAnim()
	{
		Name = "";
		Path = "";
		FlagsAnimSettings = (SkeletalAnimFlags)0u;
		FlagsScale = SkeletalAnimFlagsScale.Maya;
		FlagsRotate = SkeletalAnimFlagsRotate.EulerXYZ;
		FrameCount = 1;
		BakedSize = 0u;
		BoneAnims = new List<BoneAnim>();
		BindSkeleton = new Skeleton();
		BindIndices = new ushort[0];
		UserDatas = new List<UserData>();
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
		resFileLoader.ImportSkeletonAnimations();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class from the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to load the data from.</param>
	public void Import(string fileName)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, fileName);
		resFileLoader.ImportSkeletonAnimations();
	}

	/// <summary>
	/// Saves the contents in the given <paramref name="stream" /> and optionally leaves it open
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to save the contents into.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
	public void Export(Stream stream, ResFile ResFile, bool leaveOpen = false)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, ResFile, stream, leaveOpen);
		resFileSaver.ExportSkeletonAnimation();
	}

	/// <summary>
	/// Saves the contents in the file with the given <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to save the contents into.</param>
	public void Export(string fileName, ResFile ResFile)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, ResFile, fileName);
		resFileSaver.ExportSkeletonAnimation();
	}

	public void Copy(SkeletalAnim target, CopyFilter filter)
	{
		if (filter.CopyBoneAnims)
		{
			BakedSize = target.BakedSize;
		}
		if (filter.CopySettings)
		{
			FlagsAnimSettings = target.FlagsAnimSettings;
			FlagsRotate = target.FlagsRotate;
			FlagsScale = target.FlagsScale;
			FrameCount = target.FrameCount;
		}
		if (filter.CopyUserData)
		{
			UserDatas.Clear();
			UserDataDict.Clear();
			foreach (UserData userData in target.UserDatas)
			{
				UserDatas.Add(userData.Copy());
			}
			foreach (UserData userData2 in target.UserDatas)
			{
				UserDataDict.Add(userData2.Name);
			}
		}
		if (!filter.CopyBoneAnims)
		{
			return;
		}
		BindSkeleton = target.BindSkeleton;
		BindIndices = target.BindIndices;
		BoneAnims.Clear();
		for (int i = 0; i < target.BoneAnims?.Count; i++)
		{
			List<AnimCurve> list = new List<AnimCurve>();
			for (int j = 0; j < target.BoneAnims[i].Curves?.Count; j++)
			{
				list.Add(target.BoneAnims[i].Curves[j].Copy());
			}
			BoneAnims.Add(new BoneAnim
			{
				Name = target.BoneAnims[i].Name,
				Curves = list,
				FlagsBase = target.BoneAnims[i].FlagsBase,
				FlagsCurve = target.BoneAnims[i].FlagsCurve,
				FlagsTransform = target.BoneAnims[i].FlagsTransform,
				BeginBaseTranslate = target.BoneAnims[i].BeginBaseTranslate,
				BeginCurve = target.BoneAnims[i].BeginCurve,
				BeginRotate = target.BoneAnims[i].BeginRotate,
				BeginTranslate = target.BoneAnims[i].BeginTranslate,
				UseRotation = target.BoneAnims[i].UseRotation,
				UseScale = target.BoneAnims[i].UseScale,
				UseTranslation = target.BoneAnims[i].UseTranslation,
				BaseData = new BoneAnimData
				{
					Translate = target.BoneAnims[i].BaseData.Translate,
					Flags = target.BoneAnims[i].BaseData.Flags,
					Padding = target.BoneAnims[i].BaseData.Padding,
					Rotate = target.BoneAnims[i].BaseData.Rotate,
					Scale = target.BoneAnims[i].BaseData.Scale
				}
			});
		}
	}

	public void ExportYaml()
	{
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FSKA");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			_flags = loader.ReadUInt32();
		}
		else
		{
			loader.LoadHeaderBlock();
		}
		Name = loader.LoadString();
		Path = loader.LoadString();
		BindSkeleton = loader.Load<Skeleton>();
		long value = loader.ReadOffset();
		long value2 = loader.ReadOffset();
		long value3 = loader.ReadOffset();
		UserDataDict = loader.LoadDict();
		if (loader.ResFile.VersionMajor2 < 9)
		{
			_flags = loader.ReadUInt32();
		}
		FrameCount = loader.ReadInt32();
		int num = loader.ReadInt32();
		BakedSize = loader.ReadUInt32();
		ushort numBoneAnim = loader.ReadUInt16();
		ushort count = loader.ReadUInt16();
		if (loader.ResFile.VersionMajor2 < 9)
		{
			loader.ReadUInt32();
		}
		BoneAnims = loader.LoadList<BoneAnim>(numBoneAnim, value2);
		BindIndices = loader.LoadCustom(() => loader.ReadUInt16s(numBoneAnim), value);
		UserDatas = loader.LoadList<UserData>(count, value3);
	}

	void IResData.Save(ResFileSaver saver)
	{
		if (BindIndices == null)
		{
			BindIndices = new ushort[0];
		}
		saver.WriteSignature("FSKA");
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(_flags);
		}
		else
		{
			saver.SaveHeaderBlock();
		}
		saver.SaveString(Name);
		saver.SaveString(Path);
		saver.Write(0L);
		BindIndicesOffset = saver.SaveOffset();
		BoneAnimsOffset = saver.SaveOffset();
		UserDataOffset = saver.SaveOffset();
		UserDataDictOffset = saver.SaveOffset();
		if (saver.ResFile.VersionMajor2 < 9)
		{
			saver.Write(_flags);
		}
		saver.Write(FrameCount);
		saver.Write(BoneAnims.Sum((BoneAnim x) => x.Curves.Count));
		saver.Write(BakedSize);
		saver.Write((ushort)BoneAnims.Count);
		saver.Write((ushort)UserDataDict.Count);
		if (saver.ResFile.VersionMajor2 < 9)
		{
			saver.Write(0);
		}
	}
}
