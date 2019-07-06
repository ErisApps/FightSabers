using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS_Utils.Gameplay;
using BS_Utils.Utilities;
using DigitalRuby.Tween;
using FightSabers.Utilities;
using HMUI;
using TMPro;
using UnityEngine;

namespace FightSabers
{
    public class MonsterBehaviour : MonoBehaviour
    {
        #region Constants
        private static readonly Vector3 BasePosition = new Vector3(0, 3f, 3.75f);
        private static readonly Vector3 BaseRotation = new Vector3(0, 0, 0);
        private static readonly Vector3 BaseScale    = new Vector3(0.01f, 0.01f, 0.01f);

        private static readonly Vector2 BaseCanvasSize = new Vector2(100, 50);

        private const           float   NoteCountFontSize     = 8f;
        private static readonly Vector2 NoteCountNamePosition = new Vector2(35, 28);

        private static readonly Vector2 MonsterLabelPosition = new Vector2(0, 15);
        private static readonly Vector2 MonsterLabelSize     = new Vector2(100, 20);
        private const           float   MonsterLabelFontSize = 13f;

        private static readonly Vector2 MonsterLifeBarSize    = new Vector2(100, 10);
        private static readonly Color   MonsterLifeBarBgColor = new Color(0, 0, 0, 0.2f);
        #endregion

        #region Visuals
        private  Canvas   _canvas;
        private  TMP_Text _noteCountText;
        private  TMP_Text _monsterLabel;
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
                    _monsterLabel.text = _monsterName;
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
                _currentHealth = value;
                if (monsterLifeBar)
                    monsterLifeBar.fillAmount = (float)_currentHealth / maxHealth;
            }
        }
        #endregion

        private ScoreController _scoreController;

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
                var acsbList = _scoreController.GetPrivateField<List<AfterCutScoreBuffer>>("_afterCutScoreBuffers");

                foreach (AfterCutScoreBuffer acsb in acsbList)
                {
                    if (acsb.GetPrivateField<NoteCutInfo>("_noteCutInfo") == noteCutInfo)
                    {
                        acsb.didFinishEvent += OnNoteWasFullyCut;
                        break;
                    }
                }
            }
        }

        private void OnNoteWasFullyCut(AfterCutScoreBuffer acsb)
        {
            NoteCutInfo noteCutInfo = acsb.GetPrivateField<NoteCutInfo>("_noteCutInfo");

            if (acsb != null)
                acsb.didFinishEvent -= OnNoteWasFullyCut;

            ScoreController.RawScoreWithoutMultiplier(noteCutInfo, noteCutInfo.afterCutSwingRatingCounter,
                                                      out var score, out var afterScore, out var cutDistanceScore);
            Hurt(score + afterScore);
            if (IsAlive())
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
                Logger.log.Error("still looking for the scoreController");
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
            ConfigureVisuals();
            enabled = false;
        }

        private void ConfigureVisuals()
        {
            transform.position = BasePosition;
            transform.eulerAngles = BaseRotation;
            transform.localScale = BaseScale;

            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            var rectTransform = _canvas.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = BaseCanvasSize;
                _noteCountText = Utils.CreateText((RectTransform)_canvas.transform, NoteCountLeft + " notes left", NoteCountNamePosition);
            }
            rectTransform = _noteCountText.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                rectTransform.sizeDelta = MonsterLabelSize;
                rectTransform.anchoredPosition = NoteCountNamePosition;
                _noteCountText.fontSize = NoteCountFontSize;
                _monsterLabel = Utils.CreateText(_canvas.transform as RectTransform, MonsterName, MonsterLabelPosition);
            }
            rectTransform = _monsterLabel.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                rectTransform.anchoredPosition = MonsterLabelPosition;
                rectTransform.sizeDelta = MonsterLabelSize;
                _monsterLabel.fontSize = MonsterLabelFontSize;
                monsterLifeBarBg = new GameObject("Background").AddComponent<Image>();
            }
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

        public void ConfigureMonster(string monsterName, uint noteCountLeft, uint maxHp)
        {
            MonsterName = monsterName;
            name = "[FS|" + monsterName + "]";
            NoteCountLeft = (int)noteCountLeft;
            maxHealth = (int)maxHp;
            CurrentHealth = (int)maxHp;
            new UnityTask(ConfigureEvents());
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }

        public void Hurt(int rawScore)
        {
            CurrentHealth -= rawScore;
            if (!IsAlive())
                DisplayMonsterInformationEnd("You killed that!");
        }

        public void NotePassed()
        {
            NoteCountLeft -= 1;
            if (NoteCountLeft <= 0)
                DisplayMonsterInformationEnd("He ran away..");
        }

        private void DisplayMonsterInformationEnd(string labelInfo)
        {
            var floatingText = FloatingText.Create();
            floatingText.fadeOutText = true;
            floatingText.tweenScaleFunc = TweenScaleFunctions.QuadraticEaseOut;
            floatingText.tweenEndPosition = new Vector3(BasePosition.x, BasePosition.y + 0.75f, BasePosition.z);
            floatingText.ConfigureText();
            floatingText.DisplayText(BasePosition, BaseRotation, BaseScale, labelInfo, 3.5f);
            if (_scoreController)
            {
                _scoreController.noteWasCutEvent -= OnNoteWasCut;
                _scoreController.noteWasMissedEvent -= OnNoteWasMissed;
            }
            Destroy(gameObject);
        }
    }
}