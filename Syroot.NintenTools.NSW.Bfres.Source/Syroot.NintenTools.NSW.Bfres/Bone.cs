using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Syroot.Maths;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a single bone in a <see cref="T:Syroot.NintenTools.NSW.Bfres.Skeleton" /> section, storing its initial transform and transformation
/// effects.
/// </summary>
[DebuggerDisplay("Bone {Name}")]
public class Bone : IResData
{
	private const uint _flagsMask = 1u;

	private const uint _flagsMaskRotate = 28672u;

	private const uint _flagsMaskBillboard = 458752u;

	private const uint _flagsMaskTransform = 251658240u;

	private const uint _flagsMaskTransformCumulative = 4026531840u;

	private uint _flags;

	public long SRTPos;

	internal long PosUserDataOffset;

	internal long PosUserDataDictOffset;

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;Bone&gt;" />
	/// instances.
	/// </summary>
	[Category("Name")]
	[Browsable(true)]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the index of the parent <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" /> this instance is a child of.
	/// </summary>
	[Category("Tree")]
	[Browsable(true)]
	public short ParentIndex { get; set; }

	/// <summary>
	/// Gets or sets the index of a matrix used for smooth skinning.
	/// </summary>
	[Category("Matrices")]
	[Browsable(true)]
	public short SmoothMatrixIndex { get; set; }

	/// <summary>
	/// Gets or sets the index of a matrix used for rigid skinning.
	/// </summary>
	[Category("Matrices")]
	[Browsable(true)]
	public short RigidMatrixIndex { get; set; }

	[Category("Billboard")]
	[DisplayName("Billboard ID")]
	[Browsable(true)]
	public short BillboardIndex { get; set; }

	[Category("Flags")]
	[Browsable(true)]
	public bool Visible
	{
		get
		{
			return Flags.HasFlag(BoneFlags.Visible);
		}
		set
		{
			if (value)
			{
				Flags |= BoneFlags.Visible;
			}
			else
			{
				Flags &= BoneFlags.Visible;
			}
		}
	}

	/// <summary>
	/// Gets or sets flags controlling bone behavior.
	/// </summary>
	[Browsable(false)]
	public BoneFlags Flags
	{
		get
		{
			return (BoneFlags)(_flags & 1);
		}
		set
		{
			_flags = (_flags & 0xFFFFFFFEu) | (uint)value;
		}
	}

	/// <summary>
	/// Gets or sets the rotation method used to store bone rotations in <see cref="P:Syroot.NintenTools.NSW.Bfres.Bone.Rotation" />.
	/// </summary>
	[Category("Transform Mode")]
	[DisplayName("Rotation Mode")]
	[Browsable(true)]
	public BoneFlagsRotation FlagsRotation
	{
		get
		{
			return (BoneFlagsRotation)(_flags & 0x7000);
		}
		set
		{
			_flags = (_flags & 0xFFFF8FFFu) | (uint)value;
		}
	}

	/// <summary>
	/// Gets or sets the billboard transformation applied to the bone.
	/// </summary>
	[Category("Billboard")]
	[DisplayName("Billboard Type")]
	[Browsable(true)]
	public BoneFlagsBillboard FlagsBillboard
	{
		get
		{
			return (BoneFlagsBillboard)(_flags & 0x70000);
		}
		set
		{
			_flags = (_flags & 0xFFF8FFFFu) | (uint)value;
		}
	}

	[Category("Transform Mode")]
	[Browsable(true)]
	public BoneFlagsTransform FlagsTransform
	{
		get
		{
			return (BoneFlagsTransform)(_flags & 0xF000000);
		}
		set
		{
			_flags = (_flags & 0xF0FFFFFFu) | (uint)value;
		}
	}

	[Category("Transform Mode")]
	[Browsable(true)]
	public BoneFlagsTransformCumulative FlagsTransformCumulative
	{
		get
		{
			return (BoneFlagsTransformCumulative)(_flags & 0xF0000000u);
		}
		set
		{
			_flags = (_flags & 0xFFFFFFF) | (uint)value;
		}
	}

	/// <summary>
	/// Gets or sets the spatial scale of the bone.
	/// </summary>
	[Category("Transformation")]
	[Browsable(true)]
	public Vector3F Scale { get; set; }

	/// <summary>
	/// Gets or sets the spatial rotation of the bone. If <see cref="F:Syroot.NintenTools.NSW.Bfres.BoneFlagsRotation.EulerXYZ" /> is used, the
	/// fourth component is always <c>1.0f</c>.
	/// </summary>
	[Category("Transformation")]
	[Browsable(true)]
	public Vector4F Rotation { get; set; }

	/// <summary>
	/// Gets or sets the spatial position of the bone.
	/// </summary>
	[Category("Transformation")]
	[Browsable(true)]
	public Vector3F Position { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.Bone.UserData" /> names.
	/// </summary>
	[Browsable(false)]
	public ResDict UserDataDict { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.Bone.UserData" /> instances.
	/// </summary>
	[Browsable(true)]
	public List<UserData> UserData { get; set; }

	[Category("Transform Mode")]
	[Browsable(false)]
	public bool TransformIdentity
	{
		get
		{
			return FlagsTransform.HasFlag(BoneFlagsTransform.Identity);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneFlagsTransform.Identity;
			}
			else
			{
				FlagsTransform &= ~BoneFlagsTransform.Identity;
			}
		}
	}

	[Category("Transform Mode")]
	[Browsable(false)]
	public bool TransformRotateTranslateZero
	{
		get
		{
			return FlagsTransform.HasFlag(BoneFlagsTransform.RotateTranslateZero);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneFlagsTransform.RotateTranslateZero;
			}
			else
			{
				FlagsTransform &= ~BoneFlagsTransform.RotateTranslateZero;
			}
		}
	}

	[Category("Transform Mode")]
	[Browsable(false)]
	public bool TransformRotateZero
	{
		get
		{
			return FlagsTransform.HasFlag(BoneFlagsTransform.RotateZero);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneFlagsTransform.RotateZero;
			}
			else
			{
				FlagsTransform &= ~BoneFlagsTransform.RotateZero;
			}
		}
	}

	[Category("Transform Mode")]
	[Browsable(false)]
	public bool TransformScaleOne
	{
		get
		{
			return FlagsTransform.HasFlag(BoneFlagsTransform.ScaleOne);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneFlagsTransform.ScaleOne;
			}
			else
			{
				FlagsTransform &= ~BoneFlagsTransform.ScaleOne;
			}
		}
	}

	[Category("Transform Mode")]
	[Browsable(false)]
	public bool TransformScaleUniform
	{
		get
		{
			return FlagsTransform.HasFlag(BoneFlagsTransform.ScaleUniform);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneFlagsTransform.ScaleUniform;
			}
			else
			{
				FlagsTransform &= ~BoneFlagsTransform.ScaleUniform;
			}
		}
	}

	[Category("Transform Mode")]
	[Browsable(false)]
	public bool TransformScaleVolumeOne
	{
		get
		{
			return FlagsTransform.HasFlag(BoneFlagsTransform.ScaleVolumeOne);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneFlagsTransform.ScaleVolumeOne;
			}
			else
			{
				FlagsTransform &= ~BoneFlagsTransform.ScaleVolumeOne;
			}
		}
	}

	[Category("Transform Mode")]
	[Browsable(false)]
	public bool TransformTranslateZero
	{
		get
		{
			return FlagsTransform.HasFlag(BoneFlagsTransform.TranslateZero);
		}
		set
		{
			if (value)
			{
				FlagsTransform |= BoneFlagsTransform.TranslateZero;
			}
			else
			{
				FlagsTransform &= ~BoneFlagsTransform.TranslateZero;
			}
		}
	}

	[Category("Transform Mode Cumulative")]
	[Browsable(false)]
	public bool TransformCumulativeIdentity
	{
		get
		{
			return FlagsTransformCumulative.HasFlag(BoneFlagsTransformCumulative.Identity);
		}
		set
		{
			if (value)
			{
				FlagsTransformCumulative |= BoneFlagsTransformCumulative.Identity;
			}
			else
			{
				FlagsTransformCumulative &= ~BoneFlagsTransformCumulative.Identity;
			}
		}
	}

	[Category("Transform Mode Cumulative")]
	[Browsable(false)]
	public bool TransformCumulativeRotateTranslateZero
	{
		get
		{
			return FlagsTransformCumulative.HasFlag(BoneFlagsTransformCumulative.RotateTranslateZero);
		}
		set
		{
			if (value)
			{
				FlagsTransformCumulative |= BoneFlagsTransformCumulative.RotateTranslateZero;
			}
			else
			{
				FlagsTransformCumulative &= ~BoneFlagsTransformCumulative.RotateTranslateZero;
			}
		}
	}

	[Category("Transform Mode Cumulative")]
	[Browsable(false)]
	public bool TransformCumulativeRotateZero
	{
		get
		{
			return FlagsTransformCumulative.HasFlag(BoneFlagsTransformCumulative.RotateZero);
		}
		set
		{
			if (value)
			{
				FlagsTransformCumulative |= BoneFlagsTransformCumulative.RotateZero;
			}
			else
			{
				FlagsTransformCumulative &= ~BoneFlagsTransformCumulative.RotateZero;
			}
		}
	}

	[Category("Transform Mode Cumulative")]
	[Browsable(false)]
	public bool TransformCumulativeScaleOne
	{
		get
		{
			return FlagsTransformCumulative.HasFlag(BoneFlagsTransformCumulative.ScaleOne);
		}
		set
		{
			if (value)
			{
				FlagsTransformCumulative |= BoneFlagsTransformCumulative.ScaleOne;
			}
			else
			{
				FlagsTransformCumulative &= ~BoneFlagsTransformCumulative.ScaleOne;
			}
		}
	}

	[Category("Transform Mode Cumulative")]
	[Browsable(false)]
	public bool TransformCumulativeScaleUniform
	{
		get
		{
			return FlagsTransformCumulative.HasFlag(BoneFlagsTransformCumulative.ScaleUniform);
		}
		set
		{
			if (value)
			{
				FlagsTransformCumulative |= BoneFlagsTransformCumulative.ScaleUniform;
			}
			else
			{
				FlagsTransformCumulative &= ~BoneFlagsTransformCumulative.ScaleUniform;
			}
		}
	}

	[Category("Transform Mode Cumulative")]
	[Browsable(false)]
	public bool TransformCumulativeScaleVolumeOne
	{
		get
		{
			return FlagsTransformCumulative.HasFlag(BoneFlagsTransformCumulative.ScaleVolumeOne);
		}
		set
		{
			if (value)
			{
				FlagsTransformCumulative |= BoneFlagsTransformCumulative.ScaleVolumeOne;
			}
			else
			{
				FlagsTransformCumulative &= ~BoneFlagsTransformCumulative.ScaleVolumeOne;
			}
		}
	}

	[Category("Transform Mode Cumulative")]
	[Browsable(false)]
	public bool TransformCumulativeTranslateZero
	{
		get
		{
			return FlagsTransformCumulative.HasFlag(BoneFlagsTransformCumulative.TranslateZero);
		}
		set
		{
			if (value)
			{
				FlagsTransformCumulative |= BoneFlagsTransformCumulative.TranslateZero;
			}
			else
			{
				FlagsTransformCumulative &= ~BoneFlagsTransformCumulative.TranslateZero;
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" /> class.
	/// </summary>
	public Bone()
	{
		Name = "";
		UserData = new List<UserData>();
		UserDataDict = new ResDict();
		ParentIndex = -1;
		SmoothMatrixIndex = -1;
		RigidMatrixIndex = -1;
		BillboardIndex = -1;
		Scale = new Vector3F(1f, 1f, 1f);
		Rotation = new Vector4F(0f, 0f, 0f, 0f);
		Position = new Vector3F(0f, 0f, 0f);
		Flags = BoneFlags.Visible;
		FlagsRotation = BoneFlagsRotation.EulerXYZ;
		FlagsBillboard = BoneFlagsBillboard.None;
		FlagsTransform = BoneFlagsTransform.None;
		FlagsTransformCumulative = BoneFlagsTransformCumulative.None;
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
		resFileLoader.ImportBone();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class from the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to load the data from.</param>
	public void Import(string fileName)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, fileName);
		resFileLoader.ImportBone();
	}

	/// <summary>
	/// Saves the contents in the given <paramref name="stream" /> and optionally leaves it open
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to save the contents into.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
	public void Export(Stream stream, ResFile resFile, bool leaveOpen = false)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, stream, leaveOpen);
		resFileSaver.ExportBone();
	}

	/// <summary>
	/// Saves the contents in the file with the given <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to save the contents into.</param>
	public void Export(string fileName, ResFile resFile)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, fileName);
		resFileSaver.ExportBone();
	}

	void IResData.Load(ResFileLoader loader)
	{
		Name = loader.LoadString();
		long value = loader.ReadOffset(Relocated: true);
		UserDataDict = loader.LoadDict();
		if (loader.ResFile.VersionMajor2 >= 10)
		{
			loader.Seek(8L);
		}
		else if (loader.ResFile.VersionMajor2 == 8 || loader.ResFile.VersionMajor2 == 9)
		{
			loader.Seek(16L);
		}
		ushort num = loader.ReadUInt16();
		ParentIndex = loader.ReadInt16();
		SmoothMatrixIndex = loader.ReadInt16();
		RigidMatrixIndex = loader.ReadInt16();
		BillboardIndex = loader.ReadInt16();
		ushort count = loader.ReadUInt16();
		_flags = loader.ReadUInt32();
		SRTPos = loader.Position;
		Scale = loader.ReadVector3F();
		Rotation = loader.ReadVector4F();
		Position = loader.ReadVector3F();
		UserData = loader.LoadList<UserData>(count, value)?.ToList();
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.SaveString(Name);
		PosUserDataOffset = saver.SaveOffset();
		PosUserDataDictOffset = saver.SaveOffset();
		if (saver.ResFile.VersionMajor2 >= 10)
		{
			saver.Seek(8L);
		}
		else if (saver.ResFile.VersionMajor2 == 8 || saver.ResFile.VersionMajor2 == 9)
		{
			saver.Seek(16L);
		}
		saver.Write((ushort)saver.CurrentIndex);
		saver.Write(ParentIndex);
		saver.Write(SmoothMatrixIndex);
		saver.Write(RigidMatrixIndex);
		saver.Write(BillboardIndex);
		saver.Write((ushort)UserData.Count);
		saver.Write(_flags);
		saver.Write(Scale);
		saver.Write(Rotation);
		saver.Write(Position);
	}
}
