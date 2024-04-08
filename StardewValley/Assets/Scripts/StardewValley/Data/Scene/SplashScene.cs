using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using WATP.UI;

namespace WATP
{
    public class SplashScene : GameSceneBase
    {
        public override void Init()
        {
        }

        public override void Complete()
        {
        }

        public override IEnumerator Load()
        {
            yield return OpenSplashPage();
        }

        public override IEnumerator Unload()
        {
            yield return null;
        }

        public override bool IsLoadingPage => false;

        private async UniTaskVoid OpenSplashPage()
        {
            var splashPage = new SplashScreen();
            splashPage = await Root.UIManager.Widgets.CreateAsync<SplashScreen>(splashPage, SplashScreen.DefaultPrefabPath);

            splashPage.splashDone += () => {
                OnSplashDone();
                splashPage.splashDone = null;
            };
        }

        private void OnSplashDone()
        {
            Root.SceneLoader.SceneLoad(SceneKind.Title);
        }
    }
}