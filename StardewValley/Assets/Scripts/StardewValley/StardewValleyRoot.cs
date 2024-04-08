using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using WATP.ECS;
using WATP.Map;
using WATP.UI;
using WATP.View;

namespace WATP
{
    public class StardewValleyRoot : Root
    {
        ViewManager viewManager;
        ECSManager ecsManager;
        MapObjectManager mapObjectManager;

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
            ecsManager = new ECSManager();
            viewManager = new ViewManager();
            mapObjectManager = new MapObjectManager();

            ecsManager.Initialize();
            viewManager.Initialize();
            mapObjectManager.Init();

            SceneLoader.onSceneChangeStart += OnSceneChangeEvent;
            SceneLoader.SceneLoad(SceneKind.Splash);
        }

        protected override void Update()
        {
            base.Update();

            if (State.logicState.Value != LogicState.Normal) return;

            GameUpdate(Time.deltaTime);
            viewManager.Render();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
        }

        private void OnDestroy()
        {
        }

        /// <summary>
        /// ���� ���� ��ü ���� ���� update����
        /// ���� ������ ���� �ð��� �´� ������ ������Ʈ ���ش�.
        /// </summary>
        protected void GameUpdate(double deltaTime)
        {
            if (State.logicState.Value != LogicState.Normal) return;

            /* try
             {*/
            Profiler.BeginSample($"play");
            Root.State.TimeUpdate((float)deltaTime);
            ecsManager.Simulation(deltaTime);
            Profiler.EndSample();
            /* }
             catch (Exception e)
             {
                 FGBConsole.Log(e);
             }*/

        }

        protected void OnSceneChangeEvent(SceneKind scenekind)
        {
            uiManager.Widgets.SceneChange((int)scenekind);
        }
    }
}