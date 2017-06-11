using System.Collections.Generic;
using System.Linq;
using Rocket.API.Collections;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using AutoSkill.Storage;
using AutoSkill.Utils;

namespace AutoSkill
{
	public class AutoSkillPlugin : Rocket.Core.Plugins.RocketPlugin<AutoSkillConfiguration>
	{

		public static string WrapLog(string message)
		{
			return string.Format("[AutoSkill] {0}", message);
		}

		public static AutoSkillPlugin Instance;
		public List<SkillSet> SkillSets;
		public SkillSet DefaultSkillSet;
		IAutoSkillStorage Storage;

		public IAutoSkillStorage GetStorage()
		{
			return Storage;
		}

		protected override void Load()
		{
			Instance = this;
			ReflectConfigurationSkills();
			Storage = CreateStorage();
			Storage.Load();
			U.Events.OnPlayerConnected += U_Events_OnPlayerConnected;
			UnturnedPlayerEvents.OnPlayerRevive += UnturnedPlayerEvents_OnPlayerRevive;
		}

		internal void ReflectConfigurationSkills() {
			SkillSets = Instance.Configuration.Instance.SkillSets.Select((confSkillSet) => SkillSet.FromConfigurationSkillSet(confSkillSet)).ToList();
			DefaultSkillSet = SkillSets.Find((skillSet) => skillSet.IsDefault);
		}

		/// <summary>
		/// Creates the storage corresponding to the connfiguration
		/// </summary>
		/// <returns>The storage instance</returns>
		private IAutoSkillStorage CreateStorage()
		{
			switch (Instance.Configuration.Instance.StorageType)
			{
				case "memory":
					return new MemoryStorage();
				case "file":
					return new FileStorage();
			}
			throw new System.Exception(WrapLog(string.Format("Unknown StorageType \"{0}\"", Instance.Configuration.Instance.StorageType)));
		}

		protected override void Unload()
		{
			UnturnedPlayerEvents.OnPlayerRevive -= UnturnedPlayerEvents_OnPlayerRevive;
			U.Events.OnPlayerConnected -= U_Events_OnPlayerConnected;
			if (Storage != null) Storage.Unload();
			Storage = null;
			SkillSets = null;
			Instance = null;
		}

		#region Translation
		public override TranslationList DefaultTranslations
		{
			get
			{
				return new TranslationList()
				{
					{ "PERMISSION_MISSING", "You do not have permissions to execute this command." },
					{ "UNKNOWN_COMMAND", "Unknown command \"{0}\"" },
					{ "SETMAXSKILL_DONE", "Skills set to maximum" },
					{ "SETAUTOSKILL_ON", "Your skills will be maxed even if you die (I hope not)" },
					{ "SETAUTOSKILL_OFF", "Your skills will no longer be maxed after death automatically" }
				};
			}
		}
		#endregion

		#region Events
		public void UnturnedPlayerEvents_OnPlayerRevive(UnturnedPlayer player, UnityEngine.Vector3 position, byte angle)
		{
			string skillSetName = GetStorage().Get(player.CSteamID);
			if (skillSetName != null)
			{
				SkillsUtils.SetSkills(player, skillSetName);
			}
			else if (DefaultSkillSet != null)
			{
				SkillsUtils.SetSkills(player, DefaultSkillSet);
			}
		}

		public void U_Events_OnPlayerConnected(UnturnedPlayer player)
		{
			string skillSetName = GetStorage().Get(player.CSteamID);
			if (skillSetName != null)
			{
				SkillsUtils.SetSkills(player, skillSetName);
			}
			else if (DefaultSkillSet != null)
			{
				SkillsUtils.SetSkills(player, DefaultSkillSet);
			}
		}
		#endregion

	}
}
