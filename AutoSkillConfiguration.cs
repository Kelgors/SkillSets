using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;

namespace AutoSkill
{
	public class AutoSkillConfiguration : IRocketPluginConfiguration
    {
		/// <summary>
		/// The type of the storage (none|file)
		/// </summary>
        public string StorageType;

		/// <summary>
		/// The file path (only if StorageType == file)
		/// </summary>
        public string FilePath;

		/// <summary>
		/// The periodic save (in milliseconds)
		/// Actually only used if StorageType == file
		/// </summary>
		public uint PeriodicSave;

		[XmlArrayItem(ElementName = "SkillSet")]
		public List<Configuration_SkillSet> SkillSets;

        public void LoadDefaults()
        {
            StorageType = "file";
            FilePath = "EnabledAutoSkill.dat";
			PeriodicSave = 10000;
			SkillSets = new List<Configuration_SkillSet> {
				new Configuration_SkillSet()
				{
					Name = "default",
					Default = true,
					PermissionGroup = "default",
					Skills = new List<Configuration_Skill>() {
						new Configuration_Skill() { Name = "Agriculture", Level = 7 },
						new Configuration_Skill() { Name = "Cardio", Level = 5 },
						new Configuration_Skill() { Name = "Cooking", Level = 3 },
						new Configuration_Skill() { Name = "Crafting", Level = 3 },
						new Configuration_Skill() { Name = "Dexerity", Level = 5 },
						new Configuration_Skill() { Name = "Diving", Level = 5 },
						new Configuration_Skill() { Name = "Engineer", Level = 3 },
						new Configuration_Skill() { Name = "Exercise", Level = 5 },
						new Configuration_Skill() { Name = "Fishing", Level = 5 },
						new Configuration_Skill() { Name = "Healing", Level = 7 },
						new Configuration_Skill() { Name = "Immunity", Level = 5 },
						new Configuration_Skill() { Name = "Mechanic", Level = 5 },
						new Configuration_Skill() { Name = "Outdoors", Level = 5 },
						new Configuration_Skill() { Name = "Overkill", Level = 7 },
						new Configuration_Skill() { Name = "Parkour", Level = 5 },
						new Configuration_Skill() { Name = "Sharpshooter", Level = 7 },
						new Configuration_Skill() { Name = "Sneakybeaky", Level = 7 },
						new Configuration_Skill() { Name = "Strength", Level = 5 },
						new Configuration_Skill() { Name = "Survival", Level = 5 },
						new Configuration_Skill() { Name = "Toughness", Level = 5 },
						new Configuration_Skill() { Name = "Vitality", Level = 5 },
						new Configuration_Skill() { Name = "Warmblooded", Level = 5 }
					}
				}
			};
        }

		public class Configuration_SkillSet
		{
			public string Name;
			public string PermissionGroup;
			public bool Default;
			[XmlArrayItem(ElementName = "Skill")]
			public List<Configuration_Skill> Skills;
		}

		public class Configuration_Skill
		{
			[XmlAttribute()]
			public string Name;
			[XmlAttribute()]
			public byte Level;
		}

    }
}
