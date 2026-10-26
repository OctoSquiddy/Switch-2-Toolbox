namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the type of key values stored by this curve. This also determines the number of required elements to
/// define a key in the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> array. Use the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.ElementsPerKey" />
/// method to retrieve the number of elements required for the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.CurveType" /> of that curve.
/// </summary>
public enum AnimCurveType : ushort
{
	/// <summary>
	/// The curve uses cubic interpolation. 4 elements of the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> array form a key.
	/// </summary>
	Cubic = 0,
	/// <summary>
	/// The curve uses linear interpolation. 2 elements of the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> array form a key.
	/// </summary>
	Linear = 16,
	/// <summary>
	/// 1 element of the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> array forms a key.
	/// </summary>
	BakedFloat = 32,
	/// <summary>
	/// 1 element of the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> array forms a key.
	/// </summary>
	StepInt = 64,
	/// <summary>
	/// 1 element of the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> array forms a key.
	/// </summary>
	BakedInt = 80,
	/// <summary>
	/// 1 element of the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> array forms a key.
	/// </summary>
	StepBool = 96,
	/// <summary>
	/// 1 element of the <see cref="P:Syroot.NintenTools.NSW.Bfres.AnimCurve.Keys" /> array forms a key.
	/// </summary>
	BakedBool = 112
}
