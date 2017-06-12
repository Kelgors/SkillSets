using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using AutoSkill.Utils;

namespace AutoSkill.Commands
{
    class SkillsCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "skills";

        public string Help => "Set all skills to maximum value";

        public string Syntax => "[SkillSetName]";

		public List<string> Aliases => new List<string>() { "skill" };
		public List<string> Permissions => new List<string>();

		public void Execute(IRocketPlayer caller, string[] commands)
        {
			if (commands.Length > 0 && commands[0].Trim().Length > 0)
			{
                SetSkillSet(caller, commands[0]);
			}
			else ShowVoid(caller);
        }

		void ShowVoid(IRocketPlayer caller)
		{
			UnturnedChat.Say(caller, "AutoSkill usage");
			UnturnedChat.Say(caller, "/skills [SkillSetName]");
			ShowList(caller);
		}

		void ShowList(IRocketPlayer caller)
		{
			string currentSkillsetName = AutoSkillPlugin.Instance.GetStorage().Get(((UnturnedPlayer)caller).CSteamID);
			string[] skillSetNames = SkillsUtils.GetPermittedSkillSets(caller).Select((skillSet) =>
			{
				if (skillSet.Name == currentSkillsetName) return string.Format("[{0}]", skillSet.Name);
				return skillSet.Name;
			}).ToArray();
			UnturnedChat.Say(caller, string.Format("Available SkillSets : {0}", string.Join(", ", skillSetNames)));
		}

		void SetSkillSet(IRocketPlayer caller, string skillSetName)
		{
			SkillSet skillSet = SkillsUtils.FindSkillSetByName(skillSetName);
			if (skillSet == null)
			{
				UnturnedChat.Say(caller, string.Format("Unknown SkillSet \"{0}\"", skillSetName));
				return;
			}

			if (!IsPermitted(caller, skillSet))
			{
				CommandUtils.PermissionMissing(caller);
				return;
			}
			SkillsUtils.SetSkills((UnturnedPlayer)caller, skillSetName);
			bool saved = AutoSkillPlugin.Instance.GetStorage().Save(((UnturnedPlayer)caller).CSteamID, skillSetName);
			UnturnedChat.Say(caller, AutoSkillPlugin.Instance.Translate("SETMAXSKILL_DONE"));
		}

		bool IsPermitted(IRocketPlayer caller, SkillSet skillSet)
		{
			if (caller.IsAdmin) return true;
			if (skillSet != null && PermissionUtils.IsPermitted(caller, skillSet)) return true;
			return false;
		}

    }
}
