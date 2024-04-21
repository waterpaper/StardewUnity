using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Entities;
using UnityEngine;
using WATP.Data;
using WATP.ECS;
using WATP.Player;

namespace WATP
{
    /// <summary>
    /// state 확장 메서드
    /// </summary>
    public static class GameStateExtenstion_SaveLoad
    {
        public static void Save(this GameState gameState, int index)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + $"/SaveData_{index}.dat");
            SaveTableData saveData = new SaveTableData(gameState);
            bf.Serialize(file, saveData);
            file.Close();
            Debug.Log("Game data saved!");
        }

        public static bool Load(this GameState gameState, int index)
        {
            if (File.Exists(Application.persistentDataPath + $"/SaveData_{index}.dat") == false)
                return false;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + $"/SaveData_{index}.dat", FileMode.Open);

            try
            {
                SaveTableData saveTableData = (SaveTableData)bf.Deserialize(file);
                gameState.GameInit();

                gameState.day.Value = saveTableData.day;
                gameState.month.Value = saveTableData.month;
                gameState.dayOfWeek.Value = (DayOfWeek)(saveTableData.day % 7);

                gameState.player.name.Value = saveTableData.playerInfo.name;
                gameState.player.farmName.Value = saveTableData.playerInfo.farmName;
                gameState.player.hairIndex.Value = saveTableData.playerInfo.hairIndex;
                gameState.player.clothsIndex.Value = saveTableData.playerInfo.clothsIndex;
                gameState.player.hairColor.Value = new Vector3(saveTableData.playerInfo.hairR, saveTableData.playerInfo.hairG, saveTableData.playerInfo.hairB);
                gameState.player.clothsColor.Value = new Vector3(saveTableData.playerInfo.clothsR, saveTableData.playerInfo.clothsG, saveTableData.playerInfo.clothsB);
                gameState.player.maxHp.Value = saveTableData.playerInfo.maxHp;
                gameState.player.actingPowerMax.Value = saveTableData.playerInfo.actingPowerMax;
                gameState.player.money.Value = saveTableData.playerInfo.money;

                gameState.inventory.inventoryLevel.Value = saveTableData.inventoryInfo.inventoryLevel;

                if (saveTableData.inventoryInfo.items != null)
                    foreach (var data in saveTableData.inventoryInfo.items)
                    {
                        gameState.inventory.AddInventory(data.itemId, data.itemQty, data.itemIndex);
                    }

                if (saveTableData.saveNpcInfos != null)
                    foreach (var data in saveTableData.saveNpcInfos)
                    {
                        gameState.npcInfos.Add(new NpcInfo() { id = data.id, likeAbility = data.likeAbility });
                    }

                if (saveTableData.saveAnimalInfos != null)
                    foreach (var data in saveTableData.saveAnimalInfos)
                    {
                        gameState.animalInfos.Add(new AnimalInfo() { id = data.id, likeAbility = data.likeAbility, animalUid = data.animalUid, day = data.day, isEat = data.isEat });
                    }

                if (saveTableData.saveObjectInfos != null)
                    foreach (var data in saveTableData.saveObjectInfos)
                    {
                        if (gameState.objectInfos.ContainsKey(data.name) == false)
                            gameState.objectInfos.Add(data.name, new());

                        gameState.objectInfos[data.name].Add(new ObjectInfo() { id = data.id, hp = data.hp, posX = data.posX, posY = data.posY });
                    }

                if (saveTableData.saveFarmHoedirtInfos != null)
                    foreach (var data in saveTableData.saveFarmHoedirtInfos)
                    {
                        if (gameState.farmHoedirtInfos.ContainsKey(data.name) == false)
                            gameState.farmHoedirtInfos.Add(data.name, new());

                        gameState.farmHoedirtInfos[data.name].Add(new FarmHoedirtInfo() { posX = data.posX, posY = data.posY });
                    }

                if (saveTableData.saveFarmCropsInfos != null)
                    foreach (var data in saveTableData.saveFarmCropsInfos)
                    {
                        if (gameState.farmCropsInfos.ContainsKey(data.name) == false)
                            gameState.farmCropsInfos.Add(data.name, new());

                        gameState.farmCropsInfos[data.name].Add(new FarmCropsInfo() { posX = data.posX, posY = data.posY, id = data.id, day = data.day });
                    }

                if (saveTableData.saveLoadObjectMaps != null)
                    foreach (var mapName in saveTableData.saveLoadObjectMaps)
                    {
                        gameState.objectLoadMaps.Add(mapName);
                    }

                file.Close();
            }
            catch (Exception e)
            {
                file.Close();

                File.Delete(Application.persistentDataPath + $"/SaveData_{index}.dat");
                return false;
            }

            Debug.Log("Game data loaded!");
            return true;
        }
    }

    /// <summary>
    /// state function
    /// </summary>
    public partial class GameState
    {
        /// <summary>
        /// NPC 호감도 상승 처리
        /// </summary>
        public int NPCLikeAbiltiy(int id, int itemId)
        {
            var npcInfo = npcInfos.Find(data => data.id == id);
            if (npcInfo == null)
                return -1;

            var tableData = Root.GameDataManager.TableData.GetNPCTableData(npcInfo.id);
            var isFavor = tableData.Favorites.FirstOrDefault(id => id == itemId);

            if (isFavor <= 0)
            {
                npcInfo.likeAbility += 10;
                return 2;
            }
            else
            {
                npcInfo.likeAbility += 30;
                return 3;
            }
        }

        /// <summary>
        /// 위치에 따른 Animal 정보 반환
        /// </summary>
        public List<AnimalInfo> GetAnimals( int type)
        {
            List<AnimalInfo> list = new List<AnimalInfo>();
            for (int i = 0; i < animalInfos.Count; i++)
            {
                if (Root.GameDataManager.TableData.GetAnimalTableData(animalInfos[i].id).Type == type)
                    list.Add(animalInfos[i]);
            }

            return list;
        }

        #region Hoedirt
        //Hoedirt 관리

        public bool IsHoedirt(string str, float x, float y)
        {
            if (!farmHoedirtInfos.ContainsKey(str)) return false;

            var list = farmHoedirtInfos[str];
            foreach (var item in list)
            {
                if (item.posX == x && item.posY == y)
                    return true;
            }

            return false;
        }

        public List<FarmHoedirtInfo> GetHoedirts(string str)
        {
            if (!farmHoedirtInfos.ContainsKey(str)) return null;
            return farmHoedirtInfos[str];
        }

        public FarmHoedirtInfo GetHoedirt(string str, float x, float y)
        {
            if (!farmHoedirtInfos.ContainsKey(str)) return null;

            var list = farmHoedirtInfos[str];
            foreach (var item in list)
            {
                if (item.posX == x && item.posY == y)
                    return item;
            }

            return null;
        }

        public void AddHoedirt(string str, float x, float y)
        {
            if (IsHoedirt(str, x, y)) return;
            if (!farmHoedirtInfos.ContainsKey(str)) farmHoedirtInfos.Add(str, new());

            farmHoedirtInfos[str].Add(new FarmHoedirtInfo() { posX = x, posY = y });
        }

        public void HoedirtUpdate()
        {
            var component = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<MapUpdateOptionComponent>(mapUpdateEntity);
            component.isHoedirt = true;
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(mapUpdateEntity, component);
        }

        #endregion


        #region Crops
        //Crops 관리

        public bool IsCrops(string str, float x, float y)
        {
            if (!farmCropsInfos.ContainsKey(str)) return false;

            var list = farmCropsInfos[str];
            foreach (var item in list)
            {
                if (item.posX == x && item.posY == y)
                    return true;
            }

            return false;
        }

        public List<FarmCropsInfo> GetCrops(string str)
        {
            if (!farmCropsInfos.ContainsKey(str)) return null;
            return farmCropsInfos[str];
        }

        public FarmCropsInfo GetCrops(string str, float x, float y)
        {
            if (!farmCropsInfos.ContainsKey(str)) return null;

            var list = farmCropsInfos[str];
            foreach (var item in list)
            {
                if (item.posX == x && item.posY == y)
                    return item;
            }

            return null;
        }

        public void AddCrops(string str, int id, float x, float y, int day = 0)
        {
            if (IsCrops(str, x, y)) return;
            if (!farmCropsInfos.ContainsKey(str)) farmCropsInfos.Add(str, new());

            farmCropsInfos[str].Add(new FarmCropsInfo() { id = id, posX = x, posY = y, day = day });
        }

        public void RemoveCrops(string str, float x, float y)
        {
            if (!IsCrops(str, x, y)) return;
            if (!farmCropsInfos.ContainsKey(str)) farmCropsInfos.Add(str, new());

            var item = farmCropsInfos[str].Find(data => data.posX == x && data.posY == y);

            if (item != null)
                farmCropsInfos[str].Remove(item);
        }

        #endregion


        #region object
        //object 관리

        public ObjectInfo GetObject(string mapName, float posX, float posY)
        {
            if (!objectInfos.ContainsKey(mapName)) return null;
            var list = objectInfos[mapName];
            foreach (var item in list)
            {
                if (item.posX == posX && item.posY == posY)
                    return item;
            }

            return null;
        }

        public List<ObjectInfo> GetObjects(string mapName)
        {
            if (!objectInfos.ContainsKey(mapName)) return new();

            return objectInfos[mapName];
        }

        public void AddObject(string mapName, int id, float posX, float posY, int hp)
        {
            if (!objectInfos.ContainsKey(mapName))
                objectInfos.Add(mapName, new());

            objectInfos[mapName].Add(new ObjectInfo() { id = id, posX = posX, posY = posY, hp = hp });

            if (objectLoadMaps.Find(item => item == mapName) == null)
                objectLoadMaps.Add(mapName);
        }

        public void RemoveObject(string mapName, float posX, float posY)
        {
            if (!objectInfos.ContainsKey(mapName)) return;

            var obj = GetObject(mapName, posX, posY);
            if (obj != null)
                objectInfos[mapName].Remove(obj);
        }

        #endregion

    }
}