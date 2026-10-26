using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an FMDL subfile in a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" />, storing model vertex data, skeletons and used materials.
/// </summary>
[DebuggerDisplay("Model {Name}")]
public class Model : IResData
{
	internal class Ofs
	{
		internal long FMDLOffset = 0L;
	}

	public const string _signature = "FMDL";

	/// <summary>
	///
	/// </summary>
	public List<ShaderAssign> ShaderAssigns = new List<ShaderAssign>();

	public IList<MaterialParserV10.ShaderAssignV10> ShaderAssign = new List<MaterialParserV10.ShaderAssignV10>();

	private uint UnknownFlag;

	private ushort ShaderAssignCount = 1;

	internal long SkeletonOffset;

	internal long VertexBufferOffset;

	internal long ShapeOffset;

	internal long ShapeDictOffset;

	internal long MaterialsOffset;

	internal long MaterialsDictOffset;

	internal long PosUserDataOffset;

	internal long PosUserDataDictOffset;

	internal long ShaderAssignOffset;

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;Model&gt;" />
	/// instances.
	/// </summary>
	[Browsable(true)]
	[Category("Model")]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the path of the file which originally supplied the data of this instance.
	/// </summary>
	[Browsable(true)]
	[Category("Model")]
	public string Path { get; set; }

	/// <summary>
	/// Gets the <see cref="P:Syroot.NintenTools.NSW.Bfres.Model.Skeleton" /> instance to deform the model with animations.
	/// </summary>
	[Browsable(false)]
	public Skeleton Skeleton { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexBuffer" /> instances storing the vertex data used by the
	/// <see cref="P:Syroot.NintenTools.NSW.Bfres.Model.Shapes" />.
	/// </summary>
	[Browsable(false)]
	public IList<VertexBuffer> VertexBuffers { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" /> instances forming the surface of the model.
	/// </summary>
	[Browsable(false)]
	public ResDict ShapeDict { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" /> instances forming the surface of the model.
	/// </summary>
	[Browsable(false)]
	public IList<Shape> Shapes { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" /> names.
	/// </summary>
	[Browsable(false)]
	public ResDict MaterialDict { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" /> instance applied on the <see cref="P:Syroot.NintenTools.NSW.Bfres.Model.Shapes" /> to color their surface.
	/// </summary>
	[Browsable(false)]
	public IList<Material> Materials { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.Model.UserData" /> names.
	/// </summary>
	[Browsable(false)]
	public ResDict UserDataDict { get; set; }

	/// <summary>
	/// Gets or sets customly attached <see cref="P:Syroot.NintenTools.NSW.Bfres.Model.UserData" /> instances.
	/// </summary>
	[Browsable(false)]
	public IList<UserData> UserData { get; set; }

	[Browsable(true)]
	[ReadOnly(true)]
	[Category("Model")]
	[DisplayName("Vertex Buffer Count")]
	public int VertexBufferCount
	{
		get
		{
			if (VertexBuffers != null)
			{
				return VertexBuffers.Count;
			}
			return 0;
		}
	}

	[Browsable(true)]
	[ReadOnly(true)]
	[Category("Model")]
	[DisplayName("Shape Count")]
	public int ShapeCount
	{
		get
		{
			if (Shapes != null)
			{
				return Shapes.Count;
			}
			return 0;
		}
	}

	[Browsable(true)]
	[ReadOnly(true)]
	[Category("Model")]
	[DisplayName("Material Count")]
	public int MaterialCount
	{
		get
		{
			if (Materials != null)
			{
				return Materials.Count;
			}
			return 0;
		}
	}

	[Browsable(true)]
	[ReadOnly(true)]
	[Category("Model")]
	[DisplayName("User Data Count")]
	public int UserDataCount
	{
		get
		{
			if (UserData != null)
			{
				return UserData.Count;
			}
			return 0;
		}
	}

	/// <summary>
	/// Gets the total number of vertices to process when drawing this model.
	/// </summary>
	/// <remarks>This excludes vertices which are not processed by any shader. However, the exact value does not
	/// seem to matter, so the total count of all vertices is taken to keep things trivial for now.</remarks>
	[Browsable(true)]
	[ReadOnly(true)]
	[Category("Model")]
	[DisplayName("Total Vertex Count")]
	public uint TotalVertexCount
	{
		get
		{
			uint num = 0u;
			foreach (VertexBuffer vertexBuffer in VertexBuffers)
			{
				num += vertexBuffer.VertexCount;
			}
			return num;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> class.
	/// </summary>
	public Model()
	{
		Name = "";
		Path = "";
		Skeleton = new Skeleton();
		VertexBuffers = new List<VertexBuffer>();
		Shapes = new List<Shape>();
		Materials = new List<Material>();
		UserData = new List<UserData>();
		ShapeDict = new ResDict();
		MaterialDict = new ResDict();
		UserDataDict = new ResDict();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" /> class from the given <paramref name="stream" /> which
	/// is optionally left open.
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to load the data from.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after reading, otherwise <c>false</c>.</param>
	public void Import(Stream stream, ResFile resFile, bool leaveOpen = false)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, resFile, stream, leaveOpen);
		resFileLoader.ImportModel();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class from the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to load the data from.</param>
	public void Import(string fileName, ResFile resFile)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, resFile, fileName);
		resFileLoader.ImportModel();
	}

	/// <summary>
	/// Saves the contents in the given <paramref name="stream" /> and optionally leaves it open
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to save the contents into.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
	public void Export(Stream stream, ResFile resFile, bool leaveOpen = false)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, stream, leaveOpen);
		resFileSaver.ExportModel();
	}

	/// <summary>
	/// Saves the contents in the file with the given <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to save the contents into.</param>
	public void Export(string fileName, ResFile resFile)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, resFile, fileName);
		resFileSaver.ExportModel();
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FMDL");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			UnknownFlag = loader.ReadUInt32();
		}
		else
		{
			loader.LoadHeaderBlock();
		}
		Name = loader.LoadString();
		Path = loader.LoadString();
		Skeleton = loader.Load<Skeleton>();
		long value = loader.ReadOffset();
		long value2 = loader.ReadOffset();
		ShapeDict = loader.LoadDict();
		long value3 = loader.ReadOffset();
		if (loader.ResFile.VersionMajor2 == 9)
		{
			loader.ReadOffset();
		}
		MaterialDict = loader.LoadDict();
		if (loader.ResFile.VersionMajor2 >= 10)
		{
			ShaderAssignOffset = loader.ReadOffset();
		}
		long value4 = loader.ReadOffset();
		UserDataDict = loader.LoadDict();
		long num = loader.ReadInt64();
		ushort count = loader.ReadUInt16();
		ushort count2 = loader.ReadUInt16();
		ushort count3 = loader.ReadUInt16();
		ushort num2 = 0;
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			ShaderAssignCount = loader.ReadUInt16();
			num2 = loader.ReadUInt16();
			loader.ReadUInt16();
			uint num3 = loader.ReadUInt32();
		}
		else
		{
			num2 = loader.ReadUInt16();
			uint num4 = loader.ReadUInt32();
			uint num5 = loader.ReadUInt32();
		}
		Shapes = loader.LoadList<Shape>(count2, value2);
		ShaderAssign = loader.LoadList<MaterialParserV10.ShaderAssignV10>(ShaderAssignCount, ShaderAssignOffset);
		Materials = loader.LoadList<Material>(count3, value3);
		UserData = loader.LoadList<UserData>(num2, value4);
		VertexBuffers = loader.LoadList<VertexBuffer>(count, value);
	}

	void IResData.Save(ResFileSaver saver)
	{
		if (saver.ResFile.VersionMajor2 >= 10)
		{
			foreach (Material material in Materials)
			{
				MaterialParserV10.PrepareSave(material);
			}
			IEnumerable<MaterialParserV10.ShaderAssignV10> source = Materials.Select((Material x) => x.ShaderInfoV10.ShaderAssign).Distinct();
			ShaderAssign = (from x in source
				group x by x.GetHashCode() into g
				select g.First()).ToList();
			foreach (Material material2 in Materials)
			{
				foreach (MaterialParserV10.ShaderAssignV10 item in ShaderAssign)
				{
					if (item.GetHashCode() == material2.ShaderInfoV10.ShaderAssign.GetHashCode())
					{
						material2.ShaderInfoV10.ShaderAssign = item;
					}
				}
			}
		}
		saver.WriteSignature("FMDL");
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(UnknownFlag);
		}
		else
		{
			saver.SaveHeaderBlock();
		}
		if (saver.ResFile.VersionMajor2 == 9)
		{
			saver.SaveRelocateEntryToSection(saver.Position, 10u, 1u, 0u, 1, "Model");
		}
		else if (saver.ResFile.VersionMajor2 >= 10)
		{
			saver.SaveRelocateEntryToSection(saver.Position, 11u, 1u, 0u, 1, "Model");
		}
		else
		{
			saver.SaveRelocateEntryToSection(saver.Position, 10u, 1u, 0u, 1, "Model");
		}
		saver.SaveString(Name);
		saver.SaveString(Path);
		SkeletonOffset = saver.SaveOffset();
		VertexBufferOffset = saver.SaveOffset();
		ShapeOffset = saver.SaveOffset();
		ShapeDictOffset = saver.SaveOffset();
		MaterialsOffset = saver.SaveOffset();
		MaterialsDictOffset = saver.SaveOffset();
		if (saver.ResFile.VersionMajor2 >= 10)
		{
			saver.SaveList(ShaderAssign, 0L);
		}
		if (saver.ResFile.VersionMajor2 == 9)
		{
			saver.Write(0L);
		}
		PosUserDataOffset = saver.SaveOffset();
		PosUserDataDictOffset = saver.SaveOffset();
		saver.Write(0L);
		saver.Write((ushort)VertexBuffers.Count);
		saver.Write((ushort)Shapes.Count);
		saver.Write((ushort)Materials.Count);
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write((ushort)ShaderAssign.Count);
			saver.Write((ushort)UserData.Count);
			saver.Write((ushort)0);
			saver.Write(0);
		}
		else
		{
			saver.Write((ushort)UserData.Count);
			saver.Write(TotalVertexCount);
			saver.Write(0);
		}
	}
}
