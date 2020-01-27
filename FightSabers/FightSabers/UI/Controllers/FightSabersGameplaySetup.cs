using BeatSaberMarkupLanguage.Attributes;

namespace FightSabers.UI.Controllers
{
    class FightSabersGameplaySetup : PersistentSingleton<FightSabersGameplaySetup>
    {
        [UIValue("note-shrinker")] public bool NoteShrinkerEnabled;
        [UIValue("color-sucker")] public bool ColorSuckerEnabled;
        [UIValue("time-warper")] public bool TimeWarperEnabled;
    }
}
