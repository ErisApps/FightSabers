using System;
using FightSabers.Core;
using FightSabers.Models.Interfaces;
using Newtonsoft.Json;

namespace FightSabers.Models.Abstracts
{
    public abstract class Quest : IQuest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProgressHint { get; set; }
        public string QuestType { get; set; }
        public uint ExpReward { get; set; }
        public bool IsInitialized { get; set; }
        public bool IsActivated { get; set; }
        public bool IsCompleted { get; set; }
        public bool HasGameEventsActivated { get; set; }

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

        public delegate void ProgressHandler(object self);
        public event ProgressHandler? ProgressChanged;
        public event ProgressHandler? QuestCanceled;
        public event ProgressHandler? QuestCompleted;

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
            IsCompleted = true;
            QuestCompleted?.Invoke(this);
        }

        protected virtual void Prepare(string title, string description, string progressHint, string questType, uint expReward, float progress)
        {
            if (IsInitialized) return;
            this.Title = title;
            this.Description = description;
            this.ProgressHint = progressHint;
            this.QuestType = questType;
            this.ExpReward = expReward;
            Progress = progress;
            IsInitialized = true;
        }

        public void ForceInitialize()
        {
            IsInitialized = true;
        }

        public virtual void Activate(bool forceInitialize = false)
        {
            if (!IsInitialized && forceInitialize)
                IsInitialized = true;
            if (!IsInitialized || IsActivated) return;
            Logger.log.Debug(">>> Base quest activated!");
            IsActivated = true;
        }

        public virtual void Deactivate()
        {
            if (!IsInitialized || !IsActivated) return;
            IsActivated = false;
            OnQuestCanceled();
        }

        public virtual void Complete()
        {
            ExperienceSystem.instance.AddFightExperience(ExpReward);
            OnQuestCompleted();
        }

        public virtual void LinkGameEvents()
        {
            HasGameEventsActivated = true;
        }

        public virtual void UnlinkGameEvents()
        {
            HasGameEventsActivated = false;
        }

        protected abstract void Refresh();
    }
}