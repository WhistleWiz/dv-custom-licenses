using CL.Common;
using DV.ThingTypes;
using System;
using System.Collections.Generic;
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
            newLicense.Original = license;

            newLicense.localizationKey = license.LocalizationKey;
            newLicense.localizationKeysDescription = new[] { license.LocalizationKeyDescription };

            newLicense.color = license.Color.ToUnity();
            newLicense.price = license.Price;
            newLicense.insuranceFeeQuotaIncrease = license.InsuranceFeeQuotaIncrease;
            newLicense.bonusTimeDecreasePercentage = license.BonusTimeDecreasePercentage;

            ReflectionHelper.AssignAvailability(newLicense);

            return newLicense;
        }

        public static CustomJobLicenseV2 ToJobV2(this CustomLicense license)
        {
            var newLicense = ScriptableObject.CreateInstance<CustomJobLicenseV2>();

            newLicense.id = license.Identifier;
            newLicense.v1 = (JobLicenses)(++s_tempValue);
            newLicense.Original = license;

            newLicense.localizationKey = license.LocalizationKey;
            newLicense.localizationKeysDescription = new[] { license.LocalizationKeyDescription };

            newLicense.color = license.Color.ToUnity();
            newLicense.price = license.Price;
            newLicense.insuranceFeeQuotaIncrease = license.InsuranceFeeQuotaIncrease;
            newLicense.bonusTimeDecreasePercentage = license.BonusTimeDecreasePercentage;

            ReflectionHelper.AssignAvailability(newLicense);

            return newLicense;
        }

        public static void CalculateRequirements(this CustomGeneralLicenseV2 license)
        {
            var custom = license.Original;

            if (!string.IsNullOrEmpty(custom.RequiredGeneralLicenseId))
            {
                if (DV.Globals.G.Types.TryGetGeneralLicense(custom.RequiredGeneralLicenseId, out var requiredGeneral))
                {
                    license.requiredGeneralLicense = requiredGeneral;
                }
                else
                {
                    CLMod.Warning($"Missing general license '{custom.RequiredGeneralLicenseId}', no requirement will be added.");
                }
            }

            if (!string.IsNullOrEmpty(custom.RequiredJobLicenseId))
            {
                if (DV.Globals.G.Types.TryGetJobLicense(custom.RequiredJobLicenseId, out var requiredJob))
                {
                    license.requiredJobLicense = requiredJob;
                }
                else
                {
                    CLMod.Warning($"Missing job license '{custom.RequiredJobLicenseId}', no requirement will be added.");
                }
            }
        }

        public static void CalculateRequirements(this CustomJobLicenseV2 license)
        {
            var custom = license.Original;

            if (!string.IsNullOrEmpty(custom.RequiredGeneralLicenseId))
            {
                if (DV.Globals.G.Types.TryGetGeneralLicense(custom.RequiredGeneralLicenseId, out var requiredGeneral))
                {
                    license.requiredGeneralLicense = requiredGeneral;
                }
                else
                {
                    CLMod.Warning($"Missing general license '{custom.RequiredGeneralLicenseId}', no requirement will be added.");
                }
            }

            if (!string.IsNullOrEmpty(custom.RequiredJobLicenseId))
            {
                if (DV.Globals.G.Types.TryGetJobLicense(custom.RequiredJobLicenseId, out var requiredJob))
                {
                    license.requiredJobLicense = requiredJob;
                }
                else
                {
                    CLMod.Warning($"Missing job license '{custom.RequiredJobLicenseId}', no requirement will be added.");
                }
            }
        }

        public static bool TryFind<T>(this List<T> list, Predicate<T> match, out T value)
        {
            value = list.Find(match);

            if (value == null)
            {
                return false;
            }

            return true;
        }
    }
}
