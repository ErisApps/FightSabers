using System;
using System.IO;
using FightSabers.Models;
using FightSabers.Models.Converters;
using Newtonsoft.Json;

namespace FightSabers.Settings
{
    public class SaveDataManager : PersistentSingleton<SaveDataManager>
    {
        #region Properties

        public ProfileSaveData SaveData { get; private set; }

        private readonly string _pathConfigFolder;
        private readonly string _pathSaveFile;

        #endregion

        #region Methods

        private SaveDataManager()
        {
            _pathConfigFolder = Path.Combine(Environment.CurrentDirectory, "UserData");
            _pathSaveFile = Path.Combine(_pathConfigFolder, "FightSabersSaveData.fs");
        }

        private void InitializeSaveData(string pathSaveFile)
        {
            SaveData = new ProfileSaveData();
            var jsonData = JsonConvert.SerializeObject(SaveData);
            if (!string.IsNullOrEmpty(jsonData))
                File.WriteAllText(pathSaveFile, jsonData);
        }

        public void Setup()
        {
            if (!Directory.Exists(_pathConfigFolder))
                Directory.CreateDirectory(_pathConfigFolder);
            if (!File.Exists(_pathSaveFile))
                InitializeSaveData(_pathSaveFile);
            else
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new QuestConverter());
                var content = File.ReadAllText(_pathSaveFile);
                SaveData = JsonConvert.DeserializeObject<ProfileSaveData>(content, settings);
                if (SaveData != null) return;
                var newFileName = "FightSabersSaveData" + DateTime.Now.ToString("yy-MM-dd-hh-mm-ss") + ".fs";
                File.Move(_pathSaveFile, Path.Combine(_pathConfigFolder, newFileName));
                InitializeSaveData(_pathSaveFile);
                Logger.log.Warn($"Looks like your FightSabers save data is corrupted. Your data file has been renamed into '{newFileName}' and a new working one has been created!");
            }
        }

        public void ApplyToFile()
        {
            if (SaveData == null) return;
            var jsonData = JsonConvert.SerializeObject(SaveData, Formatting.Indented);
            if (!string.IsNullOrEmpty(jsonData))
                File.WriteAllText(_pathSaveFile, jsonData);
        }

        #endregion
    }
}
