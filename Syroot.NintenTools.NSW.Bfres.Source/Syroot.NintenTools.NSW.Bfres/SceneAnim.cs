using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an FSCN subfile in a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" />, storing scene animations controlling camera, light and
/// fog settings.
/// </summary>
[DebuggerDisplay("SceneAnim {Name}")]
public class SceneAnim : IResData
{
	private const string _signature = "FSCN";

	private uint UnknownFlags;

	internal long PosCameraAnimArrayOffset;

	internal long PosCameraAnimDictOffset;

	internal long PosLightAnimArrayOffset;

	internal long PosLightAnimDictOffset;

	internal long PosFogAnimArrayOffset;

	internal long PosFogAnimDictOffset;

	internal long PosUserDataOffset;

	internal long PosUserDataDictOffset;

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;SceneAnim&gt;" />
	/// instances.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the path of the file which originally supplied the data of this instance.
	/// </summary>
	public string Path { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.CameraAnim" /> instances.
	/// </summary>
	public IList<CameraAnim> CameraAnims { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.LightAnim" /> instances.
	/// </summary>
	public IList<LightAnim> LightAnims { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.FogAnim" /> instances.
	/// </summary>
	public IList<FogAnim> FogAnims { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.SceneAnim.UserData" /> instances.
	/// </summary>
	public IList<UserData> UserData { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.CameraAnim" /> instances.
	/// </summary>
	public ResDict CameraAnimDict { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.LightAnim" /> instances.
	/// </summary>
	public ResDict LightAnimDict { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.FogAnim" /> instances.
	/// </summary>
	public ResDict FogAnimDict { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.SceneAnim.UserData" /> instances.
	/// </summary>
	public ResDict UserDataDict { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.SceneAnim" /> class.
	/// </summary>
	public SceneAnim()
	{
		Name = "";
		Path = "";
		CameraAnims = new List<CameraAnim>();
		LightAnims = new List<LightAnim>();
		FogAnims = new List<FogAnim>();
		CameraAnimDict = new ResDict();
		LightAnimDict = new ResDict();
		FogAnimDict = new ResDict();
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
		resFileLoader.ImportSceneAnimations();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class from the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to load the data from.</param>
	public void Import(string fileName)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, fileName);
		resFileLoader.ImportSceneAnimations();
	}

	/// <summary>
	/// Saves the contents in the given <paramref name="stream" /> and optionally leaves it open
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to save the contents into.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
	public void Export(Stream stream, ResFile resFile, bool leaveOpen = false)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, stream, leaveOpen);
		resFileSaver.ExportSceneAnimation();
	}

	/// <summary>
	/// Saves the contents in the file with the given <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to save the contents into.</param>
	public void Export(string fileName, ResFile resFile)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, fileName);
		resFileSaver.ExportSceneAnimation();
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FSCN");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			UnknownFlags = loader.ReadUInt32();
		}
		else
		{
			loader.LoadHeaderBlock();
		}
		Name = loader.LoadString();
		Path = loader.LoadString();
		long value = loader.ReadOffset(Relocated: true);
		CameraAnimDict = loader.LoadDict();
		long value2 = loader.ReadOffset(Relocated: true);
		LightAnimDict = loader.LoadDict();
		long value3 = loader.ReadOffset(Relocated: true);
		FogAnimDict = loader.LoadDict();
		long value4 = loader.ReadOffset(Relocated: true);
		UserDataDict = loader.LoadDict();
		ushort count = loader.ReadUInt16();
		ushort count2 = loader.ReadUInt16();
		ushort count3 = loader.ReadUInt16();
		ushort count4 = loader.ReadUInt16();
		CameraAnims = loader.LoadList<CameraAnim>(count2, value);
		LightAnims = loader.LoadList<LightAnim>(count3, value2);
		FogAnims = loader.LoadList<FogAnim>(count4, value3);
		UserData = loader.LoadList<UserData>(count, value4);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.WriteSignature("FSCN");
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(UnknownFlags);
		}
		else
		{
			saver.SaveHeaderBlock();
		}
		saver.SaveString(Name);
		saver.SaveString(Path);
		PosCameraAnimArrayOffset = saver.SaveOffset();
		PosCameraAnimDictOffset = saver.SaveOffset();
		PosLightAnimArrayOffset = saver.SaveOffset();
		PosLightAnimDictOffset = saver.SaveOffset();
		PosFogAnimArrayOffset = saver.SaveOffset();
		PosFogAnimDictOffset = saver.SaveOffset();
		PosUserDataOffset = saver.SaveOffset();
		PosUserDataDictOffset = saver.SaveOffset();
		saver.Write((ushort)UserData.Count);
		saver.Write((ushort)CameraAnims.Count);
		saver.Write((ushort)LightAnims.Count);
		saver.Write((ushort)FogAnims.Count);
	}
}
