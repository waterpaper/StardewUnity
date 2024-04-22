# Maptool
Unity의 Map은 Editor 상에서 Tile Palette를 이용해 Tilemap을 구성하게 됩니다.<br/>
해당 기능을 이용해서는 Tile(각 cell)을 이용한 인 게임 기능 옵션들을 설정시 따로 구성이 되야하는 단점이 있었습니다. (render가 따로 구현되야 함)<br/>
해당 프로젝트에서는 Maptool을 이용하여 두 기능을 한 곳에서 설정하고 게임시 이용 가능하게 구성해보았습니다.<br/>

---

### TileMap
각 Layer에 해당하는 Tile map을 생성 관리하는 클래스입니다.<br/>
Unity Tilemap system을 동적으로 생성하여 사용하며 Cell을 기반으로 Tile을 추가합니다.<br/>

- Tile Image Add<br/>
Tile 별 sprite는 현재 runtime에서 이미지를 각각 불러와 사용중입니다.
해당 방식은 Image의 제한이 없다는 장점이 있지만 퍼포먼스 저하가 있기 때문에 
Atlas 혹은 미리 생성된 tilemap 같은 다른 방식의 변경이 필요할 것으로 예상됩니다.

- 길 찾기 알고리즘<br/>
Cell의 옵션 및 위치를 기반으로 길 찾기 알고리즘(a*)를 수행합니다.

[TileMap.cs](./WTileMap.cs)
---

### Cell
Cell은 한 칸을 의미하며 내부에 이미지정보 및 인게임 옵션정보를 가지고 있습니다.<br/>
해당 이미지 정보를 가지고 Unity TileBase에 이미지정보를 추가하는 식으로 활용합니다.<br/>

[Cell.cs](./Cell.cs)
```cs
 public class Cell : IComparable
 {
     public string ImagePrefix { get; protected set; }
     public string ImageName { get; protected set; }
     public int ImageIndex { get; protected set; }                                //어떤 이미지에 위치하는지 나타낸다(세이브, 로드시 사용)

     //a* 길찾기 알고리즘에 필요한 property
     public Cell Parent { get; set; }
     public int HCost { get; protected set; }
     public int GCost { get; protected set; }
     public int FCost { get; protected set; }
     //

     public Vector2 Position { get; protected set; }
     public Rect Rect { get; protected set; }


     public CellBlockType BlockType = CellBlockType.NotBlock;
```

[TileBase.cs](./WTileBase.cs)

```cs
public class WTileBase : TileBase
{
    public Sprite m_Sprite;                 //해당 image 설정
    public GameObject m_Prefab;

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = m_Sprite;
        //tileData.gameObject = m_Prefab;
    }
}
```

---