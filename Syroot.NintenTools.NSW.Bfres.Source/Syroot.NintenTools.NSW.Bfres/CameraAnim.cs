using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an FCAM section in a <see cref="T:Syroot.NintenTools.NSW.Bfres.SceneAnim" /> subfile, storing animations controlling camera settings.
/// </summary>
[DebuggerDisplay("CameraAnim {Name}")]
public class CameraAnim : IResData
{
	private const string _signature = "FCAM";

	internal long PosCurveArrayOffset;

	internal long PosBaseDataOffset;

	internal long PosUserDataOffset;

	internal long PosUserDataDictOffset;

	/// <summary>
	/// Gets or sets flags controlling how animation data is stored or how the animation should be played.
	/// </summary>
	public CameraAnimFlags Flags { get; set; }

	/// <summary>
	/// Gets or sets the total number of frames this animation plays.
	/// </summary>
	public int FrameCount { get; set; }

	/// <summary>
	/// Gets or sets the number of bytes required to bake all <see cref="P:Syroot.NintenTools.NSW.Bfres.CameraAnim.Curves" />.
	/// </summary>
	public uint BakedSize { get; set; }

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in
	/// <see cref="!:ResDict&lt;CameraAnim&gt;" /> instances.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> instances animating properties of objects stored in this section.
	/// </summary>
	public IList<AnimCurve> Curves { get; set; }

	/// <summary>
	/// Gets the <see cref="T:Syroot.NintenTools.NSW.Bfres.CameraAnimData" /> instance storing initial camera parameters.
	/// </summary>
	public CameraAnimData BaseData { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.CameraAnim.UserData" /> instances.
	/// </summary>
	public IList<UserData> UserData { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.CameraAnim.UserData" /> instances.
	/// </summary>
	public ResDict UserDataDict { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.CameraAnim" /> class.
	/// </summary>
	public CameraAnim()
	{
		Name = "";
		Flags = CameraAnimFlags.EulerZXY;
		FrameCount = 1;
		BakedSize = 0u;
		BaseData = default;
		Curves = new List<AnimCurve>();
		UserData = new List<UserData>();
		UserDataDict = new ResDict();
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FCAM");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			Flags = loader.ReadEnum<CameraAnimFlags>(strict: true);
			loader.Seek(2L);
		}
		else
		{
			loader.LoadHeaderBlock();
		}
		Name = loader.LoadString();
		long value = loader.ReadOffset();
		BaseData = loader.LoadCustom(() => new CameraAnimData(loader));
		long value2 = loader.ReadOffset();
		UserDataDict = loader.LoadDict();
		ushort num = 0;
		byte b = 0;
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			FrameCount = loader.ReadInt32();
			BakedSize = loader.ReadUInt32();
			num = loader.ReadUInt16();
			b = loader.ReadByte();
			loader.Seek(1L);
			loader.Seek(4L);
		}
		else
		{
			Flags = loader.ReadEnum<CameraAnimFlags>(strict: true);
			loader.Seek(2L);
			FrameCount = loader.ReadInt32();
			b = loader.ReadByte();
			loader.Seek(1L);
			num = loader.ReadUInt16();
			BakedSize = loader.ReadUInt32();
		}
		Curves = loader.LoadList<AnimCurve>(b, value);
		UserData = loader.LoadList<UserData>(num, value2);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.WriteSignature("FCAM");
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(Flags, strict: true);
			saver.Seek(2L);
		}
		else
		{
			saver.Seek(12L);
		}
		saver.SaveRelocateEntryToSection(saver.Position, 5u, 1u, 0u, 1, "Camera Animation");
		saver.SaveString(Name);
		PosCurveArrayOffset = saver.SaveOffset();
		PosBaseDataOffset = saver.SaveOffset();
		PosUserDataOffset = saver.SaveOffset();
		PosUserDataDictOffset = saver.SaveOffset();
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(FrameCount);
			saver.Write(BakedSize);
			saver.Write((ushort)UserData.Count);
			saver.Write((byte)Curves.Count);
			saver.Seek(1L);
			saver.Seek(4L);
		}
		else
		{
			saver.Write(Flags, strict: true);
			saver.Seek(2L);
			saver.Write(FrameCount);
			saver.Write((byte)Curves.Count);
			saver.Seek(1L);
			saver.Write((ushort)UserData.Count);
			saver.Write(BakedSize);
		}
	}
}
