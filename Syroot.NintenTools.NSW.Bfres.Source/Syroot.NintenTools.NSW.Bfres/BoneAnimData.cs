using Syroot.Maths;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the animatable data of a <see cref="T:Syroot.NintenTools.NSW.Bfres.Bone" /> instance.
/// </summary>
public struct BoneAnimData
{
	public uint Flags;

	/// <summary>
	/// The scaling of the bone.
	/// </summary>
	public Vector3F Scale;

	/// <summary>
	/// The translation of the bone.
	/// </summary>
	public Vector3F Translate;

	/// <summary>
	/// An unused field.
	/// </summary>
	public uint Padding;

	/// <summary>
	/// The rotation of the bone.
	/// </summary>
	public Vector4F Rotate;

	public long tempPos;

	internal BoneAnimData(ResFileLoader loader, BoneAnimFlagsBase flags)
	{
		tempPos = loader.Position;
		Flags = 0u;
		Scale = (flags.HasFlag(BoneAnimFlagsBase.Scale) ? loader.ReadVector3F() : Vector3F.Zero);
		Rotate = (flags.HasFlag(BoneAnimFlagsBase.Rotate) ? loader.ReadVector4F() : Vector4F.Zero);
		Padding = 0u;
		Translate = (flags.HasFlag(BoneAnimFlagsBase.Translate) ? loader.ReadVector3F() : Vector3F.Zero);
	}

	internal void Save(ResFileSaver saver, BoneAnimFlagsBase flags)
	{
		tempPos = saver.Position;
		if (flags.HasFlag(BoneAnimFlagsBase.Scale))
		{
			saver.Write(Scale);
		}
		if (flags.HasFlag(BoneAnimFlagsBase.Rotate))
		{
			saver.Write(Rotate);
		}
		if (flags.HasFlag(BoneAnimFlagsBase.Translate))
		{
			saver.Write(Translate);
		}
	}
}
