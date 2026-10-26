namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the possible data types in which <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> are stored. For simple library use,
/// they are always converted them to and from <see cref="F:Syroot.NintenTools.NSW.Bfres.AnimCurveKeyType.Single" /> instances.
/// </summary>
public enum AnimCurveKeyType : ushort
{
	/// <summary>
	/// The keys are stored as <see cref="T:System.Single" /> instances.
	/// </summary>
	Single = 0,
	/// <summary>
	/// The keys are stored as <see cref="T:Syroot.NintenTools.NSW.Bfres.Decimal10x5" /> instances.
	/// </summary>
	Int16 = 4,
	/// <summary>
	/// The keys are stored as <see cref="T:System.SByte" /> instances.
	/// </summary>
	SByte = 8
}
