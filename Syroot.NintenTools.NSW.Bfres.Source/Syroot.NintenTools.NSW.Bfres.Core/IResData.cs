namespace Syroot.NintenTools.NSW.Bfres.Core;

/// <summary>
/// Represents the common interface for <see cref="T:Syroot.NintenTools.NSW.Bfres.ResFile" /> data instances.
/// </summary>
public interface IResData
{
	/// <summary>
	/// Loads raw data from the <paramref name="loader" /> data stream into instances.
	/// </summary>
	/// <param name="loader">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileLoader" /> to load data with.</param>
	void Load(ResFileLoader loader);

	/// <summary>
	/// Saves header data of the instance and queues referenced data in the given <paramref name="saver" />.
	/// </summary>
	/// <param name="saver">The <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.ResFileSaver" /> to save headers and queue data with.</param>
	void Save(ResFileSaver saver);
}
