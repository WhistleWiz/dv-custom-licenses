using DV.ThingTypes;
using HarmonyLib;
using System;
using System.Linq;

namespace CL.Game.Patches
{
    [HarmonyPatch(typeof(Enum))]
    internal static class EnumPatches
    {
        // Thanks Passenger Jobs mod!

        // Extend the array of actual values with the ones added by the mod.
        [HarmonyPostfix, HarmonyPatch(nameof(Enum.GetValues))]
        public static void GetValuesPostfix(Type enumType, ref Array __result)
        {
            if (enumType == typeof(GeneralLicenseType))
            {
                __result = ExtendArray(__result, LicenseManager.AddedGeneralValues.ToArray());
            }
            else if (enumType == typeof(JobLicenses))
            {
                __result = ExtendArray(__result, LicenseManager.AddedJobValues.ToArray());
            }
        }

        private static Array ExtendArray<T>(Array source, params T[] newValues)
        {
            var result = Array.CreateInstance(typeof(T), source.Length + newValues.Length);
            Array.Copy(source, result, source.Length);
            Array.Copy(newValues, 0, result, source.Length, newValues.Length);
            return result;
        }

        // Consider values defined by the mod as valid enum values.
        [HarmonyPrefix, HarmonyPatch(nameof(Enum.IsDefined))]
        public static bool IsDefinedPrefix(Type enumType, object value, ref bool __result)
        {
            if (enumType == typeof(GeneralLicenseType))
            {
                if (value is int iVal && LicenseManager.AddedGeneralValues.Contains((GeneralLicenseType)iVal) ||
                    value is GeneralLicenseType eVal && LicenseManager.AddedGeneralValues.Contains(eVal))
                {
                    __result = true;
                    return false;
                }
            }
            else if (enumType == typeof(JobLicenses))
            {
                if (value is int iVal && LicenseManager.AddedJobValues.Contains((JobLicenses)iVal) ||
                    value is JobLicenses eVal && LicenseManager.AddedJobValues.Contains(eVal))
                {
                    __result = true;
                    return false;
                }
            }

            return true;
        }
    }
}
