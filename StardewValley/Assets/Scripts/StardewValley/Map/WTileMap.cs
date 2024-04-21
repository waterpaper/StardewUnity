using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using WATP.Player;
using WATP.Structure;

namespace WATP.Map
{
    /// <summary>
    /// 각 layer에 해당하는 tile map을 생성 관리하는 클래스
    /// cell을 통해 길 찾기와 같은 옵션처리 로직도 관리한다.
    /// </summary>
    public class WTileMap
    {
        int layerIndex;
        int mapX, mapY;

        WTileBase tileBase;
        Tilemap tilemap;

        List<Cell> cells = new();
        List<WTileBase> tileBases = new();
        List<Vector3Int> posistions = new();
        List<Portalinfo> portalInfos = new();

        protected Cell goal;
        protected Cell current;
        protected readonly PriorityQueue<Cell> openList = new(SortType.ASCENDING);
        protected readonly List<Cell> closeList = new();
        protected List<Cell> path = new List<Cell>();

        public int MapX { get => mapX; }
        public int MapY { get => mapY; }
        public List<Cell> Cells { get => cells; }

        public void Init(Transform root, WTileBase tileBase, int layerIndex = 0)
        {
            Clear();

            var obj = new GameObject($"[TileMap{layerIndex}]");
            obj.transform.SetParent(root);

            tilemap = obj.AddComponent<Tilemap>();
            var tilemapRenderer = obj.AddComponent<TilemapRenderer>();

            tilemap.orientation = Tilemap.Orientation.XY;
            tilemapRenderer.sortingOrder = layerIndex;
            this.layerIndex = layerIndex;
            this.tileBase = tileBase;
        }

        public void Clear()
        {
            InitCell();
            tileBases.Clear();
            posistions.Clear();
            portalInfos.Clear();

            if (tilemap != null)
                tilemap.ClearAllTiles();
        }

        public void SizeSetting(int x, int y)
        {
            portalInfos.Clear();

            mapX = x;
            mapY = y;
            InitCell();

            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    if (tileBases.Count <= i * x + j)
                    {
                        WTileBase nTile = (WTileBase)ScriptableObject.CreateInstance(typeof(WTileBase));
                        tileBases.Add(nTile);
                    }
                    posistions.Add(new Vector3Int(j, i, 0));
                }
            }

            if (tileBases.Count > x * y)
            {
                for (int i = x * y; i < tileBases.Count; i++)
                {
                    GameObject.Destroy(tileBases[i]);
                }

                tileBases.RemoveRange(x * y, tileBases.Count - x * y);
            }

            tilemap.SetTiles(posistions.ToArray(), tileBases.ToArray());
        }


        public void TileImageSetting(int x, int y, string prefix, string imageName, int index)
        {
            if (string.IsNullOrEmpty(imageName))
                tileBases[mapX * y + x].m_Sprite = null;
            else if (string.IsNullOrEmpty(prefix))
                tileBases[mapX * y + x].m_Sprite = AssetLoader.Load<Sprite>($"Address/Sprite/TileSheets/{imageName}.png[{imageName}_{index}]");
            else
                tileBases[mapX * y + x].m_Sprite = AssetLoader.Load<Sprite>($"Address/Sprite/TileSheets/{prefix}/{imageName}.png[{imageName}_{index}]");

            tilemap.RefreshTile(posistions[mapX * y + x]);

            cells[mapX * y + x].TileImageSetting(prefix, imageName, index);
        }

        public void TileAttributeSetting(int x, int y, char type)
        {
            if (cells[mapX * y + x].GetCellType() == CellKind.Portal)
            {
                cells[mapX * y + x].SetAttribute(type);
                var info = portalInfos.Find(data => data.portalPosX == x && data.portalPosY == y);

                if (info != null)
                {
                    portalInfos.Remove(info);
                }
            }
            else
                cells[mapX * y + x].SetAttribute(type);
        }

        public void TileAttributeSetting(int x, int y, char type, string nextMap, float nextX, float nextY)
        {
            var info = portalInfos.Find(data => data.portalPosX == x && data.portalPosY == y);

            if (info != null)
            {
                portalInfos.Remove(info);
            }

            portalInfos.Add(new Portalinfo() { portalPosX = x, portalPosY = y, portalName = nextMap, portalNextPosX = nextX, portalNextPosY = nextY });
            cells[mapX * y + x].SetAttribute(type);
        }

        public async UniTask MapSetting(SaveTileMapForm mapForm, int month = 1)
        {
            if (tilemap == null) return;

            posistions.Clear();
            portalInfos.Clear();
            tilemap.ClearAllTiles();

            mapX = mapForm.tileX;
            mapY = mapForm.tileY;

            InitCell();

            string preFix = null;
            string imageName = null;
            string monthText = "spring";
            string path = null;
            if (month == 2) monthText = "summer";
            else if (month == 3) monthText = "fall";
            else if (month == 4) monthText = "winter";

            for (int i = 0; i < mapY; i++)
            {
                for (int j = 0; j < mapX; j++)
                {
                    if (tileBases.Count <= i * mapX + j)
                    {
                        WTileBase nTile = (WTileBase)ScriptableObject.CreateInstance(typeof(WTileBase));
                        tileBases.Add(nTile);
                    }
                    var tile = tileBases[i * mapX + j];
                    var tileInfo = mapForm.tiles[i * mapX + j];

                    //spring 처리
                    //if(tileInfo.imageName)

                    preFix = tileInfo.imagePrefix;
                    imageName = tileInfo.imageName;
                    imageName = imageName.Replace("spring", monthText);
                    path = string.IsNullOrEmpty(preFix) ? (string.IsNullOrEmpty(imageName) ? null : $"Address/Sprite/TileSheets/{imageName}.png[{imageName}_{tileInfo.imageIndex}]") 
                         : $"Address/Sprite/TileSheets/{preFix}/{imageName}.png[{imageName}_{tileInfo.imageIndex}]";

                    if (string.IsNullOrEmpty(path))
                        tile.m_Sprite = null;
                    else
                        tile.m_Sprite = AssetLoader.Load<Sprite>(path);

                    var cell = GetCell(j, i);
                    cell.SetAttribute(tileInfo.type);
                    cell.SetBlock(tileInfo.block == 1);
                    cell.TileImageSetting(tileInfo.imagePrefix, tileInfo.imageName, tileInfo.imageIndex);

                    posistions.Add(new Vector3Int(j, i, 0));
                }
            }



            if (tileBases.Count > mapX * mapY)
            {
                for (int i = mapX * mapY; i < tileBases.Count; i++)
                {
                    GameObject.Destroy(tileBases[i]);
                }

                tileBases.RemoveRange(mapX * mapY, tileBases.Count - mapX * mapY);
            }

            tilemap.SetTiles(posistions.ToArray(), tileBases.ToArray());

            foreach (var portalInfo in mapForm.portals)
                portalInfos.Add(portalInfo);

            await UniTask.DelayFrame(1);
        }

        public void InitCell()
        {
            for (int i = 0; i < tileBases.Count || i < cells.Count; i++)
            {
                if (string.IsNullOrEmpty(cells[i].ImagePrefix))
                {
                    if (!string.IsNullOrEmpty(cells[i].ImageName))
                    {
                        AssetLoader.Unload<Sprite>($"Address/Sprite/TileSheets/{cells[i].ImageName}.png[{cells[i].ImageName}_{cells[i].ImageIndex}]", tileBases[i].m_Sprite);
                    }
                }
                else
                    AssetLoader.Unload<Sprite>($"Address/Sprite/TileSheets/{cells[i].ImagePrefix}/{cells[i].ImageName}.png[{cells[i].ImageName}_{cells[i].ImageIndex}]", tileBases[i].m_Sprite);

            }

            foreach (var cell in cells)
                cell.Clear();

            cells.Clear();

            Vector2 v2 = new(0, 0);
            for (int i = 0; i < mapY; i++)
            {
                for (int j = 0; j < mapX; j++)
                {
                    v2.x = j;
                    v2.y = i;
                    cells.Add(new Cell(v2, 1, default, default));
                }
            }

            for (int i = 0; i < mapY; i++)
            {
                for (int j = 0; j < mapX; j++)
                {
                    var targetCell = GetCell(j, i);

                    if (targetCell == null)
                        continue;

                    var r = targetCell.AddNeighbor(GetCell(j + 1, i), "r");
                    var l = targetCell.AddNeighbor(GetCell(j - 1, i), "l");
                    var u = targetCell.AddNeighbor(GetCell(j, i + 1), "t");
                    var d = targetCell.AddNeighbor(GetCell(j, i - 1), "b");
                    /*if (r && u) targetCell.AddNeighbor(GetCell(j + 1, i + 1));
                    if (l && u) targetCell.AddNeighbor(GetCell(j - 1, i + 1));
                    if (l && d) targetCell.AddNeighbor(GetCell(j - 1, i - 1));
                    if (r && d) targetCell.AddNeighbor(GetCell(j + 1, i - 1));*/
                }
            }
        }

        public Cell GetCell(int xIndex, int yIndex)
        {
            if (xIndex < 0 || yIndex < 0) // 최소
                return null;
            if (xIndex >= MapX || yIndex >= mapY) // 최대
                return null;

            return cells[yIndex * mapX + xIndex];
        }

        public Portalinfo GetPortalCell(int x, int y)
        {
            foreach (var portal in portalInfos)
            {
                if (portal.portalPosX == x && portal.portalPosY == y)
                    return portal;
            }

            return null;
        }

        public SaveTileMapForm SaveForm()
        {
            var saveForm = new SaveTileMapForm();

            saveForm.tileX = MapX;
            saveForm.tileY = MapY;
            saveForm.tiles = new();

            foreach (var tile in cells)
                saveForm.tiles.Add(new SaveTileInfo { imageIndex = tile.ImageIndex, imageName = tile.ImageName, imagePrefix = tile.ImagePrefix, block = tile.Block ? 1 : 0, type = tile.GetKindChar() });

            saveForm.portals = new();
            foreach (var portalInfo in portalInfos)
                saveForm.portals.Add(portalInfo);

            return saveForm;
        }


        public List<Cell> Path(Cell start_point, Cell end_point, int objectWeight)
        {
            path.Clear();
            if (start_point == null || end_point == null)
                return path;

            openList.Clear();
            closeList.Clear();
            start_point.ResetCost();

            goal = end_point;
            current = start_point;
            var lastCell = current;

            while (current != goal)
            {
                if (Vector2.SqrMagnitude(goal.Position - current.Position).CompareTo(Vector2.SqrMagnitude(goal.Position - lastCell.Position)) < 0)
                    lastCell = current;

                closeList.Add(current);
                Add_OpenCellList(current, objectWeight);

                // 업데이트의 속도를 위해 특정 길이의 Path만 생성하게 제한 - 길이 완전히 막힌경우 멀리 돌아가지 않고 버벅거리는 현상이 생길수 있음
                if (closeList.Count > 200)
                    break;

                if (openList.Count != 0)
                    current = openList.Dequeue();
                else
                    break;

                if (current == goal)
                    break;
            }

            if (current != goal)
                current = lastCell;

            while (current != start_point && current != null)
            {
                if (path.Count > 0 && path[path.Count - 1] == current)
                    break;
                path.Add(current);
                current = current.Parent;
            }

            path.Reverse();

            return path.ToList();
        }

        private void Add_OpenCellList(Cell cell, int objectWeight, bool isDirect = false)
        {
            int weight = 0;
            cell.NeighborCells.ForEach(c =>
            {
                //open list에 추가할 조건 설정
                if (c.Block || c.ObjectBlock || closeList.Contains(c)) return;

                weight = (int)(Vector2.SqrMagnitude(cell.Position - c.Position) * 10);

                if (!openList.Contains(c))
                {
                    SetCellWeight(c, weight);
                    openList.Enqueue(c);
                }
                else if (cell.GCost > current.GCost + weight)
                {
                    SetCellWeight(c, weight);
                    openList.ChangeNode(c);
                }
            });
        }

        private void SetCellWeight(Cell value, int gCost)
        {
            value.SetGCost(current.GCost + gCost);

            value.CalculateHeuristic(goal);
            value.Parent = current;
        }
    }
}
