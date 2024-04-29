using CL.Common;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CL.Unity
{
    [CreateAssetMenu(menuName = "DVCustomLicenses/Custom License")]
    internal class CustomLicenseContainer : ScriptableObject
    {
        public CustomLicense License = new CustomLicense();

        public string GetFullPath()
        {
            string path = Application.dataPath;
            string assetPath = AssetDatabase.GetAssetPath(this);
            path = path + "/" + assetPath.Substring(7);
            return Path.GetDirectoryName(path);
        }

        public void Export()
        {
            // Use a JsonSerializer for a cleaner output.
            JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.Indented };
            var path = $"{GetFullPath()}\\license.json";

            using var file = File.CreateText(path);
            using var jsonWr = new JsonTextWriter(file);
            serializer.Serialize(jsonWr, License);

            EditorUtility.RevealInFinder(path);
            AssetDatabase.Refresh();
        }
    }
}
