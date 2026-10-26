using System.Diagnostics;
using System.Text;
using Syroot.BinaryData;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a <see cref="P:Syroot.NintenTools.NSW.Bfres.ResString.String" /> which is stored in a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" />.
/// </summary>
[DebuggerDisplay("{String}")]
public class ResString : IResData
{
	/// <summary>
	/// The textual <see cref="P:Syroot.NintenTools.NSW.Bfres.ResString.String" /> represented by this instance.
	/// </summary>
	public string String { get; set; }

	/// <summary>
	/// The <see cref="P:Syroot.NintenTools.NSW.Bfres.ResString.Encoding" /> with which this string was read or will be written.
	/// </summary>
	public Encoding Encoding { get; set; }

	/// <summary>
	/// Converts the given <paramref name="value" /> value to a <see cref="T:Syroot.NintenTools.NSW.Bfres.ResString" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="P:Syroot.NintenTools.NSW.Bfres.ResString.String" /> value to represent in the new <see cref="T:Syroot.NintenTools.NSW.Bfres.ResString" /> instance.
	/// </param>
	public static implicit operator ResString(string value)
	{
		return new ResString
		{
			String = value
		};
	}

	/// <summary>
	/// Converts the given <paramref name="value" /> value to an <see cref="P:Syroot.NintenTools.NSW.Bfres.ResString.String" /> instance.
	/// </summary>
	/// <param name="value">The <see cref="T:Syroot.NintenTools.NSW.Bfres.ResString" /> value to represent in the new <see cref="P:Syroot.NintenTools.NSW.Bfres.ResString.String" /> instance.
	/// </param>
	public static implicit operator string(ResString value)
	{
		return value.String;
	}

	/// <summary>
	/// Returns the value of the <see cref="P:Syroot.NintenTools.NSW.Bfres.ResString.String" /> property.
	/// </summary>
	/// <returns>The value of the <see cref="P:Syroot.NintenTools.NSW.Bfres.ResString.String" /> property.</returns>
	public override string ToString()
	{
		return String;
	}

	void IResData.Load(ResFileLoader loader)
	{
		String = loader.ReadString(BinaryStringFormat.ZeroTerminated, Encoding ?? loader.Encoding);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.SaveString(String, Encoding);
	}
}
