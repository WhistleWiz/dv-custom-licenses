using DV.ThingTypes;
using System.Reflection;

namespace CL.Game
{
    internal static class ReflectionHelper
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private static FieldInfo? s_genField;
        private static FieldInfo? s_jobField;

        public static void AssignAvailability(CustomGeneralLicenseV2 license)
        {
            if (s_genField == null)
            {
                var t = typeof(GeneralLicenseType_v2);
                s_genField = t.GetField("freeRoamAvailability", Flags);
            }

            s_genField.SetValue(license, (FreeRoamAvailability)license.Original.Availability);
        }

        public static void AssignAvailability(CustomJobLicenseV2 license)
        {
            if (s_jobField == null)
            {
                var t = typeof(JobLicenseType_v2);
                s_jobField = t.GetField("freeRoamAvailability", Flags);
            }

            s_jobField.SetValue(license, (FreeRoamAvailability)license.Original.Availability);
        }
    }
}
