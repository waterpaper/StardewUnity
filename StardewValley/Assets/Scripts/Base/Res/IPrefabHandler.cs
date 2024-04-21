using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WATP
{
    /// <summary>
    /// 프리팹 기본 정보를 가지고 있는 인터페이스
    /// </summary>
    public interface IPrefabHandler : IDisposable
    {
        static string DefaultPrefabPath { get; }
        string PrefabPath { get; }
        Transform Parent { get; }

        /// <summary> 타겟하는 Prefab을 불러 옵니다. </summary>
        /// <param name="customPrefabPath"> prefab file name </param>
        /// <param name="parent"> GameObject가 부모로 할 대상 </param>
        UniTask<Transform> LoadAsync(string customPrefabPath, Transform parent, CancellationTokenSource cancellationToken);
    }
}