using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WATP
{
    /// <summary>
    /// 에셋을 로드 하는 클래스 입니다.
    /// </summary>
    public static class AssetLoader
    {
        private static readonly Dictionary<string, ObjectBox> _cache = new(128);

        public static void Initialize()
        {
            Clear();
        }

        public static void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// 해당 경로의 에셋을 로드합니다.<br/>
        /// 사용 이후 반드시 <see cref="Unload{T}"/>를 호출해야 합니다.
        /// </summary>
        public static async UniTask<T> LoadAsync<T>(string path, CancellationTokenSource cancellationToken = null)
            where T : UnityEngine.Object
        {
            if (!_cache.TryGetValue(path, out var asset))
            {
                _cache.Add(path, asset = new(path));
            }

            return await asset.ReferencingAsync<T>(cancellationToken);
        }
        
        /// <summary>
        /// 해당 경로의 에셋을 로드합니다.<br/>
        /// 사용 이후 반드시 <see cref="Unload{T}"/>를 호출해야 합니다.
        /// </summary>
        public static T Load<T>(string path)
            where T : UnityEngine.Object
        {
            if (!_cache.TryGetValue(path, out var asset))
            {
                _cache.Add(path, asset = new(path));
            }

            return asset.Referencing<T>() ;
        }

        /// <summary>
        /// 해당 경로의 에셋을 로드 한 뒤, 인스턴스를 생성합니다.<br/>
        /// 사용 이후 반드시 <see cref="Unload"/>를 호출해야 합니다.
        /// </summary>
        public static async UniTask<GameObject> InstantiateAsync(
            string path, Transform parent,
            Vector3 position = default,
            Quaternion rotation = default, 
            Vector3 scale = default, 
            CancellationTokenSource cancellationToken = null)
        {
            var temp = await LoadAsync<GameObject>(path, cancellationToken);

            temp.transform.localPosition = position.Equals(default) ? Vector3.zero : position;
            temp.transform.localRotation = rotation.Equals(default) ? Quaternion.identity : rotation;
            temp.transform.localScale = scale.Equals(default) ? Vector3.one : scale;
            if (parent != null) temp.transform.SetParent(parent, false);

            return temp;
        }
        
        /// <summary>
        /// 해당 경로의 에셋을 로드 한 뒤, 인스턴스를 생성합니다.<br/>
        /// 사용 이후 반드시 <see cref="Unload"/>를 호출해야 합니다.
        /// </summary>
        public static GameObject Instantiate(
            string path, Transform parent,
            Vector3 position = default,
            Quaternion rotation = default,
            Vector3 scale = default)
        {
            var temp = Load<GameObject>(path);
            temp.transform.localPosition = position.Equals(default) ? Vector3.zero : position;
            temp.transform.localRotation = rotation.Equals(default) ? Quaternion.identity : rotation;
            temp.transform.localScale = scale.Equals(default) ? Vector3.one : scale;
            if(parent != null) temp.transform.SetParent(parent, false);

            return temp;
        }

        /// <summary>
        /// 에셋 로드를 해제합니다.
        /// </summary>
        public static void Unload<T>(string path, T obj)
            where T : UnityEngine.Object
        {
            if (obj == null)
            {
                return;
            }

            if (!_cache.TryGetValue(path, out var asset))
            {
                return;
            }

            asset.Unreferencing(obj);
        }
    }
}
