using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;

namespace WATP.Data
{
    public partial class TableData
    {
        Dictionary<int, AnimalTableData> animalTables = new();
        Dictionary<int, AnimalGrowTableData> animalGrowTables = new();
        Dictionary<int, NPCTableData> npcTables = new();
        Dictionary<int, ItemTableData> itemTables = new();
        Dictionary<int, EatTableData> eatTables = new();
        Dictionary<int, ShopItemTableData> shopItemTables = new();
        Dictionary<int, NPCPosTableData> npcPosTables = new();
        Dictionary<int, NpcConversationTableData> npcConversationTables = new();
        Dictionary<int, ToolTableData> toolTableDatas = new();
        Dictionary<int, CropsTableData> cropsTableDatas = new();
        Dictionary<int, ObjectTableData> objectTableDatas = new();

        public AnimalTableData GetAnimalTableData(int id) => animalTables.ContainsKey(id) ? animalTables[id] : null;
        public AnimalGrowTableData GetAnimalGrowTableData(int id) => animalGrowTables.ContainsKey(id) ? animalGrowTables[id] : null;
        public NPCTableData GetNPCTableData(int id) => npcTables.ContainsKey(id) ? npcTables[id] : null;
        public ItemTableData GetItemTableData(int id) => itemTables.ContainsKey(id) ? itemTables[id] : null;
        public EatTableData GetEatTableData(int id) => eatTables.ContainsKey(id) ? eatTables[id] : null;
        public List<ShopItemTableData> GetShopItemTableData_Month(int month) => shopItemTables.Values.ToList().FindAll(data => data.Month == 0 || data.Month == month);
        public ShopItemTableData GetShopItemTableData(int id) => shopItemTables.ContainsKey(id) ? shopItemTables[id] : null;
        public NPCPosTableData GetNPCPosTableData(int id) => npcPosTables.ContainsKey(id) ? npcPosTables[id] : null;
        public List<NPCPosTableData> GetNPCPosTableDatas() => npcPosTables.Values.ToList();
        public NpcConversationTableData GetNpcConversationTableData(int id) => npcConversationTables.ContainsKey(id) ? npcConversationTables[id] : null;

        public ToolTableData GetToolTableData(int id) => toolTableDatas.ContainsKey(id) ? toolTableDatas[id] : null;
        public CropsTableData GetCropsTableData(int id) => cropsTableDatas.ContainsKey(id) ? cropsTableDatas[id] : null;
        public ObjectTableData GetObjectTableData(int id) => objectTableDatas.ContainsKey(id) ? objectTableDatas[id] : null;
        #region Etc


        Dictionary<string, Material> materials = new();
        Dictionary<string, UnityEngine.Object> datas = new();

        public Material GetMaterial(string key) => materials.ContainsKey(key) ? materials[key] : null;
        public string CategoryStr(ECategory category)
        {
            switch (category)
            {
                case ECategory.CATEGORY_TOOL:
                    return "도구";
                case ECategory.CATEGORY_INGREDIENT:
                    return "재료";
                case ECategory.CATEGORY_FORAGING:
                    return "채집품";
                case ECategory.CATEGORY_PRODUCTS:
                    return "생산품";
                case ECategory.CATEGORY_SEED:
                    return "씨앗";
                case ECategory.CATEGORY_FOOD:
                    return "음식";
                case ECategory.CATEGORY_ANIMALFORAGING:
                    return "동물 생산품";
                default:
                    return "";
            }
        }

        public int GetCropsIndex(int id, int day)
        {
            if (cropsTableDatas.ContainsKey(id) == false) { return 0; }

            var cropDays = cropsTableDatas[id].Days;

            for (int i = 0; i < cropDays.Length; i++)
            {
                if (cropDays[i] >= day)
                    return cropsTableDatas[id].Indexs[i];
            }

            return cropsTableDatas[id].Indexs[cropDays.Length - 1];
        }

        #endregion



        partial void AddLoadProcess()
        {
            AddLoadProcess_Json();
        }

        #region json

        private void AddLoadProcess_Json()
        {
            /*var objs = Resources.LoadAll("Data/");
            foreach (var obj in objs)
            {
                datas.Add(obj.name, obj);
            }*/

            #region data

            AddLoadProcess_Json<AnimalTableData>("AnimalData_Json", (datas) =>
            {
                foreach (var data in datas)
                {
                    animalTables.Add(data.Id, data);
                }
            });

            AddLoadProcess_Json<AnimalGrowTableData>("AnimalGrowData_Json", (datas) =>
            {
                foreach (var data in datas)
                {
                    animalGrowTables.Add(data.Id, data);
                }
            });


            AddLoadProcess_Json<NPCTableData>("NPCData_Json", (datas) =>
            {
                foreach (var data in datas)
                {
                    npcTables.Add(data.Id, data);
                }
            });


            AddLoadProcess_Json<ItemTableData>("ItemData_Json", (datas) =>
            {
                foreach (var data in datas)
                {
                    itemTables.Add(data.Id, data);
                }
            });



            AddLoadProcess_Json<EatTableData>("EatData_Json", (datas) =>
            {
                foreach (var data in datas)
                {
                    eatTables.Add(data.Id, data);
                }
            });



            AddLoadProcess_Json<ShopItemTableData>("ShopItemData_Json", (datas) =>
            {
                foreach (var data in datas)
                {
                    shopItemTables.Add(data.Id, data);
                }
            });



            AddLoadProcess_Json<NpcConversationTableData>("NPCConversationData_Json", (datas) =>
            {
                foreach (var data in datas)
                {
                    npcConversationTables.Add(data.Id, data);
                }
            });



            AddLoadProcess_Json<NPCPosTableData>("NPCPosData_Json", (datas) =>
            {
                foreach (var data in datas)
                {
                    npcPosTables.Add(data.Id, data);
                }
            });

            AddLoadProcess_Json<ToolTableData>("ToolData_Json", (datas) =>
            {
                foreach (var data in datas)
                {
                    toolTableDatas.Add(data.Id, data);
                }
            });

            AddLoadProcess_Json<CropsTableData>("CropsData_Json", (datas) =>
            {
                foreach (var data in datas)
                {
                    cropsTableDatas.Add(data.Id, data);
                }
            });

            AddLoadProcess_Json<ObjectTableData>("ObjectData_Json", (datas) =>
            {
                foreach (var data in datas)
                {
                    objectTableDatas.Add(data.Id, data);
                }
            });

            #endregion
        }


        void AddLoadProcess_Json<T>(string fileName, Action<List<T>> action, LoadingType loadingType = LoadingType.defaultProcess) where T : class
        {
            Loading.AddLoadProcess(loadingType,
                UniTask.Defer(async () => { await LoadAssetCoroutine_Json(fileName, action); }));
        }

        async UniTask LoadAssetCoroutine_Json<T>(string fileName, Action<List<T>> action) where T : class
        {
            var path = $"Address/Data/{fileName}.json";
            var tAsset = await AssetLoader.LoadAsync<TextAsset>(path);
            JsonDefaultForm<List<T>> form = Json.JsonToObject<JsonDefaultForm<List<T>>>(tAsset.text);
            action?.Invoke(form.data);

            AssetLoader.Unload(path, tAsset);
        }
        async UniTask LoadAssetCoroutine<T>(string filePath, Action<T> action) where T : UnityEngine.Object
        {
            var asset = await AssetLoader.LoadAsync<T>(filePath);
            action?.Invoke(asset);
        }

        #endregion
    }
}


