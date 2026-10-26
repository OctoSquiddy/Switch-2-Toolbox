using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an FCAM section in a <see cref="T:Syroot.NintenTools.NSW.Bfres.SceneAnim" /> subfile, storing animations controlling fog settings.
/// </summary>
[DebuggerDisplay("FogAnim {Name}")]
public class FogAnim : IResData
{
	private const string _signature = "FFOG";

	internal long PosCurveArrayOffset;

	internal long PosBaseDataOffset;

	internal long PosUserDataOffset;

	internal long PosUserDataDictOffset;

	/// <summary>
	/// Gets or sets flags controlling how animation data is stored or how the animation should be played.
	/// </summary>
	public FogAnimFlags Flags { get; set; }

	/// <summary>
	/// Gets or sets the total number of frames this animation plays.
	/// </summary>
	public int FrameCount { get; set; }

	/// <summary>
	/// Gets or sets the index of the distance attenuation function to use.
	/// </summary>
	public sbyte DistanceAttnFuncIndex { get; set; }

	/// <summary>
	/// Gets or sets the number of bytes required to bake all <see cref="P:Syroot.NintenTools.NSW.Bfres.FogAnim.Curves" />.
	/// </summary>
	public uint BakedSize { get; set; }

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;FogAnim&gt;" />
	/// instances.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the name of the distance attenuation function to use.
	/// </summary>
	public string DistanceAttnFuncName { get; set; }

	/// <summary>
	/// Gets or sets <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> instances animating properties of objects stored in this section.
	/// </summary>
	public IList<AnimCurve> Curves { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.FogAnimData" /> instance storing initial fog parameters.
	/// </summary>
	public FogAnimData BaseData { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.FogAnim.UserData" /> instances.
	/// </summary>
	public IList<UserData> UserData { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.FogAnim.UserData" /> instances.
	/// </summary>
	public ResDict UserDataDict { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.FogAnim" /> class.
	/// </summary>
	public FogAnim()
	{
		Name = "";
		DistanceAttnFuncName = "";
		Flags = (FogAnimFlags)0;
		FrameCount = 1;
		BakedSize = 0u;
		DistanceAttnFuncIndex = 0;
		BaseData = default;
		Curves = new List<AnimCurve>();
		UserData = new List<UserData>();
		UserDataDict = new ResDict();
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FFOG");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			Flags = loader.ReadEnum<FogAnimFlags>(strict: true);
			loader.Seek(2L);
		}
		else
		{
			loader.LoadHeaderBlock();
		}
		Name = loader.LoadString();
		long num = loader.ReadInt64();
		BaseData = loader.LoadCustom(() => new FogAnimData(loader));
		long value = loader.ReadInt64();
		UserDataDict = loader.LoadDict();
		DistanceAttnFuncName = loader.LoadString();
		ushort num2 = 0;
		byte b = 0;
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			FrameCount = loader.ReadInt32();
			b = loader.ReadByte();
			DistanceAttnFuncIndex = loader.ReadSByte();
			num2 = loader.ReadUInt16();
			BakedSize = loader.ReadUInt32();
			loader.Seek(4L);
		}
		else
		{
			Flags = loader.ReadEnum<FogAnimFlags>(strict: true);
			loader.Seek(2L);
			FrameCount = loader.ReadInt32();
			b = loader.ReadByte();
			DistanceAttnFuncIndex = loader.ReadSByte();
			num2 = loader.ReadUInt16();
			BakedSize = loader.ReadUInt32();
		}
		UserData = loader.LoadList<UserData>(num2, value);
		Curves = loader.LoadList<AnimCurve>(b);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.WriteSignature("FFOG");
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(Flags, strict: true);
			saver.Seek(2L);
		}
		else
		{
			saver.Seek(12L);
		}
		saver.SaveRelocateEntryToSection(saver.Position, 6u, 1u, 0u, 1, "Fog Animation");
		saver.SaveString(Name);
		PosCurveArrayOffset = saver.SaveOffset();
		PosBaseDataOffset = saver.SaveOffset();
		PosUserDataOffset = saver.SaveOffset();
		PosUserDataDictOffset = saver.SaveOffset();
		saver.SaveString(DistanceAttnFuncName);
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(FrameCount);
			saver.Write((byte)Curves.Count);
			saver.Write(DistanceAttnFuncIndex);
			saver.Write((ushort)UserData.Count);
			saver.Write(BakedSize);
			saver.Seek(4L);
		}
		else
		{
			saver.Write(Flags, strict: true);
			saver.Seek(2L);
			saver.Write(FrameCount);
			saver.Write((byte)Curves.Count);
			saver.Write(DistanceAttnFuncIndex);
			saver.Write((ushort)UserData.Count);
			saver.Write(BakedSize);
		}
	}
}
