using System.Collections;
using System.Linq;
using BeatSaberMarkupLanguage;
using DigitalRuby.Tween;
using FightSabers.Models;
using FightSabers.Settings;
using FightSabers.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FightSabers.Core
{
	public class MonsterBehaviour : MonoBehaviour
	{
		private static Vector3 BasePosition = new(0, 3f, 3.75f);
		private static readonly Vector3 BaseRotation = new(0, 0, 0);
		private static Vector3 BaseScale = new(0.01f, 0.01f, 0.01f);

		private static readonly Vector2 BaseCanvasSize = new(140, 50);

		private static readonly float NoteCountFontSize = 8f;
		private static readonly Vector2 NoteCountNamePosition = new(50, 28);

		private static readonly Vector2 MonsterLabelPosition = new(20, 15);
		private static readonly Vector2 MonsterLabelSize = new(140, 20);
		private static readonly float MonsterLabelFontSize = 13f;

		private static readonly Vector2 MonsterHpLabelPosition = new(85, 0);
		private static readonly Vector2 MonsterHpLabelSize = new(35, 20);
		private static readonly float MonsterHpLabelFontSize = 11f;

		private static readonly Vector2 MonsterLifeBarSize = new(100, 10);
		private static readonly Color MonsterLifeBarBgColor = new(0, 0, 0, 0.2f);

		public Canvas Canvas { get; private set; }
		public TMP_Text NoteCountText { get; private set; }
		public TMP_Text MonsterLabel { get; private set; }
		public TMP_Text MonsterHpLabel { get; private set; }
		public Image MonsterLifeBarBg { get; private set; }
		public Image MonsterLifeBar { get; private set; }

		private string _monsterName;

		public string MonsterName
		{
			get => _monsterName;
			private set
			{
				_monsterName = value;
				if (MonsterLabel)
				{
					MonsterLabel.text = _monsterName + " lv." + _monsterDifficulty;
				}
			}
		}

		private int _noteCountLeft;

		public int NoteCountLeft
		{
			get => _noteCountLeft;
			private set
			{
				_noteCountLeft = value;
				if (NoteCountText)
				{
					NoteCountText.text = _noteCountLeft + " notes left";
				}
			}
		}

		public int maxHealth;

		private int _currentHealth;

		public int CurrentHealth
		{
			get => _currentHealth;
			private set
			{
				gameObject.Tween("CurrentHealth" + gameObject.GetInstanceID(), _currentHealth, value,
					0.35f, TweenScaleFunctions.Linear, tween =>
					{
						if (!this)
						{
							return;
						}

						if (MonsterLifeBar)
						{
							MonsterLifeBar.fillAmount = tween.CurrentValue / maxHealth;
						}

						if (MonsterHpLabel)
						{
							MonsterHpLabel.text = (int) tween.CurrentValue + " HP";
						}
					});
				_currentHealth = value;
			}
		}

		private int _monsterDifficulty;

		public int MonsterDifficulty
		{
			get => _monsterDifficulty;
			private set
			{
				_monsterDifficulty = value;
				if (MonsterLabel)
				{
					MonsterLabel.text = _monsterName + " lv." + _monsterDifficulty;
				}
			}
		}

		public float SpawnTime { get; private set; }
		public float DeSpawnTime { get; private set; }

		public ScoreControllerManager ScoreControllerManager { get; private set; }
		public ModifierManager ModifierManager { get; private set; }

		public MonsterStatus CurrentStatus { get; private set; } = MonsterStatus.Alive;

		private bool _is360Level;

		private FloatingText _floatingText;

		public static MonsterBehaviour Create()
		{
			return new GameObject("[FS|Monster]").AddComponent<MonsterBehaviour>();
		}

		#region Events

		public delegate void MonsterHitHandler(object self, int damage);

		public event MonsterHitHandler? MonsterHurt;

		private void OnMonsterHurt(int damage)
		{
			MonsterHurt?.Invoke(this, damage);
		}

		#endregion

		private void Start()
		{
			//_gsc = Resources.FindObjectsOfTypeAll<GameSongController>().FirstOrDefault();
			//_gsc.PauseSong();
			_is360Level = BS_Utils.Plugin.LevelData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.beatmapData?.spawnRotationEventsCount > 0;
			ConfigureVisuals();
			enabled = false;
		}

		private void ConfigureVisuals()
		{
			if (_is360Level)
			{
				var flyingGameHud = Resources.FindObjectsOfTypeAll<FlyingGameHUDRotation>().FirstOrDefault(x => x.isActiveAndEnabled);
				if (flyingGameHud)
				{
					var flyingContainer = flyingGameHud.transform.Find("Container");
					transform.SetParent(flyingContainer);
					BasePosition = new Vector3(0f, 60f, 0);
					BaseScale = Vector3.one;
				}
			}
			else
			{
				BasePosition = new Vector3(0, 3f, 3.75f);
				BaseScale = new Vector3(0.01f, 0.01f, 0.01f);
			}

			transform.localPosition = BasePosition;
			transform.localEulerAngles = BaseRotation;
			transform.localScale = BaseScale;

			Canvas = gameObject.AddComponent<Canvas>();
			Canvas.renderMode = RenderMode.WorldSpace;
			var rectTransform = Canvas.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.sizeDelta = BaseCanvasSize;
			}

			NoteCountText = BeatSaberUI.CreateText((RectTransform) Canvas.transform, NoteCountLeft + " notes left", NoteCountNamePosition);
			rectTransform = NoteCountText.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(Canvas.transform, false);
				rectTransform.sizeDelta = MonsterLabelSize;
				rectTransform.anchoredPosition = NoteCountNamePosition;
				NoteCountText.fontSize = NoteCountFontSize;
			}

			MonsterLabel = BeatSaberUI.CreateText(Canvas.transform as RectTransform, MonsterName, MonsterLabelPosition);
			rectTransform = MonsterLabel.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(Canvas.transform, false);
				rectTransform.anchoredPosition = MonsterLabelPosition;
				rectTransform.sizeDelta = MonsterLabelSize;
				MonsterLabel.fontSize = MonsterLabelFontSize;
			}

			MonsterHpLabel = BeatSaberUI.CreateText(Canvas.transform as RectTransform, "0 HP", MonsterHpLabelPosition);
			rectTransform = MonsterHpLabel.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(Canvas.transform, false);
				rectTransform.anchoredPosition = MonsterHpLabelPosition;
				rectTransform.sizeDelta = MonsterHpLabelSize;
				MonsterHpLabel.fontSize = MonsterHpLabelFontSize;
			}

			MonsterLifeBarBg = new GameObject("Background").AddComponent<Image>();
			rectTransform = MonsterLifeBarBg.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(Canvas.transform, false);
				rectTransform.sizeDelta = MonsterLifeBarSize;
				MonsterLifeBarBg.color = MonsterLifeBarBgColor;
				MonsterLifeBar = new GameObject("Loading Bar").AddComponent<Image>();
			}

			rectTransform = MonsterLifeBar.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(Canvas.transform, false);
				rectTransform.sizeDelta = MonsterLifeBarSize;
			}

			var tex = Texture2D.whiteTexture;
			var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, 100, 1);
			MonsterLifeBar.sprite = sprite;
			MonsterLifeBar.type = Image.Type.Filled;
			MonsterLifeBar.fillMethod = Image.FillMethod.Horizontal;
			MonsterLifeBar.color = new Color(1, 0, 0, 0.5f);
		}

		public IEnumerator ConfigureMonster(MonsterGenerator.MonsterSpawnInfo monsterInfo)
		{
			yield return new WaitForEndOfFrame();
			ModifierManager = gameObject.AddComponent<ModifierManager>();
			ModifierManager.modifiers = monsterInfo.ModifierTypes;
			MonsterName = monsterInfo.MonsterName;
			NoteCountLeft = (int) monsterInfo.NoteCount;
			ModifierManager.noteCountDuration = NoteCountLeft;
			maxHealth = (int) monsterInfo.MonsterHp;
			CurrentHealth = (int) monsterInfo.MonsterHp;
			MonsterDifficulty = (int) monsterInfo.MonsterDifficulty;
			SpawnTime = monsterInfo.SpawnTime;
			DeSpawnTime = monsterInfo.DeSpawnTime;
			name = "[FS|" + MonsterName + "lv." + MonsterDifficulty + "]";
			new UnityTask(ModifierManager.ConfigureModifiers());
			ScoreControllerManager = gameObject.AddComponent<ScoreControllerManager>();
			ScoreControllerManager.ScoreControllerInitialized += self => enabled = true;
			ScoreControllerManager.BombCut += self => { ModifierManager.ReduceColorSuckerColorness(); };
			ScoreControllerManager.NoteCut += self => { ModifierManager.ImproveColorSuckerColorness(); };
			ScoreControllerManager.NoteFullyCut += (self, score) =>
			{
				Hurt(score);
				NotePassed();
			};
			ScoreControllerManager.NoteMissed += self =>
			{
				ModifierManager.ReduceColorSuckerColorness();
				NotePassed();
			};
		}

		public bool IsAlive()
		{
			return CurrentHealth > 0;
		}

		public void Hurt(int rawScore)
		{
			CurrentHealth -= rawScore;
			OnMonsterHurt(rawScore);
			if (IsAlive())
			{
				return;
			}

			CurrentStatus = MonsterStatus.Killed;
			DisplayMonsterInformationEnd("You killed that!");
		}

		public void NotePassed()
		{
			if (!IsAlive())
			{
				return;
			}

			//Hurt(Random.Range(10, 26)); //TODO: Remove later, FPFC testing
			NoteCountLeft -= 1;
			if (NoteCountLeft > 0)
			{
				return;
			}

			CurrentStatus = MonsterStatus.Flown;
			DisplayMonsterInformationEnd("He ran away..");
		}

		private void DisplayMonsterInformationEnd(string labelInfo)
		{
			if (_floatingText != null)
			{
				return;
			}

			Canvas.enabled = false;
			_floatingText = FloatingText.Create();
			_floatingText.fadeOutText = true;
			_floatingText.TweenScaleFunc = TweenScaleFunctions.QuadraticEaseOut;
			_floatingText.ConfigureText();
			if (_is360Level)
			{
				_floatingText.transform.SetParent(transform.parent);
			}

			_floatingText.tweenEndPosition = new Vector3(BasePosition.x, BasePosition.y + 0.75f * (_is360Level ? 60f : 1f), BasePosition.z);
			_floatingText.DisplayText(BasePosition, BaseRotation, BaseScale, labelInfo, 3.5f);
			switch (CurrentStatus)
			{
				case MonsterStatus.Killed:
					SaveDataManager.instance.SaveData.killMonsterCount += 1;
					if (SaveDataManager.instance.SaveData.killMonsterCount % 4 == 0)
					{
						QuestManager.instance.AddNewPickableQuest();
					}

					ExperienceSystem.instance.AddFightExperience(9 + (uint) _monsterDifficulty);
					break;
				case MonsterStatus.Flown:
					SaveDataManager.instance.SaveData.flownMonsterCount += 1;
					//ExperienceSystem.instance.AddFightExperience(9 + (uint)_monsterDifficulty); //TODO: Remove later, FPFC testing
					break;
			}

			ModifierManager.timeWarper?.DisableModifier();
			Destroy(ScoreControllerManager);
			MonsterGenerator.instance.EndCurrentMonsterEncounter();
		}
	}
}