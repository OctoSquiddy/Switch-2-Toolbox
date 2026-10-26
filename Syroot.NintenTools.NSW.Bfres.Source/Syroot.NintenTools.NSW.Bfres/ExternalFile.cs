using System.IO;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a file attachment to a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> which can be of arbitrary data.
/// </summary>
public class ExternalFile : IResData
{
	public long ofsData;

	public long sizData;

	/// <summary>
	/// Gets or sets the raw data stored by the external file.
	/// </summary>
	public byte[] Data { get; set; }

	public long ExternalOffPos { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ExternalFile" /> class.
	/// </summary>
	public ExternalFile()
	{
		Data = new byte[0];
	}

	/// <summary>
	/// Opens and returns a <see cref="T:System.IO.MemoryStream" /> on the raw <see cref="P:Syroot.NintenTools.NSW.Bfres.ExternalFile.Data" /> byte array, which optionally
	/// can be written to.
	/// </summary>
	/// <param name="writable"><c>true</c> to allow write access to the raw data.</param>
	/// <returns>The opened <see cref="T:System.IO.MemoryStream" /> instance.</returns>
	public MemoryStream GetStream(bool writable = false)
	{
		return new MemoryStream(Data, writable);
	}

	void IResData.Load(ResFileLoader loader)
	{
		ExternalOffPos = loader.Position;
		ofsData = loader.ReadOffset(Relocated: true);
		sizData = loader.ReadInt64();
		Data = loader.LoadCustom(() => loader.ReadBytes((int)sizData), ofsData);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.SaveRelocateEntryToSection(saver.Position, 1u, 1u, 0u, 5, "External files");
		if (Data == null)
		{
			Data = new byte[0];
		}
		if (Data.Length <= 3)
		{
			saver.SaveBlock(Data, 512, () =>
			{
				saver.Write(Data);
			});
		}
		else
		{
			saver.SaveBlock(Data, saver.ResFile.DataAlignment, () =>
			{
				saver.Write(Data);
			});
		}
		saver.Write((long)(Data?.Length).Value);
	}
}
