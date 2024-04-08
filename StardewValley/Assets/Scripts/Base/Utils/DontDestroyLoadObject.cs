using UnityEngine;

namespace WATP
{
    public class DontDestroyLoadObject : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
