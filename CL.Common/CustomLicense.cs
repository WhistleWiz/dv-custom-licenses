using DVLangHelper.Data;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace CL.Common
{
    [Serializable]
    public class CustomLicense
    {
        [Header("License properties")]
        [Tooltip("Wether this is a general license (locomotives, concurrent jobs, etc) or a job license (hazmat, train length, etc)")]
        public LicenseType LicenseType = LicenseType.General;
        [Tooltip("The name (id) of the license")]
        public string Identifier = "CL_LicenseId";

        public Colour Color = new Colour(0, 0, 1, 1);
        public float Price = 10000;
        [Tooltip("How much copay should increase after buying this license")]
        public float InsuranceFeeQuotaIncrease;
        [Tooltip("How much bonus time should decrease after buying this license")]
        public float BonusTimeDecreasePercentage;

        [Header("Requirements (Optional)")]
        [Tooltip("A general license that has to be purchased before this one")]
        public string RequiredGeneralLicenseId = "";
        [Tooltip("A job license that has to be purchased before this one")]
        public string RequiredJobLicenseId = "";

        [Tooltip("Should the license be available in sandbox?")]
        public FreeRoamAvailability Availability = FreeRoamAvailability.OnlyIfUnlockedInCareer;

        [Header("Localization")]
        public TranslationData? TranslationName;
        public TranslationData? TranslationDescription;
        public TranslationData? TranslationItem;
        public TranslationData? TranslationInfoItem;

        public string LocalizationKey => $"{Constants.LocalizeRoot}/{Identifier.Replace(" ", "_").ToLowerInvariant()}";
        public string LocalizationKeyDescription => $"{LocalizationKey}_desc";
        public string LocalizationKeyItem => $"{LocalizationKey}_item";
        public string LocalizationKeyInfoItem => $"{LocalizationKey}_sample_item";

        // Wrapper class to avoid serialization issues.
        public class Colour
        {
            public float R, G, B, A;

            public Colour() : this(0, 0, 0, 0) { }

            public Colour(float r, float g, float b, float a)
            {
                R = r;
                G = g;
                B = b;
                A = a;
            }

            public Color ToUnity()
            {
                return new Color(R, G, B, A);
            }
        }
    }
}
