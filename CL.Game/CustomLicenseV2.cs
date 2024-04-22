using DV.ThingTypes;
using UnityEngine;

namespace CL.Game
{
    internal class CustomGeneralLicenseV2 : GeneralLicenseType_v2
    {
        public string RenderPrefabName = "";
        public string SampleRenderPrefabName = "";
        public GameObject RenderPrefab = null!;
        public GameObject SampleRenderPrefab = null!;
    }

    internal class CustomJobLicenseV2 : JobLicenseType_v2
    {
        public string RenderPrefabName = "";
        public string SampleRenderPrefabName = "";
        public GameObject RenderPrefab = null!;
        public GameObject SampleRenderPrefab = null!;
    }
}
