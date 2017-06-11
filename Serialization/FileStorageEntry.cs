using System.Xml.Serialization;

namespace AutoSkill
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
