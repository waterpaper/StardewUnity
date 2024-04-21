using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WATP
{
    public enum SceneKind { Splash, Title, Ingame, End };

    /// <summary>
    /// unity scene 이동을 처리하는 클래스
    /// </summary>
    public partial class SceneLoader
    {
        private bool isLoad = false;

        private IScene nowScene;
        private float progress = 1;
        private CancellationTokenSource token = new CancellationTokenSource();

        public Action<SceneKind> onSceneChangeStart;
        public Action<float> onSceneChangeProgress;
        public Action onSceneChangeComplete;

        public bool IsLoad { get => isLoad; }
        public float Progress
        {
            get => progress; private set
            {
                progress = value;
            }
        }

        public void Dispose()
        {
            onSceneChangeStart = null;
            onSceneChangeProgress = null;
            onSceneChangeComplete = null;
            token.Cancel();
            token.Dispose();
        }

        public void Update()
        {
            if (isLoad || nowScene == null) return;
            nowScene.Update();
        }


        /// <summary>
        /// 씬 로드를 시작하는 함수
        /// isclear - 씬을 변경하면서 ui 제거하는지 이벤트
        /// </summary>
        public async void SceneLoad(SceneKind scenekind, bool isClear = false)
        {
            if (isLoad)
            {
                Debug.Log("Already loading process logic");
                return;
            }

            isLoad = true;

            try
            {
                await SceneLoadAsync(scenekind);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #region scene loading

        /// <summary>
        /// 로드할 씬에 맞게 이벤트 실행
        /// </summary>
        private async UniTask SceneLoadAsync(SceneKind scenekind)
        {
            Progress = 0.0f;

            IScene scene = CreateScene(scenekind);
            onSceneChangeStart?.Invoke(scenekind);
            onSceneChangeProgress?.Invoke(Progress);

            if (await UnloadScene() == false)
                return;

            await UnloadResource();

            if (await LoadNowScene(scene) == false)
                return;

            Progress = 1.0f;
            onSceneChangeProgress?.Invoke(Progress);
            await UniTask.DelayFrame(1);
            isLoad = false;
            scene.Complete();
            onSceneChangeComplete?.Invoke();
            nowScene = scene;
        }

        /// <summary>
        /// enum에 맞는 scene 생성
        /// </summary>
        private IScene CreateScene(SceneKind kind)
        {
            IScene scene;
            switch (kind)
            {
                case SceneKind.Splash:
                    scene = new SplashScene();
                    break;
                case SceneKind.Title:
                    scene = new TitleScene();
                    break;
                case SceneKind.Ingame:
                    scene = new IngameScene();
                    break;
                case SceneKind.End:
                default:
                    scene = new SceneNull();
                    break;
            }

            scene.Init();

            return scene;
        }

        /// <summary>
        /// 현재 Scene load
        /// </summary>
        private async UniTask<bool> LoadNowScene(IScene scene)
        {
            try
            {
                //unity scene
                if (scene.GetSceneAssetName() != null)
                {
                    var operation = SceneManager.LoadSceneAsync(scene.GetSceneAssetName());
                    operation.allowSceneActivation = false;
                    operation.completed += (op) =>
                    {
                        SceneCameraSetting(SceneManager.GetSceneByName(scene.GetSceneAssetName()));
                    };

                    while (operation != null && operation.progress < 0.9f)
                    {
                        Progress = operation.progress;
                        onSceneChangeProgress?.Invoke(Progress);
                        await UniTask.Yield(cancellationToken: token.Token);
                    }

                    if (token.Token.IsCancellationRequested)
                        return false;

                    Progress = operation.progress;
                    operation.allowSceneActivation = true;
                    while (operation != null && !operation.isDone)
                        await UniTask.Yield(cancellationToken: token.Token);

                    if (token.Token.IsCancellationRequested)
                        return false;
                }

                await scene.Load(token);
                return true;
            }
            catch (Exception e)
            {
                throw new OperationCanceledException("Load Scene Error");
            }
        }

        /// <summary>
        /// 현재 Scene Unload
        /// </summary>
        private async UniTask<bool> UnloadScene()
        {
            try
            {
                if (nowScene != null)
                {
                    await nowScene.Unload(token);

                    if (nowScene.GetSceneAssetName() != null)
                    {
                        var beforeOp = SceneManager.UnloadSceneAsync(nowScene.GetSceneAssetName());

                        if (beforeOp.isDone != true)
                            await UniTask.Yield(cancellationToken: token.Token);
                    }
                }

                if (token.Token.IsCancellationRequested)
                    return false;

                return true;
            }
            catch (Exception e)
            {
                throw new OperationCanceledException("UnLoad Scene Error");
            }
        }

        /// <summary>
        /// Resource Unload
        /// </summary>
        private async UniTask UnloadResource()
        {
            var unLoadResourceop = Resources.UnloadUnusedAssets();
            if (unLoadResourceop.isDone != true)
                await UniTask.Yield(cancellationToken: token.Token);
        }


        /// <summary>
        /// scene에 존재하는 카메라의 고정 해상도를 처리한다
        /// </summary>
        private void SceneCameraSetting(UnityEngine.SceneManagement.Scene scene)
        {
            var rootObjects = scene.GetRootGameObjects();
            for (int i = 0; i < rootObjects.Length; i++)
            {
                var cameras = rootObjects[i].GetComponentsInChildren<Camera>();
                for (int j = 0; j < cameras.Length; j++)
                {
                    cameras[j].gameObject.AddComponent<CameraResolution>();
                }
            }
        }

        #endregion
    }
}