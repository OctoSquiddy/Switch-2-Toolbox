using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Syroot.Maths;
using Syroot.NintenTools.NSW.Bfres.Core;
using Syroot.NintenTools.NSW.Bfres.GFX;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents an FSHP section in a <see cref="T:Syroot.NintenTools.NSW.Bfres.Model" /> subfile.
/// </summary>
[DebuggerDisplay("Shape {Name}")]
public class Shape : IResData
{
	private const string _signature = "FSHP";

	[Browsable(false)]
	public long RadiusOffset;

	[Browsable(false)]
	public long BoundingBoxArrayOffset;

	public List<Vector4F> BoundingRadiusList = new List<Vector4F>();

	internal long PosMeshArrayOffset;

	internal long PosSkinBoneIndicesOffset;

	internal long PosKeyShapesOffset;

	internal long PosKeyShapeDictOffset;

	internal long PosSubMeshBoundingsOffset;

	internal long PosRadiusArrayOffset;

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;Shape&gt;" />
	/// instances.
	/// </summary>
	[Category("Polygon")]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets flags determining which data is available for this instance.
	/// </summary>
	[Browsable(false)]
	public ShapeFlags Flags { get; set; }

	[Category("Flags")]
	public bool HasVertexBuffer
	{
		get
		{
			return Flags.HasFlag(ShapeFlags.HasVertexBuffer);
		}
		set
		{
			if (value)
			{
				Flags |= ShapeFlags.HasVertexBuffer;
			}
			else
			{
				Flags &= ShapeFlags.HasVertexBuffer;
			}
		}
	}

	[Category("Flags")]
	public bool SubMeshBoundaryConsistent
	{
		get
		{
			return Flags.HasFlag(ShapeFlags.SubMeshBoundaryConsistent);
		}
		set
		{
			if (value)
			{
				Flags |= ShapeFlags.SubMeshBoundaryConsistent;
			}
			else
			{
				Flags &= ShapeFlags.SubMeshBoundaryConsistent;
			}
		}
	}

	/// <summary>
	/// Gets or sets the index of the material to apply to the shapes surface in the owning
	/// <see cref="P:Syroot.NintenTools.NSW.Bfres.Model.Materials" /> list.
	/// </summary>
	[Category("Polygon")]
	[DisplayName("Material Index")]
	public ushort MaterialIndex { get; set; }

	/// <summary>
	/// Gets or sets the index of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" /> to which this instance is directly attached to. The bone
	/// must be part of the skeleton referenced by the owning <see cref="P:Syroot.NintenTools.NSW.Bfres.Model.Skeleton" /> instance.
	/// </summary>
	[Category("Polygon")]
	[DisplayName("Bone Index")]
	public ushort BoneIndex { get; set; }

	/// <summary>
	/// Gets or sets the index of the <see cref="P:Syroot.NintenTools.NSW.Bfres.Shape.VertexBuffer" /> in the owning <see cref="P:Syroot.NintenTools.NSW.Bfres.Model.VertexBuffers" />
	/// list.
	/// </summary>
	[Category("Polygon")]
	[DisplayName("Vertex Buffer Index")]
	public ushort VertexBufferIndex { get; set; }

	/// <summary>
	/// Gets or sets the bounding radius/radii spanning the shape for each LOD mesh
	/// </summary>
	[Category("Visibility")]
	[DisplayName("Bounding Radius")]
	public List<float> RadiusArray { get; set; }

	/// <summary>
	/// Gets or sets the number of bones influencing the vertices stored in this buffer. 0 influences equal
	/// rigidbodies (no skinning), 1 equal rigid skinning and 2 or more smooth skinning.
	/// </summary>
	[Category("Polygon")]
	[DisplayName("Max Vertex Skin Influence")]
	public byte VertexSkinCount { get; set; }

	/// <summary>
	/// Gets or sets a value with unknown purpose.
	/// </summary>
	[Category("Morph Data")]
	[DisplayName("Target Attribiute Count")]
	public byte TargetAttribCount { get; set; }

	/// <summary>
	/// Gets or sets the list of <see cref="P:Syroot.NintenTools.NSW.Bfres.Shape.Meshes" /> which are used to represent different level of details of the
	/// shape.
	/// </summary>
	[Category("Polygon")]
	[DisplayName("LOD Meshes")]
	public List<Mesh> Meshes { get; set; }

	[Category("Bones")]
	[DisplayName("Bone Indices")]
	public List<ushort> SkinBoneIndices { get; set; }

	[Category("Morph Data")]
	[DisplayName("Key Shapes")]
	public List<KeyShape> KeyShapes { get; set; }

	[Category("Visibility")]
	[DisplayName("Bounding Boxes")]
	public List<Bounding> SubMeshBoundings { get; set; }

	[Browsable(false)]
	public ResDict KeyShapeDict { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:Syroot.NintenTools.NSW.Bfres.BoundingNode" /> instances forming the bounding tree with which parts of a mesh
	/// are culled when not visible.
	/// </summary>
	public List<BoundingNode> SubMeshBoundingNodes { get; set; }

	public List<ushort> SubMeshBoundingIndices { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="P:Syroot.NintenTools.NSW.Bfres.Shape.VertexBuffer" /> instance storing the data which forms the shape's surface. Saved
	/// depending on <see cref="P:Syroot.NintenTools.NSW.Bfres.Shape.VertexBufferIndex" />.
	/// </summary>
	internal VertexBuffer VertexBuffer { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" /> class.
	/// </summary>
	public Shape()
	{
		Name = "";
		Flags = ShapeFlags.HasVertexBuffer;
		MaterialIndex = 0;
		BoneIndex = 0;
		VertexBufferIndex = 0;
		RadiusArray = new List<float>();
		VertexSkinCount = 0;
		TargetAttribCount = 0;
		Meshes = new List<Mesh>();
		SkinBoneIndices = new List<ushort>();
		KeyShapeDict = new ResDict();
		KeyShapes = new List<KeyShape>();
		SubMeshBoundings = new List<Bounding>();
		SubMeshBoundingNodes = new List<BoundingNode>();
		SubMeshBoundingIndices = new List<ushort>();
		VertexBuffer = new VertexBuffer();
	}

	public void CreateEmptyMesh()
	{
		uint[] indices = new uint[6] { 0u, 1u, 2u, 1u, 3u, 2u };
		Mesh mesh = new Mesh();
		mesh.SetIndices(indices, IndexFormat.UInt16);
		mesh.SubMeshes.Add(new SubMesh
		{
			Count = 6u
		});
		Meshes = new List<Mesh>();
		Meshes.Add(mesh);
		RadiusArray.Add(1f);
		SubMeshBoundings = new List<Bounding>();
		SubMeshBoundings.Add(new Bounding
		{
			Center = new Vector3F(0f, 0f, 0f),
			Extent = new Vector3F(50f, 50f, 50f)
		});
		SubMeshBoundings.Add(new Bounding
		{
			Center = new Vector3F(0f, 0f, 0f),
			Extent = new Vector3F(50f, 50f, 50f)
		});
		SubMeshBoundingIndices = new List<ushort>();
		SubMeshBoundingIndices.Add(0);
		SubMeshBoundingNodes = new List<BoundingNode>();
		SubMeshBoundingNodes.Add(new BoundingNode
		{
			LeftChildIndex = 0,
			NextSibling = 0,
			SubMeshIndex = 0,
			RightChildIndex = 0,
			Unknown = 0,
			SubMeshCount = 1
		});
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" /> class from the given <paramref name="stream" /> which
	/// is optionally left open.
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to load the data from.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after reading, otherwise <c>false</c>.</param>
	public void Import(Stream stream, VertexBuffer vtx, bool leaveOpen = false)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, stream, leaveOpen);
		resFileLoader.ImportShapes(vtx);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> class from the file with the given
	/// <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to load the data from.</param>
	public void Import(string fileName, VertexBuffer vtx)
	{
		using ResFileLoader resFileLoader = new ResFileLoader(this, fileName);
		resFileLoader.ImportShapes(vtx);
	}

	/// <summary>
	/// Saves the contents in the given <paramref name="stream" /> and optionally leaves it open
	/// </summary>
	/// <param name="stream">The <see cref="T:System.IO.Stream" /> to save the contents into.</param>
	/// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
	public void Export(Stream stream, ResFile ResFile, bool leaveOpen = false)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, ResFile, stream, leaveOpen);
		resFileSaver.ExportShape();
	}

	/// <summary>
	/// Saves the contents in the file with the given <paramref name="fileName" />.
	/// </summary>
	/// <param name="fileName">The name of the file to save the contents into.</param>
	public void Export(string fileName, ResFile ResFile)
	{
		using ResFileSaver resFileSaver = new ResFileSaver(this, ResFile, fileName);
		resFileSaver.ExportShape();
	}

	void IResData.Load(ResFileLoader loader)
	{
		loader.CheckSignature("FSHP");
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			Flags = loader.ReadEnum<ShapeFlags>(strict: false);
		}
		else
		{
			loader.LoadHeaderBlock();
		}
		Name = loader.LoadString();
		VertexBuffer = loader.Load<VertexBuffer>();
		long value = loader.ReadOffset();
		long value2 = loader.ReadOffset();
		long value3 = loader.ReadOffset();
		KeyShapeDict = loader.LoadDict();
		BoundingBoxArrayOffset = loader.ReadOffset();
		if (loader.ResFile.VersionMajor2 > 2 || loader.ResFile.VersionMajor != 0)
		{
			RadiusOffset = loader.ReadOffset();
			long num = loader.ReadInt64();
		}
		else
		{
			RadiusOffset = 0L;
			long num2 = loader.ReadInt64();
		}
		if (loader.ResFile.VersionMajor2 < 9)
		{
			Flags = loader.ReadEnum<ShapeFlags>(strict: true);
		}
		ushort num3 = loader.ReadUInt16();
		MaterialIndex = loader.ReadUInt16();
		BoneIndex = loader.ReadUInt16();
		VertexBufferIndex = loader.ReadUInt16();
		ushort numSkinBoneIndex = loader.ReadUInt16();
		VertexSkinCount = loader.ReadByte();
		byte b = loader.ReadByte();
		byte b2 = loader.ReadByte();
		byte b3 = loader.ReadByte();
		if (loader.ResFile.VersionMajor2 >= 9)
		{
			loader.Seek(2L);
		}
		else
		{
			loader.Seek(6L);
		}
		Meshes = ((b == 0) ? new List<Mesh>() : loader.LoadList<Mesh>(b, value).ToList());
		SkinBoneIndices = ((numSkinBoneIndex == 0) ? new List<ushort>() : loader.LoadCustom(() => loader.ReadUInt16s(numSkinBoneIndex), value2)?.ToList());
		KeyShapes = ((b2 == 0) ? new List<KeyShape>() : loader.LoadList<KeyShape>(b2, value3).ToList());
		RadiusArray = new List<float>();
		if (RadiusOffset != 0L && b > 0)
		{
			using (loader.TemporarySeek(RadiusOffset, SeekOrigin.Begin))
			{
				if (loader.ResFile.VersionMajor2 >= 10)
				{
					int num4 = ((numSkinBoneIndex == 0) ? b : numSkinBoneIndex);
					for (int num5 = 0; num5 < num4; num5++)
					{
						BoundingRadiusList.Add(loader.ReadVector4F());
					}
					float item = BoundingRadiusList.Max((Vector4F x) => x.W);
					RadiusArray.Add(item);
				}
				else
				{
					RadiusArray = loader.ReadSingles(b).ToList();
				}
			}
		}
		int boundingboxCount = 0;
		for (int num6 = 0; num6 < b; num6++)
		{
			for (int num7 = 0; num7 < Meshes[num6].SubMeshes.Count; num7++)
			{
				boundingboxCount++;
			}
			boundingboxCount++;
		}
		SubMeshBoundings = ((boundingboxCount == 0) ? new List<Bounding>() : loader.LoadCustom(() => loader.ReadBoundings(boundingboxCount), BoundingBoxArrayOffset)?.ToList());
		SubMeshBoundingNodes = new List<BoundingNode>();
	}

	internal void WriteBoudnings(ResFileSaver saver)
	{
		saver.Write(SubMeshBoundings);
	}

	void IResData.Save(ResFileSaver saver)
	{
		if (saver.ResFile.VersionMajor2 >= 10)
		{
			int num = ((SkinBoneIndices.Count == 0) ? Meshes.Count : SkinBoneIndices.Count);
			if (num != BoundingRadiusList.Count)
			{
				BoundingRadiusList.Clear();
				for (int i = 0; i < num; i++)
				{
					BoundingRadiusList.Add(new Vector4F(0f, 0f, 0f, RadiusArray.Max((float x) => x)));
				}
			}
		}
		saver.WriteSignature("FSHP");
		if (saver.ResFile.VersionMajor2 >= 9)
		{
			saver.Write(Flags, strict: false);
		}
		else
		{
			saver.Seek(12L);
		}
		if (SkinBoneIndices == null)
		{
			SkinBoneIndices = new List<ushort>();
		}
		saver.SaveRelocateEntryToSection(saver.Position, 8u, 1u, 0u, 1, "FSHP");
		saver.SaveString(Name);
		saver.Write(VertexBuffer.Position);
		PosMeshArrayOffset = saver.SaveOffset();
		PosSkinBoneIndicesOffset = saver.SaveOffset();
		PosKeyShapesOffset = saver.SaveOffset();
		PosKeyShapeDictOffset = saver.SaveOffset();
		PosSubMeshBoundingsOffset = saver.SaveOffset();
		PosRadiusArrayOffset = saver.SaveOffset();
		saver.Write(0L);
		if (saver.ResFile.VersionMajor2 < 9)
		{
			saver.Write(Flags, strict: true);
		}
		saver.Write((ushort)saver.CurrentIndex);
		saver.Write(MaterialIndex);
		saver.Write(BoneIndex);
		saver.Write(VertexBufferIndex);
		saver.Write((ushort)SkinBoneIndices.Count);
		saver.Write(VertexSkinCount);
		saver.Write((byte)Meshes.Count);
		saver.Write((byte)KeyShapeDict.Count);
		saver.Write(TargetAttribCount);
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
