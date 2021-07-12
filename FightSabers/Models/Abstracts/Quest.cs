using System;
using FightSabers.Core;
using FightSabers.Models.Interfaces;
using Newtonsoft.Json;
using SiraUtil.Tools;

namespace FightSabers.Models.Abstracts
{
    internal abstract class Quest : IQuest
    {
	    protected readonly SiraLog Logger;
	    protected readonly ExperienceSystem ExperienceSystem;

	    public string Title { get; set; } = null!;
	    public string Description { get; set; } = null!;
	    public string ProgressHint { get; set; } = null!;
	    public string QuestType { get; set; } = null!;
	    public uint ExpReward { get; set; }
        public bool IsInitialized { get; set; }
        public bool IsActivated { get; set; }
        public bool IsCompleted { get; set; }
        public bool HasGameEventsActivated { get; set; }

        public Quest(SiraLog logger, ExperienceSystem experienceSystem)
        {
	        Logger = logger;
	        ExperienceSystem = experienceSystem;
        }

        private float _progress { get; set; }
        [JsonProperty("progress")]
        public float Progress {
            get => _progress;
            set
            {
                if (Math.Abs(_progress - value) < 0.001f)
                {
	                return;
                }

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
            if (IsInitialized)
            {
	            return;
            }

            Title = title;
            Description = description;
            ProgressHint = progressHint;
            QuestType = questType;
            ExpReward = expReward;
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
            {
	            IsInitialized = true;
            }

            if (!IsInitialized || IsActivated)
            {
	            return;
            }

            Logger.Debug(">>> Base quest activated!");
            IsActivated = true;
        }

        public virtual void Deactivate()
        {
            if (!IsInitialized || !IsActivated)
            {
	            return;
            }

            IsActivated = false;
            OnQuestCanceled();
        }

        public virtual void Complete()
        {
            ExperienceSystem.AddFightExperience(ExpReward);
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