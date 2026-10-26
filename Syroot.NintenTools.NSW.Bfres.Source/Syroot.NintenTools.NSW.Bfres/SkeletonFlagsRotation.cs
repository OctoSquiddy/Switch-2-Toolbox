namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the rotation method used to store bone rotations.
/// </summary>
public enum SkeletonFlagsRotation : uint
{
	/// <summary>
	/// A quaternion represents the rotation.
	/// </summary>
	Quaternion = 0u,
	/// <summary>
	/// A <see cref="T:Syroot.Maths.Vector3F" /> represents the Euler rotation in XYZ order.
	/// </summary>
	EulerXYZ = 0x1000u
}
