using UnityEngine;

namespace WATP.Structure
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;

        public static T Instance
        {
            get
            {
                if (object.ReferenceEquals(instance, null))
                {
                    Init();
                }

                return instance;
            }
        }

        protected static void Init()
        {
            instance = (T)FindObjectOfType(typeof(T));

            if (instance == null)
            {
                Debug.LogError("An instance of " + typeof(T) +
                    " is needed in the scene, but there is none.");
            }
        }
    }
}