using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using WATP.Map;

namespace WATP.UI
{
    public class GridMap : MonoBehaviour
    {
        Tilemap tilemap;

        LineRenderer lr;
        TileMapManager tileMapManager;

        List<WTileBase> tileBases = new();
        List<Vector3Int> positions = new();

        int rowCount, columnCount;
        bool isView = true;

        public float sr, sc;
        public float gridSize = 1;

        void Start()
        {
            var line = new GameObject($"[Line Grid]");
            line.transform.SetParent(transform);

            lr = line.AddComponent<LineRenderer>();
            InitLineRenderer(lr);
            lr.sortingLayerName = "Grid";
            lr.sortingOrder = 1;

            var obj = new GameObject($"[TileMap Grid]");
            obj.transform.SetParent(transform);

            var grid = obj.AddComponent<Grid>();
            grid.cellLayout = GridLayout.CellLayout.Rectangle;

            var attributeTileMap = new GameObject($"[AttributeTileMap]");
            attributeTileMap.transform.SetParent(obj.transform);

            tilemap = attributeTileMap.AddComponent<Tilemap>();
            var tilemapRenderer = tilemap.gameObject.AddComponent<TilemapRenderer>();

            tilemap.orientation = Tilemap.Orientation.XY;
            tilemapRenderer.sortingLayerName = "Grid";

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(Root.GameDataManager.Preferences.IsGrid);
            }

            Root.GameDataManager.Preferences.OnIsGridChange += OnViewGrid;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F3))
            {
                Root.GameDataManager.Preferences.SetIsGrid(!Root.GameDataManager.Preferences.IsGrid);
            }
        }


        private void OnDestroy()
        {
            tileMapManager = null;
            Root.GameDataManager.Preferences.OnIsGridChange -= OnViewGrid;
        }

        void InitLineRenderer(LineRenderer lr)
        {
            lr.startWidth = lr.endWidth = 0.05f;
            lr.material.color = Color.white;
        }

        void MakeGrid(LineRenderer lr, float sr, float sc, int rowCount, int colCount)
        {
            List<Vector3> gridPos = new List<Vector3>();

            float ec = sc + colCount * gridSize;

            gridPos.Add(new Vector3(sr, sc, this.transform.position.z));
            gridPos.Add(new Vector3(sr, ec, this.transform.position.z));

            int toggle = -1;
            Vector3 currentPos = new Vector3(sr, ec, this.transform.position.z);
            for (int i = 0; i < rowCount; i++)
            {
                Vector3 nextPos = currentPos;

                nextPos.x += gridSize;
                gridPos.Add(nextPos);

                nextPos.y += (colCount * toggle * gridSize);
                gridPos.Add(nextPos);

                currentPos = nextPos;
                toggle *= -1;
            }

            currentPos.x = sr;
            gridPos.Add(currentPos);

            int colToggle = toggle = 1;
            if (currentPos.y == ec) colToggle = -1;

            for (int i = 0; i < colCount; i++)
            {
                Vector3 nextPos = currentPos;

                nextPos.y += (colToggle * gridSize);
                gridPos.Add(nextPos);

                nextPos.x += (rowCount * toggle * gridSize);
                gridPos.Add(nextPos);

                currentPos = nextPos;
                toggle *= -1;
            }

            this.rowCount = rowCount;
            this.columnCount = colCount;

            if (lr == null) return;
            lr.positionCount = gridPos.Count;
            lr.SetPositions(gridPos.ToArray());
        }

        public void SetTileMapManager(TileMapManager tileMapManager)
        {
            this.tileMapManager = tileMapManager;
        }

        public void SetSize(int x, int y)
        {
            tileBases.Clear();
            positions.Clear();

            if (x + y > 0)
            {
                MakeGrid(lr, sr, sc, x, y);

                for (int i = 0; i < y; i++)
                {
                    for (int j = 0; j < x; j++)
                    {
                        if (tileBases.Count <= i * x + j)
                        {
                            WTileBase nTile = (WTileBase)ScriptableObject.CreateInstance(typeof(WTileBase));
                            tileBases.Add(nTile);
                        }
                        positions.Add(new Vector3Int(j, i, 0));
                    }
                }

                tilemap.ClearAllTiles();
                tilemap.SetTiles(positions.ToArray(), tileBases.ToArray());
            }
            else
            {
                tilemap.ClearAllTiles();
            }
        }

        public void SetAttribute(int x, int y, char type)
        {
            int index = 0;
            switch (type)
            {
                case 'C':
                    break;
                case 'B':
                    index = 1;
                    break;
                case 'F':
                    index = 2;
                    break;
                case 'W':
                    index = 3;
                    break;
                case 'P':
                    index = 4;
                    break;
            }

            if(index == 0)
                tileBases[rowCount * y + x].m_Sprite = null;
            else
                tileBases[rowCount * y + x].m_Sprite = AssetLoader.Load<Sprite>($"Address/Sprite/LooseSprites/UI_MapAttribute.png[UI_MapAttribute_{index-1}]");

            tilemap.RefreshTile(positions[rowCount * y + x]);
        }


        public void SetAttribute(int x, int y, CellKind type, CellBlockType blockType)
        {
            int index = 0;

            switch (type)
            {
                case CellKind.Cell:
                    break;
                case CellKind.FarmLand:
                    index = 2;
                    break;
                case CellKind.Water:
                    index = 3;
                    break;
                case CellKind.Portal:
                    index = 4;
                    break;
            }

            if (blockType == CellBlockType.Block)
                index = 1;

            if (index == 0)
                tileBases[rowCount * y + x].m_Sprite = null;
            else
                tileBases[rowCount * y + x].m_Sprite = AssetLoader.Load<Sprite>($"Address/Sprite/LooseSprites/UI_MapAttribute.png[UI_MapAttribute_{index - 1}]");

            tilemap.RefreshTile(positions[rowCount * y + x]);
        }

        private void OnViewGrid(bool isView)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(isView);
            }
        }
    }
}
