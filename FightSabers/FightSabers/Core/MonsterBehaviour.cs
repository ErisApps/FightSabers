using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BS_Utils.Utilities;
using DigitalRuby.Tween;
using FightSabers.Models;
using FightSabers.Settings;
using FightSabers.Utilities;
using HMUI;
using TMPro;
using UnityEngine;

namespace FightSabers.Core
{
    public class MonsterBehaviour : MonoBehaviour
    {
        #region Constants
        private static          Vector3 BasePosition = new Vector3(0, 3f, 3.75f);
        private static readonly Vector3 BaseRotation = new Vector3(0, 0, 0);
        private static          Vector3 BaseScale    = new Vector3(0.01f, 0.01f, 0.01f);

        private static readonly Vector2 BaseCanvasSize = new Vector2(140, 50);

        private static readonly float   NoteCountFontSize     = 8f;
        private static readonly Vector2 NoteCountNamePosition = new Vector2(50, 28);

        private static readonly Vector2 MonsterLabelPosition = new Vector2(20, 15);
        private static readonly Vector2 MonsterLabelSize     = new Vector2(140, 20);
        private static readonly float   MonsterLabelFontSize = 13f;

        private static readonly Vector2 MonsterHpLabelPosition = new Vector2(85, 0);
        private static readonly Vector2 MonsterHpLabelSize     = new Vector2(35, 20);
        private static readonly float   MonsterHpLabelFontSize = 11f;

        private static readonly Vector2 MonsterLifeBarSize    = new Vector2(100, 10);
        private static readonly Color   MonsterLifeBarBgColor = new Color(0, 0, 0, 0.2f);
        #endregion

        #region Visuals
        private  Canvas   _canvas;
        private  TMP_Text _noteCountText;
        private  TMP_Text _monsterLabel;
        private  TMP_Text _monsterHpLabel;
        internal Image    monsterLifeBarBg;
        internal Image    monsterLifeBar;
        #endregion

        #region Properties
        private string _monsterName;

        public string MonsterName {
            get { return _monsterName; }
            private set {
                _monsterName = value;
                if (_monsterLabel)
                    _monsterLabel.text = _monsterName + " lv." + _monsterDifficulty;
            }
        }

        private int _noteCountLeft;

        public int NoteCountLeft {
            get { return _noteCountLeft; }
            private set {
                _noteCountLeft = value;
                if (_noteCountText)
                    _noteCountText.text = _noteCountLeft + " notes left";
            }
        }

        public int maxHealth;

        private int _currentHealth;

        public int CurrentHealth {
            get { return _currentHealth; }
            private set {
                gameObject.Tween("CurrentHealth" + gameObject.GetInstanceID(), _currentHealth, value,
                                 0.35f, TweenScaleFunctions.Linear, tween => {
                                     if (!this) return;
                                     if (monsterLifeBar)
                                         monsterLifeBar.fillAmount = tween.CurrentValue / maxHealth;
                                     if (_monsterHpLabel)
                                         _monsterHpLabel.text = (int)tween.CurrentValue + " HP";
                                 });
                _currentHealth = value;
            }
        }

        private int _monsterDifficulty;

        public int MonsterDifficulty {
            get { return _monsterDifficulty; }
            private set {
                _monsterDifficulty = value;
                if (_monsterLabel)
                    _monsterLabel.text = _monsterName + " lv." + _monsterDifficulty;
            }
        }

        public MonsterStatus CurrentStatus { get; private set; } = MonsterStatus.Alive;

        private bool _is360Level;
        #endregion

        private ScoreController _scoreController;
        private FloatingText    _floatingText;

        public static MonsterBehaviour Create()
        {
            return new GameObject("[FS|Monster]").AddComponent<MonsterBehaviour>();
        }

        #region Events
        private void OnNoteWasCut(NoteData noteData, NoteCutInfo noteCutInfo, int multiplier)
        {
            if (noteData.noteType == NoteType.Bomb)
                return;

            if (!noteCutInfo.allIsOK)
                OnNoteWasMissed(noteData, 0);
            else
            {
                var acsbList = _scoreController.GetPrivateField<List<CutScoreBuffer>>("_cutScoreBuffers");

                foreach (CutScoreBuffer csb in acsbList)
                {
                    if (csb.GetPrivateField<NoteCutInfo>("_noteCutInfo") == noteCutInfo)
                    {
                        csb.didFinishEvent += OnNoteWasFullyCut;
                        break;
                    }
                }
            }
        }

        private void OnNoteWasFullyCut(CutScoreBuffer csb)
        {
            var noteCutInfo = csb.GetPrivateField<NoteCutInfo>("_noteCutInfo");

            if (csb != null)
                csb.didFinishEvent -= OnNoteWasFullyCut;

            ScoreController.RawScoreWithoutMultiplier(noteCutInfo, out var score, out var afterScore, out var cutDistanceScore);
            Hurt(score + afterScore + cutDistanceScore);
            NotePassed();
        }

        private void OnNoteWasMissed(NoteData noteData, int score)
        {
            if (noteData.noteType == NoteType.Bomb)
                return;

            NotePassed();
        }
        #endregion

        private IEnumerator ConfigureEvents()
        {
            while (true)
            {
                _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();

                if (_scoreController == null || _scoreController == default(ScoreController))
                    yield return new WaitForSeconds(0.1f);
                else
                {
                    _scoreController.noteWasCutEvent += OnNoteWasCut;
                    _scoreController.noteWasMissedEvent += OnNoteWasMissed;
                    enabled = true;
                    break;
                }
            }
        }

        private void Start()
        {
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

            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            var rectTransform = _canvas.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = BaseCanvasSize;
            }
            _noteCountText = Utils.CreateText((RectTransform)_canvas.transform, NoteCountLeft + " notes left", NoteCountNamePosition);
            rectTransform = _noteCountText.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                rectTransform.sizeDelta = MonsterLabelSize;
                rectTransform.anchoredPosition = NoteCountNamePosition;
                _noteCountText.fontSize = NoteCountFontSize;
            }
            _monsterLabel = Utils.CreateText(_canvas.transform as RectTransform, MonsterName, MonsterLabelPosition);
            rectTransform = _monsterLabel.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                rectTransform.anchoredPosition = MonsterLabelPosition;
                rectTransform.sizeDelta = MonsterLabelSize;
                _monsterLabel.fontSize = MonsterLabelFontSize;
            }
            _monsterHpLabel = Utils.CreateText(_canvas.transform as RectTransform, "0 HP", MonsterHpLabelPosition);
            rectTransform = _monsterHpLabel.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                rectTransform.anchoredPosition = MonsterHpLabelPosition;
                rectTransform.sizeDelta = MonsterHpLabelSize;
                _monsterHpLabel.fontSize = MonsterHpLabelFontSize;
            }
            monsterLifeBarBg = new GameObject("Background").AddComponent<Image>();
            rectTransform = monsterLifeBarBg.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                rectTransform.sizeDelta = MonsterLifeBarSize;
                monsterLifeBarBg.color = MonsterLifeBarBgColor;
                monsterLifeBar = new GameObject("Loading Bar").AddComponent<Image>();
            }
            rectTransform = monsterLifeBar.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                rectTransform.sizeDelta = MonsterLifeBarSize;
            }
            var tex = Texture2D.whiteTexture;
            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, 100, 1);
            monsterLifeBar.sprite = sprite;
            monsterLifeBar.type = Image.Type.Filled;
            monsterLifeBar.fillMethod = Image.FillMethod.Horizontal;
            monsterLifeBar.color = new Color(1, 0, 0, 0.5f);
        }

        public IEnumerator ConfigureMonster(string monsterName, uint noteCountLeft, uint maxHp, uint monsterDifficulty)
        {
            yield return new WaitForEndOfFrame();
            MonsterName = monsterName;
            name = "[FS|" + monsterName + "]";
            NoteCountLeft = (int)noteCountLeft;
            maxHealth = (int)maxHp;
            CurrentHealth = (int)maxHp;
            MonsterDifficulty = (int)monsterDifficulty;
            new UnityTask(ConfigureEvents());
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }

        public void Hurt(int rawScore)
        {
            CurrentHealth -= rawScore;
            if (IsAlive()) return;
            CurrentStatus = MonsterStatus.Killed;
            DisplayMonsterInformationEnd("You killed that!");
        }

        public void NotePassed()
        {
            if (!IsAlive()) return;
            //Hurt(Random.Range(10, 26)); //TODO: Remove later, FPFC testing
            NoteCountLeft -= 1;
            if (NoteCountLeft > 0) return;
            CurrentStatus = MonsterStatus.Flown;
            DisplayMonsterInformationEnd("He ran away..");
        }

        private void DisplayMonsterInformationEnd(string labelInfo)
        {
            if (_floatingText != null) return;
            _canvas.enabled = false;
            _floatingText = FloatingText.Create();
            _floatingText.fadeOutText = true;
            _floatingText.tweenScaleFunc = TweenScaleFunctions.QuadraticEaseOut;
            _floatingText.ConfigureText();
            if (_is360Level)
                _floatingText.transform.SetParent(transform.parent);
            _floatingText.tweenEndPosition = new Vector3(BasePosition.x, BasePosition.y + 0.75f * (_is360Level ? 60f : 1f), BasePosition.z);
            _floatingText.DisplayText(BasePosition, BaseRotation, BaseScale, labelInfo, 3.5f);
            if (_scoreController)
            {
                _scoreController.noteWasCutEvent -= OnNoteWasCut;
                _scoreController.noteWasMissedEvent -= OnNoteWasMissed;
            }
            switch (CurrentStatus)
            {
                case MonsterStatus.Killed:
                    SaveDataManager.instance.SaveData.killMonsterCount += 1;
                    ExperienceSystem.instance.AddFightExperience(9 + (uint)_monsterDifficulty);
                    break;
                case MonsterStatus.Flown:
                    SaveDataManager.instance.SaveData.flownMonsterCount += 1;
                    //ExperienceSystem.instance.AddFightExperience(9 + (uint)_monsterDifficulty); //TODO: Remove later, FPFC testing
                    break;
            }
            Destroy(gameObject, 4);
        }
    }
}