using Syroot.Maths;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the animatable data of scene fog.
/// </summary>
public struct FogAnimData
{
	/// <summary>
	/// The distance attenuation of the fog depth.
	/// </summary>
	public Vector2F DistanceAttn;

	/// <summary>
	/// The color of the fog.
	/// </summary>
	public Vector3F Color;

	internal FogAnimData(ResFileLoader loader)
	{
		DistanceAttn = loader.ReadVector2F();
		Color = loader.ReadVector3F();
	}

	internal void Save(ResFileSaver saver)
	{
		saver.Write(DistanceAttn);
		saver.Write(Color);
	}
}
