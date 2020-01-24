using System;
using FightSabers.Core;
using FightSabers.Models.Interfaces;
using Newtonsoft.Json;

namespace FightSabers.Models.Abstracts
{
    public abstract class Quest : IQuest
    {
        public string title { get; set; }
        public string description { get; set; }
        public string progressHint { get; set; }
        public string questType { get; set; }
        public uint expReward { get; set; }
        public bool isInitialized { get; set; }
        public bool isActivated { get; set; }
        public bool isCompleted { get; set; }
        public bool hasGameEventsActivated { get; set; }

        private float _progress { get; set; }
        [JsonProperty("progress")]
        public float Progress {
            get { return _progress; }
            set
            {
                if (Math.Abs(_progress - value) < 0.001f) return;
                _progress = value;
                OnProgressChanged();
            }
        }

        #region Events

        public delegate void ProgressHandler(object self);
        public event ProgressHandler ProgressChanged;
        public event ProgressHandler QuestCanceled;
        public event ProgressHandler QuestCompleted;

        protected void OnProgressChanged()
        {
            ProgressChanged?.Invoke(this);
            Refresh();
        }

        protected void OnQuestCanceled()
        {
            QuestCanceled?.Invoke(this);
        }

        protected void OnQuestCompleted()
        {
            isCompleted = true;
            QuestCompleted?.Invoke(this);
        }

        #endregion

        protected virtual void Prepare(string title, string description, string progressHint, string questType, uint expReward, float progress)
        {
            if (isInitialized) return;
            this.title = title;
            this.description = description;
            this.progressHint = progressHint;
            this.questType = questType;
            this.expReward = expReward;
            Progress = progress;
            isInitialized = true;
        }

        public void ForceInitialize()
        {
            isInitialized = true;
        }

        public virtual void Activate(bool forceInitialize = false)
        {
            if (!isInitialized && forceInitialize)
                isInitialized = true;
            if (!isInitialized || isActivated) return;
            Logger.log.Debug(">>> Base quest activated!");
            isActivated = true;
        }

        public virtual void Deactivate()
        {
            if (!isInitialized || !isActivated) return;
            isActivated = false;
            OnQuestCanceled();
        }

        public virtual void Complete()
        {
            ExperienceSystem.instance.AddFightExperience(expReward);
            OnQuestCompleted();
        }

        public virtual void LinkGameEvents()
        {
            hasGameEventsActivated = true;
        }

        public virtual void UnlinkGameEvents()
        {
            hasGameEventsActivated = false;
        }

        protected abstract void Refresh();
    }
}