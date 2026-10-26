using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the animation of a single <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" /> in a <see cref="T:Syroot.NintenTools.NSW.Bfres.SkeletalAnim" /> subfile.
/// </summary>
[DebuggerDisplay("BoneAnim {Name}")]
public class BoneAnim : IResData
{
	private const uint _flagsMaskBase = 56u;

	private const uint _flagsMaskCurve = 65472u;

	private const uint _flagsMaskTransform = 260046848u;

	private uint _flags;

	internal long PosCurvesOffset;

	internal long PosBaseDataOffset;

	/// <summary>
	/// Gets or sets a set of flags indicating whether initial transformation values exist in
	/// <see cref="P:Syroot.NintenTools.NSW.Bfres.BoneAnim.BaseData" />.
	/// </summary>
	[Browsable(false)]
	public BoneAnimFlagsBase FlagsBase
	{
		get
		{
			return (BoneAnimFlagsBase)(_flags & 0x38);
		}
		set
		{
			_flags = (_flags & 0xFFFFFFC7u) | (uint)value;
		}
	}

	/// <summary>
	/// Gets or sets a set of flags indicating whether curves animating the corresponding transformation exist.
	/// </summary>
	[Browsable(false)]
	public BoneAnimFlagsCurve FlagsCurve
	{
		get
		{
			return (BoneAnimFlagsCurve)(_flags & 0xFFC0);
		}
		set
		{
			_flags = (_flags & 0xFFFF003Fu) | (uint)value;
		}
	}

	/// <summary>
	/// Gets or sets a set of flags controlling how to transform bones.
	/// </summary>
	[Browsable(false)]
	public BoneAnimFlagsTransform FlagsTransform
	{
		get
		{
			return (BoneAnimFlagsTransform)(_flags & 0xF800000);
		}
		set
		{
			_flags = (_flags & 0xF07FFFFFu) | (uint)value;
		}
	}

	/// <summary>
	/// Gets or sets the name of the animated <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" />.
	/// </summary>
	[Browsable(true)]
	[Category("Bone")]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets a field with unknown purpose.
	/// </summary>
	[Browsable(true)]
	[Category("Bone")]
	[DisplayName("Begin Rotate")]
	public byte BeginRotate { get; set; }

	/// <summary>
	/// Gets or sets a field with unknown purpose.
	/// </summary>
	[Browsable(true)]
	[Category("Bone")]
	[DisplayName("Begin Translate")]
	public byte BeginTranslate { get; set; }

	/// <summary>
	/// Gets or sets the element offset in the <see cref="P:Syroot.NintenTools.NSW.Bfres.BoneAnim.BaseData" /> to an initial translation.
	/// </summary>
	[Browsable(true)]
	[Category("Bone")]
	[DisplayName("Begin Base Translate")]
	public byte BeginBaseTranslate { get; set; }

	/// <summary>
	/// Gets the index of the first <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> relative to all curves of the parent
	/// <see cref="P:Syroot.NintenTools.NSW.Bfres.SkeletalAnim.BoneAnims" /> instances.
	/// </summary>
	[Browsable(true)]
	[Category("Bone")]
	[DisplayName("Begin Curve")]
	internal int BeginCurve { get; set; }

	/// <summary>
	/// Gets or sets <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> instances animating properties of objects stored in this section.
	/// </summary>
	[Browsable(false)]
	public IList<AnimCurve> Curves { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.BoneAnim.UserData" /> names.
	/// </summary>
	[Browsable(false)]
	public ResDict UserDataDict { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.BoneAnim.UserData" /> instances.
	/// </summary>
	[Browsable(false)]
	public IList<UserData> UserData { get; set; }

	/// <summary>
	/// Gets or sets initial transformation values. Only stores specific transformations according to
	/// <see cref="P:Syroot.NintenTools.NSW.Bfres.BoneAnim.FlagsBase" />.
	/// </summary>
	[Browsable(false)]
	public BoneAnimData BaseData { get; set; }

	[Browsable(true)]
	[Category("Base Data")]
	[DisplayName("Use Translation")]
	public bool UseTranslation
	{
		get
		{
			return FlagsBase.HasFlag(BoneAnimFlagsBase.Translate);
		}
		set
		{
			if (value)
			{
				FlagsBase |= BoneAnimFlagsBase.Translate;
			}
			else
			{
				FlagsBase &= ~BoneAnimFlagsBase.Translate;
			}
		}
	}

	[Browsable(true)]
	[Category("Base Data")]
	[DisplayName("Use Scale")]
	public bool UseScale
	{
		get
		{
			return FlagsBase.HasFlag(BoneAnimFlagsBase.Scale);
		}
		set
		{
			if (value)
			{
				FlagsBase |= BoneAnimFlagsBase.Scale;
			}
			else
			{
				FlagsBase &= ~BoneAnimFlagsBase.Scale;
			}
		}
	}

	[Browsable(true)]
	[Category("Base Data")]
	[DisplayName("Use Rotation")]
	public bool UseRotation
	{
		get
		{
			return FlagsBase.HasFlag(BoneAnimFlagsBase.Rotate);
		}
		set
		{
			if (value)
			{
				FlagsBase |= BoneAnimFlagsBase.Rotate;
			}
			else
			{
				FlagsBase &= ~BoneAnimFlagsBase.Rotate;
			}
		}
	}

	[Browsable(true)]
	[Category("Transform Modes")]
	[DisplayName("Identity")]
	public bool ApplyIdentity
	{
		get
		{
			return FlagsTransform.HasFlag(BoneAnimFlagsTransform.Identity);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneAnimFlagsTransform.Identity;
			}
			else
			{
				FlagsTransform &= ~BoneAnimFlagsTransform.Identity;
			}
		}
	}

	[Browsable(true)]
	[Category("Transform Modes")]
	[DisplayName("Rotate Translate Zero")]
	public bool ApplyRotateTranslateZero
	{
		get
		{
			return FlagsTransform.HasFlag(BoneAnimFlagsTransform.RotateTranslateZero);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneAnimFlagsTransform.RotateTranslateZero;
			}
			else
			{
				FlagsTransform &= ~BoneAnimFlagsTransform.RotateTranslateZero;
			}
		}
	}

	[Browsable(true)]
	[Category("Transform Modes")]
	[DisplayName("Rotate Zero")]
	public bool ApplyRotateZero
	{
		get
		{
			return FlagsTransform.HasFlag(BoneAnimFlagsTransform.RotateZero);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneAnimFlagsTransform.RotateZero;
			}
			else
			{
				FlagsTransform &= ~BoneAnimFlagsTransform.RotateZero;
			}
		}
	}

	[Browsable(true)]
	[Category("Transform Modes")]
	[DisplayName("Scale One")]
	public bool ApplyScaleOne
	{
		get
		{
			return FlagsTransform.HasFlag(BoneAnimFlagsTransform.ScaleOne);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneAnimFlagsTransform.ScaleOne;
			}
			else
			{
				FlagsTransform &= ~BoneAnimFlagsTransform.ScaleOne;
			}
		}
	}

	[Browsable(true)]
	[Category("Transform Modes")]
	[DisplayName("Scale Volume One")]
	public bool ApplyScaleVolumeOne
	{
		get
		{
			return FlagsTransform.HasFlag(BoneAnimFlagsTransform.ScaleVolumeOne);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneAnimFlagsTransform.ScaleVolumeOne;
			}
			else
			{
				FlagsTransform &= ~BoneAnimFlagsTransform.ScaleVolumeOne;
			}
		}
	}

	[Browsable(true)]
	[Category("Transform Modes")]
	[DisplayName("Scale Uniform")]
	public bool ApplyScaleUniform
	{
		get
		{
			return FlagsTransform.HasFlag(BoneAnimFlagsTransform.ScaleUniform);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneAnimFlagsTransform.ScaleUniform;
			}
			else
			{
				FlagsTransform &= ~BoneAnimFlagsTransform.ScaleUniform;
			}
		}
	}

	[Browsable(true)]
	[Category("Transform Modes")]
	[DisplayName("Segment Scale Compensate")]
	public bool ApplySegmentScaleCompensate
	{
		get
		{
			return FlagsTransform.HasFlag(BoneAnimFlagsTransform.SegmentScaleCompensate);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneAnimFlagsTransform.SegmentScaleCompensate;
			}
			else
			{
				FlagsTransform &= ~BoneAnimFlagsTransform.SegmentScaleCompensate;
			}
		}
	}

	[Browsable(true)]
	[Category("Transform Modes")]
	[DisplayName("Translate Zero")]
	public bool ApplyTranslateZero
	{
		get
		{
			return FlagsTransform.HasFlag(BoneAnimFlagsTransform.TranslateZero);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneAnimFlagsTransform.TranslateZero;
			}
			else
			{
				FlagsTransform &= ~BoneAnimFlagsTransform.TranslateZero;
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.BoneAnim" /> class.
	/// </summary>
	public BoneAnim()
	{
		Name = "";
		_flags = 0u;
		FlagsBase = (BoneAnimFlagsBase)0u;
		BeginRotate = 0;
		BeginTranslate = 0;
		BeginBaseTranslate = 0;
		Curves = new List<AnimCurve>();
		BaseData = default;
		BeginCurve = 0;
	}

	void IResData.Load(ResFileLoader loader)
	{
		Name = loader.LoadString();
		long value = loader.ReadOffset(Relocated: true);
		long value2 = loader.ReadOffset(Relocated: true);
		long value3 = 0L;
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			value3 = loader.ReadInt64();
			UserDataDict = loader.LoadDict();
		}
		_flags = loader.ReadUInt32();
		BeginRotate = loader.ReadByte();
		BeginTranslate = loader.ReadByte();
		byte count = loader.ReadByte();
		BeginBaseTranslate = loader.ReadByte();
		BeginCurve = loader.ReadInt32();
		ushort count2 = loader.ReadUInt16();
		loader.ReadInt16();
		BaseData = loader.LoadCustom(() => new BoneAnimData(loader, FlagsBase), value2);
		Curves = loader.LoadList<AnimCurve>(count, value);
		UserData = loader.LoadList<UserData>(count2, value3);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.SaveString(Name);
		PosCurvesOffset = saver.SaveOffset();
		PosBaseDataOffset = saver.SaveOffset();
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Seek(16L);
		}
		saver.Write(_flags);
		saver.Write(BeginRotate);
		saver.Write(BeginTranslate);
		saver.Write((byte)Curves.Count);
		saver.Write(BeginBaseTranslate);
		saver.Write(BeginCurve);
		saver.Write(0);
	}
}
