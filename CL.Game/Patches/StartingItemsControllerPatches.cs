using HarmonyLib;

namespace CL.Game.Patches
{
    [HarmonyPatch(typeof(StartingItemsController))]
    internal class StartingItemsControllerPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(StartingItemsController.AddStartingItems))]
        public static void AddStartingItemsPostfix()
        {
            SaveInjector.AcquireLicenses();
        }
    }
}
