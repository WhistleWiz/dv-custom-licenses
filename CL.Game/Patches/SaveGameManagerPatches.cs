using HarmonyLib;

namespace CL.Game.Patches
{
    [HarmonyPatch(typeof(SaveGameManager))]
    internal class SaveGameManagerPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(SaveGameManager.DoSaveIO))]
        public static void InjectSaveData(SaveGameData data)
        {
            SaveInjector.InjectDataIntoSaveGame(data);

            // Refund all licenses so the mod can be removed without
            // throwing money into the void.
            float totalRefund = 0;

            foreach (var (_, V2) in LicenseManager.AddedGeneralLicenses)
            {
                if (global::LicenseManager.Instance.IsGeneralLicenseAcquired(V2))
                {
                    totalRefund += V2.price;
                }
            }

            foreach (var (_, V2) in LicenseManager.AddedJobLicenses)
            {
                if (global::LicenseManager.Instance.IsJobLicenseAcquired(V2))
                {
                    totalRefund += V2.price;
                }
            }

            float? money = data.GetFloat(SaveGameKeys.Player_money);

            if (money.HasValue)
            {
                data.SetFloat(SaveGameKeys.Player_money, money.Value + totalRefund);
            }
        }

        [HarmonyPostfix, HarmonyPatch(nameof(SaveGameManager.FindStartGameData))]
        public static void ExtractSaveData(SaveGameManager __instance)
        {
            SaveInjector.LoadedData = null;
            if (__instance.data == null) return;

            SaveInjector.ExtractDataFromSaveGame(__instance.data);
            LicenseManager.ApplyGeneralMappings();
            LicenseManager.ApplyJobMappings();
        }
    }
}
