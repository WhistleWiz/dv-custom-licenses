using CL.Common;
using DV.InventorySystem;
using DV.JObjectExtstensions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace CL.Game
{
    internal class SaveInjector
    {
        internal static JObject? LoadedData;

        internal static void ExtractDataFromSaveGame(SaveGameData data)
        {
            LoadedData = data.GetJObject(Constants.SaveKey);

            if (LoadedData != null)
            {
                var mappingData = LoadedData.GetObjectViaJSON<LicenseMappingDataHolder>(Constants.SaveKeyMapping);

                if (mappingData != null)
                {
                    ProcessGeneralMapping(mappingData);
                    ProcessJobMapping(mappingData);
                    return;
                }
            }

            CLMod.Warning("No data found in save file, using new cache.");
        }

        private static void ProcessGeneralMapping(LicenseMappingDataHolder data)
        {
            var keys = data.GeneralKeys;
            var values = data.GeneralValues;
            var dictionary = new Dictionary<string, int>();

            for (int i = 0; i < keys.Length; i++)
            {
                dictionary.Add(keys[i], values[i]);
            }

            LicenseManager.GeneralMapping = dictionary;
            CLMod.Log("General mapping cache sucessfully loaded.");
        }

        private static void ProcessJobMapping(LicenseMappingDataHolder data)
        {
            var keys = data.JobKeys;
            var values = data.JobValues;

            var dictionary = new Dictionary<string, int>();

            for (int i = 0; i < keys.Length; i++)
            {
                dictionary.Add(keys[i], values[i]);
            }

            LicenseManager.JobMapping = dictionary;
            CLMod.Log("Job mapping cache sucessfully loaded.");
        }

        internal static void InjectDataIntoSaveGame(SaveGameData data)
        {
            LoadedData = data.GetJObject(Constants.SaveKey);
            LoadedData ??= new JObject();

            LoadedData.SetObjectViaJSON(Constants.SaveKeyMapping, new LicenseMappingDataHolder
            {
                GeneralKeys = LicenseManager.GeneralMapping.Keys.ToArray(),
                GeneralValues = LicenseManager.GeneralMapping.Values.ToArray(),
                JobKeys = LicenseManager.JobMapping.Keys.ToArray(),
                JobValues = LicenseManager.JobMapping.Values.ToArray()
            });

            foreach (var (_, V2) in LicenseManager.AddedGeneralLicenses)
            {
                LoadedData.SetBool($"{Constants.SaveKeyAcquiredLicense}{V2.id}", global::LicenseManager.Instance.IsGeneralLicenseAcquired(V2));
            }

            foreach (var (_, V2) in LicenseManager.AddedJobLicenses)
            {
                LoadedData.SetBool($"{Constants.SaveKeyAcquiredLicense}{V2.id}", global::LicenseManager.Instance.IsJobLicenseAcquired(V2));
            }

            data.SetJObject(Constants.SaveKey, LoadedData);
        }

        internal static void AcquireLicenses()
        {
            if (LoadedData != null && Inventory.Instance)
            {
                CLMod.Log("Acquiring licenses...");
                List<string> general = new List<string>();
                List<string> job = new List<string>();
                float totalCost = 0;

                foreach (var (_, V2) in LicenseManager.AddedGeneralLicenses)
                {
                    var result = LoadedData.GetBool($"{Constants.SaveKeyAcquiredLicense}{V2.id}");

                    if (result != null && result.Value)
                    {
                        global::LicenseManager.Instance.AcquireGeneralLicense(V2);
                        general.Add(V2.id);
                        totalCost += V2.price;
                    }
                }

                CLMod.Log($"GL: {string.Join(", ", general)}");

                foreach (var (_, V2) in LicenseManager.AddedJobLicenses)
                {
                    var result = LoadedData.GetBool($"{Constants.SaveKeyAcquiredLicense}{V2.id}");

                    if (result != null && result.Value)
                    {
                        global::LicenseManager.Instance.AcquireJobLicense(V2);
                        job.Add(V2.id);
                        totalCost += V2.price;
                    }
                }

                CLMod.Log($"JL: {string.Join(", ", job)}");

                // Remove total license cost.
                Inventory.Instance.RemoveMoney(totalCost);
            }
        }

        private class LicenseMappingDataHolder
        {
            public string[] GeneralKeys;
            public int[] GeneralValues;
            public string[] JobKeys;
            public int[] JobValues;

            public LicenseMappingDataHolder()
            {
                GeneralKeys = new string[0];
                GeneralValues = new int[0];
                JobKeys = new string[0];
                JobValues = new int[0];
            }
        }
    }
}
