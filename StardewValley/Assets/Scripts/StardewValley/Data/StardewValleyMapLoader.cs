using WATP.Map;

namespace WATP
{
    public enum MapKind { Main, Title, Custom, Load, Home, Form, Road1, Road2, Town, MapTool, End };

    /// <summary>
    /// unity scene �̵��� ó���ϴ� Ŭ����
    /// �߰������� ������Ʈ���� Tilemap�� ���������
    /// tilemap ���� �Ŵ����� �߰��Ѵ�.
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