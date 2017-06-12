using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using Rocket.Unturned.Skills;

namespace AutoSkill.Utils
{
	public static class SkillsUtils
	{

		public static readonly Dictionary<UnturnedSkill, byte> MaxSkillsLevel = new Dictionary<UnturnedSkill, byte>()
		{
			{ UnturnedSkill.Agriculture, 7 },
			{ UnturnedSkill.Cardio, 5 },
			{ UnturnedSkill.Cooking, 3 },
			{ UnturnedSkill.Crafting, 3 },
			{ UnturnedSkill.Dexerity, 5 },
			{ UnturnedSkill.Diving, 5 },
			{ UnturnedSkill.Engineer, 3 },
			{ UnturnedSkill.Exercise, 5 },
			{ UnturnedSkill.Fishing, 5 },
			{ UnturnedSkill.Healing, 7 },
			{ UnturnedSkill.Immunity, 5 },
			{ UnturnedSkill.Mechanic, 5 },
			{ UnturnedSkill.Outdoors, 5 },
			{ UnturnedSkill.Overkill, 7 },
			{ UnturnedSkill.Parkour, 5 },
			{ UnturnedSkill.Sharpshooter, 7 },
			{ UnturnedSkill.Sneakybeaky, 7 },
			{ UnturnedSkill.Strength, 5 },
			{ UnturnedSkill.Survival, 5 },
			{ UnturnedSkill.Toughness, 5 },
			{ UnturnedSkill.Vitality, 5 },
			{ UnturnedSkill.Warmblooded, 5 }
		};

		public static void SetSkills(UnturnedPlayer player, string skillSetName)
		{
			SetSkills(player, FindSkillSetByName(skillSetName));
		}

		public static void SetSkills(UnturnedPlayer player, SkillSet skillSet)
		{
			if (skillSet == null || !PermissionUtils.IsPermitted(player, skillSet)) return;

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
			if (name == null) return null;
			return AutoSkillPlugin.Instance.SkillSets.Find((skillSet) => skillSet.Name == name);
		}

		public static List<SkillSet> GetDefaultPermittedSkillSets(IRocketPlayer player)
		{
			return GetPermittedSkillSets(player).Where((SkillSet skillset) => skillset.IsDefault).ToList();
		}

		public static SkillSet GetHigherSkillSet(List<SkillSet> skillsets)
		{
			return skillsets.OrderByDescending((SkillSet skillset) => skillset.GetScore()).FirstOrDefault();
		}

		public static List<SkillSet> GetPermittedSkillSets(IRocketPlayer player) 
		{
			return AutoSkillPlugin.Instance.SkillSets.Where((SkillSet skillset) =>
			{
				return PermissionUtils.IsPermitted(player, skillset);
			}).ToList();
		}

		/// <summary>
		/// Gets all unturned skills.
		/// </summary>
		/// <returns>All unturned skills.</returns>
		public static UnturnedSkill[] GetAllUnturnedSkills()
		{
			return MaxSkillsLevel.Keys.ToArray();
		}

	}
}
