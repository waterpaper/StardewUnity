using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace WATP.UI
{
    public class SplashScreen : Widget
    {
        public static string DefaultPrefabPath { get => "Page/SplashPage"; }

        private float fadeInOutDuration = 1.5f;
        private float stayDuration = 1;
        private CanvasGroup canvasGroup;

        private delegate void FadeAction();
        private FadeAction handler;

        public Action splashDone;
        public bool IsStart { get; private set; }
        public bool IsSplashEnd { get; private set; }

        private float timer = 0f;

        public SplashScreen()
        {
            pathDefault = true;
        }

        protected override void OnLoad()
        {
            canvasGroup = rectTransform.GetComponent<CanvasGroup>();

            FadeAlpha(0);
            handler = FadeIn;
            SplashStart();
        }

        protected override void OnDestroy()
        {
        }


        protected override void OnUpdate()
        {
            if (IsStart == false || IsSplashEnd == true) return;

            timer += Time.deltaTime;
            handler?.Invoke();
        }

        private void FadeIn()
        {
            if (timer < fadeInOutDuration)
            {
                FadeAlpha(timer / fadeInOutDuration);
            }
            else
            {
                FadeAlpha(1);
                timer -= fadeInOutDuration;
                handler = FadeStay;
                handler();
            }
        }

        private void FadeStay()
        {
            if (timer > stayDuration)
            {
                timer -= stayDuration;
                handler = FadeOut;
                handler();
            }
        }

        private void FadeOut()
        {
            if (timer < fadeInOutDuration)
                FadeAlpha(1 - timer / fadeInOutDuration);
            else
            {
                FadeAlpha(0);
                SplashEnd();
            }
        }


        private void FadeAlpha(float a)
        {
            canvasGroup.alpha = a;
        }


        public void SplashStart()
        {
            IsStart = true;
            IsSplashEnd = false;
            //StartCoroutine(SplashLoad());
        }

       /* private IEnumerator SplashLoad()
        {
        }
       */

        private void SplashEnd()
        {
            IsSplashEnd = true;
            splashDone?.Invoke();
        }
    }
}
