using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class LoadingPage : PageWidget
    {
        public static string DefaultPrefabPath { get => "LoadingPage"; }

        protected Image background;
        protected RectTransform loadingPanel;
        protected Text text;

        protected int objectIndex = 1;
        protected float objectTimer;
        protected float fadeTimer;
        protected bool isFade;
        protected bool isScene;


        protected override void OnLoad()
        {
            background = rectTransform.RecursiveFindChild("Background").GetComponent<Image>();
            loadingPanel = rectTransform.RecursiveFindChild("Img_Fixed");
            text = rectTransform.RecursiveFindChild("Txt_Loading").GetComponent<Text>();

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
            Root.SceneLoader.onSceneChangeComplete += OnClose;

            Root.SceneLoader.TileMapManager.onMapChangeStart += OnOpen;
            Root.SceneLoader.TileMapManager.onMapChangeEnd += OnClose;
        }

        private void UnBind()
        {
        }


        private void OnOpen(string mapName)
        {
            if (!isScene) return;
            Clear();
            Show();
            Root.State.logicState.Value = LogicState.Parse;
        }


        private void OnOpen(SceneKind kind)
        {
            isScene = SceneKind.Ingame == kind;

            if (!isScene) return;
            Clear();
            Show();
        }

        private void OnClose()
        {
            if (!isScene) return;
            isFade = true;
        }

        private void OnClose(string mapName)
        {
            if (!isScene) return;
            isFade = true;
        }

        protected override void OnUpdate()
        {
            if(isFade)
            {
                FadeOut();
                return;
            }

            objectTimer += Time.deltaTime;
            if (objectTimer > 0.5f)
                objectIndex = (objectIndex >= 3) ? 1 : objectIndex + 1;
            else
                return;

            objectTimer -= 1;

            text.text = "·ÎµùÁß";
            for (int i = 0; i < objectIndex; i++)
                text.text += ".";
        }

        protected virtual void Clear()
        {
            objectTimer = 1;
            fadeTimer = 0;
            isFade = false;
            objectIndex = 0;

            loadingPanel.gameObject.SetActive(true);

            if (background != null)
            background.color = new Color(background.color.r, background.color.g, background.color.b, 1);
        }

        private void FadeOut()
        {
            fadeTimer += Time.deltaTime;
            if (fadeTimer < 0.5f)
                background.color = new Color(background.color.r, background.color.g, background.color.b, 1);
            else if (fadeTimer < 1.5f)
            {
                loadingPanel.gameObject.SetActive(false);
                background.color = new Color(background.color.r, background.color.g, background.color.b, 1 - fadeTimer / 1.5f);

                if (isScene && Root.State.logicState.Value != LogicState.Normal)
                    Root.State.logicState.Value = LogicState.Normal;
            }
            else
            {
                background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
                Hide();
                isFade = false;
            }
        }


    }
}