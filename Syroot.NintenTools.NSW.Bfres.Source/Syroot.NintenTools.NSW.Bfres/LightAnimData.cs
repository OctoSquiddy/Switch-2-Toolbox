using Syroot.Maths;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the animatable data of scene lighting.
/// </summary>
public struct LightAnimData
{
	/// <summary>
	/// Enables or disables the light in total.
	/// </summary>
	public int Enable;

	/// <summary>
	/// The spatial origin of the light source for point or spot lights.
	/// </summary>
	public Vector3F Position;

	/// <summary>
	/// The spatial rotation of the light source.
	/// </summary>
	public Vector3F Rotation;

	/// <summary>
	/// The distance attenuation of the light.
	/// </summary>
	public Vector2F DistanceAttn;

	/// <summary>
	/// The angle attenuation of the light in degrees.
	/// </summary>
	public Vector2F AngleAttn;

	/// <summary>
	/// The first light source color.
	/// </summary>
	public Vector3F Color0;

	/// <summary>
	/// The second light source color.
	/// </summary>
	public Vector3F Color1;

	internal LightAnimData(ResFileLoader loader, LightAnimField flags)
	{
		Enable = (flags.HasFlag(LightAnimField.Enable) ? loader.ReadInt32() : 0);
		Position = (flags.HasFlag(LightAnimField.Position) ? loader.ReadVector3F() : Vector3F.Zero);
		Rotation = (flags.HasFlag(LightAnimField.Rotation) ? loader.ReadVector3F() : Vector3F.Zero);
		DistanceAttn = (flags.HasFlag(LightAnimField.DistanceAttn) ? loader.ReadVector2F() : Vector2F.Zero);
		AngleAttn = (flags.HasFlag(LightAnimField.AngleAttn) ? loader.ReadVector2F() : Vector2F.Zero);
		Color0 = (flags.HasFlag(LightAnimField.Color0) ? loader.ReadVector3F() : Vector3F.Zero);
		Color1 = (flags.HasFlag(LightAnimField.Color1) ? loader.ReadVector3F() : Vector3F.Zero);
	}

	internal void Save(ResFileSaver saver, LightAnimField flags)
	{
		if (flags.HasFlag(LightAnimField.Enable))
		{
			saver.Write(Enable);
		}
		if (flags.HasFlag(LightAnimField.Position))
		{
			saver.Write(Position);
		}
		if (flags.HasFlag(LightAnimField.Rotation))
		{
			saver.Write(Rotation);
		}
		if (flags.HasFlag(LightAnimField.DistanceAttn))
		{
			saver.Write(DistanceAttn);
		}
		if (flags.HasFlag(LightAnimField.AngleAttn))
		{
			saver.Write(AngleAttn);
		}
		if (flags.HasFlag(LightAnimField.Color0))
		{
			saver.Write(Color0);
		}
		if (flags.HasFlag(LightAnimField.Color1))
		{
			saver.Write(Color1);
		}
	}
}
