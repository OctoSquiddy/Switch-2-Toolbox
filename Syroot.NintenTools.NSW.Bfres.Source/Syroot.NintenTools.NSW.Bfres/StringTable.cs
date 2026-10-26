using System.Collections.Generic;
using System.IO;
using Syroot.BinaryData;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

public class StringTable : IResData
{
	public List<string> Strings = new List<string>();

	void IResData.Load(ResFileLoader loader)
	{
		Strings.Clear();
		loader.Seek(-20L, SeekOrigin.Current);
		uint num = loader.ReadUInt32();
		uint num2 = loader.ReadUInt32();
		long num3 = loader.ReadInt64();
		uint num4 = loader.ReadUInt32();
		for (int i = 0; i < num4 + 1; i++)
		{
			ushort num5 = loader.ReadUInt16();
			Strings.Add(loader.ReadString(BinaryStringFormat.ZeroTerminated));
			loader.Align(2);
		}
	}

	void IResData.Save(ResFileSaver saver)
	{
	}
}
