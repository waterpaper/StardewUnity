using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class TitlePage : PageWidget
    {
        RectTransform back1;
        RectTransform back2;
        RectTransform back3;
        RectTransform back4;

        Image logo;

        CanvasGroup btnGroup;
        Button backButton;
        Button startBtn;
        Button loadBtn;
        Button mapBtn;
        Button exitBtn;

        TweenerCore<float, float, FloatOptions> tween1, tween2, tween3, tween4;


        public static string DefaultPrefabPath { get => "TitlePage"; }

        protected override void OnInit()
        {
            back1 = rectTransform.RecursiveFindChild("Img_Background1", "BackGround");
            back2 = rectTransform.RecursiveFindChild("Img_Background2", "BackGround");
            back3 = rectTransform.RecursiveFindChild("Img_Background3", "BackGround");
            back4 = rectTransform.RecursiveFindChild("Img_Background4", "BackGround");

            logo = rectTransform.RecursiveFindChild("Img_Logo").GetComponent<Image>();

            backButton = rectTransform.RecursiveFindChild("Bt_Background").GetComponent<Button>();

            btnGroup = rectTransform.RecursiveFindChild("ButtonGroup").GetComponent<CanvasGroup>();
            startBtn = rectTransform.RecursiveFindChild("Bt_Start").GetComponent<Button>();
            loadBtn = rectTransform.RecursiveFindChild("Bt_Load").GetComponent<Button>();
            mapBtn = rectTransform.RecursiveFindChild("Bt_Map").GetComponent<Button>();
            exitBtn = rectTransform.RecursiveFindChild("Bt_Exit").GetComponent<Button>();

            Bind();
            OnLoadingPage().Forget();
            AnimStart();
        }

        protected override void OnDestroy()
        {
            UnBind();
        }

        private void AnimStart()
        {
            tween1 = DOTween.To(() => back1.anchoredPosition.y, y => back1.anchoredPosition = new Vector2(back1.anchoredPosition.x, y), -800.0f, 10f);
            tween2 = DOTween.To(() => back2.anchoredPosition.y, y => back2.anchoredPosition = new Vector2(back2.anchoredPosition.x, y), -400.0f, 6f);
            tween3 = DOTween.To(() => back3.anchoredPosition.y, y => back3.anchoredPosition = new Vector2(back3.anchoredPosition.x, y), -350.0f, 4.5f);
            tween4 = DOTween.To(() => back4.anchoredPosition.y, y => back4.anchoredPosition = new Vector2(back4.anchoredPosition.x, y), -300.0f, 3f);
            tween1.onComplete += () =>
            {
                AnimEnd();
            };

            btnGroup.interactable = false;
            startBtn.gameObject.SetActive(false);
            loadBtn.gameObject.SetActive(false);
            mapBtn.gameObject.SetActive(false);
            exitBtn.gameObject.SetActive(false);
        }

        private void AnimEnd()
        {
            backButton.interactable = false;
            DOTween.Clear();

            back1.anchoredPosition = new Vector2(back1.anchoredPosition.x, -800.0f);
            back2.anchoredPosition = new Vector2(back2.anchoredPosition.x, -400.0f);
            back3.anchoredPosition = new Vector2(back3.anchoredPosition.x, -350.0f);
            back4.anchoredPosition = new Vector2(back4.anchoredPosition.x, -300.0f);

            ButtonAnimStart().Forget();
        }

        private async UniTaskVoid ButtonAnimStart()
        {
            await UniTask.Delay(500);
            startBtn.gameObject.SetActive(true);
            await UniTask.Delay(500);
            loadBtn.gameObject.SetActive(true);
            await UniTask.Delay(500);
            mapBtn.gameObject.SetActive(true);
            await UniTask.Delay(500);
            exitBtn.gameObject.SetActive(true);

            ButtonAnimEnd();
        }

        private void ButtonAnimEnd()
        {
            btnGroup.interactable = true;
        }

        #region event

        private void Bind()
        {
            startBtn.onClick.AddListener(OnNewButton);
            loadBtn.onClick.AddListener(OnLoadButton);
            mapBtn.onClick.AddListener(OnMapButton);
            exitBtn.onClick.AddListener(OnExitButton);

            backButton.onClick.AddListener(OnBackgroundButton);
        }

        private void UnBind()
        {
            startBtn.onClick.RemoveAllListeners();
            loadBtn.onClick.RemoveAllListeners();
            mapBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
        }

        private void OnNewButton()
        {
            OnCustomPage().Forget();
        }

        private void OnLoadButton()
        {
            OnLoadPage().Forget();
        }

        private void OnMapButton()
        {
            OnMaptoolPage().Forget();
        }

        private void OnExitButton()
        {
            App.Quit();
        }

        private void OnBackgroundButton()
        {
            AnimEnd();
        }

        private async UniTaskVoid OnMaptoolPage()
        {
            var maptoolPage = new MaptoolPage();
            maptoolPage = await UIManager.Widgets.CreateAsync<MaptoolPage>(maptoolPage, MaptoolPage.DefaultPrefabPath);
        }

        private async UniTaskVoid OnLoadPage()
        {
            var inGameSaveLoadPage = new InGameSaveLoadPage();
            inGameSaveLoadPage = await UIManager.Widgets.CreateAsync<InGameSaveLoadPage>(inGameSaveLoadPage, InGameSaveLoadPage.DefaultPrefabPath);
            inGameSaveLoadPage.SetType(2);
        }

        private async UniTaskVoid OnCustomPage()
        {
            var customPage = new CustomPage();
            customPage = await UIManager.Widgets.CreateAsync<CustomPage>(customPage, CustomPage.DefaultPrefabPath);
        }

        private async UniTaskVoid OnLoadingPage()
        {
            var loadingPage = new LoadingPage();
            loadingPage = await UIManager.Widgets.CreateAsync<LoadingPage>(loadingPage, LoadingPage.DefaultPrefabPath, null, false, false);
            loadingPage.Hide();
        }
        #endregion

    }
}
