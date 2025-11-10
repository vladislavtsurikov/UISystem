#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public static class UISpawnExtensions
    {
        public static UISpawnOperation Spawn(this ChildSpawningUIHandler handler) => new();
    }
}
#endif
