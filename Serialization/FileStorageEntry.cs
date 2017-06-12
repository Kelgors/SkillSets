using System.Xml.Serialization;

namespace SkillSets
{
	
	public class FileStorageEntry
	{
		[XmlAttribute()]
		public ulong SteamId;

		[XmlAttribute()]
		public string SkillSetName;

		public FileStorageEntry() { }

		public FileStorageEntry(ulong steamId, string skillSetName)
		{
			SteamId = steamId;
			SkillSetName = skillSetName;
		}
	}
}
