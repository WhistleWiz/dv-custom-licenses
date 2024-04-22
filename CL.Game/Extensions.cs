using CL.Common;
using DV.ThingTypes;
using UnityEngine;

namespace CL.Game
{
    internal static class Extensions
    {
        private static int s_tempValue = Constants.DefaultLicenseValue;

        public static GeneralLicenseType_v2 ToGeneralV2(this CustomLicense license)
        {
            var newLicense = ScriptableObject.CreateInstance<GeneralLicenseType_v2>();

            newLicense.id = license.Identifier;
            newLicense.v1 = (GeneralLicenseType)(++s_tempValue);

            newLicense.localizationKey = license.LocalizationKey;
            newLicense.localizationKeysDescription = license.LocalizationKeysDescription;

            newLicense.color = license.Color;
            newLicense.price = license.Price;
            newLicense.insuranceFeeQuotaIncrease = license.InsuranceFeeQuotaIncrease;
            newLicense.bonusTimeDecreasePercentage = license.BonusTimeDecreasePercentage;

            return newLicense;
        }

        public static JobLicenseType_v2 ToJobV2(this CustomLicense license)
        {
            var newLicense = ScriptableObject.CreateInstance<JobLicenseType_v2>();

            newLicense.id = license.Identifier;
            newLicense.v1 = (JobLicenses)(++s_tempValue);

            newLicense.localizationKey = license.LocalizationKey;
            newLicense.localizationKeysDescription = license.LocalizationKeysDescription;

            newLicense.color = license.Color;
            newLicense.price = license.Price;
            newLicense.insuranceFeeQuotaIncrease = license.InsuranceFeeQuotaIncrease;
            newLicense.bonusTimeDecreasePercentage = license.BonusTimeDecreasePercentage;

            return newLicense;
        }
    }
}
