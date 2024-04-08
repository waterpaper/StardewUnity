using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class UIManager
    {
        public const int MAX_ORDER = 9999;

        private RectTransform rectTransform;
        private Canvas uiCanvas;
        private Camera uiCamera;

        private InputHelper inputHelper;
        private WidgetContainer widgetContainer;


        public RectTransform RectTransform { get => rectTransform; }
        public Canvas Canvas { get => uiCanvas; }
        public Camera Camera { get => uiCamera; }
        public WidgetContainer Widgets { get => widgetContainer; }


        public void Initialize(Vector2Int resolution)
        {
            if (uiCanvas != null) return;

            var obj = new GameObject("[UI Canvas]");
            var canvas = obj.AddComponent<Canvas>();
            var scaler = obj.AddComponent<CanvasScaler>();
            obj.AddComponent<GraphicRaycaster>();
            obj.AddComponent<DontDestroyLoadObject>();

            var camera = new GameObject("[UI Camera]").AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Nothing;
            camera.orthographic = true;
            camera.cullingMask = 1 << 5;
            camera.depth = 2;
            camera.gameObject.AddComponent<CameraResolution>();
            camera.gameObject.AddComponent<DontDestroyLoadObject>();

            obj.layer = 5;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.sortingOrder = 3;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = resolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f;
            scaler.referencePixelsPerUnit = 100;

            uiCanvas = canvas;
            uiCamera = camera;
            rectTransform = canvas.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            UIElement.SetManager(this);
            widgetContainer = new();
            inputHelper = new();

            widgetContainer.Initialize(this, rectTransform);
            inputHelper.Initialize();

            Bind();
        }

        public void Destroy()
        {
            widgetContainer.Destroy();
            inputHelper.Destroy();
            UnBind();
        }

        public void Update()
        {
            widgetContainer.Update();
            inputHelper.Update();
        }


        protected virtual void Bind()
        {
            UIController.ServiceEvents.On<UIException>(OnUIException);
        }

        protected virtual void UnBind()
        {
            UIController.ServiceEvents.Off<UIException>(OnUIException);
        }

        private async void OnUIException(UIException e)
        {
            Debugger.Log("UI 에러가 발생햇습니다.");
        }
    }
}
