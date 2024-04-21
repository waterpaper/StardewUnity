using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace WATP.UI
{
    /// <summary>
    /// 기본 ui page widget
    /// </summary>
    public abstract class PageWidget : Widget
    {
        protected Button closeButton;

        protected Canvas canvas;
        protected CanvasGroup canvasGroup;

        protected bool isBlur;
        protected int backUID;
        protected GameObject blurObject;

        public Canvas Canvas { get => canvas; }
        public CanvasGroup CanvasGroup { get => canvasGroup; }
        public bool IsBlur { get => isBlur; }
        public GameObject BlurObject { get => blurObject; }
        public int BackUID { get => backUID; }

        public PageWidget() { }

        public sealed override void Initialize(RectTransform rectTransform)
        {
            base.Initialize(rectTransform);
            RectTransform.anchorMin = Vector2.zero;
            RectTransform.anchorMax = Vector2.one;
            RectTransform.offsetMin = Vector2.zero;
            RectTransform.offsetMax = Vector2.zero;

            OnInit();
            closeButton?.onClick.AddListener(BackPage);
        }

        public sealed override void Dispose()
        {
            if (canvas != null) GameObject.Destroy(canvas.gameObject);
            base.Dispose();
        }

        public void BackPageUID(int backUID)
        {
            this.backUID = backUID;
        }

        /// <summary>
        /// page close 설정
        /// 이 함수로 제거 권장
        /// </summary>
        public void ClosePage()
        {
            UIManager.Widgets.Remove(this);
        }

        public void BackPage()
        {
            if (backUID == 0) return;
            UIManager.Widgets.BackPage(this);
        }


        public override void Show()
        {
            base.Show();
            canvas?.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();
            canvas?.gameObject.SetActive(false);
        }

        public override Transform Load(string customPrefabPath, Transform parent)
        {
            this.parent = parent.GetComponent<RectTransform>();

            var obj = AssetLoader.Instantiate(customPrefabPath, parent);
            name = obj.name;
            PrefabPath = customPrefabPath;

            Initialize(obj.GetComponent<RectTransform>());

            if (isDestroy)
                return null;

            return obj.transform;
        }

        public override async UniTask<Transform> LoadAsync(string customPrefabPath, Transform parent, CancellationTokenSource cancellationToken = null)
        {
            this.parent = parent.GetComponent<RectTransform>();

            var obj = await AssetLoader.InstantiateAsync(customPrefabPath, parent, default, default, default, cancellationToken);
            name = obj.name;
            PrefabPath = customPrefabPath;

            Initialize(obj.GetComponent<RectTransform>());

            if (isDestroy)
                return null;

            return obj.transform;
        }

        public void SetCanvas(RectTransform canvasRect)
        {
            canvas = canvasRect.GetComponent<Canvas>();
            canvasGroup = canvasRect.GetComponent<CanvasGroup>();

            if (isBlur)
            {
                blurObject = new GameObject("Blur");
                RectTransform blurRect = blurObject.AddComponent<RectTransform>();
                blurObject.transform.SetParent(canvasRect, false);
                blurRect.anchorMin = Vector3.zero;
                blurRect.anchorMax = Vector3.one;
                blurRect.sizeDelta = Vector2.zero;

                blurRect.anchoredPosition3D = new Vector3(blurRect.anchoredPosition.x, blurRect.anchoredPosition.y, 100f);

                /* WATPImage blurImage = blurObject.AddComponent<WATPImage>();
                 blurImage.color = new Color(1, 1, 1, 1);
                 blurImage.material = BaseDataManager.BaseData.GetMaterial("BlurMaterial");
                 blurRect.SetAsFirstSibling();*/
            }
        }

        public async UniTask LoadPage()
        {
            await LoadPageContents();
            OnLoad();
        }

        /// <summary>
        /// set page info
        /// </summary>
        public virtual void PageInfoSetting()
        {

        }

#pragma warning disable CS1998
        protected virtual async UniTask LoadPageContents()
#pragma warning restore CS1998
        {
        }



        protected virtual void OnInit() { }
    }
}
