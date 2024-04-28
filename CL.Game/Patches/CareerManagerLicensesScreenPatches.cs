using DV.ServicePenalty.UI;
using HarmonyLib;

namespace CL.Game.Patches
{
    [HarmonyPatch(typeof(CareerManagerLicensesScreen))]
    internal class CareerManagerLicensesScreenPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(CareerManagerLicensesScreen.Awake))]
        public static void AwakePrefix(CareerManagerLicensesScreen __instance)
        {
            foreach (var (_, V2) in LicenseManager.AddedGeneralLicenses)
            {
                __instance.licensesDisplayOrder.Add(
                    new CareerManagerLicensesScreen.GeneralOrJobLicense { generalLicense = V2 });
            }

            foreach (var (_, V2) in LicenseManager.AddedJobLicenses)
            {
                __instance.licensesDisplayOrder.Add(
                    new CareerManagerLicensesScreen.GeneralOrJobLicense { jobLicense = V2 });
            }
        }
    }
}
