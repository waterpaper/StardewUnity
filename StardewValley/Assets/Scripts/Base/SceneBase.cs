using Cysharp.Threading.Tasks;
using System.Threading;

namespace WATP
{
    /// <summary>
    /// ��� ��(�ڵ�)�� �⺻ ���̽�
    /// ���� unity scene�� ó���Ϸ��� sceneloader���� �ε� ���μ��� �߰� �ʿ�
    /// </summary>
    public interface IScene
    {
        void Init();
        UniTask Load(CancellationTokenSource cancellationToken);
        UniTask Unload(CancellationTokenSource cancellationToken);

        void Complete();

        void Update();

        string GetSceneAssetName();

        bool IsLoadingPage { get; }
    }

    public sealed class SceneNull : IScene
    {
        public void Init()
        {
            UnityEngine.Debug.Log("Empty Scene Init");
        }

        public async UniTask Load(CancellationTokenSource cancellationToken)
        {
            UnityEngine.Debug.Log("Empty Scene Load");
            await UniTask.Yield(cancellationToken: cancellationToken.Token);
        }

        public async UniTask Unload(CancellationTokenSource cancellationToken)
        {
            UnityEngine.Debug.Log("Empty Scene Destroy");
            await UniTask.Yield(cancellationToken: cancellationToken.Token);
        }

        public void Update()
        {

        }

        public void Complete()
        {
            UnityEngine.Debug.Log("To Empty Scene Change Complete");
        }

        public string GetSceneAssetName()
        {
            UnityEngine.Debug.Log("Empty Scene Asset Name");
            return null;
        }

        public bool IsLoadingPage => true;

    }

    public abstract class GameSceneBase : IScene
    {
        public abstract void Init();
        public abstract void Complete();

        public virtual bool IsLoadingPage => true;

        public virtual UniTask Load(CancellationTokenSource cancellationToken)
        {
            return UniTask.Yield(cancellationToken: cancellationToken.Token);
        }

        public virtual UniTask Unload(CancellationTokenSource cancellationToken)
        {
            return UniTask.Yield(cancellationToken: cancellationToken.Token);
        }

        public virtual void Update()
        {
        }

        public virtual string GetSceneAssetName()
        {
            return null;
        }

    }
}