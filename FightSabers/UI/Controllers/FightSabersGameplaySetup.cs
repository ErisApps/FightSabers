using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using FightSabers.Models.Modifiers;
using FightSabers.Settings;
using Zenject;

namespace FightSabers.UI.Controllers
{
	internal class FightSabersGameplaySetup : IInitializable
	{
		private readonly PluginConfig _config;

		private const string NOTE_SHRINKER_HINT = "If enabled, notes will be scaled at <color=#FFA500>x0.65</color> of their size.";
		private const string COLOR_SUCKER_HINT = "If enabled, notes will restore <color=#FFA500>2.56%</color> of their color when cut.";
		private const string TIME_WARPER_HINT = "If enabled, the song will be speed up by <color=#FFA500>x1.2</color>.";

		public FightSabersGameplaySetup(PluginConfig config)
		{
			_config = config;
		}

		public void Initialize()
		{
			GameplaySetup.instance.AddTab("FS Modifiers", "FightSabers.UI.Views.FightSabersGameplaySetupView.bsml", this, MenuType.Solo | MenuType.Custom);
		}

		[UIValue("warning-hint-text")]
		internal string WarningHintText => "<color=#e74c3c>You will not encounter any monsters if you enable one of these modifiers, and will also disable the score submission.</color>";


		[UIValue("note-shrinker")]
		internal bool NoteShrinkerEnabled
		{
			get => _config.NoteShrinkerEnabled;
			set => _config.NoteShrinkerEnabled = value;
		}

		[UIValue("note-shrinker-strength")]
		internal float NoteShrinkerStrength
		{
			get => _config.NoteShrinkerStrength;
			set => _config.NoteShrinkerStrength = value;
		}

		[UIValue("note-shrinker-hint")]
		internal string NoteShrinkerHint { get; private set; } = NOTE_SHRINKER_HINT;


		[UIValue("color-sucker")]
		internal bool ColorSuckerEnabled
		{
			get => _config.ColorSuckerEnabled;
			set => _config.ColorSuckerEnabled = value;
		}

		[UIValue("color-sucker-strength")]
		internal float ColorSuckerStrength
		{
			get => _config.ColorSuckerStrength;
			set => _config.ColorSuckerStrength = value;
		}

		[UIValue("color-sucker-hint")]
		internal string ColorSuckerHint { get; private set; } = COLOR_SUCKER_HINT;


		[UIValue("time-warper")]
		internal bool TimeWarperEnabled
		{
			get => _config.TimeWarperEnabled;
			set => _config.TimeWarperEnabled = value;
		}

		[UIValue("time-warper-strength")]
		internal float TimeWarperStrength
		{
			get => _config.TimeWarperStrength;
			set => _config.TimeWarperStrength = value;
		}

		[UIValue("time-warper-hint")]
		internal string TimeWarperHint { get; private set; } = TIME_WARPER_HINT;


		[UIAction("update-ns-hint-act")]
		internal void UpdateNoteShrinkerHint(float value)
		{
			NoteShrinkerHint = $"If enabled, notes will be scaled at <color=#FFA500>x{NoteShrinker.BaseScaleApplied.x * (1 / NoteShrinkerStrength):F}</color> of their size.";
		}

		[UIAction("update-cs-hint-act")]
		internal void UpdateColorSuckerHint(float value)
		{
			ColorSuckerHint = $"If enabled, notes will restore <color=#FFA500>{(float) (0.8 / (35 * ColorSuckerStrength * 0.85) * 100):F}%</color> of their color when cut.";
		}

		[UIAction("update-tw-hint-act")]
		internal void UpdateTimeWarperHint(float value)
		{
			TimeWarperHint = $"If enabled, the song will be speed up by <color=#FFA500>x{1 + 0.2 * TimeWarperStrength:F}</color>.";
		}
	}
}