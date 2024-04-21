using Cysharp.Threading.Tasks;
using System.Threading;
using WATP.UI;

namespace WATP
{
    /// <summary>
    /// Maptool ¾À
    /// </summary>
    public class MaptoolScene : GameSceneBase
    {
        public override void Init()
        {
            Root.State.logicState.Value = LogicState.Parse;
        }

        public override void Complete()
        {
            Root.SoundManager.PlaySound(SoundTrack.BGM, "maptool", true);
        }

        public async override UniTask Load(CancellationTokenSource cancellationToken)
        {
            await OpenMaptoolPage();
            if (cancellationToken.IsCancellationRequested) return;
            Root.SceneLoader.TileMapManager.Clear();
        }

        public async override UniTask Unload(CancellationTokenSource cancellationToken)
        {
            Root.SceneLoader.TileMapManager.Clear();
            await UniTask.Yield(cancellationToken: cancellationToken.Token);
        }

        private async UniTask OpenMaptoolPage()
        {
            var ingamePage = new IngamePage();
            ingamePage = await Root.UIManager.Widgets.CreateAsync<IngamePage>(ingamePage, IngamePage.DefaultPrefabPath);
        }
    }
}