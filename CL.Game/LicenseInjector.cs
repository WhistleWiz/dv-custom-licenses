using UnityEngine;

namespace CL.Game
{
    internal class LicenseInjector
    {
        public static void SetLicenseProperties(ICustomLicense license, GameObject licenseObj)
        {
            SetBookletProperties(licenseObj, license.PrefabName, license.Original.LocalizationKeyItem);
        }

        public static void SetLicenseSampleProperties(ICustomLicense license, GameObject licenseObj)
        {
            SetBookletProperties(licenseObj, license.InfoPrefabName, license.Original.LocalizationKeyInfoItem);
        }

        private static void SetBookletProperties(GameObject licenseObj, string bookletName, string nameLocalKey)
        {
            licenseObj.name = bookletName;
            InventoryItemSpec itemSpec = licenseObj.GetComponent<InventoryItemSpec>();

            if (itemSpec)
            {
                itemSpec.localizationKeyName = nameLocalKey;
                itemSpec.itemPrefabName = bookletName;
            }
            else
            {
                CLMod.Warning($"Couldn't set inventory name on '{bookletName}'");
            }

            licenseObj.SetActive(true);
        }
    }
}
