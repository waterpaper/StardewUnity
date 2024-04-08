using Cysharp.Threading.Tasks;
using System;
using System.Collections;
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

        public Action<SceneKind> onSceneChangeStart;
        public Action<float> onSceneChangeProgress;
        public Action onSceneChangeComplete;

        public bool IsLoad { get => isLoad; }
        public float Progress { get=> progress; private set {
                progress = value;
            }  
        }

        public void Dispose()
        {
            onSceneChangeStart = null;
            onSceneChangeProgress = null;
            onSceneChangeComplete = null;
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
            await SceneLoadCoroutine(scenekind);
        }

        #region scene loading

        /// <summary>
        /// 로드할 씬에 맞게 이벤트 실행
        /// </summary>
        private IEnumerator SceneLoadCoroutine(SceneKind scenekind)
        {
            Progress = 0.0f;
            onSceneChangeStart?.Invoke(scenekind);
            onSceneChangeProgress?.Invoke(Progress);
            yield return null;

            if (nowScene != null)
            {
                yield return nowScene.Unload();

                if (nowScene.GetSceneAssetName() != null)
                {
                    var beforeOp = SceneManager.UnloadSceneAsync(nowScene.GetSceneAssetName());

                    if (beforeOp.isDone != true)
                        yield return null;
                }
            }

            var unLoadResourceop = Resources.UnloadUnusedAssets();
            yield return new WaitForSeconds(0.2f);

            if (unLoadResourceop.isDone != true)
                yield return null;

            IScene scene = CreateScene(scenekind);

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
                    yield return null;
                }

                Progress = operation.progress;

                operation.allowSceneActivation = true;
                while (operation != null && !operation.isDone)
                    yield return null;

                var objs = SceneManager.GetSceneByName(scene.GetSceneAssetName()).GetRootGameObjects();
            }

            yield return scene.Load();
            Progress = 1.0f;
            onSceneChangeProgress?.Invoke(Progress);
            yield return new WaitForSeconds(0.5f);
            isLoad = false;
            scene.Complete();
            onSceneChangeComplete?.Invoke();
            nowScene = scene;
        }

        /// <summary>
        /// enum에 맞는 scene 생성
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
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