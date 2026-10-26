using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a material animation in a <see cref="T:Syroot.NintenTools.NSW.Bfres.MaterialAnim" /> subfile, storing material animation data.
/// </summary>
public class TexturePatternAnimInfo : IResData
{
	/// <summary>
	/// Gets or sets the index of the curve in the <see cref="T:Syroot.NintenTools.NSW.Bfres.TexturePatternAnimInfo" />.
	/// </summary>
	public ushort CurveIndex;

	/// <summary>
	/// Gets or sets the index of the <see cref="T:Syroot.NintenTools.NSW.Bfres.KeyShape" /> in the <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" />.
	/// </summary>
	public sbyte SubBindIndex;

	/// <summary>
	/// Gets or sets the index of the first <see cref="T:Syroot.NintenTools.NSW.Bfres.AnimConstant" /> instance in the parent
	/// <see cref="!:ShaderParamMatAnim" />.
	/// </summary>
	public ushort BeginConstant { get; set; }

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;MaterialAnim&gt;" />
	/// instances.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.TexturePatternAnimInfo" /> class.
	/// </summary>
	public TexturePatternAnimInfo()
	{
		Name = "";
		CurveIndex = ushort.MaxValue;
		BeginConstant = ushort.MaxValue;
		SubBindIndex = -1;
	}

	void IResData.Load(ResFileLoader loader)
	{
		Name = loader.LoadString();
		CurveIndex = loader.ReadUInt16();
		BeginConstant = loader.ReadUInt16();
		SubBindIndex = loader.ReadSByte();
		loader.Seek(3L);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.SaveString(Name);
		saver.Write(CurveIndex);
		saver.Write(BeginConstant);
		saver.Write(SubBindIndex);
		saver.Seek(3L);
	}
}
