using CL.Common;
using DV;
using DV.Booklets.Rendered;
using DV.RenderTextureSystem.BookletRender;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
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
        public static Dictionary<string, GameObject> KeyToPrefab = new Dictionary<string, GameObject>();

        private static GeneralLicenseType_v2? s_de2;
        private static JobLicenseType_v2? s_hazmat1;

        public static GeneralLicenseType_v2 LicenseDE2 => s_de2 != null ? s_de2 : s_de2 = GeneralLicenseType.DE2.ToV2();
        public static JobLicenseType_v2 LicenseHazmat1 => s_hazmat1 != null ? s_hazmat1 : s_hazmat1 = JobLicenses.Hazmat1.ToV2();

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
        }

        private static bool TryLoadGeneralLicense(string jsonPath, CustomLicense l, out CustomGeneralLicenseV2 v2)
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

            CreateGeneralLicensePrefabs(l, v2);
            AddTranslations(l);
            AddedGeneralLicenses.Add((l, v2));
            Globals.G.Types.generalLicenses.Add(v2);

            return true;
        }

        private static void CreateGeneralLicensePrefabs(CustomLicense l, CustomGeneralLicenseV2 v2)
        {
            // Once again thanks to Passenger Jobs for figuring this out.

            v2.RenderPrefabName = $"License{l.Identifier}Render";
            v2.SampleRenderPrefabName = $"License{l.Identifier}RenderInfo";

            // Book.
            v2.licensePrefab = Utilities.CreateMockPrefab(LicenseDE2.licensePrefab);
            v2.licensePrefab.name = $"License{l.Identifier}";

            var render = v2.licensePrefab.GetComponent<RuntimeRenderedStaticTextureBooklet>();
            var de2RenderName = render.renderPrefabName;
            render.renderPrefabName = v2.RenderPrefabName;

            // Render prefab.
            v2.RenderPrefab = Utilities.CreateMockPrefab(Resources.Load<GameObject>(de2RenderName));
            v2.RenderPrefab.name = v2.RenderPrefabName;
            v2.RenderPrefab.GetComponent<StaticLicenseBookletRender>().generalLicense = v2;

            // Sample book.
            v2.licenseInfoPrefab = Utilities.CreateMockPrefab(LicenseDE2.licenseInfoPrefab);
            v2.licenseInfoPrefab.name = $"License{l.Identifier}Info";

            var infoRender = v2.licenseInfoPrefab.GetComponent<RuntimeRenderedStaticTextureBooklet>();
            var de2InfoRenderName = infoRender.renderPrefabName;
            infoRender.renderPrefabName = v2.SampleRenderPrefabName;

            // Sample render prefab.
            v2.SampleRenderPrefab = Utilities.CreateMockPrefab(Resources.Load<GameObject>(de2InfoRenderName));
            v2.SampleRenderPrefab.name = v2.SampleRenderPrefabName;
            v2.SampleRenderPrefab.GetComponent<StaticLicenseBookletRender>().generalLicense = v2;

            // Add to dictionary for access in patch.
            KeyToPrefab.Add(v2.RenderPrefabName, v2.RenderPrefab);
            KeyToPrefab.Add(v2.SampleRenderPrefabName, v2.SampleRenderPrefab);
        }

        private static bool TryLoadJobLicense(string jsonPath, CustomLicense l, out CustomJobLicenseV2 v2)
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

            CreateJobLicensePrefabs(l, v2);
            AddTranslations(l);
            AddedJobLicenses.Add((l, v2));
            Globals.G.Types.jobLicenses.Add(v2);

            return true;
        }

        private static void CreateJobLicensePrefabs(CustomLicense l, CustomJobLicenseV2 v2)
        {
            v2.RenderPrefabName = $"License{l.Identifier}Render";
            v2.SampleRenderPrefabName = $"License{l.Identifier}RenderInfo";

            // Book.
            v2.licensePrefab = Utilities.CreateMockPrefab(LicenseHazmat1.licensePrefab);
            v2.licensePrefab.name = $"License{l.Identifier}";

            var render = v2.licensePrefab.GetComponent<RuntimeRenderedStaticTextureBooklet>();
            var hazmatRenderName = render.renderPrefabName;
            render.renderPrefabName = v2.RenderPrefabName;

            // Render prefab.
            v2.RenderPrefab = Utilities.CreateMockPrefab(Resources.Load<GameObject>(hazmatRenderName));
            v2.RenderPrefab.name = v2.RenderPrefabName;
            v2.RenderPrefab.GetComponent<StaticLicenseBookletRender>().jobLicense = v2;

            // Sample book.
            v2.licenseInfoPrefab = Utilities.CreateMockPrefab(LicenseHazmat1.licenseInfoPrefab);
            v2.licenseInfoPrefab.name = $"License{l.Identifier}Info";

            var infoRender = v2.licenseInfoPrefab.GetComponent<RuntimeRenderedStaticTextureBooklet>();
            var hazmatInfoRenderName = infoRender.renderPrefabName;
            infoRender.renderPrefabName = v2.SampleRenderPrefabName;

            // Sample render prefab.
            v2.SampleRenderPrefab = Utilities.CreateMockPrefab(Resources.Load<GameObject>(hazmatInfoRenderName));
            v2.SampleRenderPrefab.name = v2.SampleRenderPrefabName;
            v2.SampleRenderPrefab.GetComponent<StaticLicenseBookletRender>().jobLicense = v2;

            // Add to dictionary for access in patch.
            KeyToPrefab.Add(v2.RenderPrefabName, v2.RenderPrefab);
            KeyToPrefab.Add(v2.SampleRenderPrefabName, v2.SampleRenderPrefab);
        }

        private static void AddTranslations(CustomLicense license)
        {
            // If there are no translations, use the name as default.
            if (license.TranslationName == null)
            {
                CLMod.Translations.AddTranslations(
                    license.LocalizationKey,
                    TranslationData.Default(license.Identifier));
            }
            else
            {
                CLMod.Translations.AddTranslations(
                    license.LocalizationKey,
                    license.TranslationName);
            }

            if (license.TranslationDescription != null)
            {
                CLMod.Translations.AddTranslations(
                    license.LocalizationKeyDescription,
                    license.TranslationDescription);
            }

            if (license.TranslationItem != null)
            {
                CLMod.Translations.AddTranslations(
                    license.LocalizationKeyItem,
                    license.TranslationItem);
            }

            if (license.TranslationInfoItem != null)
            {
                CLMod.Translations.AddTranslations(
                    license.LocalizationKeyInfoItem,
                    license.TranslationInfoItem);
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
                return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100, 1, SpriteMeshType.Tight);
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
