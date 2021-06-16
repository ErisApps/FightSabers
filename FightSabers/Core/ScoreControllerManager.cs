using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS_Utils.Utilities;
using FightSabers.Utilities;
using UnityEngine;

namespace FightSabers.Core
{
    public class ScoreControllerManager : MonoBehaviour
    {
        public bool IsInitialized { get; private set; }

        private ScoreController _scoreController;

        public delegate void ScoreHandler(object self);
        public delegate void ScoreArgsHandler(object self, int cutScore);

        #region Events
        public event ScoreHandler ScoreControllerInitialized;
        public event ScoreHandler BombCut;
        public event ScoreHandler NoteMissed;
        public event ScoreHandler NoteCut;
        public event ScoreArgsHandler NoteFullyCut;

        private void OnScoreControllerInitialized()
        {
            ScoreControllerInitialized?.Invoke(this);
        }

        private void OnBombCut()
        {
            BombCut?.Invoke(this);
        }

        private void OnNoteMissed()
        {
            NoteMissed?.Invoke(this);
        }

        private void OnNoteCut()
        {
            NoteCut?.Invoke(this);
        }

        private void OnNoteFullyCut(int cutScore)
        {
            NoteFullyCut?.Invoke(this, cutScore);
        }

        #endregion

        private void Start()
        {
            new UnityTask(ConfigureEvents());
        }

        private void OnDestroy()
        {
            if (!_scoreController) return;
            _scoreController.noteWasCutEvent -= OnNoteWasCut;
            _scoreController.noteWasMissedEvent -= OnNoteWasMissed;
        }

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
                    OnScoreControllerInitialized();
                    break;
                }
            }
        }

        private void OnNoteWasCut(NoteData noteData, NoteCutInfo noteCutInfo, int multiplier)
        {
            if (noteData.noteType == NoteType.Bomb)
            {
                OnBombCut();
                return;
            }

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
                OnNoteCut();
            }
        }

        private void OnNoteWasFullyCut(CutScoreBuffer csb)
        {
            var noteCutInfo = csb.GetPrivateField<NoteCutInfo>("_noteCutInfo");

            if (csb != null)
                csb.didFinishEvent -= OnNoteWasFullyCut;

            ScoreController.RawScoreWithoutMultiplier(noteCutInfo, out var score, out var afterScore, out var cutDistanceScore);
            OnNoteFullyCut(score + afterScore + cutDistanceScore);
        }

        private void OnNoteWasMissed(NoteData noteData, int score)
        {
            if (noteData.noteType == NoteType.Bomb)
                return;
            OnNoteMissed();
        }
    }
}
