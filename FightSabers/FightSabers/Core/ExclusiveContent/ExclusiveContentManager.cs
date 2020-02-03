namespace FightSabers.Core.ExclusiveContent
{
    public class ExclusiveContentManager : PersistentSingleton<ExclusiveContentManager>
    {
        public void LoadExclusiveContent()
        {
            ExclusiveSabersManager.instance.LoadUnlockableSabers();
        }
    }
}
