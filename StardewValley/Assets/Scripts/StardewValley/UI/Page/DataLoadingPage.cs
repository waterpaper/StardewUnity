using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

namespace WATP.UI
{
    public class DataLoadingPage : PageWidget
    {
        public static string DefaultPrefabPath { get => "DataLoadingPage"; }

        protected override void OnLoad()
        {
            Loading.loadingDone += OnLoadingDone;
            Loading.LoadingProcess(Root.Mono);
        }

        protected override void OnDestroy()
        {
            Loading.loadingDone -= OnLoadingDone;
        }

        private void OnLoadingDone()
        {
            OpenTitlePage().Forget();
        }

        private async UniTaskVoid OpenTitlePage()
        {
            var titlePage = new TitlePage();
            titlePage = await UIManager.Widgets.CreateAsync<TitlePage>(titlePage, TitlePage.DefaultPrefabPath);
            UIManager.Widgets.Remove(this);
        }
    }
}
