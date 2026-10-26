namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the possible data types in which <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Frames" /> are stored. For simple library use,
/// they are always converted them to and from <see cref="F:Syroot.NintenTools.NSW.Bfres.AnimCurveFrameType.Single" /> instances.
/// </summary>
public enum AnimCurveFrameType : ushort
{
	/// <summary>
	/// The frames are stored as <see cref="T:System.Single" /> instances.
	/// </summary>
	Single,
	/// <summary>
	/// The frames are stored as <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instances.
	/// </summary>
	Decimal10x5,
	/// <summary>
	/// The frames are stored as <see cref="T:System.Byte" /> instances.
	/// </summary>
	Byte
}
