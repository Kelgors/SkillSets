using Steamworks;

namespace AutoSkill
{
	/// <summary>
	/// Abstraction of data storing
	/// </summary>
    public interface IAutoSkillStorage
    {
		/// <summary>
		/// Load all necessary stuff for the storage
		/// </summary>
        void Load();
		/// <summary>
		/// Unload all stuff created in Load()
		/// </summary>
        void Unload();

		/// <summary>
		/// Save the steamId/value to the storage
		/// </summary>
		/// <returns>Saved or not</returns>
		/// <param name="steamId">Steam identifier.</param>
		/// <param name="skillSetName">The name of the SkillSet</param>
		/// <param name="enabled"></param>
        bool Save(CSteamID steamId, string skillSetName, bool enabled);

		/// <summary>
		/// Get if the specified steamId skills should be maxed or not
		/// </summary>
		/// <returns>The name of the SkillSet</returns>
		/// <param name="steamId">Steam identifier.</param>
        string Get(CSteamID steamId);
    }
}
