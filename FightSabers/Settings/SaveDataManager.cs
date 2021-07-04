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
	internal class SaveDataManager : IInitializable, IDisposable
	{
		private const string SAVE_DATA_FILE_NAME = "FightSabersSaveData";
		private const string SAVE_DATA_EXTENSION = ".fs";

		private readonly SiraLog _logger;

		private readonly string _saveDirectoryPath;
		private readonly string _saveFilePath;
		public ProfileSaveData? SaveData { get; private set; }

		private SaveDataManager(SiraLog logger, [Inject(Id = Constants.BindingIds.METADATA)] PluginMetadata metadata)
		{
			_logger = logger;

			_saveDirectoryPath = Path.Combine(UnityGame.UserDataPath, metadata.Id);
			_saveFilePath = Path.Combine(_saveDirectoryPath, SAVE_DATA_FILE_NAME + SAVE_DATA_EXTENSION);
		}

		public void Initialize()
		{
			if (!Directory.Exists(_saveDirectoryPath))
			{
				Directory.CreateDirectory(_saveDirectoryPath);
			}

			void InitializeSaveData(string pathSaveFile)
			{
				SaveData = new ProfileSaveData();
				var jsonData = JsonConvert.SerializeObject(SaveData);
				if (!string.IsNullOrEmpty(jsonData))
				{
					File.WriteAllText(pathSaveFile, jsonData);
				}
			}

			if (!File.Exists(_saveFilePath))
			{
				InitializeSaveData(_saveFilePath);
			}
			else
			{
				var settings = new JsonSerializerSettings();
				settings.Converters.Add(new QuestConverter());
				var content = File.ReadAllText(_saveFilePath);
				SaveData = JsonConvert.DeserializeObject<ProfileSaveData>(content, settings);
				if (SaveData != null)
				{
					return;
				}

				var newFileName = SAVE_DATA_FILE_NAME + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + SAVE_DATA_EXTENSION;
				File.Move(_saveFilePath, Path.Combine(_saveDirectoryPath, newFileName));
				InitializeSaveData(_saveFilePath);
				_logger.Warning($"Looks like your FightSabers save data is corrupted. Your data file has been renamed into '{newFileName}' and a new working one has been created!");
			}
		}

		public void Dispose()
		{
			ApplyToFile();
		}

		public void ApplyToFile()
		{
			if (SaveData == null)
			{
				return;
			}

			var jsonData = JsonConvert.SerializeObject(SaveData, Formatting.Indented);
			if (!string.IsNullOrEmpty(jsonData))
			{
				File.WriteAllText(_saveFilePath, jsonData);
			}
		}
	}
}