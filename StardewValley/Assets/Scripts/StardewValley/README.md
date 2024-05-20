# Root
게임 최상단 MonoBehaviour입니다.<br/>
필요한 모듈을 추가하여 각 프로젝트별로 한 Root를 사용합니다.<br/>
Static을 이용하여 프로젝트 내부에서 모듈 최상단에 접근 할 수 있게 설계하였습니다.<br/>


```cs
public class Root : MonoBehaviour
{
    protected static Root root;

    [SerializeField] protected Camera worldCamera;
    [SerializeField] protected SceneLoader sceneLoader;
    
    [SerializeField] protected UIManager uiManager;
    [SerializeField] protected DataManager dataManager;
    [SerializeField] protected GameState gameState;
    [SerializeField] protected GameUTCTime gameTime;
    [SerializeField] protected SoundManager soundManager;
}
```