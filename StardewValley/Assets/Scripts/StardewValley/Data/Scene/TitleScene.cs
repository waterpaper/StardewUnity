using Cysharp.Threading.Tasks;
using System.Threading;
using WATP.UI;

namespace WATP
{
    /// <summary>
    /// Title 씬
    /// AddressableLoad process를 실행한다 
    /// </summary>
    public class TitleScene : GameSceneBase
    {
        public override void Init()
        {
            Root.State.logicState.Value = LogicState.Parse;
        }

        public override void Complete()
        {
            Root.SoundManager.PlaySound(SoundTrack.BGM, "opening", true);
        }

        public async override UniTask Load(CancellationTokenSource cancellationToken)
        {
            AddressableLoader loader = new AddressableLoader();
            loader.Initialize();
            await loader.DownCheckAssetsCoroutine();

            if (Loading.isFirst)
                await OpenLoadingPage();
            else
                await OpenTitlePage();
        }

        public async override UniTask Unload(CancellationTokenSource cancellationToken)
        {
            await UniTask.Yield(cancellationToken: cancellationToken.Token);
        }

        public override bool IsLoadingPage => false;

        private async UniTask OpenTitlePage()
        {
            var titlePage = new TitlePage();
            titlePage = await Root.UIManager.Widgets.CreateAsync<TitlePage>(titlePage, TitlePage.DefaultPrefabPath);
        }

        private async UniTask OpenLoadingPage()
        {
            var dataLoadingPage = new DataLoadingPage();
            dataLoadingPage = await Root.UIManager.Widgets.CreateAsync<DataLoadingPage>(dataLoadingPage, DataLoadingPage.DefaultPrefabPath);

        }
    }
}