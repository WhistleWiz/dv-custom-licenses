using CL.Common;
using DVLangHelper.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static CL.Common.CustomLicense;

namespace CL.Unity.Inspector
{
    [CustomEditor(typeof(CustomLicenseContainer))]
    internal class CustomLicenseContainerEditor : Editor
    {
        private static Dictionary<LicenseColour, Color32> LicenseToColour = new Dictionary<LicenseColour, Color32>()
        {
            { LicenseColour.Generic,    new Color32( 43, 166, 166, byte.MaxValue) },
            { LicenseColour.Locomotive, new Color32( 83,  83,  83, byte.MaxValue) },
            { LicenseColour.Concurrent, new Color32(115,  86, 146, byte.MaxValue) },
            { LicenseColour.Hazmat,     new Color32(166,  43,  43, byte.MaxValue) },
            { LicenseColour.Military,   new Color32(126, 152,  95, byte.MaxValue) },
            { LicenseColour.Length,     new Color32(146,  98,  59, byte.MaxValue) }
        };

        private CustomLicenseContainer _container = null!;
        private SerializedProperty _license = null!;
        private LicenseColour _selectedColour;
        private FillType _selectedFill;

        private void OnEnable()
        {
            _license = serializedObject.FindProperty(nameof(CustomLicenseContainer.License));
        }

        public override void OnInspectorGUI()
        {
            var current = _license.FindPropertyRelative(nameof(CustomLicense.LicenseType));

            do
            {
                EditorGUILayout.PropertyField(current);
            } while (current.Next(false));

            _container = (CustomLicenseContainer)target;
            bool dirty = false;

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                _selectedColour = (LicenseColour)EditorGUILayout.EnumPopup(_selectedColour);

                if (GUILayout.Button(new GUIContent("Set colour",
                    "Sets the colour to one of the base game colours"),
                    GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.45f)))
                {
                    _container.License.Color = Colour.FromUnity(LicenseToColour[_selectedColour]);
                    dirty = true;
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                _selectedFill = (FillType)EditorGUILayout.EnumPopup(_selectedFill);

                if (GUILayout.Button(new GUIContent("Autofill",
                    "Auto fills english translations based on the name and type\n" +
                    "Will not replace already written text"),
                    GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.45f)))
                {
                    Autofill(_container.License, _selectedFill);
                    dirty = true;
                }
            }

            if (dirty)
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                serializedObject.Update();
            }

            if (GUILayout.Button("Export"))
            {
                _container.Export();
            }
        }

        private static void Autofill(CustomLicense license, FillType fillType)
        {
            var translation = license.TranslationName!.Items.Where(x => x.Language == DVLanguage.English).FirstOrDefault();

            if (translation == null || string.IsNullOrEmpty(translation.Value))
            {
                Debug.LogWarning("No english name has been assigned to the license, cannot autofill!");
                return;
            }

            var name = translation.Value;

            // Item name.
            var itemName = license.TranslationItem!.Items.Where(x => x.Language == DVLanguage.English).FirstOrDefault();
            itemName ??= new TranslationItem(DVLanguage.English, string.Empty);

            if (string.IsNullOrEmpty(itemName.Value))
            {
                itemName.Value = $"License {name}";
            }

            // Info item name.
            var infoItemName = license.TranslationInfoItem!.Items.Where(x => x.Language == DVLanguage.English).FirstOrDefault();
            infoItemName ??= new TranslationItem(DVLanguage.English, string.Empty);

            if (string.IsNullOrEmpty(infoItemName.Value))
            {
                infoItemName.Value = $"License {name} Info";
            }

            // Description.
            var description = license.TranslationDescription!.Items.Where(x => x.Language == DVLanguage.English).FirstOrDefault();
            description ??= new TranslationItem(DVLanguage.English, string.Empty);

            if (string.IsNullOrEmpty(description.Value))
            {
                description.Value = fillType switch
                {
                    FillType.SteamLocomotive => "This license grants you access to all the [WHYTE WHEEL ARRANGEMENT] steam-powered locomotives in Derail Valley. " +
                        "Use them responsibly! Refer to your vehicle catalog for detailed specifications.",
                    FillType.DELocomotive => "This license grants you access to all the [# AXLES]-axle diesel-electric locomotives in Derail Valley. " +
                        "Use them responsibly! Refer to your vehicle catalog for detailed specifications.",
                    FillType.DHLocomotive => "This license grants you access to all the [# AXLES]-axle diesel-hydraulic locomotives in Derail Valley. " +
                        "Use them responsibly! Refer to your vehicle catalog for detailed specifications.",
                    FillType.DMLocomotive => "This license grants you access to all the [# AXLES]-axle diesel-mechanical locomotives in Derail Valley. " +
                        "Use them responsibly! Refer to your vehicle catalog for detailed specifications.",
                    FillType.JobType => $"This license grants you access to all orders of the {name} type. This order type involves [DESCRIPTION].",
                    _ => string.Empty
                };

                if (!string.IsNullOrEmpty(description.Value))
                {
                    EditorUtility.DisplayDialog("Attention",
                        "Please fill out the fields marked [LIKE THIS] in the description of the license!",
                        "I will thanks");
                }
            }
        }

        private enum LicenseColour
        {
            Generic,
            Locomotive,
            Concurrent,
            Hazmat,
            Military,
            Length
        }

        private enum FillType
        {
            NoDescription,
            SteamLocomotive,
            DELocomotive,
            DHLocomotive,
            DMLocomotive,
            JobType
        }
    }
}
