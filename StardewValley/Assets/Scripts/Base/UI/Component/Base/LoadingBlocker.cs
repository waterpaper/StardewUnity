using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class LoadingBlocker : Widget
    {
        public static string DefaultPrefabPath { get => "LoadingBlocker"; }

        [SerializeField]
        Image circleIcon;
        private float rotateSpeed = 200f;

        public LoadingBlocker()
        {
            pathDefault = true;
        }

        public void Init()
        {
            circleIcon.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        protected override void OnLoad()
        {
            circleIcon = rectTransform.RecursiveFindChild("LoadingIcon").GetComponent<Image>();
            Init();
        }

        protected override void OnUpdate()
        {
            circleIcon.transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        }
    }
}

