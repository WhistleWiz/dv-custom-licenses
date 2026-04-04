using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using HarmonyLib;
using System.Collections.Generic;

namespace CL.Game.Patches
{
    [HarmonyPatch(typeof(JobLicenseType_v2))]
    internal class JobLicenseType_v2Patches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(JobLicenseType_v2.ToV2List))]
        private static void ToV2ListPostfix(JobLicenses flags, ref List<JobLicenseType_v2> __result)
        {
            foreach (var item in LicenseManager.AddedJobValues)
            {
                if ((flags & item) != 0)
                {
                    __result.Add(item.ToV2());
                }
            }
        }
    }
}
