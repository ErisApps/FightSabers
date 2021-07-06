using System;
using System.IO;
using FightSabers.Models;
using FightSabers.Models.Converters;
using IPA.Loader;
using IPA.Utilities;
using Newtonsoft.Json;
using SiraUtil.Tools;
using Zenject;

namespace FightSabers.Settings
{
	internal class SaveDataManager : IDisposable
	{
		private const string SAVE_DATA_FILE_NAME = "FightSabersSaveData";
		private const string SAVE_DATA_EXTENSION = ".fs";

		private readonly string _saveFilePath;

		internal ProfileSaveData SaveData { get; }

		public SaveDataManager(SiraLog logger, [Inject(Id = Constants.BindingIds.METADATA)] PluginMetadata metadata)
		{
			var saveDirectoryPath = Path.Combine(UnityGame.UserDataPath, metadata.Id);
			_saveFilePath = Path.Combine(saveDirectoryPath, SAVE_DATA_FILE_NAME + SAVE_DATA_EXTENSION);

			if (!Directory.Exists(saveDirectoryPath))
			{
				Directory.CreateDirectory(saveDirectoryPath);
			}

			if (File.Exists(_saveFilePath))
			{
				var settings = new JsonSerializerSettings
				{
					Converters = {new QuestConverter()}
				};

				var content = File.ReadAllText(_saveFilePath);
				var saveData = JsonConvert.DeserializeObject<ProfileSaveData>(content, settings);
				if (saveData != null)
				{
					SaveData = saveData;
					return;
				}

				var newFileName = SAVE_DATA_FILE_NAME + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + SAVE_DATA_EXTENSION;
				File.Move(_saveFilePath, Path.Combine(saveDirectoryPath, newFileName));

				logger.Warning($"Looks like your FightSabers save data is corrupted. Your data file has been renamed into '{newFileName}' and a new working one has been created!");
			}

			SaveData = new ProfileSaveData();
			var jsonData = JsonConvert.SerializeObject(SaveData);
			if (!string.IsNullOrEmpty(jsonData))
			{
				File.WriteAllText(_saveFilePath, jsonData);
			}
		}

		public void Dispose()
		{
			ApplyToFile();
		}

		public void ApplyToFile()
		{
			var jsonData = JsonConvert.SerializeObject(SaveData);
			if (!string.IsNullOrEmpty(jsonData))
			{
				File.WriteAllText(_saveFilePath, jsonData);
			}
		}
	}
}