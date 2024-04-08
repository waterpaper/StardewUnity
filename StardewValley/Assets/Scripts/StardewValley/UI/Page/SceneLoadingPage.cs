using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class SceneLoadingPage : PageWidget
    {
        public static string DefaultPrefabPath { get => "SceneLoadingPage"; }

        protected float objectTimer;
        protected float progressTimer;
        protected float progress;

        protected ProgressBar progressBar;
        protected Image background;
        protected Text progressText;
        protected Text progressTip;

        protected int loadingType = 0;
        protected string loadingImagePath = string.Empty;


        protected override void OnLoad()
        {
            progressBar = rectTransform.RecursiveFindChild("ProgressBar").GetComponent<ProgressBarSlice>();
            background = rectTransform.RecursiveFindChild("Background").GetComponent<Image>();
            progressText = rectTransform.RecursiveFindChild("ProgressText").GetComponent<Text>();
            progressTip = rectTransform.RecursiveFindChild("TipText").GetComponent<Text>();

            Bind();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            UnBind();
        }

        private void Bind()
        {
            Root.SceneLoader.onSceneChangeStart += OnOpen;
            Root.SceneLoader.onSceneChangeProgress += OnProgress;
            Root.SceneLoader.onSceneChangeComplete += OnClose;
        }

        private void UnBind()
        {
            Root.SceneLoader.onSceneChangeStart -= OnOpen;
            Root.SceneLoader.onSceneChangeProgress -= OnProgress;
            Root.SceneLoader.onSceneChangeComplete -= OnClose;

        }


        private void OnOpen(SceneKind kind)
        {
            Clear();
            LoadingSetting(kind);
            Show();
        }

        private void OnClose()
        {
            Hide();
        }

        private void OnProgress(float progress)
        {
            this.progress = progress;
        }


        protected override void OnUpdate()
        {
            objectTimer += Time.deltaTime;
            if (objectTimer > 15)
                LoadingSetting((SceneKind)loadingType);

            progressTimer += (Time.deltaTime > 0.3f ? 1.0f : Time.deltaTime);

            var progressPercent = Mathf.Lerp(progressBar.Amount, progress, progressTimer);

            if (progressPercent >= progress)
                progressTimer = 0f;

            progressBar.Progress(progressPercent);
            progressText.text = $"{(progressPercent * 100).ToString("F2")} %";
        }

        protected virtual void Clear()
        {
            progressTimer = 0f;
            progress = 0;
            objectTimer = 0;

            if (progress == 0)
                progressBar.Progress(0);
        }


        protected virtual void LoadingSetting(SceneKind kind)
        {
            loadingType = (int)kind;
            objectTimer = 0;
        }
    }
}