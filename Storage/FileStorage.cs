using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;
using Rocket.Core.Logging;
using System.Xml;
using System.Xml.Serialization;

namespace AutoSkill.Storage
{
    class FileStorage : IAutoSkillStorage
    {
		List<FileStorageEntry> Entries;

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

            Logger.LogWarning(AutoSkillPlugin.WrapLog(string.Format("Load File {0}", FilePath)));
			Entries = ReadFile();
        }

		List<FileStorageEntry> ReadFile() 
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

        public bool Save(CSteamID steamId, string skillSetName, bool enabled)
        {
			int index = Entries.FindIndex((_entry) => _entry.SteamId == steamId.m_SteamID);
			if (enabled && index == -1)
			{
				Entries.Add(new FileStorageEntry(steamId.m_SteamID, skillSetName));
				SaveList();
				return true;
			} 
			else if (enabled && index > -1) 
			{
				Entries[index].SkillSetName = skillSetName;
				SaveList();
				return true;
            } 
			else if (!enabled && index > -1)
            {
                Entries.RemoveAt(index);
                SaveList();
				return true;
            }
			return false;
        }

	    void EnsureDirectoryIsCreated()
        {
			string dirname = Path.GetDirectoryName(FilePath);
			if (!Directory.Exists(dirname))
			{
				Directory.CreateDirectory(dirname);
			}
        }

        void SaveList()
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

			}
			catch (Exception ex)
			{
				Logger.LogError(AutoSkillPlugin.WrapLog("Cannot save list into filesystem"));
				Logger.LogException(ex);
			}
			finally 
			{
				if (writer != null) writer.Close();
			}
        }

        public void Unload()
        {
            Logger.LogError(AutoSkillPlugin.WrapLog("Unloading file"));
            SaveList();
            Entries = null;
        }
    }
}
