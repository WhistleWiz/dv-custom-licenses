using DVLangHelper.Data;
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
        public string Identifier = "";

        public Color Color;
        public float Price;
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
        public TranslationData? TranslationNameData;
        public TranslationData? TranslationDescriptionData;

        public string LocalizationKey => $"{Constants.LocalizeRoot}/{Identifier.Replace(" ", "_").ToLowerInvariant()}";
        public string[] LocalizationKeysDescription => new[] { $"{LocalizationKey}_desc" };
    }
}
