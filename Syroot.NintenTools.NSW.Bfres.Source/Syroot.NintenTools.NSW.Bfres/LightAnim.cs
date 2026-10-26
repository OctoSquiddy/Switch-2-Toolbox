using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an FLIT section in a <see cref="T:Syroot.NintenTools.NSW.Bfres.SceneAnim" /> subfile, storing animations controlling light settings.
/// </summary>
[DebuggerDisplay("LightAnim {Name}")]
public class LightAnim : IResData
{
	private const string _signature = "FLIT";

	private const ushort _flagsMask = 261;

	private const ushort _flagsMaskFields = 65024;

	private ushort _flags;

	internal long PosCurveArrayOffset;

	internal long PosBaseDataOffset;

	internal long PosUserDataOffset;

	internal long PosUserDataDictOffset;

	/// <summary>
	/// Gets or sets flags controlling how the animation should be played.
	/// </summary>
	public LightAnimFlags Flags
	{
		get
		{
			return (LightAnimFlags)(_flags & 0x105);
		}
		set
		{
			_flags = (ushort)((uint)(_flags & -262) | (uint)value);
		}
	}

	/// <summary>
	/// Gets or sets flags controlling how animation data is stored or how the animation should be played.
	/// </summary>
	public LightAnimField AnimatedFields
	{
		get
		{
			return (LightAnimField)(_flags & 0xFE00);
		}
		set
		{
			_flags = (ushort)((uint)(_flags & -65025) | (uint)value);
		}
	}

	/// <summary>
	/// Gets or sets the total number of frames this animation plays.
	/// </summary>
	public int FrameCount { get; set; }

	/// <summary>
	/// Gets or sets the index of the light type.
	/// </summary>
	public sbyte LightTypeIndex { get; set; }

	/// <summary>
	/// Gets or sets the index of the distance attenuation function to use.
	/// </summary>
	public sbyte DistanceAttnFuncIndex { get; set; }

	/// <summary>
	/// Gets or sets the index of the angle attenuation function to use.
	/// </summary>
	public sbyte AngleAttnFuncIndex { get; set; }

	/// <summary>
	/// Gets or sets the number of bytes required to bake all <see cref="P:Syroot.NintenTools.NSW.Bfres.LightAnim.Curves" />.
	/// </summary>
	public uint BakedSize { get; set; }

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;LightAnim&gt;" />
	/// instances.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the name of the light type.
	/// </summary>
	public string LightTypeName { get; set; }

	/// <summary>
	/// Gets or sets the name of the distance attenuation function to use.
	/// </summary>
	public string DistanceAttnFuncName { get; set; }

	/// <summary>
	/// Gets or sets the name of the angle attenuation function to use.
	/// </summary>
	public string AngleAttnFuncName { get; set; }

	/// <summary>
	/// Gets or sets <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> instances animating properties of objects stored in this section.
	/// </summary>
	public IList<AnimCurve> Curves { get; set; }

	/// <summary>
	/// Gets the <see cref="T:Syroot.NintenTools.NSW.Bfres.LightAnimData" /> instance storing initial light parameters.
	/// </summary>
	public LightAnimData BaseData { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.LightAnim.UserData" /> instances.
	/// </summary>
	public IList<UserData> UserData { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.LightAnim.UserData" /> instances.
	/// </summary>
	public ResDict UserDataDict { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.LightAnim" /> class.
	/// </summary>
	public LightAnim()
	{
		Name = "";
		DistanceAttnFuncName = "";
		AngleAttnFuncName = "";
		Flags = (LightAnimFlags)0;
		LightTypeIndex = 0;
		AnimatedFields = (LightAnimField)0;
		FrameCount = 1;
		BakedSize = 0u;
		DistanceAttnFuncIndex = 0;
		AngleAttnFuncIndex = 0;
		BaseData = default;
		Curves = new List<AnimCurve>();
		UserData = new List<UserData>();
		UserDataDict = new ResDict();
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FLIT");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			Flags = loader.ReadEnum<LightAnimFlags>(strict: true);
			loader.Seek(2L);
		}
		else
		{
			loader.LoadHeaderBlock();
		}
		Name = loader.LoadString();
		long value = loader.ReadOffset();
		BaseData = loader.LoadCustom(() => new LightAnimData(loader, AnimatedFields));
		long value2 = loader.ReadOffset();
		UserDataDict = loader.LoadDict();
		LightTypeName = loader.LoadString();
		DistanceAttnFuncName = loader.LoadString();
		AngleAttnFuncName = loader.LoadString();
		ushort num = 0;
		byte b = 0;
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			FrameCount = loader.ReadInt32();
			b = loader.ReadByte();
			LightTypeIndex = loader.ReadSByte();
			DistanceAttnFuncIndex = loader.ReadSByte();
			AngleAttnFuncIndex = loader.ReadSByte();
			BakedSize = loader.ReadUInt32();
			num = loader.ReadUInt16();
			loader.Seek(2L);
		}
		else
		{
			Flags = loader.ReadEnum<LightAnimFlags>(strict: true);
			num = loader.ReadUInt16();
			FrameCount = loader.ReadInt32();
			b = loader.ReadByte();
			LightTypeIndex = loader.ReadSByte();
			DistanceAttnFuncIndex = loader.ReadSByte();
			AngleAttnFuncIndex = loader.ReadSByte();
			BakedSize = loader.ReadUInt32();
		}
		UserData = loader.LoadList<UserData>(num, value2);
		Curves = loader.LoadList<AnimCurve>(b, value);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.WriteSignature("FLIT");
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(Flags, strict: true);
			saver.Seek(2L);
		}
		else
		{
			saver.Seek(12L);
		}
		saver.SaveRelocateEntryToSection(saver.Position, 8u, 1u, 0u, 1, "Light Animation");
		saver.SaveString(Name);
		PosCurveArrayOffset = saver.SaveOffset();
		PosBaseDataOffset = saver.SaveOffset();
		PosUserDataOffset = saver.SaveOffset();
		PosUserDataDictOffset = saver.SaveOffset();
		saver.SaveString(LightTypeName);
		saver.SaveString(DistanceAttnFuncName);
		saver.SaveString(AngleAttnFuncName);
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(FrameCount);
			saver.Write((byte)Curves.Count);
			saver.Write(LightTypeIndex);
			saver.Write(DistanceAttnFuncIndex);
			saver.Write(AngleAttnFuncIndex);
			saver.Write(BakedSize);
			saver.Write((ushort)UserData.Count);
			saver.Seek(2L);
		}
		else
		{
			saver.Write(Flags, strict: true);
			saver.Write((ushort)UserData.Count);
			saver.Write(FrameCount);
			saver.Write((byte)Curves.Count);
			saver.Write(LightTypeIndex);
			saver.Write(DistanceAttnFuncIndex);
			saver.Write(AngleAttnFuncIndex);
			saver.Write(BakedSize);
		}
	}
}
