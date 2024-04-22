using UnityEngine;

namespace CL.Game
{
    internal class Utilities
    {
        public static GameObject CreateMockPrefab(GameObject source)
        {
            bool sourceActive = source.activeSelf;
            source.SetActive(false);

            var result = Object.Instantiate(source);
            Object.DontDestroyOnLoad(result);

            source.SetActive(sourceActive);

            return result;
        }
    }
}
