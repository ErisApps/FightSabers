using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using FightSabers.Models.Modifiers;

namespace FightSabers.UI.Controllers
{
	internal class FightSabersGameplaySetup : NotifiableSingleton<FightSabersGameplaySetup>
	{
		[UIValue("warning-hint-text")]
		public string _warningHintText = "<color=#e74c3c>You will not encounter any monsters if you enable one of these modifiers, and will also disable the score submission.</color>";

		[UIValue("note-shrinker")]
		public bool NoteShrinkerEnabled;

		[UIValue("note-shrinker-strength")]
		public float NoteShrinkerStrength = 1f;

		private string _noteShrinkerHint = "If enabled, notes will be scaled at <color=#FFA500>x0.65</color> of their size.";

		[UIValue("note-shrinker-hint")]
		public string NoteShrinkerHint
		{
			get => _noteShrinkerHint;
			private set
			{
				_noteShrinkerHint = value;
				NotifyPropertyChanged();
			}
		}

		[UIValue("color-sucker")]
		public bool ColorSuckerEnabled;

		[UIValue("color-sucker-strength")]
		public float ColorSuckerStrength = 1f;

		private string _colorSuckerHint = "If enabled, notes will restore <color=#FFA500>2.56%</color> of their color when cut.";

		[UIValue("color-sucker-hint")]
		public string ColorSuckerHint
		{
			get => _colorSuckerHint;
			private set
			{
				_colorSuckerHint = value;
				NotifyPropertyChanged();
			}
		}

		[UIValue("time-warper")]
		public bool TimeWarperEnabled;

		[UIValue("time-warper-strength")]
		public float TimeWarperStrength = 1f;

		private string _timeWarperHint = "If enabled, the song will be speed up by <color=#FFA500>x1.2</color>.";

		[UIValue("time-warper-hint")]
		public string TimeWarperHint
		{
			get => _timeWarperHint;
			private set
			{
				_timeWarperHint = value;
				NotifyPropertyChanged();
			}
		}

		[UIAction("update-ns-hint-act")]
		public void UpdateNoteShrinkerHint(float value)
		{
			NoteShrinkerHint = $"If enabled, notes will be scaled at <color=#FFA500>x{NoteShrinker.BaseScaleApplied.x * (1 / NoteShrinkerStrength):F}</color> of their size.";
		}

		[UIAction("update-cs-hint-act")]
		public void UpdateColorSuckerHint(float value)
		{
			ColorSuckerHint = $"If enabled, notes will restore <color=#FFA500>{(float) (0.8 / (35 * ColorSuckerStrength * 0.85) * 100):F}%</color> of their color when cut.";
		}

		[UIAction("update-tw-hint-act")]
		public void UpdateTimeWarperHint(float value)
		{
			TimeWarperHint = $"If enabled, the song will be speed up by <color=#FFA500>x{1 + 0.2 * TimeWarperStrength:F}</color>.";
		}
	}
}