using DV.Booklets;
using HarmonyLib;
using UnityEngine;
using DV.Booklets.Rendered;
using DV.RenderTextureSystem.BookletRender;

namespace CL.Game.Patches
{
    [HarmonyPatch(typeof(BookletCreator_StaticRenderBooklet))]
    internal class BookletCreator_StaticRenderBookletPatches
    {

        [HarmonyPrefix, HarmonyPatch(nameof(BookletCreator_StaticRenderBooklet.Render))]
        public static bool RenderPrefix(GameObject existingBooklet, string renderPrefabName, ref RenderedTexturesBase __result)
        {
            if (!LicenseManager.KeyToPrefab.TryGetValue(renderPrefabName, out var result))
            {
                return true;
            }

            StaticTextureRenderBase component = Object.Instantiate(result, DV.RenderTextureSystem.RenderTextureSystem.Instance.transform.position,
                Quaternion.identity).GetComponent<StaticTextureRenderBase>();

            // Since this isn't a resource we have to reactive the object or it won't draw anything.
            component.gameObject.SetActive(true);

            RenderedTexturesBooklet component2 = existingBooklet.GetComponent<RenderedTexturesBooklet>();
            component2.RegisterTexturesGeneratedEvent(component);
            component.GenerateStaticPagesTextures();
            __result = component2;

            return false;
        }
    }
}
