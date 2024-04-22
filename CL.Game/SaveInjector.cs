using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using CL.Common;
using System.Linq;

namespace CL.Game
{
    internal class SaveInjector
    {
        private const string s_gKeys = "GeneralKeys";
        private const string s_gValues = "GeneralValues";
        private const string s_jKeys = "JobKeys";
        private const string s_jValues = "JobValues";

        internal static JObject? LoadedData;

        internal static void ExtractDataFromSaveGame(SaveGameData data)
        {
            LoadedData = data.GetJObject(Constants.SaveKey);

            if (LoadedData != null)
            {
                ProcessGeneralMapping();
                ProcessJobMapping();
            }
            else
            {
                CLMod.Warning("No data found in save file, using new cache.");
            }
        }

        private static void ProcessGeneralMapping()
        {
            var keys = LoadedData![s_gKeys]!.ToObject<string[]>();
            var values = LoadedData[s_gValues]!.ToObject<int[]>();

            if (keys != null && values != null)
            {
                var dictionary = new Dictionary<string, int>();

                for (int i = 0; i < keys.Length; i++)
                {
                    dictionary.Add(keys[i], values[i]);
                }

                LicenseManager.GeneralMapping = dictionary;
                CLMod.Log("General mapping cache sucessfully loaded.");
            }
            else
            {
                CLMod.Error("Error loading data: general mapping is null!");
            }
        }

        private static void ProcessJobMapping()
        {
            var keys = LoadedData![s_jKeys]!.ToObject<string[]>();
            var values = LoadedData[s_jValues]!.ToObject<int[]>();

            if (keys != null && values != null)
            {
                var dictionary = new Dictionary<string, int>();

                for (int i = 0; i < keys.Length; i++)
                {
                    dictionary.Add(keys[i], values[i]);
                }

                LicenseManager.JobMapping = dictionary;
                CLMod.Log("Job mapping cache sucessfully loaded.");
            }
            else
            {
                CLMod.Error("Error loading data: job mapping is null!");
            }
        }

        internal static void InjectDataIntoSaveGame(SaveGameData data)
        {
            LoadedData = new JObject
            {
                { s_gKeys, JToken.FromObject(LicenseManager.GeneralMapping.Keys.ToArray()) },
                { s_gValues, JToken.FromObject(LicenseManager.GeneralMapping.Values.ToArray()) },
                { s_jKeys, JToken.FromObject(LicenseManager.JobMapping.Keys.ToArray()) },
                { s_jValues, JToken.FromObject(LicenseManager.JobMapping.Values.ToArray()) }
            };

            data.SetJObject(Constants.SaveKey, LoadedData);
        }
    }
}
