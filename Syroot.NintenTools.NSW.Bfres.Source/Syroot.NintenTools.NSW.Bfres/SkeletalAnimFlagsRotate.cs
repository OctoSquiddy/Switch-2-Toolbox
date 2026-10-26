namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the data format in which rotation values are stored.
/// </summary>
public enum SkeletalAnimFlagsRotate : uint
{
	/// <summary>
	/// Quaternion, 4 components.
	/// </summary>
	Quaternion = 0u,
	/// <summary>
	/// Euler XYZ, 3 components.
	/// </summary>
	EulerXYZ = 0x1000u
}
