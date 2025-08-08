using UnityEngine;

namespace CL.Game
{
    internal class Utilities
    {
        private static Transform? s_holder;
        private static Transform Holder
        {
            get
            {
                if (s_holder == null)
                {
                    s_holder = new GameObject("[CL HOLDER]").transform;
                    s_holder.gameObject.SetActive(false);

                    Object.DontDestroyOnLoad(s_holder.gameObject);
                }

                return s_holder;
            }
        }

        public static T CreateMockPrefab<T>(T source)
             where T : Object
        {
            var result = Object.Instantiate(source, Holder);
            return result;
        }
    }
}
