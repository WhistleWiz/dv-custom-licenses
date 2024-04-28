using CL.Common;
using DV.ThingTypes;
using UnityEngine;

namespace CL.Game
{
    internal interface ICustomLicense
    {
        public CustomLicense Original { get; set; }
        public string PrefabName { get; }
        public string InfoPrefabName { get; }
    }

    internal class CustomGeneralLicenseV2 : GeneralLicenseType_v2, ICustomLicense
    {
        public string RenderPrefabName = "";
        public string SampleRenderPrefabName = "";
        public GameObject RenderPrefab = null!;
        public GameObject SampleRenderPrefab = null!;

        public CustomLicense Original { get; set; } = null!;
        public string PrefabName => licensePrefab.name;
        public string InfoPrefabName => licenseInfoPrefab.name;

    }

    internal class CustomJobLicenseV2 : JobLicenseType_v2, ICustomLicense
    {
        public string RenderPrefabName = "";
        public string SampleRenderPrefabName = "";
        public GameObject RenderPrefab = null!;
        public GameObject SampleRenderPrefab = null!;

        public CustomLicense Original { get; set; } = null!;
        public string PrefabName => licensePrefab.name;
        public string InfoPrefabName => licenseInfoPrefab.name;
    }
}
