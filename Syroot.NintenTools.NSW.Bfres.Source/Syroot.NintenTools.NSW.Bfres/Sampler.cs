using System.ComponentModel;
using System.Diagnostics;
using Syroot.NintenTools.NSW.Bfres.Core;
using Syroot.NintenTools.NSW.Bfres.GFX;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a <see cref="!:Texture" /> sampler in a <see cref="T:Syroot.NintenTools.NSW.Bfres.UserData" /> section, storing configuration on how to
/// draw and interpolate textures.
/// </summary>
[DebuggerDisplay("Sampler {Name}")]
public class Sampler : IResData
{
	public enum MipFilterModes : ushort
	{
		None,
		Points,
		Linear
	}

	public enum ExpandFilterModes : ushort
	{
		Points = 4,
		Linear = 8
	}

	public enum ShrinkFilterModes : ushort
	{
		Points = 0x10,
		Linear = 0x20
	}

	private const ushort _flagsShrinkMask = 48;

	private const ushort _flagsExpandMask = 12;

	private const ushort _flagsMipmapMask = 3;

	private ushort _filterFlags;

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in <see cref="!:ResDict&lt;Sampler&gt;" />
	/// instances.
	/// </summary>
	[Category("Sampler")]
	[DisplayName("Name")]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the UV wrap mode in the U direction
	/// </summary>
	[Description("The texture repetition mode on the X axis.")]
	[Category("Wrap")]
	[DisplayName("Wrap X")]
	public TexClamp WrapModeU { get; set; }

	/// <summary>
	/// Gets or sets the UV wrap mode in the V direction
	/// </summary>
	[Description("The texture repetition mode on the Y axis.")]
	[Category("Wrap")]
	[DisplayName("Wrap Y")]
	public TexClamp WrapModeV { get; set; }

	/// <summary>
	/// Gets or sets the UV wrap mode in the W direction
	/// </summary>
	[Description("The texture repetition mode on the Z axis.")]
	[Category("Wrap")]
	[DisplayName("Wrap Z")]
	public TexClamp WrapModeW { get; set; }

	/// <summary>
	/// Gets or sets the compare function
	/// </summary>
	[Description("The depth comparison function.")]
	[Category("Depth")]
	[DisplayName("Function")]
	public CompareFunction CompareFunc { get; set; }

	/// <summary>
	/// Gets or sets the border color
	/// </summary>
	[Description("Color to draw at places not reached by a texture if the clamp mode does not repeat it.")]
	[Category("Filter")]
	[DisplayName("Border Type")]
	public TexBorderType BorderColorType { get; set; }

	/// <summary>
	/// Gets or sets the max anisotropic filtering value
	/// </summary>
	[Description("The maximum anisotropic filtering level to use.")]
	[Category("Filter")]
	[DisplayName("Anisotropic Ratio")]
	public MaxAnisotropic MaxAnisotropic { get; set; }

	[Description("The texture filtering on the X and Y axes when the texture is drawn smaller than the actual texture's resolution.")]
	[Category("Filter")]
	[DisplayName("Shrink XY")]
	public ShrinkFilterModes ShrinkXY
	{
		get
		{
			return (ShrinkFilterModes)(_filterFlags & 0x30);
		}
		set
		{
			_filterFlags = (ushort)((uint)(_filterFlags & -49) | (uint)value);
		}
	}

	[Description("The texture filtering on the X and Y axes when the texture is drawn larger than the actual texture's resolution.")]
	[Category("Filter")]
	[DisplayName("Exapnd XY")]
	public ExpandFilterModes ExpandXY
	{
		get
		{
			return (ExpandFilterModes)(_filterFlags & 0xC);
		}
		set
		{
			_filterFlags = (ushort)((uint)(_filterFlags & -13) | (uint)value);
		}
	}

	[Description("The texture filtering for mipmaps.")]
	[Category("Filter")]
	[DisplayName("MipMap")]
	public MipFilterModes Mipmap
	{
		get
		{
			return (MipFilterModes)(_filterFlags & 3);
		}
		set
		{
			_filterFlags = (ushort)((uint)(_filterFlags & -4) | (uint)value);
		}
	}

	[Description("The minimum LoD level.")]
	[Category("LOD")]
	[DisplayName("Min")]
	public float MinLOD { get; set; }

	[Description("The maximum LoD level.")]
	[Category("LOD")]
	[DisplayName("Max")]
	public float MaxLOD { get; set; }

	[Description("The LoD bias.")]
	[Category("LOD")]
	[DisplayName("Max")]
	public float LODBias { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.Sampler" /> class.
	/// </summary>
	public Sampler()
	{
		Name = "";
		WrapModeU = TexClamp.Repeat;
		WrapModeV = TexClamp.Repeat;
		WrapModeW = TexClamp.Clamp;
		CompareFunc = CompareFunction.Never;
		BorderColorType = TexBorderType.White;
		MaxAnisotropic = MaxAnisotropic.Ratio_1_1;
		LODBias = 0f;
		MinLOD = 0f;
		MaxLOD = 13f;
		_filterFlags = 42;
	}

	void IResData.Load(ResFileLoader loader)
	{
		WrapModeU = loader.ReadEnum<TexClamp>(strict: false);
		WrapModeV = loader.ReadEnum<TexClamp>(strict: false);
		WrapModeW = loader.ReadEnum<TexClamp>(strict: false);
		CompareFunc = loader.ReadEnum<CompareFunction>(strict: false);
		BorderColorType = loader.ReadEnum<TexBorderType>(strict: false);
		MaxAnisotropic = loader.ReadEnum<MaxAnisotropic>(strict: false);
		_filterFlags = loader.ReadUInt16();
		MinLOD = loader.ReadSingle();
		MaxLOD = loader.ReadSingle();
		LODBias = loader.ReadSingle();
		loader.Seek(12L);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.Write(WrapModeU, strict: false);
		saver.Write(WrapModeV, strict: false);
		saver.Write(WrapModeW, strict: false);
		saver.Write((byte)CompareFunc);
		saver.Write((byte)BorderColorType);
		saver.Write(MaxAnisotropic, strict: false);
		saver.Write(_filterFlags);
		saver.Write(MinLOD);
		saver.Write(MaxLOD);
		saver.Write(LODBias);
		saver.Seek(12L);
	}
}
