using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using WATP.ECS;
using UnityEngine;

namespace WATP.View
{
    [Serializable]
    public abstract class View<T> : IView, IPrefabHandler where T : IEntity
    {
        protected T entity;
        protected int uid;

        public int UID { get => uid; }
        public T Entity { get => entity; }
        public bool CanShow { get; private set; }
        public bool IsShow { get; private set; }

        public string PrefabPath { get; protected set; }
        public Transform Parent { get; protected set; }
        public Transform Transform { get; protected set; }
        public bool IsAlreadyDisposed { get=> isAlreadyDisposed; }


        //<summary> Disposed 되엇는지 여부 </summary>
        protected bool isAlreadyDisposed = false;
        //<summary> prefab 생성 여부 </summary>
        protected bool isPrefab = false;


        private Dictionary<string, Transform> childs = new();

        protected abstract void OnLoad();
        protected abstract void OnRender();
        protected abstract void OnDestroy();

        public virtual async UniTask<Transform> LoadAsync(string customPrefabPath, Transform parent)
        {
            if (!isPrefab)
            {
                PrefabPath = customPrefabPath ?? "";
                Transform = new GameObject().transform;
                Transform.SetParent(parent, true);
                Transform.position = entity.TransformComponent.position;

                await AssetLoader.InstantiateAsync(PrefabPath, Transform);
                isPrefab = true;

                if (!isAlreadyDisposed && entity != null)
                    OnLoad();
            }

            return isAlreadyDisposed && entity != null ? null : Transform;
        }

        public virtual void Load(string customPrefabPath, Transform parent)
        {
            if (!isPrefab)
            {
                Transform = new GameObject().transform;
                Transform.SetParent(parent, true);
                Transform.position = entity.TransformComponent.position;

                PrefabPath = customPrefabPath ?? "";
                AssetLoader.Instantiate(PrefabPath, Transform);
                isPrefab = true;

                /*if (Owner?.Name != null)
                {
                    Transform.name = Owner.Name;
                }*/

                if (!isAlreadyDisposed && entity != null)
                    OnLoad();
            }
        }

        public async void Dispose()
        {
            if (!isAlreadyDisposed)
            {
                isAlreadyDisposed = true;
                OnDestroy();
                childs = null;
                entity = default(T);
            }

            await UniTask.WaitUntil(() => { return isPrefab; });

            if (isPrefab)
            {
                isPrefab = false;
                AssetLoader.Unload(PrefabPath, Transform == null || Transform.childCount == 0 ? null : Transform.GetChild(0).gameObject);
                if (Transform != null)
                    GameObject.Destroy(Transform.gameObject);
                /* if ((object)Transform.gameObject != null)
                     UnityEngine.GameObject.Destroy(Transform.gameObject);*/

                GC.SuppressFinalize(this);
            }
        }

        public void Show()
        {
            if (isAlreadyDisposed) return;
            if (!isPrefab) return;

            Transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (isAlreadyDisposed) return;
            if (!isPrefab) return;

            Transform.gameObject.SetActive(false);
        }

        internal void SyncPosition(Vector3 pos)
        {
            if (isAlreadyDisposed) return;
            if (!isPrefab) return;

            Transform.position = new(pos.x, pos.y, pos.z);
        }

        internal void SyncRotation(Quaternion rot)
        {
            if (isAlreadyDisposed) return;
            if (!isPrefab) return;

            Transform.rotation = new(rot.x, rot.y, rot.z, rot.w);
        }

        public void Render()
        {
            if (isAlreadyDisposed) return;
            if (!isPrefab) return;

            OnRender();
        }

        /// <summary>
        /// 자신의 컴포넌트를 가져옵니다.<br/>
        /// OnLoad()이후에만 사용가능합니다.
        /// </summary>
        /// <typeparam name="T">찾을 컴포넌트</typeparam>
        /// <returns>찾은 컴포넌트</returns>
        protected T GetComponent<T>()
            where T : class
        {
            if (Transform == null)
            {
                Debugger.LogWarning($"Please GetComponent<T>() do not used when not OnLoad().");
                return default;
            }

            var temp = Transform.GetComponent<T>();
            if (temp == null)
            {
                Debugger.LogWarning($"{Transform.name} has no {typeof(T)} component");
                return default;
            }

            return temp;
        }

        /// <summary>
        /// 자식의 컴포넌트를 가져옵니다.<br/>
        /// OnLoad()이후에 사용가능합니다.
        /// </summary>
        /// <param name="childName">자식의 이름</param>
        /// <typeparam name="T">찾을 컴포넌트</typeparam>
        /// <returns>찾은 컴포넌트</returns>
        protected T GetChildComponent<T>(string childName)
            where T : class
        {
            if (Transform == null)
            {
                Debugger.LogWarning($"Please GetComponent<T>(string childName) do not used when not OnLoad().");
                return default;
            }

            if (string.IsNullOrEmpty(childName))
            {
                Debugger.LogWarning($"childName is null or empty");
                return default;
            }

            if (!childs.TryGetValue(childName, out var t)) // 캐시에서 먼처 찾는다.
            {
                // 캐시에서 찾는것을 실패시 찾아서 캐싱
                t = Transform.gameObject.transform.Find(childName);

                // 해당 자식이 없으면 없다는 것도 캐싱
                childs.Add(childName, t);
            }

            // 자식이 없으면 null을 반환
            if (t == null)
            {
                Debugger.LogWarning($"{Transform.name} has no {childName} child");
                return default;
            }

            var temp = t.GetComponent<T>();
            if (temp == null)
            {
                Debugger.LogWarning($"{Transform.name} has no {typeof(T)} component");
                return default;
            }

            return temp;
        }


        public virtual void SetMultiply(float multiply)
        {
        }

        public virtual void SetContinue(float multiply)
        {
        }

        public virtual void SetPause()
        {
        }
    }
}