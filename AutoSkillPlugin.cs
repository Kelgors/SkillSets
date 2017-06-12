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

		internal IAutoSkillStorage Storage;
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
			LoadSkillSets();
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

		internal void LoadSkillSets() {
			SkillSets = Configuration.Instance.SkillSets.Select((confSkillSet) => SkillSet.FromConfigurationSkillSet(confSkillSet)).ToList();

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

		internal void ApplySkillSetToPlayer(UnturnedPlayer player)
		{
			string skillSetName = GetStorage().Get(player.CSteamID);
			SkillSet skillset = SkillsUtils.FindSkillSetByName(skillSetName);
			if (skillset != null && !PermissionUtils.IsPermitted(player, skillset))
			{
				// The SkillSet disappears or player doesn't have Permission anymore
				// Need to remove it from Storage
				GetStorage().Remove(player.CSteamID);
				skillset = null;
			}

			if (skillset == null)
			{
				skillset = SkillsUtils.GetHigherSkillSet(SkillsUtils.GetDefaultPermittedSkillSets(player));
			}
			if (skillset != null)
			{
				SkillsUtils.SetSkills(player, skillset);
			}
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
					{ "INVALID_USAGE", "Invalid usage" }
				};
			}
		}
		#endregion

		#region Events
		public void UnturnedPlayerEvents_OnPlayerRevive(UnturnedPlayer player, UnityEngine.Vector3 position, byte angle)
		{
			ApplySkillSetToPlayer(player);
		}

		public void U_Events_OnPlayerConnected(UnturnedPlayer player)
		{
            ApplySkillSetToPlayer(player);
		}
		#endregion

	}
}
