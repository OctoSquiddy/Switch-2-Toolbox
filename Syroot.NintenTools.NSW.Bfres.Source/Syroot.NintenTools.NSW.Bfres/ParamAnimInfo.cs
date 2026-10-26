using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a material animation in a <see cref="T:Syroot.NintenTools.NSW.Bfres.MaterialAnim" /> subfile, storing material animation data.
/// </summary>
public class ParamAnimInfo : IResData
{
	/// <summary>
	/// Gets or sets the index of the first <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimCurve" /> instance in the parent
	/// <see cref="!:ShaderParamMatAnim" />.
	/// </summary>
	public ushort BeginCurve { get; set; }

	public ushort FloatCurveCount { get; set; }

	public ushort IntCurveCount { get; set; }

	/// <summary>
	/// Gets or sets the index of the first <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimConstant" /> instance in the parent
	/// <see cref="!:ShaderParamMatAnim" />.
	/// </summary>
	public ushort BeginConstant { get; set; }

	/// <summary>
	/// Gets or sets the number of <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimConstant" /> instances used in the parent
	/// <see cref="!:ShaderParamMatAnim" />.
	/// </summary>
	public ushort ConstantCount { get; set; }

	/// <summary>
	/// Gets or sets the index of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ShaderParam" /> in the <see cref="T:Syroot.NintenTools.NSW.Bfres.Material" />.
	/// </summary>
	public ushort SubBindIndex { get; set; }

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;MaterialAnim&gt;" />
	/// instances.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ParamAnimInfo" /> class.
	/// </summary>
	public ParamAnimInfo()
	{
		Name = "";
		BeginCurve = 0;
		FloatCurveCount = 0;
		IntCurveCount = 0;
		BeginConstant = 0;
		ConstantCount = 0;
		SubBindIndex = ushort.MaxValue;
	}

	void IResData.Load(ResFileLoader loader)
	{
		Name = loader.LoadString();
		BeginCurve = loader.ReadUInt16();
		FloatCurveCount = loader.ReadUInt16();
		IntCurveCount = loader.ReadUInt16();
		BeginConstant = loader.ReadUInt16();
		ConstantCount = loader.ReadUInt16();
		SubBindIndex = loader.ReadUInt16();
		loader.Seek(4L);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.SaveString(Name);
		saver.Write(BeginCurve);
		saver.Write(FloatCurveCount);
		saver.Write(IntCurveCount);
		saver.Write(BeginConstant);
		saver.Write(ConstantCount);
		saver.Write(SubBindIndex);
		saver.Seek(4L);
	}
}
