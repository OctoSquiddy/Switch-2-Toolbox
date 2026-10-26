using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents a buffer info section
/// </summary>
public class MemoryPool : IResData
{
	private const int Size = 288;

	public byte[] PoolArray { get; set; }

	void IResData.Load(ResFileLoader loader)
	{
	}

	void IResData.Save(ResFileSaver saver)
	{
	}
}
