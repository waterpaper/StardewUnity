using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace WATP
{
    /// <summary>
    /// Addressable빌드용 코드
    /// 필요한 lable을 로드 할 수 있게 도와준다.
    /// </summary>
    public class AddressableLoader : IResLoader
    {
        readonly string[] downloadAssetsLabelKeys = { };

        long downBytes = 0;
        long nowBytes = 0;

        bool isInit = false;
        List<string> downKeys = new List<string>();
        List<long> downSizes = new List<long>();

        public long TotalDownSize
        {
            get
            {
                long totalSize = 0;
                for (int i = 0; i < downSizes.Count; i++)
                {
                    totalSize += downSizes[i];
                }

                return totalSize;
            }
        }

        public long NowBytes
        {
            get
            {
                return nowBytes + downBytes;
            }
        }

        public float Percent
        {
            get
            {
                if (TotalDownSize <= 0) return 1;
                return NowBytes / (float)TotalDownSize;
            }
        }

        public void Initialize()
        {
            var op = Addressables.InitializeAsync();

            op.Completed += (oper) =>
            {
                Addressables.Release(op);
                isInit = true;
            };
        }


        public IEnumerator DownCheckAssetsCoroutine(Action doneEvent = null, Action errorEvent = null)
        {
#if UNITY_EDITOR
            /* var playModeIndex = AddressableAssetSettings.BuildPlayerContent.ActivePlayModeDataBuilderIndex;
             if (playModeIndex == 0)
             {
                 doneEvent?.Invoke();
                 yield break;
             }*/
#endif

            var defaultOper = Addressables.DownloadDependenciesAsync("default");
            while (!defaultOper.IsDone)
                yield return null;

            for (int i = 0; i < downloadAssetsLabelKeys.Length; i++)
            {
                var op = Addressables.GetDownloadSizeAsync(downloadAssetsLabelKeys[i]);

                while (!op.IsDone)
                    yield return null;

                Debug.Log("Load size : " + op.Result);

                if (op.Status == AsyncOperationStatus.Succeeded && op.Result > 0)
                {
                    downKeys.Add(downloadAssetsLabelKeys[i]);
                    downSizes.Add(op.Result);
                }
                else if (op.Status == AsyncOperationStatus.Failed)
                {
                    errorEvent?.Invoke();
                    yield break;
                }
            }

            doneEvent?.Invoke();
            Addressables.Release(defaultOper);
            yield return null;
        }

        public IEnumerator DownAssetsCoroutine(Action<AsyncOperationHandle> progressEvent = null, Action errorEvent = null)
        {
            for (int i = 0; i < downKeys.Count; i++)
            {
                var op = Addressables.DownloadDependenciesAsync(downKeys[i]);

                while (!op.IsDone)
                {
                    downBytes = op.GetDownloadStatus().DownloadedBytes;
                    progressEvent(op);
                    yield return null;
                }

                if (op.Status == AsyncOperationStatus.Failed)
                {
                    Debug.Log("Load Error");
                    errorEvent?.Invoke();
                    yield break;
                }

                nowBytes += downSizes[i];
                downBytes = 0;
                Addressables.Release(op);
            }
        }


        void ShaderReExport(GameObject obj)
        {
#if UNITY_EDITOR && PLATFORM_ANDROID == false
            //var playModeIndex = AddressableAssetSettingsDefaultObject.Settings.ActivePlayModeDataBuilderIndex;
            //if (playModeIndex == 0)
            //    return;

            //if (obj == null) return;

            //obj.SetActive(false);
            //Renderer[] renderers = obj.transform.GetComponentsInChildren<Renderer>(true);

            //foreach (Renderer item in renderers)
            //{
            //    if (item.materials != null)
            //    {
            //        foreach (Material mat in item.materials)
            //        {
            //            Shader sha = mat.shader;
            //            sha = Shader.Find(sha.name);
            //            mat.shader = sha;
            //        }
            //    }
            //}
            //renderers = null;
            //obj.SetActive(true);
#endif
        }
    }
}
