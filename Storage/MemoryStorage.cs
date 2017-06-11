using System.Collections.Generic;
using Steamworks;

namespace AutoSkill.Storage
{
	public class MemoryStorage : IAutoSkillStorage
	{
		Dictionary<ulong, string> Memory;

		public string Get(CSteamID steamId)
		{
			string result;
			Memory.TryGetValue(steamId.m_SteamID, out result);
			return result;
		}

		public void Load()
		{
			Memory = new Dictionary<ulong, string>();
		}

		public bool Save(CSteamID steamId, string skillSetName, bool enabled)
		{
			if (enabled)
				Memory[steamId.m_SteamID] = skillSetName;
			else
				Memory.Remove(steamId.m_SteamID);
			return true;
		}

		public void Unload()
		{
			Memory.Clear();
			Memory = null;
		}
	}
}
