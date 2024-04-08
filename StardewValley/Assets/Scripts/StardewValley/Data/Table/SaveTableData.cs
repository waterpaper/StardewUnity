using System.Collections.Generic;

namespace WATP.Data
{
    [System.Serializable]
    public class SaveTableData
    {
        public int month;
        public int day;
        public SavePlayerInfo playerInfo;
        public SaveInventoryInfo inventoryInfo;
        public List<SaveNpcInfo> saveNpcInfos;
        public List<SaveAnimalInfo> saveAnimalInfos;
        public List<SaveObjectInfo> saveObjectInfos;
        public List<SaveFarmHoedirtInfo> saveFarmHoedirtInfos;
        public List<SaveFarmCropsInfo> saveFarmCropsInfos;
        public List<string> saveLoadObjectMaps;

        public SaveTableData() { }

        public SaveTableData(GameState state)
        {
            month = state.month.Value;
            day = state.day.Value;

            playerInfo = new();
            playerInfo.name = state.player.name.Value;
            playerInfo.farmName = state.player.farmName.Value;
            playerInfo.hairIndex = state.player.hairIndex.Value;
            playerInfo.clothsIndex = state.player.clothsIndex.Value;
            playerInfo.hairR = (int)state.player.hairColor.Value.x;
            playerInfo.hairG = (int)state.player.hairColor.Value.y;
            playerInfo.hairB = (int)state.player.hairColor.Value.z;
            playerInfo.clothsR = (int)state.player.clothsColor.Value.x;
            playerInfo.clothsG = (int)state.player.clothsColor.Value.y;
            playerInfo.clothsB = (int)state.player.clothsColor.Value.z;
            playerInfo.maxHp = state.player.maxHp.Value;
            playerInfo.actingPowerMax = state.player.actingPowerMax.Value;
            playerInfo.money = state.player.money.Value;

            inventoryInfo = new();
            inventoryInfo.inventoryLevel = state.inventory.inventoryLevel.Value;
            inventoryInfo.items = new();
            foreach (var data in state.inventory.Items)
                inventoryInfo.items.Add(new SaveItemInfo() { itemId = data.itemId, itemQty = data.itemQty, itemIndex = data.itemIndex });

            saveNpcInfos = new();
            foreach (var data in state.npcInfos)
                saveNpcInfos.Add(new SaveNpcInfo() { id = data.id, likeAbility = data.likeAbility });

            saveAnimalInfos = new();
            foreach (var data in state.animalInfos)
                saveAnimalInfos.Add(new SaveAnimalInfo() { animalUid = data.animalUid, id = data.id, day = data.day, isEat = data.isEat, likeAbility = data.likeAbility });

            saveObjectInfos = new();
            foreach (var key in state.objectInfos.Keys)
            {
                foreach (var data in state.objectInfos[key])
                    saveObjectInfos.Add(new SaveObjectInfo() { name = key, id = data.id, posX = data.posX, posY = data.posY, hp = data.hp });
            }

            saveFarmHoedirtInfos = new();
            foreach (var key in state.farmHoedirtInfos.Keys)
            {
                foreach (var data in state.farmHoedirtInfos[key])
                    saveFarmHoedirtInfos.Add(new SaveFarmHoedirtInfo() { name = key, posX = data.posX, posY = data.posY });
            }

            saveFarmCropsInfos = new();
            foreach (var key in state.farmCropsInfos.Keys)
            {
                foreach (var data in state.farmCropsInfos[key])
                    saveFarmCropsInfos.Add(new SaveFarmCropsInfo() { name = key, id = data.id, posX = data.posX, posY = data.posY, day = data.day });
            }

            saveLoadObjectMaps = new();
            foreach (var mapName in state.objectLoadMaps)
                saveLoadObjectMaps.Add(mapName);
        }
    }

    [System.Serializable]
    public class SavePlayerInfo
    {
        public string name;
        public string farmName;

        public int hairIndex;
        public int clothsIndex;

        public int hairR;
        public int hairG;
        public int hairB;

        public int clothsR;
        public int clothsG;
        public int clothsB;

        public int maxHp;
        public int actingPowerMax;
        public int money;
    }

    [System.Serializable]
    public class SaveInventoryInfo
    {
        public int inventoryLevel;
        public List<SaveItemInfo> items;
    }

    [System.Serializable]
    public class SaveItemInfo
    {
        public int itemId;
        public int itemQty;
        public int itemIndex;
    }

    [System.Serializable]
    public class SaveNpcInfo
    {
        public int id;
        public int likeAbility;
    }

    [System.Serializable]
    public class SaveAnimalInfo
    {
        public int animalUid;
        public int id;
        public int likeAbility;
        public int day;
        public bool isEat;
    }

    [System.Serializable]
    public class SaveObjectInfo
    {
        public string name;
        public int id;
        public float posX;
        public float posY;
        public int hp;
    }

    [System.Serializable]
    public class SaveFarmHoedirtInfo
    {
        public string name;
        public float posX;
        public float posY;
    }

    [System.Serializable]
    public class SaveFarmCropsInfo
    {
        public string name;
        public int id;
        public float posX;
        public float posY;
        public int day;
    }

}