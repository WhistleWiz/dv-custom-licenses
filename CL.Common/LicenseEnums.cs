using UnityEngine;

namespace CL.Common
{
    public enum LicenseType
    {
        [Tooltip("General licenses are used for locomotives, generic utility like manual service or concurrent jobs, and most mod functionality")]
        General,
        [Tooltip("Job licenses are the ones that show up on job papers, like job type, hazmat and military cargo, or train length\n" +
            "Please be aware there's a limit of 21 JOB LICENSES that can be active at a given time")]
        Job
    }

    public enum FreeRoamAvailability
    {
        OnlyIfUnlockedInCareer,
        Always,
        Never
    }
}
