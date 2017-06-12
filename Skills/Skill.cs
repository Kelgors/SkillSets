using Rocket.Unturned.Skills;
using SkillSets.Utils;

namespace SkillSets
{
	public class Skill
	{
		public static Skill FromConfigurationSkill(SkillSetsConfiguration.Configuration_Skill confSkill)
		{
			UnturnedSkill skill = SkillsUtils.GetSkillByName(confSkill.Name);
			if (skill != null) return new Skill(skill, confSkill.Level);
			return null;
		}

		public readonly UnturnedSkill USkill;
		public readonly byte Level;

		public byte MaxLevel
		{
			get
			{
				if (USkill == null || !SkillsUtils.MaxSkillsLevel.ContainsKey(USkill)) return 0;
				return SkillsUtils.MaxSkillsLevel[USkill];
			}
		}

		internal Skill(UnturnedSkill skill, byte level)
		{
			USkill = skill;
			Level = level;		
		}
	}
}
