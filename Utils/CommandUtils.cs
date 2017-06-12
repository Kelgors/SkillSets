using Rocket.API;
using Rocket.Unturned.Chat;

namespace SkillSets.Utils
{
    public static class CommandUtils
    {
		public static UnityEngine.Color ErrorColor = new UnityEngine.Color(0xC0, 0x2C, 0x36);


		public static void InvalidUsage(IRocketPlayer caller)
		{
			UnturnedChat.Say(caller, SkillSetsPlugin.Instance.Translate("INVALID_USAGE"), ErrorColor);
		}
		/// <summary>
		/// Say to player he cannot execute the command
		/// </summary>
		/// <param name="caller">The player who try to execute a command</param>
        public static void PermissionMissing(IRocketPlayer caller)
        {
            UnturnedChat.Say(caller, SkillSetsPlugin.Instance.Translate("PERMISSION_MISSING"), ErrorColor);
        }

		/// <summary>
		/// Say to player the command is unknown
		/// </summary>
		/// <param name="caller">The player who try to execute a command.</param>
		/// <param name="command">The command he tried to execute</param>
        public static void UnknownCommand(IRocketPlayer caller, string command)
        {
            UnturnedChat.Say(caller, SkillSetsPlugin.Instance.Translate("UNKNOWN_COMMAND", new string[] { command }), ErrorColor);
        }
    }
}
