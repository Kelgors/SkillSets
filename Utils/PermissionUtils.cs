using Rocket.API;

namespace AutoSkill
{
	public static class PermissionUtils
	{

		public static bool IsPermitted(IRocketPlayer player, string skillsetName)
		{
			SkillSet skillSet = Utils.SkillsUtils.FindSkillSetByName(skillsetName);
			if (skillSet == null) return false;
			return player.HasPermission(skillSet.getPermissionName());
		}

		public static bool IsPermitted(IRocketPlayer player, SkillSet skillSet)
		{
			return player.HasPermission(skillSet.getPermissionName());
		}
	}
}
