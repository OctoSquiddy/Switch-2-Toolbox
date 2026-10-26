using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a vertex shape animation in a <see cref="T:Syroot.NintenTools.NSW.Bfres.ShapeAnim" /> subfile.
/// </summary>
[DebuggerDisplay("VertexShapeAnim {Name}")]
public class VertexShapeAnim : IResData
{
	internal long PosBaseDataOffset;

	internal long PosCurvesOffset;

	internal long PosKeyShapeAnimInfosOffset;

	/// <summary>
	/// Gets or sets the name of the animated <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" />.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the list of <see cref="T:Syroot.NintenTools.NSW.Bfres.KeyShapeAnimInfo" /> instances.
	/// </summary>
	public IList<KeyShapeAnimInfo> KeyShapeAnimInfos { get; set; }

	/// <summary>
	/// Gets or sets <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> instances animating properties of objects stored in this section.
	/// </summary>
	public IList<AnimCurve> Curves { get; set; }

	/// <summary>
	/// Gets or sets the list of base values, excluding the base shape (which is always being initialized with 0f).
	/// </summary>
	public float[] BaseDataList { get; set; }

	/// <summary>
	/// Gets or sets the index of the first <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> relative to all curves of the parent
	/// <see cref="P:Syroot.NintenTools.NSW.Bfres.ShapeAnim.VertexShapeAnims" /> instances.
	/// </summary>
	internal int BeginCurve { get; set; }

	/// <summary>
	/// Gets or sets the index of the first <see cref="T:Syroot.NintenTools.NSW.Bfres.KeyShapeAnimInfo" /> relative to all key shape anim infos of
	/// the parent <see cref="P:Syroot.NintenTools.NSW.Bfres.ShapeAnim.VertexShapeAnims" /> instances.
	/// </summary>
	internal int BeginKeyShapeAnim { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexShapeAnim" /> class.
	/// </summary>
	public VertexShapeAnim()
	{
		Name = "";
		KeyShapeAnimInfos = new List<KeyShapeAnimInfo>();
		Curves = new List<AnimCurve>();
		BaseDataList = new float[0];
		BeginCurve = 0;
		BeginKeyShapeAnim = 0;
	}

	void IResData.Load(ResFileLoader loader)
	{
		Name = loader.LoadString();
		long value = loader.ReadOffset(Relocated: true);
		long value2 = loader.ReadOffset(Relocated: true);
		long value3 = loader.ReadOffset(Relocated: true);
		ushort count = loader.ReadUInt16();
		ushort numKeyShapeAnim = loader.ReadUInt16();
		BeginCurve = loader.ReadInt32();
		BeginKeyShapeAnim = loader.ReadInt32();
		loader.Seek(4L);
		KeyShapeAnimInfos = loader.LoadList<KeyShapeAnimInfo>(numKeyShapeAnim, value3);
		Curves = loader.LoadList<AnimCurve>(count, value);
		BaseDataList = loader.LoadCustom(() => loader.ReadSingles(numKeyShapeAnim - 1), value2);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.SaveString(Name);
		PosCurvesOffset = saver.SaveOffset();
		PosBaseDataOffset = saver.SaveOffset();
		PosKeyShapeAnimInfosOffset = saver.SaveOffset();
		saver.Write((ushort)Curves.Count);
		saver.Write((ushort)KeyShapeAnimInfos.Count);
		saver.Write(BeginCurve);
		saver.Write(BeginKeyShapeAnim);
		saver.Seek(4L);
	}
}
