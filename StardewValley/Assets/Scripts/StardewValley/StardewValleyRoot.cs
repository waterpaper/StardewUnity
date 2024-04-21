using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Profiling;
using WATP.ECS;
using WATP.View;

namespace WATP
{
    /// <summary>
    /// ���� �ֻ�� mono
    /// game object�� ���Ӻ� root�� �߰��ؼ� ������ �����Ѵ�.
    /// </summary>
    public class StardewValleyRoot : Root
    {
        ViewManager viewManager;
        MapObjectManager mapObjectManager;

        //getter
        public static MapObjectManager MapObjectManager => (root as StardewValleyRoot).mapObjectManager;

        protected override void Awake()
        {
            base.Awake();

            uiManager.Initialize(new Vector2Int(1200, 800));
            dataManager.Init();
            SceneLoader.Initialize();
            soundManager.Init();
        }

        protected void Start()
        {
            viewManager = new ViewManager();
            mapObjectManager = new MapObjectManager();

            GameInit();

            SceneLoader.onSceneChangeStart += OnSceneChangeEvent;
            SceneLoader.SceneLoad(SceneKind.Splash);
        }

        protected override void Update()
        {
            base.Update();

            GameUpdate(Time.deltaTime);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            viewManager.Render();
        }

        private void OnDestroy()
        {
            State.Dispose();
            mapObjectManager.Dispose();
        }

        protected void GameInit()
        {
            State.Init();
            viewManager.Initialize();
            mapObjectManager.Init();
        }

        /// <summary>
        /// ���� ���� ��ü ���� ���� update����
        /// ���� ������ ���� �ð��� �´� ������ ������Ʈ ���ش�.
        /// </summary>
        protected void GameUpdate(double deltaTime)
        {
            try
            {
                Profiler.BeginSample($"play");
                mapObjectManager.Update();
                if (State.logicState.Value == LogicState.Normal)
                    Root.State.TimeUpdate((float)deltaTime);
                Profiler.EndSample();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        }

        protected void OnSceneChangeEvent(SceneKind scenekind)
        {
            uiManager.Widgets.SceneChange((int)scenekind);
        }
    }
}