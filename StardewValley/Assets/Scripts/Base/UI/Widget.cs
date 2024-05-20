using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace WATP.UI
{
    /// <summary>
    /// widget의 기본 정보를 갖는 추상 클래스
    /// 필요 함수를 override해 사용 가능하다.
    /// </summary>
    public abstract class Widget : UIElement, IPrefabHandler
    {
        #region static

        private static readonly Vector3 DEFAULT_START_POSITION = Vector3.zero;
        private static readonly Vector3 DEFAULT_START_ROTATION = Vector3.zero;
        private static readonly Vector3 DEFAULT_START_SCALE = Vector3.one;
        private static readonly float DEFAULT_START_ALPHA = 1.0f;

        #endregion


        protected Vector3 startPosition = DEFAULT_START_POSITION;
        protected Vector3 startRotation = DEFAULT_START_ROTATION;
        protected Vector3 startScale = DEFAULT_START_SCALE;
        protected float startAlpha = DEFAULT_START_ALPHA;

        protected string name;
        protected RectTransform parent;

        protected bool pathDefault = false;
        protected bool isShow = true;
        protected bool isDestroy = false;

        private bool isPrefab = false;
        private bool isStart = false;

        public string Name { get => name; }
        public bool IsShow { get => isShow; }
        public bool IsDestroy { get => isDestroy; }
        public string PrefabPath { get; protected set; }
        public Transform Parent { get => parent; }

        public bool PathDefault { get => pathDefault; }

        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);
            isPrefab = true;

            UpdateStartPosition();
            UpdateStartRotation();
            UpdateStartScale();
            UpdateStartAlpha();
        }

        public override async void Dispose()
        {
            if (isDestroy == true) return;
            isDestroy = true;

            await UniTask.WaitUntil(() => { return isPrefab; });
            AssetLoader.Unload(PrefabPath, RectTransform == null ? null : RectTransform.gameObject);

            OnDestroy();
            base.Dispose();
        }

        public void Update()
        {
            if(isStart == false)
            {
                OnStart();
                isStart = true;
            }

            OnUpdate();
        }

        public virtual Transform Load(string customPrefabPath, Transform parent)
        {
            this.parent = parent.GetComponent<RectTransform>();

            var obj = AssetLoader.Instantiate(customPrefabPath, parent);
            if (obj == null)
            {
                throw new Exception("not prefab");
            }

            name = obj.name;
            PrefabPath = customPrefabPath;

            Initialize(obj.GetComponent<RectTransform>());
            OnLoad();

            if (isDestroy)
            {
                return null;
            }

            return obj.transform;
        }

        public virtual async UniTask<Transform> LoadAsync(string customPrefabPath, Transform parent)
        {
            this.parent = parent.GetComponent<RectTransform>();

            var obj = await AssetLoader.InstantiateAsync(customPrefabPath, parent);
            if(obj == null)
            {
                throw new Exception("not prefab");
            }

            name = obj.name;
            PrefabPath = customPrefabPath;
            isPrefab = true;

            Initialize(obj.GetComponent<RectTransform>());
            OnLoad();

            if (isDestroy)
            {
                return null;
            }

            return obj.transform;
        }

        public virtual void Show()
        {
            if (isShow == true) return;
            rectTransform.gameObject.SetActive(true);
            isShow = true;
        }

        public virtual void Hide()
        {
            if (isShow == false) return;
            rectTransform.gameObject.SetActive(false);
            isShow = false;
        }

        protected virtual void UpdateStartPosition()
        {
            startPosition = RectTransform.anchoredPosition3D;
        }

        protected virtual void UpdateStartRotation()
        {
            startRotation = RectTransform.localEulerAngles;
        }

        protected virtual void UpdateStartScale()
        {
            Vector3 localScale = RectTransform.localScale;
            startScale = new Vector3(localScale.x, localScale.y, 1f);
        }

        protected virtual void UpdateStartAlpha()
        {
            startAlpha = RectTransform.GetComponent<CanvasGroup>() == null ? 1 : RectTransform.GetComponent<CanvasGroup>().alpha;
        }

        protected virtual void OnLoad()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnDestroy()
        {
        }
    }
}