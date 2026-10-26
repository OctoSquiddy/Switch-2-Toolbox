using System.Diagnostics;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a parameter value in a <see cref="T:Syroot.NintenTools.NSW.Bfres.UserData" /> section, passing data to shader variables.
/// </summary>
[DebuggerDisplay("ShaderParam {Name}")]
public class ShaderParam : IResData
{
	/// <summary>
	/// Gets or sets the type of the value.
	/// </summary>
	public ShaderParamType Type { get; set; }

	/// <summary>
	/// Gets the offset in the <see cref="P:Syroot.NintenTools.NSW.Bfres.Material.ShaderParamData" /> byte array in bytes.
	/// </summary>
	public ushort DataOffset { get; set; }

	public ushort DependedIndex { get; set; }

	public ushort DependIndex { get; set; }

	/// <summary>
	/// Gets or sets the name with which the instance can be referenced uniquely in
	/// <see cref="!:ResDict&lt;ShaderParam&gt;" /> instances.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets the size of the value in bytes.
	/// </summary>
	public uint DataSize
	{
		get
		{
			if ((int)Type <= 15)
			{
				return 4u * (uint)((Type & ShaderParamType.Bool4) + 1);
			}
			if ((int)Type <= 27)
			{
				uint num = (uint)((Type & ShaderParamType.Bool4) + 1);
				uint num2 = ((uint)(Type - 16) >> 2) + 2;
				return 4 * num * num2;
			}
			return Type switch
			{
				ShaderParamType.Srt2D => 20u, 
				ShaderParamType.Srt3D => 36u, 
				ShaderParamType.TexSrt => 24u, 
				ShaderParamType.TexSrtEx => 28u, 
				_ => throw new ResException(string.Format("Cannot retrieve size of unknown {0} {1}.", "ShaderParamType", Type)), 
			};
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ShaderParam" /> class.
	/// </summary>
	public ShaderParam()
	{
		Name = "";
		Type = ShaderParamType.Float;
		DataOffset = 0;
		DependedIndex = 0;
		DependIndex = 0;
	}

	void IResData.Load(ResFileLoader loader)
	{
		long num = loader.ReadOffset();
		Name = loader.LoadString();
		Type = loader.ReadEnum<ShaderParamType>(strict: true);
		byte b = loader.ReadByte();
		DataOffset = loader.ReadUInt16();
		int num2 = loader.ReadInt32();
		DependedIndex = loader.ReadUInt16();
		DependIndex = loader.ReadUInt16();
		uint num3 = loader.ReadUInt32();
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.Write(0L);
		saver.SaveString(Name);
		saver.Write(Type, strict: true);
		saver.Write((byte)DataSize);
		saver.Write(DataOffset);
		saver.Write(-1);
		saver.Write(DependedIndex);
		saver.Write(DependIndex);
		saver.Write(0);
	}
}
