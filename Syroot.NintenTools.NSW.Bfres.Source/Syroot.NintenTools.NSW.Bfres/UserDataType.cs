namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the possible data types of values stored in <see cref="T:Syroot.NintenTools.NSW.Bfres.UserData" /> instances.
/// </summary>
public enum UserDataType : byte
{
	/// <summary>
	/// The values is an <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.Int32" /> array.
	/// </summary>
	Int32,
	/// <summary>
	/// The values is a <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.Single" /> array.
	/// </summary>
	Single,
	/// <summary>
	/// The values is a <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.String" /> array encoded in ASCII.
	/// </summary>
	String,
	/// <summary>
	/// The values is a <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.Byte" /> array.
	/// </summary>
	Byte,
	/// <summary>
	/// The values is a <see cref="F:Syroot.NintenTools.NSW.Bfres.UserDataType.String" /> array encoded in UTF-16.
	/// </summary>
	WString
}
