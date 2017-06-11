using Rocket.Unturned.Skills;
using AutoSkill.Utils;

namespace AutoSkill
{
	public class Skill
	{
		public static Skill FromConfigurationSkill(AutoSkillConfiguration.Configuration_Skill confSkill)
		{
			UnturnedSkill skill = SkillsUtils.GetSkillByName(confSkill.Name);
			if (skill != null) return new Skill(skill, confSkill.Level);
			return null;
		}

		public readonly UnturnedSkill USkill;
		public readonly byte Level;

		internal Skill(UnturnedSkill skill, byte level)
		{
			USkill = skill;
			Level = level;		
		}
	}
}
