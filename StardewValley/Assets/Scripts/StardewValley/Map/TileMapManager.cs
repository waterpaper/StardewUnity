using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using WATP.Data;
using WATP.UI;


namespace WATP.Map
{
    /// <summary>
    /// 전체 타일맵을 관리하는 클래스
    /// 초기 생성자를 통해 기본 오브젝트를 생성한다.
    /// GridMap을 통해 화면에 옵션을 출력하며 tileMap을 layer 별로 관리한다.
    /// </summary>
    public class TileMapManager
    {
        private string mapName;
        private List<WTileMap> tileMaps = new();
        private List<SaveObjectInfo> saveObjectInfos = new();
        private WTileBase wTileBase;
        private GridMap gridMap;

        private Transform root;

        public Action<string> onMapChangeStart;
        public Action<string> onMapChangeEnd;

        public string MapName { get => mapName; }
        public List<SaveObjectInfo> SaveObjectInfos { get => saveObjectInfos; }
        public Vector2 TileSize { get => tileMaps.Count > 0 ? new Vector2(tileMaps[0].MapX, tileMaps[0].MapY) : Vector2.zero; }

        public void Init()
        {
            var obj = new GameObject($"[TileMap Grid]");
            obj.AddComponent<DontDestroyLoadObject>();

            var grid = obj.AddComponent<Grid>();
            grid.cellLayout = GridLayout.CellLayout.Rectangle;

            root = obj.transform;

            wTileBase = Resources.Load<WTileBase>("WTileBase");

            var gridMap = new GameObject($"[GridMapRenderer]");
            this.gridMap = gridMap.AddComponent<GridMap>();

            if (tileMaps.Count == 0)
            {
                for (int i = 0; i < Config.MAP_LAYER_MAX; i++)
                {
                    var tileMap = new WTileMap();
                    tileMap.Init(root, wTileBase, i+1);
                    tileMaps.Add(tileMap);
                }
            }

            this.gridMap.SetTileMapManager(this);
            Root.State.month.onChange += OnMonthChange;
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.F10))
            {
                gridMap.gameObject.SetActive(!gridMap.gameObject.activeSelf);
            }
        }

        public void Clear()
        {
            foreach(var tileMap in tileMaps)
                tileMap.Clear();

            gridMap.SetSize(0, 0);
            saveObjectInfos.Clear();
            mapName = "";
        }


        public void MapSetting(string mapName, int month = 1)
        {
            if (File.Exists(Application.dataPath + $"/Resources/Map/{mapName}.dat") == false)
            {
                Debug.Log("not file!");
                return;
            }

            onMapChangeStart?.Invoke(mapName);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + $"/Resources/Map/{mapName}.dat", FileMode.Open);
            SaveForm mapSaveData = null;
            try
            {
                var str = (string)bf.Deserialize(file);
                mapSaveData = Json.JsonToObject<SaveForm>(str);
                file.Close();
            }
            catch (Exception e)
            {
                Debug.Log("error!");
                file.Close();
                onMapChangeEnd?.Invoke("");
                return;
            }

            for (int i = 0; i < mapSaveData.tilemaps.Count || i < tileMaps.Count; i++)
                tileMaps[i].MapSetting(mapSaveData.tilemaps[i], month);

            gridMap.SetSize(tileMaps[0].MapX, tileMaps[0].MapY);

            var cells = tileMaps[0].Cells;

            foreach (var cell in cells)
                gridMap.SetAttribute((int)cell.Position.x, (int)cell.Position.y, cell.GetCellType(), cell.BlockType);

            saveObjectInfos.Clear();
            foreach (var obj in mapSaveData.objectInfos)
                saveObjectInfos.Add(new SaveObjectInfo() { objectId = obj.objectId, posX = obj.posX, posY = obj.posY });

            this.mapName = mapName;
            onMapChangeEnd?.Invoke(mapName);
        }

        public void SizeSetting(int xPos, int yPos)
        {
            foreach (var tileMap in tileMaps)
                tileMap.SizeSetting(xPos, yPos);

            gridMap.SetSize(xPos, yPos);
            saveObjectInfos.Clear();
        }

        public void TileImageSetting(int layer, int x, int y, string prefix, string imageName, int index)
        {
            if (tileMaps.Count < layer)
                return;

            tileMaps[layer-1].TileImageSetting(x, y, prefix, imageName, index);
        }

        public void TileAttributeSetting(int x, int y, char type)
        {
            tileMaps[0].TileAttributeSetting(x, y, type);
            gridMap.SetAttribute(x, y, type);
        }

        public void TileAttributeSetting(int x, int y, char type, string nextMap, float nextX, float nextY)
        {
            tileMaps[0].TileAttributeSetting(x, y, type, nextMap, nextX, nextY);
            gridMap.SetAttribute(x, y, type);
        }

        public SaveObjectInfo TileObjectAdd(int x, int y, int id)
        {
            var cell = tileMaps[0].GetCell(x, y);
            foreach(var info in saveObjectInfos)
            {
                if (info.posX == cell.Position.x && info.posY == cell.Position.y) return null;
            }

            var saveObjectInfo= new SaveObjectInfo() { posX = cell.Position.x, posY = cell.Position.y, objectId = id };
            saveObjectInfos.Add(saveObjectInfo);
            return saveObjectInfo;
        }

        public void TileObjectRemove(int x, int y)
        {
            var cell = tileMaps[0].GetCell(x, y);
            SaveObjectInfo target = null;
            foreach (var info in saveObjectInfos)
            {
                if (info.posX == cell.Position.x && info.posY == cell.Position.y)
                {
                    target = info;
                    break;
                }
            }

            if (target != null)
                saveObjectInfos.Remove(target);
        }


        public SaveForm GetSaveForm()
        {
            var saveForm = new SaveForm();
            saveForm.tilemaps = new();
            saveForm.objectInfos = new();

            foreach (var tileMap in tileMaps)
                saveForm.tilemaps.Add(tileMap.SaveForm());

            foreach (var infos in saveObjectInfos)
                saveForm.objectInfos.Add(infos);

            return saveForm;
        }

        public Cell GetCell(int xIndex, int yIndex)
        {
            if (tileMaps.Count == 0) return null;

            return tileMaps[0].GetCell(xIndex, yIndex);
        }

        public Portalinfo GetPortalCell(int x, int y)
        {
            if (tileMaps.Count == 0) return null;

            return tileMaps[0].GetPortalCell(x, y);
        }


        public List<Cell> GetPath(Vector2 now, Vector2 target)
        {
            if (tileMaps.Count == 0) return null;

            return tileMaps[0].Path(GetCell((int)now.x, (int)now.y), GetCell((int)target.x, (int)target.y), 0);
        }

        public void OnMonthChange(int month)
        {
            MapSetting(mapName, month);
        }
    }
}
