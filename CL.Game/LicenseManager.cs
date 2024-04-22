using CL.Common;
using DV;
using DV.ThingTypes;
using DVLangHelper.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;

namespace CL.Game
{
    internal class LicenseManager
    {
        public static HashSet<GeneralLicenseType> AddedGeneralValues = new HashSet<GeneralLicenseType>();
        public static HashSet<JobLicenses> AddedJobValues = new HashSet<JobLicenses>();
        public static Dictionary<string, int> GeneralMapping = new Dictionary<string, int>();
        public static Dictionary<string, int> JobMapping = new Dictionary<string, int>();
        public static List<(CustomLicense Custom, GeneralLicenseType_v2 V2)> AddedGeneralLicenses = new List<(CustomLicense, GeneralLicenseType_v2)>();
        public static List<(CustomLicense Custom, JobLicenseType_v2 V2)> AddedJobLicenses = new List<(CustomLicense, JobLicenseType_v2)>();

        public static void LoadLicenses(UnityModManager.ModEntry mod)
        {
            List<GeneralLicenseType_v2> newGeneralLicenses = new List<GeneralLicenseType_v2>();
            List<JobLicenseType_v2> newJobLicenses = new List<JobLicenseType_v2>();

            // Find the 'cargo.json' files.
            foreach (string jsonPath in Directory.EnumerateFiles(mod.Path, Constants.LicenseFile, SearchOption.AllDirectories))
            {
                CustomLicense? l;

                try
                {
                    using StreamReader reader = File.OpenText(jsonPath);
                    l = JsonConvert.DeserializeObject<CustomLicense>(reader.ReadToEnd());
                }
                catch (Exception ex)
                {
                    CLMod.Error($"Error loading file {jsonPath}:\n{ex}");
                    continue;
                }

                // Something is wrong with the file.
                if (l == null)
                {
                    CLMod.Error($"Could not load license from file '{jsonPath}'");
                    continue;
                }

                // Try to load the license if we have one of those files.
                switch (l.LicenseType)
                {
                    case LicenseType.General:
                        if (TryLoadGeneralLicense(jsonPath, l, out var generalLicense))
                        {
                            newGeneralLicenses.Add(generalLicense);
                        }
                        break;
                    case LicenseType.Job:
                        if (TryLoadJobLicense(jsonPath, l, out var jobLicense))
                        {
                            newJobLicenses.Add(jobLicense);
                        }
                        break;
                    default:
                        CLMod.Error($"Unknown license type in file '{jsonPath}'");
                        continue;
                }
            }

            // If we did load any licenses...
            if (newGeneralLicenses.Count > 0)
            {
                CLMod.Log($"Loaded {newGeneralLicenses.Count} general licenses from {mod.Path}");
                CLMod.Log(string.Join(", ", newGeneralLicenses.Select(x => x.id)));
            }

            if (newJobLicenses.Count > 0)
            {
                CLMod.Log($"Loaded {newJobLicenses.Count} job licenses from {mod.Path}");
                CLMod.Log(string.Join(", ", newJobLicenses.Select(x => x.id)));
            }

            if (newGeneralLicenses.Count > 0 || newJobLicenses.Count > 0)
            {
                Globals.G.Types.RecalculateCaches();
            }
        }

        private static bool TryLoadGeneralLicense(string jsonPath, CustomLicense l, out GeneralLicenseType_v2 v2)
        {
            var directory = Path.GetDirectoryName(jsonPath);

            // Handle duplicate names (not).
            if (Globals.G.Types.generalLicenses.Any(x => x.id == l.Identifier))
            {
                CLMod.Error($"License with name '{l.Identifier}' already exists!");
                v2 = null!;
                return false;
            }

            v2 = l.ToGeneralV2();
            v2.icon = TryLoadIcon(directory);

            AddTranslations(l);

            return true;
        }

        private static bool TryLoadJobLicense(string jsonPath, CustomLicense l, out JobLicenseType_v2 v2)
        {
            var directory = Path.GetDirectoryName(jsonPath);

            if (Globals.G.Types.jobLicenses.Any(x => x.id == l.Identifier))
            {
                CLMod.Error($"License with name '{l.Identifier}' already exists!");
                v2 = null!;
                return false;
            }

            v2 = l.ToJobV2();
            v2.icon = TryLoadIcon(directory);

            AddTranslations(l);

            return true;
        }

        private static void AddTranslations(CustomLicense license)
        {
            // If there are no translations, use the name as default.
            if (license.TranslationNameData == null)
            {
                CLMod.Translations.AddTranslations(
                    license.LocalizationKey,
                    TranslationData.Default(license.Identifier));
            }
            else
            {
                CLMod.Translations.AddTranslations(
                    license.LocalizationKey,
                    license.TranslationNameData);
            }

            if (license.TranslationDescriptionData != null)
            {
                CLMod.Translations.AddTranslations(
                    license.LocalizationKeysDescription[0],
                    license.TranslationDescriptionData);
            }
        }

        private static Sprite TryLoadIcon(string directory)
        {
            byte[] data;

            // Path to the image.
            var path = Path.Combine(directory, Constants.IconFile);

            if (File.Exists(path))
            {
                // Shove the raw bytes of the image into the texture.
                // Texture size is not important and will be automatically changed.
                data = File.ReadAllBytes(path);
                var tex = new Texture2D(2, 2);
                tex.LoadImage(data);

                // Create a sprite that covers the whole texture.
                return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
            }

            return null!;
        }

        public static void ApplyGeneralMappings()
        {
            CLMod.Log("Applying general mappings...");

            int highest = Constants.DefaultLicenseValue;

            // Get the highest ID, to start counting from there.
            foreach (var item in GeneralMapping)
            {
                highest = Mathf.Max(highest, (int)item.Value);
            }

            AddedGeneralValues = new HashSet<GeneralLicenseType>();
            int newTypes = 0;

            foreach (var (_, v2) in AddedGeneralLicenses)
            {
                if (!GeneralMapping.TryGetValue(v2.id, out int type))
                {
                    type = ++highest;
                    GeneralMapping.Add(v2.id, type);
                    newTypes++;
                }

                v2.v1 = (GeneralLicenseType)type;
                AddedGeneralValues.Add(v2.v1);
            }

            // Recalculate caches with the new values.
            Globals.G.Types.RecalculateCaches();
            CLMod.Log($"Mappings applied: {AddedGeneralValues.Count}/{GeneralMapping.Count} (new: {newTypes}), highest value is {highest}");
        }

        public static void ApplyJobMappings()
        {
            CLMod.Log("Applying job mappings...");

            int highest = Constants.DefaultLicenseValue;

            // Get the highest ID, to start counting from there.
            foreach (var item in JobMapping)
            {
                highest = Mathf.Max(highest, (int)item.Value);
            }

            AddedJobValues = new HashSet<JobLicenses>();
            int newTypes = 0;

            foreach (var (_, v2) in AddedJobLicenses)
            {
                if (!JobMapping.TryGetValue(v2.id, out int type))
                {
                    type = ++highest;
                    JobMapping.Add(v2.id, type);
                    newTypes++;
                }

                v2.v1 = (JobLicenses)type;
                AddedJobValues.Add(v2.v1);
            }

            // Recalculate caches with the new values.
            Globals.G.Types.RecalculateCaches();
            CLMod.Log($"Mappings applied: {AddedJobValues.Count}/{JobMapping.Count} (new: {newTypes}), highest value is {highest}");
        }
    }
}
