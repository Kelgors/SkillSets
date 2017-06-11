using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using AutoSkill.Storage;
using AutoSkill.Utils;

namespace AutoSkill
{
	public class AutoSkillPlugin : RocketPlugin<AutoSkillConfiguration>
	{

		public static string WrapLog(string message)
		{
			return string.Format("[AutoSkill] {0}", message);
		}

		public static AutoSkillPlugin Instance;
		public List<SkillSet> SkillSets;
		public SkillSet DefaultSkillSet;
		IAutoSkillStorage Storage;
		internal DateTime LastPeriodicSave;
		internal bool PeriodicSaveEnabled = false;
		internal uint PeriodicSaveMs;

		public IAutoSkillStorage GetStorage()
		{
			return Storage;
		}

		protected override void Load()
		{
			Instance = this;

			LoadPeriodicSave();
			ReflectConfigurationSkills();
			Storage = CreateStorage();
			Storage.Load();

			U.Events.OnPlayerConnected += U_Events_OnPlayerConnected;
			UnturnedPlayerEvents.OnPlayerRevive += UnturnedPlayerEvents_OnPlayerRevive;
		}

		internal void LoadPeriodicSave()
		{
			PeriodicSaveEnabled = Configuration.Instance.PeriodicSave > 0;
			LastPeriodicSave = DateTime.UtcNow;
			if (PeriodicSaveEnabled)
			{
				PeriodicSaveMs = Configuration.Instance.PeriodicSave;
				if (Configuration.Instance.PeriodicSave < 5000)
				{
					Logger.LogWarning(WrapLog("WARNING: <PeriodicSave> is set under 5000ms. PeriodicSave will be disabled."));
					PeriodicSaveEnabled = false;
				}
				else
				{
					Logger.LogWarning(WrapLog(string.Format("PeriodicSave enabled : {0}ms", PeriodicSaveMs)));
				}
			}
			else
			{
				Logger.LogWarning(WrapLog("PeriodicSave disabled"));
			}
		}

		public void FixedUpdate()
		{
			if (!PeriodicSaveEnabled || State != PluginState.Loaded) return;
			CheckPeriodicSave(DateTime.UtcNow);
		}

		internal void CheckPeriodicSave(DateTime now) 
		{
			if ((now - LastPeriodicSave).TotalMilliseconds < Configuration.Instance.PeriodicSave) return;
			try
			{
				GetStorage().PeriodicSave();
			}
			catch (Exception ex)
			{
				Logger.LogError(WrapLog("An error occured during PeriodicSave. Please check the value in <PeriodicSave> is in milliseconds and the path/permissions to the file is correct"));
				Logger.LogException(ex);
			}
			finally
			{
				LastPeriodicSave = now;
			}
		}

		internal void ReflectConfigurationSkills() {
			SkillSets = Configuration.Instance.SkillSets.Select((confSkillSet) => SkillSet.FromConfigurationSkillSet(confSkillSet)).ToList();
			DefaultSkillSet = SkillSets.Find((skillSet) => skillSet.IsDefault);
		}

		/// <summary>
		/// Creates the storage corresponding to the connfiguration
		/// </summary>
		/// <returns>The storage instance</returns>
		private IAutoSkillStorage CreateStorage()
		{
			switch (Configuration.Instance.StorageType)
			{
				case "memory":
					return new MemoryStorage();
				case "file":
					return new FileStorage();
			}
			throw new Exception(WrapLog(string.Format("Unknown <StorageType> \"{0}\"", Configuration.Instance.StorageType)));
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
					{ "SETMAXSKILL_DONE", "Skillset applied" },
					{ "SETAUTOSKILL_ON", "Your skills will be set even if you die (I hope not)" },
					{ "SETAUTOSKILL_OFF", "Your skills will no longer be set after death automatically" },
					{ "INVALID_USAGE", "Invalid usage" }
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
