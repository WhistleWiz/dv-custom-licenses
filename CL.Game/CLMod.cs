using DVLangHelper.Runtime;
using HarmonyLib;
using System.Reflection;
using UnityModManagerNet;

namespace CL.Game
{
    public static class CLMod
    {
        public const string Guid = "wiz.customlicenses";

        public static UnityModManager.ModEntry Instance { get; private set; } = null!;
        public static TranslationInjector Translations { get; private set; } = null!;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            Instance = modEntry;
            Translations = new TranslationInjector(Guid);

            ScanMods();
            UnityModManager.toggleModsListen += HandleModToggled;

            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }

        private static void ScanMods()
        {
            foreach (var mod in UnityModManager.modEntries)
            {
                if (mod.Active)
                {
                    LicenseManager.LoadLicenses(mod);
                }
            }
        }

        private static void HandleModToggled(UnityModManager.ModEntry modEntry, bool newState)
        {
            if (newState)
            {
                LicenseManager.LoadLicenses(modEntry);
            }
        }

        public static void Log(string message)
        {
            Instance.Logger.Log(message);
        }

        public static void Warning(string message)
        {
            Instance.Logger.Warning(message);
        }

        public static void Error(string message)
        {
            Instance.Logger.Error(message);
        }
    }
}
