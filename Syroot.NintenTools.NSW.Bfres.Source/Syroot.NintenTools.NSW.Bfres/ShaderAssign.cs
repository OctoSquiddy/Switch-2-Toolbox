using System.Collections.Generic;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

public class ShaderAssign : IResData
{
	internal long PosAttribAssigns;

	internal long PosAttribAssignDict;

	internal long PosSamplerAssigns;

	internal long PosSamplerAssignDict;

	internal long PosShaderOptions;

	internal long PosShaderOptionsDict;

	public string ShaderArchiveName { get; set; }

	public string ShadingModelName { get; set; }

	public uint Revision { get; set; }

	public ResDict AttribAssignDict { get; set; }

	public IList<string> AttribAssigns { get; set; }

	public ResDict SamplerAssignDict { get; set; }

	public IList<string> SamplerAssigns { get; set; }

	public ResDict ShaderOptionDict { get; set; }

	public IList<string> ShaderOptions { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ShaderAssign" /> class.
	/// </summary>
	public ShaderAssign()
	{
		ShaderArchiveName = "";
		ShadingModelName = "";
		Revision = 0u;
		AttribAssigns = new List<string>();
		SamplerAssigns = new List<string>();
		ShaderOptions = new List<string>();
		AttribAssignDict = new ResDict();
		SamplerAssignDict = new ResDict();
		ShaderOptionDict = new ResDict();
	}

	void IResData.Load(ResFileLoader loader)
	{
		ShaderArchiveName = loader.LoadString();
		ShadingModelName = loader.LoadString();
		long value = loader.ReadOffset();
		AttribAssignDict = loader.LoadDict();
		long value2 = loader.ReadOffset(Relocated: true);
		SamplerAssignDict = loader.LoadDict();
		long value3 = loader.ReadOffset(Relocated: true);
		ShaderOptionDict = loader.LoadDict();
		Revision = loader.ReadUInt32();
		byte numAttribAssign = loader.ReadByte();
		byte numSamplerAssign = loader.ReadByte();
		ushort numShaderOption = loader.ReadUInt16();
		AttribAssigns = loader.LoadCustom(() => loader.LoadStrings(numAttribAssign), value);
		SamplerAssigns = loader.LoadCustom(() => loader.LoadStrings(numSamplerAssign), value2);
		ShaderOptions = loader.LoadCustom(() => loader.LoadStrings(numShaderOption), value3);
	}

	void IResData.Save(ResFileSaver saver)
	{
		if (AttribAssigns == null)
		{
			AttribAssigns = new List<string>();
		}
		if (SamplerAssigns == null)
		{
			SamplerAssigns = new List<string>();
		}
		if (ShaderOptions == null)
		{
			ShaderOptions = new List<string>();
		}
		if (AttribAssignDict == null)
		{
			AttribAssignDict = new ResDict();
		}
		if (SamplerAssignDict == null)
		{
			SamplerAssignDict = new ResDict();
		}
		if (ShaderOptionDict == null)
		{
			ShaderOptionDict = new ResDict();
		}
		saver.SaveString(ShaderArchiveName);
		saver.SaveString(ShadingModelName);
		PosAttribAssigns = saver.SaveOffset();
		PosAttribAssignDict = saver.SaveOffset();
		PosSamplerAssigns = saver.SaveOffset();
		PosSamplerAssignDict = saver.SaveOffset();
		PosShaderOptions = saver.SaveOffset();
		PosShaderOptionsDict = saver.SaveOffset();
		saver.Write(Revision);
		saver.Write((byte)AttribAssignDict.Count);
		saver.Write((byte)SamplerAssignDict.Count);
		saver.Write((ushort)ShaderOptionDict.Count);
	}
}
