using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CL.Game.Patches
{
    [HarmonyPatch(typeof(Resources))]
    internal class ResourcesPatches
    {
        // Everything uses Resources.Load() so no way to patch something like item.LoadPrefab()...
        [HarmonyTargetMethods]
        private static IEnumerable<MethodBase> LoadTargets()
        {
            return typeof(Resources).GetMethods().Where(x => x.Name == nameof(Resources.Load) && x.GetParameters().Length == 1 && !x.IsGenericMethod);
        }

        [HarmonyPrefix]
        private static bool LoadPrefix(string path, ref Object __result)
        {
            if (LicenseManager.KeyToPrefab.TryGetValue(path, out var result))
            {
                __result = result;
                return false;
            }

            return true;
        }
    }
}
