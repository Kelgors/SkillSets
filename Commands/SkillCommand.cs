using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using AutoSkill.Utils;

namespace AutoSkill.Commands
{
    class SkillCommand : IRocketCommand
    {

		public enum SWITCH
		{
			NULL,
			ON,
			OFF
		};
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "skills";

        public string Help => "Set all skills to maximum value";

        public string Syntax => "<SkillSetName|list> [on/off]";

        public List<string> Aliases => new List<string>();
		public List<string> Permissions => new List<string>() { "AutoSkill" };

		public void Execute(IRocketPlayer caller, string[] commands)
        {
			string skillSetName = null;
			bool showList = false;
			SWITCH swt = SWITCH.NULL;

			if (commands.Length > 0 && commands[0].Trim().Length > 0)
			{
				if (commands[0] == "list")
				{
					showList = true;
				}
				else
				{
					skillSetName = commands[0];
				}
			}
			if (commands.Length > 1 && commands[1].Trim().Length > 0)
			{
				if (commands[1] == "on") swt = SWITCH.ON;
				else if (commands[1] == "off") swt = SWITCH.OFF;
			}

			if (showList) ShowList(caller);
			else if (skillSetName != null) SetSkillSet(caller, skillSetName, swt);
        }

		void ShowList(IRocketPlayer caller)
		{
			string[] skillSetNames = SkillsUtils.FindSkillSetsForPlayer((UnturnedPlayer)caller).Select((skillSet) => skillSet.Name).ToArray();
			UnturnedChat.Say(caller, string.Format("SkillSets : {0}", string.Join(", ", skillSetNames)));
		}

		void SetSkillSet(IRocketPlayer caller, string skillSetName, SWITCH swt)
		{
			SkillSet skillSet = SkillsUtils.FindSkillSetByName(skillSetName);
			if (skillSet == null)
			{
				UnturnedChat.Say(caller, string.Format("Unknown SkillSet \"{0}\"", skillSetName));
				return;
			}

			if (!IsPermitted(caller, skillSet, swt))
			{
				CommandUtils.PermissionMissing(caller);
				return;
			}

			if (swt != SWITCH.OFF)
			{
				SkillsUtils.SetSkills((UnturnedPlayer)caller, skillSetName);
				UnturnedChat.Say(caller, AutoSkillPlugin.Instance.Translate("SETMAXSKILL_DONE"));
			}
			if (swt != SWITCH.NULL)
			{
				bool isOn = swt == SWITCH.ON;
				bool saved = AutoSkillPlugin.Instance.GetStorage().Save(((UnturnedPlayer)caller).CSteamID, skillSetName, isOn);
				if (saved)
				{
					UnturnedChat.Say(caller, AutoSkillPlugin.Instance.Translate(string.Format("SETAUTOSKILL_{0}", isOn? "ON" : "OFF")));
				}

			}
		}

		bool IsPermitted(IRocketPlayer caller, SkillSet skillSet, SWITCH swt)
		{
			if (caller.IsAdmin) return true;
			if (!SkillsUtils.CanUseSkillSet((UnturnedPlayer)caller, skillSet)) return false;

			if (swt == SWITCH.NULL && caller.HasPermission("AutoSkill.set")) return true;
			if (caller.HasPermission(string.Format("AutoSkill.{0}", swt == SWITCH.ON ? "on" : "off"))) return true;
			return false;
		}

    }
}
