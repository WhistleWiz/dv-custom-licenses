using DV.Booklets;
using DV.ThingTypes;
using HarmonyLib;
using System;
using UnityEngine;

namespace CL.Game.Patches
{
    [HarmonyPatch(typeof(BookletCreator_Licenses))]
    internal class BookletCreator_LicensesPatches
    {
        // General licenses.
        [HarmonyPostfix, HarmonyPatch(nameof(BookletCreator_Licenses.CreateLicense)),
            HarmonyPatch(new Type[] { typeof(GeneralLicenseType_v2), typeof(Vector3), typeof(Quaternion), typeof(Transform), typeof(bool) })]
        public static void CreateLicensePostfix(GeneralLicenseType_v2 license, ref GameObject __result)
        {
            if (license is CustomGeneralLicenseV2 custom)
            {
                LicenseInjector.SetLicenseProperties(custom, __result);
            }
        }

        [HarmonyPostfix, HarmonyPatch(nameof(BookletCreator_Licenses.CreateLicenseInfo)),
            HarmonyPatch(new Type[] { typeof(GeneralLicenseType_v2), typeof(Vector3), typeof(Quaternion), typeof(Transform), typeof(bool) })]
        public static void CreateLicenseInfoPostfix(GeneralLicenseType_v2 license, ref GameObject __result)
        {
            if (license is CustomGeneralLicenseV2 custom)
            {
                LicenseInjector.SetLicenseSampleProperties(custom, __result);
            }
        }

        // Job licenses.
        [HarmonyPostfix, HarmonyPatch(nameof(BookletCreator_Licenses.CreateLicense)),
            HarmonyPatch(new Type[] { typeof(JobLicenseType_v2), typeof(Vector3), typeof(Quaternion), typeof(Transform), typeof(bool) })]
        public static void CreateLicensePostfix(JobLicenseType_v2 license, ref GameObject __result)
        {
            if (license is CustomJobLicenseV2 custom)
            {
                LicenseInjector.SetLicenseProperties(custom, __result);
            }
        }

        [HarmonyPostfix, HarmonyPatch(nameof(BookletCreator_Licenses.CreateLicenseInfo)),
            HarmonyPatch(new Type[] { typeof(JobLicenseType_v2), typeof(Vector3), typeof(Quaternion), typeof(Transform), typeof(bool) })]
        public static void CreateLicenseInfoPostfix(JobLicenseType_v2 license, ref GameObject __result)
        {
            if (license is CustomJobLicenseV2 custom)
            {
                LicenseInjector.SetLicenseSampleProperties(custom, __result);
            }
        }
    }
}
