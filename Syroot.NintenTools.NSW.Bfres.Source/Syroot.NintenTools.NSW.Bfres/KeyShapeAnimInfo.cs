using System.Diagnostics;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a key shape animation info in a <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexShapeAnim" /> instance.
/// </summary>
[DebuggerDisplay("ParamAnimInfo {Name}")]
public class KeyShapeAnimInfo : IResData
{
	/// <summary>
	/// Gets or sets the index of the curve in the <see cref="T:Syroot.NintenTools.NSW.Bfres.VertexShapeAnim" />.
	/// </summary>
	public sbyte CurveIndex;

	/// <summary>
	/// Gets or sets the index of the <see cref="T:Syroot.NintenTools.NSW.Bfres.KeyShape" /> in the <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" />.
	/// </summary>
	public sbyte SubBindIndex;

	/// <summary>
	/// Gets or sets the name of the <see cref="T:Syroot.NintenTools.NSW.Bfres.KeyShape" /> in the <see cref="T:Syroot.NintenTools.NSW.Bfres.Shape" />.
	/// </summary>
	public string Name;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.KeyShapeAnimInfo" /> class.
	/// </summary>
	public KeyShapeAnimInfo()
	{
		Name = "";
		CurveIndex = 0;
		SubBindIndex = 0;
	}

	void IResData.Load(ResFileLoader loader)
	{
		Name = loader.LoadString();
		CurveIndex = loader.ReadSByte();
		SubBindIndex = loader.ReadSByte();
		loader.Seek(6L);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.SaveString(Name);
		saver.Write(CurveIndex);
		saver.Write(SubBindIndex);
		saver.Seek(6L);
	}
}
