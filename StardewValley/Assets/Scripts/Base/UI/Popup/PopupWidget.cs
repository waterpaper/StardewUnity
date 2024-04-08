using System;
using UnityEngine;
using UnityEngine.UI;

using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace WATP.UI
{
    public abstract class PopupWidget : Widget
    {
        protected Button closeButton;
        protected Image blockerImg;
        protected GameObject blurObject;

        protected Canvas canvas;
        protected CanvasGroup canvasGroup;
        protected CanvasGroup popupCanvasGroup;

        protected int pageUID;

        private PopupOption popupOption = new();
        private bool isAvailClose = true;
        private bool isOpenAction = false, isCloseAction = false, isLast = false;

        public Canvas Canvas { get => canvas; }
        public CanvasGroup CanvasGroup { get => canvasGroup; }
        public bool IsOutClickClose { get => popupOption.isOutClickClose; set => popupOption.isOutClickClose = value; }
        public bool IsAlphaAni { get => popupOption.isAlphaAni; }
        public bool IsSizeAni { get => popupOption.isSizeAni; }
        public bool IsBlockerAni { get => popupOption.isBlockerAni; }
        public bool IsLast { get => isLast; }
        public bool IsBlur { get => popupOption.isBlur; }
        public bool IsCloseAction { get => isCloseAction; }
        public GameObject BlurObject { get => blurObject; }
        public bool IsAvailClose { get => isAvailClose; set => isAvailClose = value; }

        public int PageUID { get => pageUID; }



        public sealed override void Initialize(RectTransform rectTransform)
        {
            base.Initialize(rectTransform);

            if (rectTransform.GetComponent<CanvasGroup>())
                popupCanvasGroup = rectTransform.GetComponent<CanvasGroup>();
            else
                popupCanvasGroup = rectTransform.gameObject.AddComponent<CanvasGroup>();

            OnInit();
            PopupOptionSetting(popupOption);
        }

        public sealed override void Dispose()
        {
            DOTween.Kill(this);
            if (canvas != null) GameObject.Destroy(canvas.gameObject);
            base.Dispose();
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

        public override async UniTask<Transform> LoadAsync(string customPrefabPath, Transform parent)
        {
            this.parent = parent.GetComponent<RectTransform>();

            var obj = await AssetLoader.InstantiateAsync(customPrefabPath, parent);
            name = obj.name;
            PrefabPath = customPrefabPath;

            Initialize(obj.GetComponent<RectTransform>());

            if (isDestroy)
                return null;

            return obj.transform;
        }

        public async UniTask LoadPopup()
        {
            await LoadPopupContents();
            OnLoad();
        }

#pragma warning disable CS1998 
        protected virtual async UniTask LoadPopupContents()
#pragma warning restore CS1998
        {

        }

        public void SetPageUID(int pageUID)
        {
            this.pageUID = pageUID;
        }

        /// <summary>
        /// popup open 설정
        /// </summary>
        public void OpenPopup()
        {
            canvas.GetComponent<RectTransform>().SetAsLastSibling();
            AlphaTween(() => { isOpenAction = true; isLast = true; });
            SizeTween();
            BlockerTween();
            OnOpen();
            Show();
        }

        /// <summary>
        /// popup close 설정
        /// 같은 page에서는 이 함수로 제거 권장
        /// </summary>
        public void ClosePopup()
        {
            this.ClosePopup(false);
        }

        /// <summary>
        /// popup close 설정
        /// 같은 page에서는 이 함수로 제거 권장
        /// </summary>
        public void ClosePopup(bool isOutClick)
        {
            if (isAvailClose == false) return;
            if (isCloseAction || isOpenAction == false) return;

            isCloseAction = true;
            UIManager.Widgets.StartCloseAction(this);
            AlphaTween(RemovePopup, false);
            BlockerTween(false);
            OnClose();
        }

        /// <summary>
        /// popup remove 설정
        /// 즉시 제거시 사용
        /// </summary>
        public void RemovePopup()
        {
            UIManager.Widgets.Remove(this);
        }

        /// <summary>
        /// 아웃 클릭 제거 이벤트
        /// </summary>
        /// <param name="isOutClick"></param>
        protected virtual void OutClickPopup()
        {
            OnOutClick();

            if (IsOutClickClose)
                ClosePopup(true);
        }

        /// <summary>
        /// popup이 최 상단에 올라갈때 실행
        /// </summary>
        public void OverPopup(bool isBeforeBlockerAni)
        {
            this.isLast = true;
            if (IsBlockerAni == false) return;

            if (isBeforeBlockerAni)
                DOTween.To(() => blockerImg.color, x => blockerImg.color = x, popupOption.blockerColor, 0.2f);
            else
                blockerImg.color = popupOption.blockerColor;
        }


        /// <summary>
        /// popup이 최 상단에서 내려갈때 실행
        /// </summary>
        public void UnderPopup(bool isBeforeBlockerAni)
        {
            this.isLast = false;
            if (IsBlockerAni == false) return;

            if (isCloseAction) return;

            if (isBeforeBlockerAni)
                DOTween.To(() => blockerImg.color, x => blockerImg.color = x, new Color(0, 0, 0, 0), 0.2f);
            else
                blockerImg.color = new Color(0, 0, 0, 0);
        }

        /// <summary>
        /// canvas get
        /// </summary>
        /// <param name="canvasRect"></param>
        public void SetCanvas(RectTransform canvasRect)
        {
            canvas = canvasRect.GetComponent<Canvas>();
            canvasGroup = canvasRect.GetComponent<CanvasGroup>();

            if (canvas == null) return;

            if (popupOption.isBlur)
            {
                blurObject = new GameObject("Blur");
                RectTransform blurRect = blurObject.AddComponent<RectTransform>();
                blurObject.transform.SetParent(canvasRect, false);
                blurRect.anchorMin = Vector3.zero;
                blurRect.anchorMax = Vector3.one;
                blurRect.sizeDelta = Vector2.zero;
                blurRect.SetAsFirstSibling();

                Image blurImage = blurObject.AddComponent<Image>();
                blurImage.color = new Color(1, 1, 1, 1);
                blurImage.material.shader = Shader.Find("Custom/UI/Blur Fast");
            }

            GameObject blockObject = new GameObject("Blocker");
            RectTransform blockRect = blockObject.AddComponent<RectTransform>();
            blockObject.transform.SetParent(canvasRect, false);
            blockRect.anchorMin = Vector3.zero;
            blockRect.anchorMax = Vector3.one;
            blockRect.sizeDelta = Vector2.zero;

            // Add image since it's needed to block, but make it clear.
            Image blockerImage = blockObject.AddComponent<Image>();
            blockerImage.color = new Color(0, 0, 0, 0);

            // Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
            Button blockerButton = blockObject.AddComponent<Button>();


            popupOption.orderIndex = canvas.sortingOrder;
            blockerImage.color = popupOption.isBlockerAni ? popupOption.blockerStartColor : popupOption.blockerColor;
            blockerButton.onClick.AddListener(OutClickPopup);

            this.blockerImg = blockerImage;
            rectTransform.SetAsLastSibling();

            canvasGroup.interactable = canvasGroup.blocksRaycasts = popupOption.isInteraction;
        }

        /// <summary>
        /// popup alpha tween
        /// </summary>
        /// <param name="endAction"></param>
        /// <param name="isOpen"></param>
        protected virtual void AlphaTween(Action endAction, bool isOpen = true)
        {
            if (IsAlphaAni == false)
            {
                endAction();
                return;
            }

            float startSize = isOpen ? popupOption.alphaStart : 1.0f;
            float endSize = isOpen ? 1.0f : popupOption.alphaStart;

            var tween = DOTween.To(() => startSize, x => popupCanvasGroup.alpha = x, endSize, 0.2f);
            tween.onComplete += () =>
            {
                endAction();
            };
        }

        /// <summary>
        /// popup size tween
        /// </summary>
        protected virtual void SizeTween(bool isOpen = true)
        {
            if (IsSizeAni == false) return;

            Vector3 startSize = isOpen ? popupOption.sizeStart : Vector3.one;
            Vector3 endSize = isOpen ? Vector3.one : popupOption.sizeStart;
            rectTransform.localScale = startSize;
            var tween = DOTween.To(() => startSize, x => rectTransform.localScale = x, endSize, 0.2f);
        }

        /// <summary>
        /// popup blocker tween
        /// </summary>
        protected virtual void BlockerTween(bool isOpen = true)
        {
            if (IsBlockerAni == false) return;

            Color startColor = isOpen ? popupOption.blockerStartColor : popupOption.blockerColor;
            Color endColor = isOpen ? popupOption.blockerColor : popupOption.blockerStartColor;

            var tween = DOTween.To(() => startColor, x => blockerImg.color = x, endColor, 0.15f);
        }

        /// <summary>
        /// set popup option setting
        /// </summary>
        protected virtual void PopupOptionSetting(PopupOption popupOption)
        {
        }

        protected abstract void OnInit();

        protected virtual void OnOpen() { }
        protected virtual void OnOutClick() { }
        protected virtual void OnClose() { }
    }
}
