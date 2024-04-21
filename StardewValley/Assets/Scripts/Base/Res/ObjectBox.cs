using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WATP
{
    /// <summary>
    /// <see cref="AssetLoader.LoadAsync{T}"/>로 불러온 에셋을 담고 관리하는 클래스 입니다.
    /// 스마트 포인터 역할도 수행하며, 복사시 참조 카운트가 증가합니다. 
    /// </summary>
    internal sealed class ObjectBox
    {
        private int refCount;
        private UnityEngine.Object obj;

        private readonly string assetPath;

        /// <param name="assetPath"> 참조대상의 Asset addressable key값(key값의 기본값은 경로) 입니다.</param>
        internal ObjectBox(string assetPath)
        {
            this.assetPath = assetPath;
        }

        /// <summary>
        /// 참조 합니다.
        /// </summary>
        public async UniTask<T> ReferencingAsync<T>(CancellationTokenSource cancellationToken = null) where T : UnityEngine.Object
        {
            try
            {
                refCount++;

                if (obj == null && refCount == 1)
                {
                    var lhandle = Addressables.LoadAssetAsync<T>(assetPath);
                    obj = await lhandle.Task;
                }
                else if (obj == null && refCount > 1)
                {
                    await UniTask.WaitUntil(() => { return obj != null; });
                }

                //cancel
                if (cancellationToken != null && cancellationToken.IsCancellationRequested)
                {
                    Unreferencing(obj);
                    throw new Exception("Cancel");
                }


                if (obj is GameObject)
                {
                    //메모리 관련 오버헤드로 생성 및 해제 제한 발생 가능(무시하고 addressable load 사용하고 싶을시 사용)
                    //var handle = Addressables.InstantiateAsync(assetPath);
                    //return await handle.ToUniTask() as T;

                    return GameObject.Instantiate(obj) as T;
                }
                else
                    return obj as T;
            }
            catch (InvalidKeyException e)
            {
                Debugger.LogError($"InvalidKeyException: {assetPath}");
                refCount--;
                throw new OperationCanceledException($"InvalidKeyException: {assetPath}");
            }
            catch (Exception e)
            {
                Debug.Log("error\n" + e);
                throw e;
            }
        }

        /// <summary>
        /// 참조 합니다.
        /// </summary>
        public T Referencing<T>() where T : UnityEngine.Object
        {
            try
            {
                refCount++;

                if (obj == null)
                {
                    var lhandle = Addressables.LoadAssetAsync<T>(assetPath);
                    obj = lhandle.WaitForCompletion();
                }


                if (obj is GameObject)
                {
                    //var handle = Addressables.InstantiateAsync(assetPath);
                    //return handle.WaitForCompletion() as T;

                    return GameObject.Instantiate(obj) as T;
                }
                else
                    return obj as T;
            }
            catch (InvalidKeyException e)
            {
                Debugger.LogError($"InvalidKeyException: {assetPath}");
                refCount--;
                throw new OperationCanceledException($"InvalidKeyException: {assetPath}");
            }
            catch (Exception e)
            {
                Debug.Log("error");
                throw e;
            }
        }

        /// <summary>
        /// 참조를 해제합니다.
        /// </summary>
        public async void Unreferencing<T>(T target)
            where T : UnityEngine.Object
        {
            try
            {
                if (target == null) throw new ArgumentNullException(nameof(target));

                refCount--;

                if (refCount > 0)
                {
                    //if (target is GameObject)
                    //    Addressables.ReleaseInstance(target as GameObject);

                    if (target is GameObject)
                        GameObject.Destroy(target);
                    target = null;
                    return;
                }

                if (target is GameObject)
                {
                    //Addressables.ReleaseInstance(target as GameObject);
                    GameObject.Destroy(target);
                }

                if (refCount < 0) refCount = 0;

                await UniTask.Delay(1000);
                if (refCount > 0 || this.obj == null) return;

                Addressables.Release(this.obj);
                this.obj = target = null;
            }
            catch(Exception e)
            {
                Debug.Log("error");
            }
        }
    }
}
