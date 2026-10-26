using System.Collections.Generic;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a material animation in a <see cref="T:Syroot.NintenTools.NSW.Bfres.MaterialAnim" /> subfile, storing material animation data.
/// </summary>
public class MaterialAnimData : IResData
{
	internal long PosParamInfoOffset;

	internal long PosTexPatInfoOffset;

	internal long PosCurvesOffset;

	internal long PosConstansOffset;

	/// <summary>
	/// Gets or sets the list of <see cref="T:Syroot.NintenTools.NSW.Bfres.ParamAnimInfo" /> instances.
	/// </summary>
	public IList<ParamAnimInfo> ParamAnimInfos { get; set; }

	/// <summary>
	/// Gets or sets the list of <see cref="T:Syroot.NintenTools.NSW.Bfres.TexturePatternAnimInfo" /> instances.
	/// </summary>
	public IList<TexturePatternAnimInfo> TexturePatternAnimInfos { get; set; }

	/// <summary>
	/// Gets or sets the list of <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimConstant" /> instances.
	/// </summary>
	public IList<AnimConstant> Constants { get; set; }

	/// <summary>
	/// Gets or sets <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> instances animating properties of objects stored in this section.
	/// </summary>
	public IList<AnimCurve> Curves { get; set; }

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;MaterialAnim&gt;" />
	/// instances.
	/// </summary>
	public string Name { get; set; }

	public int ShaderParamCurveIndex { get; set; }

	public int TexturePatternCurveIndex { get; set; }

	public int BeginVisalConstantIndex { get; set; }

	public int VisalCurveIndex { get; set; }

	public int VisualConstantIndex { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.MaterialAnimData" /> class.
	/// </summary>
	public MaterialAnimData()
	{
		Name = "";
		ParamAnimInfos = new List<ParamAnimInfo>();
		TexturePatternAnimInfos = new List<TexturePatternAnimInfo>();
		Constants = new List<AnimConstant>();
		Curves = new List<AnimCurve>();
		ShaderParamCurveIndex = -1;
		TexturePatternCurveIndex = -1;
		BeginVisalConstantIndex = -1;
		VisalCurveIndex = -1;
		VisualConstantIndex = -1;
	}

	void IResData.Load(ResFileLoader loader)
	{
		Name = loader.LoadString();
		long value = loader.ReadOffset();
		long value2 = loader.ReadOffset();
		long value3 = loader.ReadOffset();
		long value4 = loader.ReadOffset();
		ShaderParamCurveIndex = loader.ReadUInt16();
		TexturePatternCurveIndex = loader.ReadUInt16();
		BeginVisalConstantIndex = loader.ReadUInt16();
		VisalCurveIndex = loader.ReadUInt16();
		VisualConstantIndex = loader.ReadUInt16();
		ushort count = loader.ReadUInt16();
		ushort count2 = loader.ReadUInt16();
		ushort ConstantAnimCount = loader.ReadUInt16();
		ushort count3 = loader.ReadUInt16();
		loader.Seek(6L);
		Curves = loader.LoadList<AnimCurve>(count3, value3);
		ParamAnimInfos = loader.LoadList<ParamAnimInfo>(count, value);
		TexturePatternAnimInfos = loader.LoadList<TexturePatternAnimInfo>(count2, value2);
		Constants = loader.LoadCustom(() => loader.ReadAnimConstants(ConstantAnimCount), value4);
	}

	internal void WriteConstansts(ResFileSaver saver)
	{
		saver.Write(Constants);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.SaveRelocateEntryToSection(saver.Position, 5u, 1u, 0u, 1, "Material Animation Data");
		saver.SaveString(Name);
		PosParamInfoOffset = saver.SaveOffset();
		PosTexPatInfoOffset = saver.SaveOffset();
		PosCurvesOffset = saver.SaveOffset();
		PosConstansOffset = saver.SaveOffset();
		saver.Write((ushort)ShaderParamCurveIndex);
		saver.Write((ushort)TexturePatternCurveIndex);
		saver.Write((ushort)BeginVisalConstantIndex);
		saver.Write((ushort)VisalCurveIndex);
		saver.Write((ushort)VisualConstantIndex);
		saver.Write((ushort)ParamAnimInfos.Count);
		saver.Write((ushort)TexturePatternAnimInfos.Count);
		if (Constants != null)
		{
			saver.Write((ushort)Constants.Count);
		}
		else
		{
			saver.Write((ushort)0);
		}
		saver.Write((ushort)Curves.Count);
		saver.Seek(6L);
	}
}
