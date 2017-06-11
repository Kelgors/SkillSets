using System;
using System.Reflection;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using Rocket.Unturned.Skills;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core;

namespace AutoSkill.Utils
{
	public static class SkillsUtils
	{

		public static void SetSkills(UnturnedPlayer player)
		{
            SetSkills(player, FindSkillSetForPlayer(player));
		}

		public static void SetSkills(UnturnedPlayer player, string skillSetName)
		{
			SetSkills(player, FindSkillSetByName(skillSetName));
		}

		public static void SetSkills(UnturnedPlayer player, SkillSet skillSet)
		{
			
			if (skillSet == null)
			{
				return;
			}
			if (!CanUseSkillSet(player, skillSet))
			{
				return;
			}

			List<Skill> skills = skillSet.Skills;
			UnturnedSkill[] allSkills = GetAllUnturnedSkills();
			foreach (UnturnedSkill uSkill in allSkills)
			{
				Skill skill = skills.Find((Skill _skill) => _skill.USkill.Equals(uSkill));
				if (skill != null)
				{
					player.SetSkillLevel(skill.USkill, skill.Level);
				}
				else
				{
					player.SetSkillLevel(uSkill, 0);
				}

			}
		}

		public static bool CanUseSkillSet(UnturnedPlayer player, SkillSet skillSet)
		{
			if (skillSet.PermissionGroup == null) return true;
			return R.Permissions.GetGroups(player, true).Where((group) => group.Id == skillSet.PermissionGroup.Id).Count() > 0;
		}

		public static UnturnedSkill GetSkillByName(string skillName) 
		{ 
			try
			{
				FieldInfo fieldInfo = typeof(UnturnedSkill).GetField(skillName, BindingFlags.Static | BindingFlags.Public);
				if (fieldInfo != null)
				{
					return (UnturnedSkill) fieldInfo.GetValue(null);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(AutoSkillPlugin.WrapLog(string.Format("The skill \"{0}\" is not found", skillName)));
				Logger.LogException(ex);
			}
			return null;
		}

		public static SkillSet FindSkillSetByName(string name) 
		{
			return AutoSkillPlugin.Instance.SkillSets.Find((skillSet) => skillSet.Name == name);
		}

		public static List<SkillSet> FindSkillSetsForPlayer(UnturnedPlayer player)
		{
			List<string> groupIds = R.Permissions.GetGroups(player, true).Select((group) => group.Id).ToList();
			return AutoSkillPlugin.Instance.SkillSets
								  .Where((_skillSet) => _skillSet.PermissionGroup == null || groupIds.Contains(_skillSet.PermissionGroup.Id))
								  .OrderBy((_skillSet) => _skillSet.PermissionGroupPriority)
								  .ToList();
		}

		public static SkillSet FindSkillSetForPlayer(UnturnedPlayer player) 
		{
			return FindSkillSetsForPlayer(player).AsEnumerable().Last();
		}

		public static UnturnedSkill[] GetAllUnturnedSkills()
		{
			return new UnturnedSkill[] 
			{
				UnturnedSkill.Agriculture,
				UnturnedSkill.Cardio,
				UnturnedSkill.Cooking,
				UnturnedSkill.Crafting,
				UnturnedSkill.Dexerity,
				UnturnedSkill.Diving,
				UnturnedSkill.Engineer,
				UnturnedSkill.Exercise,
				UnturnedSkill.Fishing,
				UnturnedSkill.Healing,
				UnturnedSkill.Immunity,
				UnturnedSkill.Mechanic,
				UnturnedSkill.Outdoors,
				UnturnedSkill.Overkill,
				UnturnedSkill.Parkour,
				UnturnedSkill.Sharpshooter,
				UnturnedSkill.Sneakybeaky,
				UnturnedSkill.Strength,
				UnturnedSkill.Survival,
				UnturnedSkill.Toughness,
				UnturnedSkill.Vitality,
				UnturnedSkill.Warmblooded
			};
		}

	}
}
