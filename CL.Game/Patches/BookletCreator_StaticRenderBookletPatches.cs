using DV.Booklets;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;
using System;

namespace CL.Game.Patches
{
    // Once again thanks to Passenger Jobs.
    [HarmonyPatch(typeof(BookletCreator_StaticRenderBooklet))]
    internal class BookletCreator_StaticRenderBookletPatches
    {
        private static MethodInfo s_resourcesLoadMethod = AccessTools.Method(
            typeof(Resources), nameof(Resources.Load), new[] { typeof(string), typeof(Type) });

        [HarmonyPatch(nameof(BookletCreator_StaticRenderBooklet.Render))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RenderTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            bool skipping = false;

            foreach (var instruction in instructions)
            {
                if (skipping)
                {
                    if (instruction.Calls(s_resourcesLoadMethod))
                    {
                        skipping = false;
                    }
                    continue;
                }

                if (instruction.opcode == OpCodes.Ldtoken)
                {
                    yield return CodeInstruction.Call((string s) => LoadRenderPrefab(s));
                    skipping = true;
                    continue;
                }

                yield return instruction;
            }
        }

        private static UnityEngine.Object LoadRenderPrefab(string name)
        {
            if (!LicenseManager.KeyToPrefab.TryGetValue(name, out var result))
            {
                result = Resources.Load<GameObject>(name);
            }

            result!.SetActive(true);
            return result;
        }
    }
}
