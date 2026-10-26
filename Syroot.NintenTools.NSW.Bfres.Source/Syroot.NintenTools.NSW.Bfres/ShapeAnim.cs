using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an FSHA subfile in a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" />, storing shape animations of a <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" />
/// instance.
/// </summary>
[DebuggerDisplay("ShapeAnim {Name}")]
public class ShapeAnim : IResData
{
	private const string _signature = "FSHA";

	internal long PosBindModelOffset;

	internal long PosBindIndicesOffset;

	internal long PosVertexShapeAnimsOffset;

	internal long PosUserDataOffset;

	internal long PosUserDataDictOffset;

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;ShapeAnim&gt;" />
	/// instances.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the path of the file which originally supplied the data of this instance.
	/// </summary>
	public string Path { get; set; }

	/// <summary>
	/// Gets or sets flags controlling how animation data is stored or how the animation should be played.
	/// </summary>
	public ShapeAnimFlags Flags { get; set; }

	/// <summary>
	/// Gets or sets the total number of frames this animation plays.
	/// </summary>
	public int FrameCount { get; set; }

	/// <summary>
	/// Gets or sets the number of bytes required to bake all <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> instances of all
	/// <see cref="P:Syroot.NintenTools.NSW.Bfres.ShapeAnim.VertexShapeAnims" />.
	/// </summary>
	public uint BakedSize { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> instance affected by this animation.
	/// </summary>
	public Model BindModel { get; set; }

	/// <summary>
	/// Gets or sets the indices of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" /> instances in the <see cref="P:Syroot.NintenTools.NSW.Bfres.Model.Shapes" /> dictionary
	/// to bind for each animation. <see cref="F:System.UInt16.MaxValue" /> specifies no binding.
	/// </summary>
	public ushort[] BindIndices { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexShapeAnim" /> instances creating the animation.
	/// </summary>
	public IList<VertexShapeAnim> VertexShapeAnims { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.ShapeAnim.UserData" /> instances.
	/// </summary>
	public ResDict UserDataDict { get; set; }

	public IList<UserData> UserData { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ShapeAnim" /> class.
	/// </summary>
	public ShapeAnim()
	{
		Name = "";
		Path = "";
		Flags = (ShapeAnimFlags)0;
		BindModel = new Model();
		BindIndices = new ushort[0];
		VertexShapeAnims = new List<VertexShapeAnim>();
		FrameCount = 1;
		BakedSize = 0u;
		BindIndices = new ushort[0];
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
		resFileLoader.ImportShapeAnimations();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class from the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to load the data from.</param>
	public void Import(string fileName)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, fileName);
		resFileLoader.ImportShapeAnimations();
	}

	/// <summary>
	/// Saves the contents in the given <paramref name="stream" /> and optionally leaves it open
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to save the contents into.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
	public void Export(Stream stream, ResFile resFile, bool leaveOpen = false)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, stream, leaveOpen);
		resFileSaver.ExportShapeAnimation();
	}

	/// <summary>
	/// Saves the contents in the file with the given <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to save the contents into.</param>
	public void Export(string fileName, ResFile resFile)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, fileName);
		resFileSaver.ExportShapeAnimation();
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FSHA");
		if (loader.ResFile.VersionMajor2 == 9)
		{
			Flags = loader.ReadEnum<ShapeAnimFlags>(strict: false);
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
		UserDataDict = loader.LoadDict();
		ushort num = 0;
		ushort numVertexShapeAnim = 0;
		ushort num2 = 0;
		if (loader.ResFile.VersionMajor2 == 9)
		{
			BakedSize = loader.ReadUInt32();
			FrameCount = loader.ReadInt32();
			num = loader.ReadUInt16();
			numVertexShapeAnim = loader.ReadUInt16();
			ushort num3 = loader.ReadUInt16();
			num2 = loader.ReadUInt16();
		}
		else
		{
			Flags = loader.ReadEnum<ShapeAnimFlags>(strict: false);
			num = loader.ReadUInt16();
			numVertexShapeAnim = loader.ReadUInt16();
			ushort num4 = loader.ReadUInt16();
			FrameCount = loader.ReadInt32();
			BakedSize = loader.ReadUInt32();
			ushort num5 = loader.ReadUInt16();
		}
		BindIndices = loader.LoadCustom(() => loader.ReadUInt16s(numVertexShapeAnim), value);
		VertexShapeAnims = loader.LoadList<VertexShapeAnim>(numVertexShapeAnim, value2);
		UserData = loader.LoadList<UserData>(num, value3);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.WriteSignature("FSHA");
		if (saver.ResFile.VersionMajor2 == 9)
		{
			saver.Write(Flags, strict: false);
			saver.Write((ushort)0);
		}
		else
		{
			saver.SaveHeaderBlock();
		}
		saver.SaveString(Name);
		saver.SaveString(Path);
		PosBindModelOffset = saver.SaveOffset();
		PosBindIndicesOffset = saver.SaveOffset();
		PosVertexShapeAnimsOffset = saver.SaveOffset();
		PosUserDataOffset = saver.SaveOffset();
		PosUserDataDictOffset = saver.SaveOffset();
		if (saver.ResFile.VersionMajor2 == 9)
		{
			saver.Write(BakedSize);
			saver.Write(FrameCount);
			saver.Write((ushort)UserData.Count);
			saver.Write((ushort)VertexShapeAnims.Count);
			saver.Write((ushort)VertexShapeAnims.Sum((VertexShapeAnim x) => x.KeyShapeAnimInfos.Count));
			saver.Write((ushort)VertexShapeAnims.Sum((VertexShapeAnim x) => x.Curves.Count));
			return;
		}
		saver.Write(Flags, strict: false);
		saver.Write((ushort)UserData.Count);
		saver.Write((ushort)VertexShapeAnims.Count);
		saver.Write((ushort)VertexShapeAnims.Sum((VertexShapeAnim x) => x.KeyShapeAnimInfos.Count));
		saver.Write(FrameCount);
		saver.Write(BakedSize);
		saver.Write((ushort)VertexShapeAnims.Sum((VertexShapeAnim x) => x.Curves.Count));
	}
}
