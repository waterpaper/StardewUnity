using Cysharp.Threading.Tasks;
using System.Threading;
using WATP.UI;

namespace WATP
{
    /// <summary>
    /// custom splash ¾À
    /// </summary>
    public class SplashScene : GameSceneBase
    {
        public override void Init()
        {
        }

        public override void Complete()
        {
        }

        public async override UniTask Load(CancellationTokenSource cancellationToken)
        {
            await OpenSplashPage();
        }

        public override bool IsLoadingPage => false;

        private async UniTask OpenSplashPage()
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