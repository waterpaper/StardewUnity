using WATP.Map;

namespace WATP
{
    public enum MapKind { Main, Title, Custom, Load, Home, Form, Road1, Road2, Town, MapTool, End };

    /// <summary>
    /// unity scene 이동을 처리하는 클래스
    /// 추가적으로 프로젝트에서 Tilemap을 사용함으로
    /// tilemap 관리 매니저를 추가한다.
    /// </summary>
    public partial class SceneLoader
    {
        TileMapManager tileMapManager;

        public TileMapManager TileMapManager => tileMapManager;


        public void Initialize()
        {
            tileMapManager = new TileMapManager();
            tileMapManager.Init();
        }
    }
}