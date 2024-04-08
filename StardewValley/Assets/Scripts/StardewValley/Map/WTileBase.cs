using UnityEngine.Tilemaps;
using UnityEngine;


namespace WATP.Map
{
    [CreateAssetMenu]
    public class WTileBase : TileBase
    {
        public Sprite m_Sprite;
        public GameObject m_Prefab;

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = m_Sprite;
            //tileData.gameObject = m_Prefab;
        }
    }
}