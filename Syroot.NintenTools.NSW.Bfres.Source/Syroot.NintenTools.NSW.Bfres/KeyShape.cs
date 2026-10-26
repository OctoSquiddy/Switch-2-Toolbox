using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

public class KeyShape : IResData
{
	private byte[] padding;

	/// <summary>
	/// Index for <see cref="P:Syroot.NintenTools.NSW.Bfres.VertexBuffer.Attributes" /> for morhping the shape with <see cref="T:Syroot.NintenTools.NSW.Bfres.ShapeAnim" /> instances.
	/// </summary>
	public byte TargetAttribIndexPosition { get; set; }

	/// <summary>
	/// Index for <see cref="P:Syroot.NintenTools.NSW.Bfres.VertexBuffer.Attributes" /> for morhping the shape with <see cref="T:Syroot.NintenTools.NSW.Bfres.ShapeAnim" /> instances.
	/// </summary>
	public byte TargetAttribIndexNormal { get; set; }

	/// <summary>
	/// Index for <see cref="P:Syroot.NintenTools.NSW.Bfres.VertexBuffer.Attributes" /> for morhping the shape with <see cref="T:Syroot.NintenTools.NSW.Bfres.ShapeAnim" /> instances.
	/// </summary>
	public byte[] TargetAttribIndexTangent { get; set; }

	/// <summary>
	/// Index for <see cref="P:Syroot.NintenTools.NSW.Bfres.VertexBuffer.Attributes" /> for morhping the shape with <see cref="T:Syroot.NintenTools.NSW.Bfres.ShapeAnim" /> instances.
	/// </summary>
	public byte[] TargetAttribIndexBinormal { get; set; }

	/// <summary>
	/// Index for <see cref="P:Syroot.NintenTools.NSW.Bfres.VertexBuffer.Attributes" /> for morhping the shape with <see cref="T:Syroot.NintenTools.NSW.Bfres.ShapeAnim" /> instances.
	/// </summary>
	public byte[] TargetAttribColor { get; set; }

	void IResData.Load(ResFileLoader loader)
	{
		TargetAttribIndexPosition = loader.ReadByte();
		TargetAttribIndexNormal = loader.ReadByte();
		TargetAttribIndexTangent = loader.ReadBytes(4);
		TargetAttribIndexBinormal = loader.ReadBytes(4);
		TargetAttribColor = loader.ReadBytes(8);
		padding = loader.ReadBytes(2);
	}

	void IResData.Save(ResFileSaver saver)
	{
		saver.Write(TargetAttribIndexPosition);
		saver.Write(TargetAttribIndexNormal);
		saver.Write(TargetAttribIndexBinormal);
		saver.Write(TargetAttribColor);
		saver.Write(padding);
	}
}
