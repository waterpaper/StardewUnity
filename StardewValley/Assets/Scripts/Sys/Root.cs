using UnityEngine;
using WATP.Data;
using WATP.UI;

namespace WATP
{
    /// <summary>
    /// 게임 최상단 mono
    /// game object에 게임별 root를 추가해서 게임을 실행한다.
    /// </summary>
    public class Root : MonoBehaviour
    {
        protected static Root root;
        //전체 카메라
        [SerializeField] protected Camera worldCamera;
        //씬 관리 클래스(unity scene 관리 방법 변경시 수정 필요)
        [SerializeField] protected SceneLoader sceneLoader;
        
        [SerializeField] protected UIManager uiManager;
        [SerializeField] protected DataManager dataManager;
        [SerializeField] protected GameState gameState;
        [SerializeField] protected GameUTCTime gameTime;
        [SerializeField] protected SoundManager soundManager;

        //getter
        public static Camera WorldCamera => root.worldCamera;

        public static SceneLoader SceneLoader => root.sceneLoader;
        public static UIManager UIManager => root.uiManager;
        public static DataManager GameDataManager => root.dataManager;
        public static GameState State => root.gameState;
        public static GameUTCTime GameTime => root.gameTime;
        public static SoundManager SoundManager => root.soundManager;
        public static MonoBehaviour Mono => root;


        protected virtual void Awake()
        {
            root = this;
            worldCamera = Camera.main;

            sceneLoader = new SceneLoader();
            uiManager = new UIManager();
            dataManager = new DataManager();
            gameState = new GameState();
            gameTime = new GameUTCTime();
            soundManager = new SoundManager();
        }

        protected virtual void Update()
        {
            gameTime?.Update();
            sceneLoader?.Update();
        }

        protected virtual void LateUpdate()
        {
            uiManager?.Update();
        }

        private void OnDestroy()
        {
            soundManager.Dispose();
        }
    }
}