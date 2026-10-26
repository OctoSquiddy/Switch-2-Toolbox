using Syroot.Maths;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the animatable data of scene cameras.
/// </summary>
public struct CameraAnimData
{
	/// <summary>
	/// The near clipping plane distance.
	/// </summary>
	public float ClipNear;

	/// <summary>
	/// The far clipping plane distance.
	/// </summary>
	public float ClipFar;

	/// <summary>
	/// The aspect ratio of the projected image.
	/// </summary>
	public float AspectRatio;

	/// <summary>
	/// The field of view of the projected image.
	/// </summary>
	public float FieldOfView;

	/// <summary>
	/// The spatial position of the camera.
	/// </summary>
	public Vector3F Position;

	/// <summary>
	/// The spatial rotation of the camera.
	/// </summary>
	public Vector3F Rotation;

	/// <summary>
	/// The spatial twist of the camera.
	/// </summary>
	public float Twist;

	internal CameraAnimData(ResFileLoader loader)
	{
		ClipNear = loader.ReadSingle();
		ClipFar = loader.ReadSingle();
		AspectRatio = loader.ReadSingle();
		FieldOfView = loader.ReadSingle();
		Position = loader.ReadVector3F();
		Rotation = loader.ReadVector3F();
		Twist = loader.ReadSingle();
	}

	internal void Save(ResFileSaver saver)
	{
		saver.Write(ClipNear);
		saver.Write(ClipFar);
		saver.Write(AspectRatio);
		saver.Write(FieldOfView);
		saver.Write(Position);
		saver.Write(Rotation);
		saver.Write(Twist);
	}
}
