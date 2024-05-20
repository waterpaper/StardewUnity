using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace WATP
{
    public enum LoadingType
    {
        preProcess,
        defaultProcess,
        postProcess
    }

    /// <summary>
    /// 초기 전체 데이터 로드 프로세스 처리
    /// pre -> default -> post 순으로 처리되며
    /// default만 percent 계산
    /// </summary>
    public static class Loading
    {
        private static List<IEnumerator> preLoads = new List<IEnumerator>();
        private static List<IEnumerator> loads = new List<IEnumerator>();
        private static List<IEnumerator> postLoads = new List<IEnumerator>();

        private static int preCount, loadCount, postCount = 0;

        public static bool isFirst = true;
        public static bool isDone = false;

        public static Action loadingDone;
        public static float Percent { get => loads.Count == 0 ? 1 : loadCount * 1.0f / loads.Count; }

        public static async void LoadingProcess(MonoBehaviour mono)
        {
            await LoadingTask(mono);
        }

        /// <summary>
        /// main load coroutine logic
        /// </summary>
        /// <returns></returns>
        public static async UniTask LoadingTask(MonoBehaviour mono)
        {
            if (!isFirst) return;

            isFirst = false;
            isDone = false;
            //load start
            //Debug.Log("data load start");
            //Debug.Log($"data preLoads start({preCount}/ {preLoads.Count})");

            foreach (var process in preLoads)
            {
                mono.StartCoroutine(process);
            }

            while (preCount < preLoads.Count)
            {
                await UniTask.Yield();
            }

            //Debug.Log($"data preLoads end({preCount}/ {preLoads.Count})");

            //Debug.Log($"data process start({loadCount}/ {loads.Count})");
            foreach (var process in loads)
            {
                mono.StartCoroutine(process);
            }

            while (loadCount < loads.Count)
            {
                await UniTask.Yield();
            }

            //Debug.Log($"data process end({loadCount}/ {loads.Count})");

            //Debug.Log($"data postLoads start({postCount}/ {postLoads.Count})");
            foreach (var process in postLoads)
            {
                mono.StartCoroutine(process);
            }

            while (postCount < postLoads.Count)
            {
                await UniTask.Yield();
            }

            //Debug.Log($"data postLoads end({postCount}/ {postLoads.Count})");

            preLoads.Clear();
            loads.Clear();
            postLoads.Clear();
            preCount = loadCount = postCount = 0;
            isDone = true;
            loadingDone.Invoke();
        }

        public static void AddLoadProcess(LoadingType type, Action func, int weight = 1)
        {
            if (type == LoadingType.preProcess)
                preLoads.Add(Load(type, func, weight));
            else if (type == LoadingType.defaultProcess)
                loads.Add(Load(type, func, weight));
            else if (type == LoadingType.postProcess)
                postLoads.Add(Load(type, func, weight));
        }

        public static void AddLoadProcess(LoadingType type, IEnumerator func, int weight = 1)
        {
            if (type == LoadingType.preProcess)
                preLoads.Add(Load(type, func, weight));
            else if (type == LoadingType.defaultProcess)
                loads.Add(Load(type, func, weight));
            else if (type == LoadingType.postProcess)
                postLoads.Add(Load(type, func, weight));
        }


        public static void AddLoadProcess(LoadingType type, UniTask func, int weight = 1)
        {
            if (type == LoadingType.preProcess)
                preLoads.Add(Load(type, func, weight));
            else if (type == LoadingType.defaultProcess)
                loads.Add(Load(type, func, weight));
            else if (type == LoadingType.postProcess)
                postLoads.Add(Load(type, func, weight));
        }

        private static IEnumerator Load(LoadingType type, Action func, int weight)
        {
            yield return null;
            func?.Invoke();

            yield return null;

            if (type == LoadingType.preProcess)
                preCount += weight;
            else if (type == LoadingType.defaultProcess)
                loadCount += weight;
            else if (type == LoadingType.postProcess)
                postCount += weight;

            func = null;
        }

        private static IEnumerator Load(LoadingType type, IEnumerator func, int weight)
        {
            yield return func;

            if (type == LoadingType.preProcess)
                preCount += weight;
            else if (type == LoadingType.defaultProcess)
                loadCount += weight;
            else if (type == LoadingType.postProcess)
                postCount += weight;

            func = null;
        }


        private static IEnumerator Load(LoadingType type, UniTask func, int weight)
        {
            yield return func.ToCoroutine();

            if (type == LoadingType.preProcess)
                preCount += weight;
            else if (type == LoadingType.defaultProcess)
                loadCount += weight;
            else if (type == LoadingType.postProcess)
                postCount += weight;

            func = default;
        }
    }
}