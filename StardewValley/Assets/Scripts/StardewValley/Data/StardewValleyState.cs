using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using WATP.Data;
using WATP.Player;

namespace WATP
{
    public partial class GameState
    {
        public SubjectData<int> month;
        public SubjectData<int> day;
        public SubjectData<DayOfWeek> dayOfWeek;
        public SubjectData<int> time;
        public float timer;

        public PlayerInfo player = new();
        public Inventory inventory = new();
        public List<NpcInfo> npcInfos = new();
        public List<AnimalInfo> animalInfos = new();

        public List<string> objectLoadMaps = new();
        public Dictionary<string, List<ObjectInfo>> objectInfos = new();
        public Dictionary<string, List<FarmHoedirtInfo>> farmHoedirtInfos = new();
        public Dictionary<string, List<FarmCropsInfo>> farmCropsInfos = new();

        public void GameInit()
        {
            player = new();
            inventory.Init();
            npcInfos.Clear();
            animalInfos.Clear();
            objectInfos.Clear();
            farmHoedirtInfos.Clear();
            farmCropsInfos.Clear();

            player.money.Value = 10000;
            player.actingPower.Value = player.actingPowerMax.Value = 100;
            player.hp.Value = player.maxHp.Value = 100;

            month.Value = 1;
            day.Value = 1;
            dayOfWeek.Value = DayOfWeek.Monday;
            time.Value = 360;
            timer = 0;
        }

        public void GameStartSetting()
        {
            player.money.Value = 10000;
            player.actingPower.Value = player.actingPowerMax.Value = 100;
            player.hp.Value = player.maxHp.Value = 100;

            for (int i = 10001; i < 10001 + Config.NPC_MAX; i++)
            {
                npcInfos.Add(new NpcInfo() { id = i, likeAbility = 10 });
            }

            inventory.AddInventory(101, 1);
            inventory.AddInventory(106, 1);
            inventory.AddInventory(111, 1);
            inventory.AddInventory(116, 1);
            inventory.AddInventory(121, 1);
            inventory.AddInventory(501, 5);
            inventory.AddInventory(503, 5);
            inventory.AddInventory(505, 5);

            animalInfos.Add(new AnimalInfo() { animalUid = animalInfos.Count() + 1, id = 100001, likeAbility = 0, day = 0, isEat = true });
            animalInfos.Add(new AnimalInfo() { animalUid = animalInfos.Count() + 1, id = 100005, likeAbility = 0, day = 0, isEat = true });
            animalInfos.Add(new AnimalInfo() { animalUid = animalInfos.Count() + 1, id = 100002, likeAbility = 0, day = 0, isEat = true });
            animalInfos.Add(new AnimalInfo() { animalUid = animalInfos.Count() + 1, id = 100003, likeAbility = 0, day = 0, isEat = true });
            animalInfos.Add(new AnimalInfo() { animalUid = animalInfos.Count() + 1, id = 100004, likeAbility = 0, day = 0, isEat = true });
            animalInfos.Add(new AnimalInfo() { animalUid = animalInfos.Count() + 1, id = 100006, likeAbility = 0, day = 0, isEat = true });

        }

        public void TodayUpdateSetting()
        {
            dayOfWeek.Value = dayOfWeek.Value + 1;

            if (dayOfWeek.Value > DayOfWeek.Saturday)
                dayOfWeek.Value = DayOfWeek.Sunday;


            var keys = farmHoedirtInfos.Keys.ToArray();
            foreach (var key in keys)
            {
                foreach (var item in farmHoedirtInfos[key])
                {
                    if (item.watering)
                    {
                        item.watering = false;
                        if (farmCropsInfos.ContainsKey(key) == false) continue;
                        var crops = farmCropsInfos[key].Find(data => data.posX == item.posX && data.posY == item.posY);

                        if (crops == null) continue;
                        crops.day += 1;
                    }
                }
            }

            if (day.Value >= 28)
            {
                MonthUpdateSetting();
            }
            else
            {
                day.Value = day.Value + 1;
            }

            time.Value = 360;
            timer = 0;
        }

        public void MonthUpdateSetting()
        {
            dayOfWeek.Value = DayOfWeek.Monday;
            if (month.Value >= 4)
                month.Value = 1;
            else
                month.Value = month.Value + 1;


            var keys = farmHoedirtInfos.Keys.ToArray();
            foreach (var key in keys)
            {
                if (month.Value == 4)
                    farmHoedirtInfos[key].Clear();

                if (farmCropsInfos.ContainsKey(key) == false) continue;
                farmCropsInfos[key].Clear();
            }

            if (month.Value == 4)
                farmHoedirtInfos.Clear();
            farmCropsInfos.Clear();

            day.Value = 1;
            time.Value = 360;
            timer = 0;
        }

        public void TimeUpdate(float deltaTime)
        {
            timer += deltaTime;

            if (timer > 10)
            {
                timer -= 10;
                time.Value += 10;
            }

            if (time.Value > 1560)
            {
                TodayUpdateSetting();
            }
        }

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

        public List<AnimalInfo> GetAnimals(int type)
        {
            List<AnimalInfo> list = new List<AnimalInfo>();
            for (int i = 0; i < animalInfos.Count; i++)
            {
                if (Root.GameDataManager.TableData.GetAnimalTableData(animalInfos[i].id).Type == type)
                    list.Add(animalInfos[i]);
            }

            return list;
        }

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
            if(obj != null)
                objectInfos[mapName].Remove(obj);
        }

        public void Save(int index)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + $"/SaveData_{index}.dat");
            SaveTableData saveData = new SaveTableData(this);
            bf.Serialize(file, saveData);
            file.Close();
            Debug.Log("Game data saved!");
        }

        public bool Load(int index)
        {
            if (File.Exists(Application.persistentDataPath + $"/SaveData_{index}.dat") == false)
                return false;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + $"/SaveData_{index}.dat", FileMode.Open);

            try
            {
                SaveTableData saveTableData = (SaveTableData)bf.Deserialize(file);
                GameInit();

                day.Value = saveTableData.day;
                month.Value = saveTableData.month;
                dayOfWeek.Value = (DayOfWeek)(saveTableData.day % 7);

                player.name.Value = saveTableData.playerInfo.name;
                player.farmName.Value = saveTableData.playerInfo.farmName;
                player.hairIndex.Value = saveTableData.playerInfo.hairIndex;
                player.clothsIndex.Value = saveTableData.playerInfo.clothsIndex;
                player.hairColor.Value = new Vector3(saveTableData.playerInfo.hairR, saveTableData.playerInfo.hairG, saveTableData.playerInfo.hairB);
                player.clothsColor.Value = new Vector3(saveTableData.playerInfo.clothsR, saveTableData.playerInfo.clothsG, saveTableData.playerInfo.clothsB);
                player.maxHp.Value = saveTableData.playerInfo.maxHp;
                player.actingPowerMax.Value = saveTableData.playerInfo.actingPowerMax;
                player.money.Value = saveTableData.playerInfo.money;

                inventory.inventoryLevel.Value = saveTableData.inventoryInfo.inventoryLevel;

                if (saveTableData.inventoryInfo.items != null)
                    foreach (var data in saveTableData.inventoryInfo.items)
                    {
                        inventory.AddInventory(data.itemId, data.itemQty, data.itemIndex);
                    }

                if (saveTableData.saveNpcInfos != null)
                    foreach (var data in saveTableData.saveNpcInfos)
                    {
                        npcInfos.Add(new NpcInfo() { id = data.id, likeAbility = data.likeAbility });
                    }

                if (saveTableData.saveAnimalInfos != null)
                    foreach (var data in saveTableData.saveAnimalInfos)
                    {
                        animalInfos.Add(new AnimalInfo() { id = data.id, likeAbility = data.likeAbility, animalUid = data.animalUid, day = data.day, isEat = data.isEat });
                    }

                if (saveTableData.saveObjectInfos != null)
                    foreach (var data in saveTableData.saveObjectInfos)
                    {
                        if (objectInfos.ContainsKey(data.name) == false)
                            objectInfos.Add(data.name, new());

                        objectInfos[data.name].Add(new ObjectInfo() { id = data.id, hp = data.hp, posX = data.posX, posY = data.posY });
                    }

                if (saveTableData.saveFarmHoedirtInfos != null)
                    foreach (var data in saveTableData.saveFarmHoedirtInfos)
                    {
                        if (farmHoedirtInfos.ContainsKey(data.name) == false)
                            farmHoedirtInfos.Add(data.name, new());

                        farmHoedirtInfos[data.name].Add(new FarmHoedirtInfo() { posX = data.posX, posY = data.posY });
                    }

                if (saveTableData.saveFarmCropsInfos != null)
                    foreach (var data in saveTableData.saveFarmCropsInfos)
                    {
                        if (farmCropsInfos.ContainsKey(data.name) == false)
                            farmCropsInfos.Add(data.name, new());

                        farmCropsInfos[data.name].Add(new FarmCropsInfo() { posX = data.posX, posY = data.posY, id = data.id, day = data.day });
                    }

                if (saveTableData.saveLoadObjectMaps != null)
                    foreach (var mapName in saveTableData.saveLoadObjectMaps)
                    {
                        objectLoadMaps.Add(mapName);
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
}