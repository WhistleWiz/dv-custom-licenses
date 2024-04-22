using CL.Common;
using DV.ThingTypes;
using UnityEngine;

namespace CL.Game
{
    internal static class Extensions
    {
        private static int s_tempValue = Constants.DefaultLicenseValue;

        public static CustomGeneralLicenseV2 ToGeneralV2(this CustomLicense license)
        {
            var newLicense = ScriptableObject.CreateInstance<CustomGeneralLicenseV2>();

            newLicense.id = license.Identifier;
            newLicense.v1 = (GeneralLicenseType)(++s_tempValue);

            newLicense.localizationKey = license.LocalizationKey;
            newLicense.localizationKeysDescription = license.LocalizationKeysDescription;

            newLicense.color = license.Color;
            newLicense.price = license.Price;
            newLicense.insuranceFeeQuotaIncrease = license.InsuranceFeeQuotaIncrease;
            newLicense.bonusTimeDecreasePercentage = license.BonusTimeDecreasePercentage;

            if (!string.IsNullOrEmpty(license.RequiredGeneralLicenseId))
            {
                if (DV.Globals.G.Types.TryGetGeneralLicense(license.RequiredGeneralLicenseId, out var requiredGeneral))
                {
                    newLicense.requiredGeneralLicense = requiredGeneral;
                }
                else
                {
                    CLMod.Warning($"Missing general license '{license.RequiredGeneralLicenseId}', no requirement will be added.");
                }
            }

            if (!string.IsNullOrEmpty(license.RequiredJobLicenseId))
            {
                if (DV.Globals.G.Types.TryGetJobLicense(license.RequiredJobLicenseId, out var requiredJob))
                {
                    newLicense.requiredJobLicense = requiredJob;
                }
                else
                {
                    CLMod.Warning($"Missing job license '{license.RequiredJobLicenseId}', no requirement will be added.");
                }
            }

            return newLicense;
        }

        public static CustomJobLicenseV2 ToJobV2(this CustomLicense license)
        {
            var newLicense = ScriptableObject.CreateInstance<CustomJobLicenseV2>();

            newLicense.id = license.Identifier;
            newLicense.v1 = (JobLicenses)(++s_tempValue);

            newLicense.localizationKey = license.LocalizationKey;
            newLicense.localizationKeysDescription = license.LocalizationKeysDescription;

            newLicense.color = license.Color;
            newLicense.price = license.Price;
            newLicense.insuranceFeeQuotaIncrease = license.InsuranceFeeQuotaIncrease;
            newLicense.bonusTimeDecreasePercentage = license.BonusTimeDecreasePercentage;

            if (!string.IsNullOrEmpty(license.RequiredGeneralLicenseId))
            {
                if (DV.Globals.G.Types.TryGetGeneralLicense(license.RequiredGeneralLicenseId, out var requiredGeneral))
                {
                    newLicense.requiredGeneralLicense = requiredGeneral;
                }
                else
                {
                    CLMod.Warning($"Missing general license '{license.RequiredGeneralLicenseId}', no requirement will be added.");
                }
            }

            if (!string.IsNullOrEmpty(license.RequiredJobLicenseId))
            {
                if (DV.Globals.G.Types.TryGetJobLicense(license.RequiredJobLicenseId, out var requiredJob))
                {
                    newLicense.requiredJobLicense = requiredJob;
                }
                else
                {
                    CLMod.Warning($"Missing job license '{license.RequiredJobLicenseId}', no requirement will be added.");
                }
            }

            return newLicense;
        }
    }
}
