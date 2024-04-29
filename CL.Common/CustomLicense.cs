using DVLangHelper.Data;
using System;
using UnityEngine;

namespace CL.Common
{
    [Serializable]
    public class CustomLicense
    {
        [Header("License properties")]
        [Tooltip("Whether this is a general license (locomotives, concurrent jobs, etc) or a job license (hazmat, train length, etc)")]
        public LicenseType LicenseType = LicenseType.General;
        [Tooltip("The name (id) of the license")]
        public string Identifier = "CL_LicenseId";

        public Colour Color = Colour.FromUnity(new Color32(43, 166, 166, byte.MaxValue));
        public float Price = 10000;
        [Tooltip("How much copay should increase after buying this license")]
        public float InsuranceFeeQuotaIncrease;
        [Tooltip("How much bonus time should decrease after buying this license\n" +
            "1% is written as 0.01\n" +
            "Negative values will give more bonus time instead")]
        public float BonusTimeDecreasePercentage;

        [Header("Requirements (Optional)")]
        [Tooltip("A general license that has to be purchased before this one")]
        public string RequiredGeneralLicenseId = "";
        [Tooltip("A job license that has to be purchased before this one")]
        public string RequiredJobLicenseId = "";

        [Tooltip("Should the license be available in sandbox?")]
        public FreeRoamAvailability Availability = FreeRoamAvailability.OnlyIfUnlockedInCareer;

        [Header("Localization")]
        [Tooltip("The name of the license (DE2, S282, Hazmat 2)")]
        public TranslationData? TranslationName;
        [Tooltip("The description of the license")]
        public TranslationData? TranslationDescription;
        [Tooltip("The name of the inventory item for the license")]
        public TranslationData? TranslationItem;
        [Tooltip("The name of the inventory item for the sample version of the license")]
        public TranslationData? TranslationInfoItem;

        public string LocalizationKey => $"{Constants.LocalizeRoot}/{Identifier.Replace(" ", "_").ToLowerInvariant()}";
        public string LocalizationKeyDescription => $"{LocalizationKey}_desc";
        public string LocalizationKeyItem => $"{LocalizationKey}_item";
        public string LocalizationKeyInfoItem => $"{LocalizationKey}_sample_item";

        // Wrapper class to avoid serialization issues.
        [Serializable]
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

            public static Color ToUnity(Colour c)
            {
                return c.ToUnity();
            }

            public static Colour FromUnity(Color color)
            {
                return new Colour(color.r, color.g, color.b, color.a);
            }
        }
    }
}
