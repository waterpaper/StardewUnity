using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using WATP.UI;

namespace WATP
{
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

        public override IEnumerator Load()
        {
            if(Loading.isFirst)
                yield return OpenLoadingPage();
            else
                yield return OpenTitlePage();
        }

        public override IEnumerator Unload()
        {
            yield return null;
        }

        public override bool IsLoadingPage => false;

        private async UniTaskVoid OpenTitlePage()
        {
            var titlePage = new TitlePage();
            titlePage = await Root.UIManager.Widgets.CreateAsync<TitlePage>(titlePage, TitlePage.DefaultPrefabPath);
        }

        private async UniTaskVoid OpenLoadingPage()
        {
            var dataLoadingPage = new DataLoadingPage();
            dataLoadingPage = await Root.UIManager.Widgets.CreateAsync<DataLoadingPage>(dataLoadingPage, DataLoadingPage.DefaultPrefabPath);

        }
    }
}