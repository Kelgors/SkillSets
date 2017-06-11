using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Rocket.Core.Logging;
using Steamworks;

namespace AutoSkill.Storage
{
    class FileStorage : IAutoSkillStorage
    {
		internal List<FileStorageEntry> Entries;
		internal bool isValid;

		string FilePath => Path.Combine(AutoSkillPlugin.Instance.Directory, AutoSkillPlugin.Instance.Configuration.Instance.FilePath);

        public string Get(CSteamID steamId)
        {
			FileStorageEntry entry = Entries.Find((_entry) => _entry.SteamId == steamId.m_SteamID);
			if (entry != null) return entry.SkillSetName;
			return null;
        }

        public void Load()
        {
			if (FilePath == null || FilePath.Trim().Length == 0)
			{
				throw new Exception(AutoSkillPlugin.WrapLog("ConfigurationError : <FilePath> is not defined or empty"));
			}
            EnsureDirectoryIsCreated();

            Logger.LogWarning(AutoSkillPlugin.WrapLog(string.Format("FileStorage: Load {0}", FilePath)));
			Entries = ReadFile();
			isValid = File.Exists(FilePath);
        }

		public void Unload()
        {
            Logger.LogError(AutoSkillPlugin.WrapLog("FileStorage: Unloading"));
            SaveList();
            Entries = null;
        }

		public void PeriodicSave()
		{
			if (!isValid)
			{
				SaveList();
			}
		}

		/// <summary>
		/// Save the specified steamId, skillSetName and enabled.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="steamId">Steam identifier.</param>
		/// <param name="skillSetName">Skill set name.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
        public bool Save(CSteamID steamId, string skillSetName, bool enabled)
        {
			int index = Entries.FindIndex((_entry) => _entry.SteamId == steamId.m_SteamID);
			if (enabled)
			{
				if (index == -1)
				{
					Entries.Add(new FileStorageEntry(steamId.m_SteamID, skillSetName));
				}
				else
				{
					Entries[index].SkillSetName = skillSetName;
				}
				isValid = false;
				return true;
			} 
			else if (index > -1)
            {
                Entries.RemoveAt(index);
				isValid = false;
				return true;
            }
			return false;
        }

	    internal void EnsureDirectoryIsCreated()
        {
			string dirname = Path.GetDirectoryName(FilePath);
			if (!Directory.Exists(dirname))
			{
				Directory.CreateDirectory(dirname);
			}
        }

        internal void SaveList()
        {
			XmlWriter writer = null;
			try
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.IndentChars = ("  ");
				settings.NewLineChars = System.Environment.NewLine;
				settings.NewLineHandling = NewLineHandling.Replace;
				settings.OmitXmlDeclaration = false;
				writer = XmlWriter.Create(FilePath, settings);
				new XmlSerializer(typeof(List<FileStorageEntry>)).Serialize(writer, Entries);
				isValid = true;
			}
			catch (Exception ex)
			{
				Logger.LogError(AutoSkillPlugin.WrapLog("FileStorage: Cannot save list into filesystem"));
				Logger.LogException(ex);
			}
			finally 
			{
				if (writer != null) writer.Close();
			}
        }

		

		internal List<FileStorageEntry> ReadFile() 
		{
			List<FileStorageEntry> entries = new List<FileStorageEntry>();
			object output = null;
			XmlReader reader = null;
			if (!File.Exists(FilePath)) return entries;
			try
			{
				reader = XmlReader.Create(FilePath);
				reader.Settings.IgnoreComments = true;
				output = new XmlSerializer(typeof(List<FileStorageEntry>)).Deserialize(reader);
			}
			catch (Exception ex)
			{
				Logger.LogException(ex);
			}
			finally
			{
				if (reader != null) reader.Close();
			}

			if (output is List<FileStorageEntry>)
			{
				entries = (List<FileStorageEntry>) output;
			}
			return entries;
		}
	}
}
