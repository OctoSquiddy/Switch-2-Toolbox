namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the data type of elements of the <see cref="T:Syroot.NintenTools.NSW.Bfres.RenderInfo" /> value array.
/// </summary>
public enum RenderInfoType : byte
{
	/// <summary>
	/// The elements are <see cref="T:System.Int32" /> instances.
	/// </summary>
	Int32,
	/// <summary>
	/// The elements are <see cref="T:System.Single" /> instances.
	/// </summary>
	Single,
	/// <summary>
	/// The elements are <see cref="F:Syroot.NintenTools.NSW.Bfres.RenderInfoType.String" /> instances.
	/// </summary>
	String
}
