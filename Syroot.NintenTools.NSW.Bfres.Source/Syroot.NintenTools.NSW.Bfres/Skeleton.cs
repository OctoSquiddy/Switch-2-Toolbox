using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Syroot.Maths;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an FSKL section in a <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> subfile, storing armature data.
/// </summary>
public class Skeleton : IResData
{
	private const string _signature = "FSKL";

	private const uint _flagsScalingMask = 768u;

	private const uint _flagsRotationMask = 28672u;

	private uint _flags;

	public ushort[] userIndices;

	internal long PosBoneDictOffset;

	internal long PosBoneArrayOffset;

	internal long PosMatrixToBoneListOffset;

	internal long PosInverseModelMatricesOffset;

	internal long PosUserPointer;

	[Browsable(true)]
	[Category("Skeleton")]
	[DisplayName("Scaling")]
	public SkeletonFlagsScaling FlagsScaling
	{
		get
		{
			return (SkeletonFlagsScaling)(_flags & 0x300);
		}
		set
		{
			_flags = (_flags & 0xFFFFFCFFu) | (uint)value;
		}
	}

	/// <summary>
	/// Gets or sets the rotation method used to store bone rotations.
	/// </summary>
	[Browsable(true)]
	[Category("Skeleton")]
	[DisplayName("Rotation")]
	public SkeletonFlagsRotation FlagsRotation
	{
		get
		{
			return (SkeletonFlagsRotation)(_flags & 0x7000);
		}
		set
		{
			_flags = (_flags & 0xFFFF8FFFu) | (uint)value;
		}
	}

	/// <summary>
	/// Gets or sets the list of <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" /> names.
	/// </summary>
	[Browsable(false)]
	public ResDict BoneDict { get; set; }

	/// <summary>
	/// Gets or sets the list of <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" /> instances forming the skeleton.
	/// </summary>
	[Browsable(false)]
	public IList<Bone> Bones { get; set; }

	[Browsable(false)]
	public IList<ushort> MatrixToBoneList { get; set; }

	[Browsable(false)]
	public IList<Matrix3x4> InverseModelMatrices { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Skeleton" /> class.
	/// </summary>
	public Skeleton()
	{
		MatrixToBoneList = new List<ushort>();
		InverseModelMatrices = new List<Matrix3x4>();
		Bones = new List<Bone>();
		BoneDict = new ResDict();
		FlagsRotation = SkeletonFlagsRotation.EulerXYZ;
		FlagsScaling = SkeletonFlagsScaling.Maya;
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
		resFileLoader.ImportSkeleton();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class from the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to load the data from.</param>
	public void Import(string fileName)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, fileName);
		resFileLoader.ImportSkeleton();
	}

	/// <summary>
	/// Saves the contents in the given <paramref name="stream" /> and optionally leaves it open
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to save the contents into.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
	public void Export(Stream stream, ResFile resFile, bool leaveOpen = false)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, stream, leaveOpen);
		resFileSaver.ExportSkeleton();
	}

	/// <summary>
	/// Saves the contents in the file with the given <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to save the contents into.</param>
	public void Export(string fileName, ResFile resFile)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, fileName);
		resFileSaver.ExportSkeleton();
	}

	public IList<ushort> GetSmoothIndices()
	{
		List<ushort> list = new List<ushort>();
		foreach (Bone bone in Bones)
		{
			if (bone.SmoothMatrixIndex != -1)
			{
				list.Add((ushort)bone.SmoothMatrixIndex);
			}
		}
		return list;
	}

	public IList<ushort> GetRigidIndices()
	{
		List<ushort> list = new List<ushort>();
		foreach (Bone bone in Bones)
		{
			if (bone.RigidMatrixIndex != -1)
			{
				list.Add((ushort)bone.RigidMatrixIndex);
			}
		}
		return list;
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FSKL");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			_flags = loader.ReadUInt32();
		}
		else
		{
			loader.LoadHeaderBlock();
		}
		BoneDict = loader.LoadDict();
		long value = loader.ReadOffset();
		long value2 = loader.ReadOffset();
		long value3 = loader.ReadOffset();
		if (loader.ResFile.VersionMajor2 == 8)
		{
			loader.Seek(16L);
		}
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			loader.Seek(8L);
		}
		long value4 = loader.ReadOffset();
		if (loader.ResFile.VersionMajor2 < 9)
		{
			_flags = loader.ReadUInt32();
		}
		ushort numBone = loader.ReadUInt16();
		ushort numSmoothMatrix = loader.ReadUInt16();
		ushort numRigidMatrix = loader.ReadUInt16();
		loader.Seek(6L);
		userIndices = loader.LoadCustom(() => loader.ReadUInt16s(numBone), value4);
		MatrixToBoneList = loader.LoadCustom(() => loader.ReadUInt16s(numSmoothMatrix + numRigidMatrix), value2);
		InverseModelMatrices = loader.LoadCustom(() => loader.ReadMatrix3x4s(numSmoothMatrix), value3);
		Bones = loader.LoadList<Bone>(numBone, value);
	}

	internal void WriteMatrices(ResFileSaver saver)
	{
		saver.Write(InverseModelMatrices);
	}

	void IResData.Save(ResFileSaver saver)
	{
		if (InverseModelMatrices == null)
		{
			InverseModelMatrices = new List<Matrix3x4>();
		}
		if (MatrixToBoneList == null)
		{
			MatrixToBoneList = new List<ushort>();
		}
		saver.WriteSignature("FSKL");
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(_flags);
		}
		else
		{
			saver.SaveHeaderBlock();
		}
		saver.SaveRelocateEntryToSection(saver.Position, 4u, 1u, 0u, 1, "FSKL");
		PosBoneDictOffset = saver.SaveOffset();
		PosBoneArrayOffset = saver.SaveOffset();
		PosMatrixToBoneListOffset = saver.SaveOffset();
		PosInverseModelMatricesOffset = saver.SaveOffset();
		if (saver.ResFile.VersionMajor2 == 8)
		{
			saver.Seek(16L);
		}
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Seek(8L);
		}
		saver.SaveRelocateEntryToSection(saver.Position, 1u, 1u, 0u, 1, "FSKL UserPointer");
		PosUserPointer = saver.SaveOffset();
		if (saver.ResFile.VersionMajor2 < 9)
		{
			saver.Write(_flags);
		}
		saver.Write((ushort)BoneDict.Count);
		saver.Write((ushort)InverseModelMatrices.Count);
		saver.Write((ushort)(MatrixToBoneList.Count - InverseModelMatrices.Count));
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Seek(2L);
		}
		else
		{
			saver.Seek(6L);
		}
	}
}
