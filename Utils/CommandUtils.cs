using Rocket.API;
using Rocket.Unturned.Chat;

namespace AutoSkill.Utils
{
    class CommandUtils
    {
		/// <summary>
		/// Say to player he cannot execute the command
		/// </summary>
		/// <param name="caller">The player who try to execute a command</param>
        public static void PermissionMissing(IRocketPlayer caller)
        {
            UnturnedChat.Say(caller, AutoSkillPlugin.Instance.Translate("PERMISSION_MISSING"));
        }

		/// <summary>
		/// Say to player the command is unknown
		/// </summary>
		/// <param name="caller">The player who try to execute a command.</param>
		/// <param name="command">The command he tried to execute</param>
        public static void UnknownCommand(IRocketPlayer caller, string command)
        {
            UnturnedChat.Say(caller, AutoSkillPlugin.Instance.Translate("UNKNOWN_COMMAND", new string[] { command }));
        }
    }
}
