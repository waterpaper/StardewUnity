using System.Collections;

namespace WATP
{
    public interface IScene
    {
        void Init();
        IEnumerator Load();

        IEnumerator Unload();

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

        public IEnumerator Load()
        {
            UnityEngine.Debug.Log("Empty Scene Load");
            yield return null;
        }

        public IEnumerator Unload()
        {
            UnityEngine.Debug.Log("Empty Scene Destroy");
            yield return null;
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

        public virtual IEnumerator Load()
        {
            yield return null;
        }

        public virtual IEnumerator Unload()
        {
            yield return null;
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