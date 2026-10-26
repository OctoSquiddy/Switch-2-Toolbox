namespace Syroot.NintenTools.NSW.Bfres;

public struct AnimConstant
{
	/// <summary>
	/// Gets or sets the memory offset relative to the start of the corresponding animation data structure to
	/// animate the field stored at that address. Note that enums exist in the specific animation which map offsets
	/// to names.
	/// </summary>
	public uint AnimDataOffset;

	public DWord Value;
}
