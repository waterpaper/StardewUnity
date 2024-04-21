using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using WATP.ECS;


namespace WATP
{
    /// <summary>
    /// 각 tile map에 해당하는 aspect를 생성 및 관리하는 클래스
    /// entity 만으로는 관리하기 쉽지 않다고 판단하여서(어떤 entity인지 조회가 쉽지 않음) aspect로 component를 모아서 관리한다.
    /// 이 곳에서는 structural change 발생을 제어하기 위해 aspect 생성, 제거, 재참조을 모아 처리한다.
    /// </summary>
    public class MapObjectManager
    {
        private bool isRefUpdate = false;
        private IWATPObjectAspect player;

        protected List<IEntityBuilder> addBuilderList = new();
        protected List<IWATPObjectAspect> removeList = new();
        protected List<IWATPObjectAspect> aspects = new();

        public float3 PlayerPosision { get => player.Position; }

        public void Init()
        {
            Bind();
        }

        public void Dispose()
        {
            UnBind();
        }

        public void Clear(bool isPlayer)
        {
            foreach (var aspect in aspects)
                aspect.DeleteReservation = true;

            if (player != null)
                player.DeleteReservation = isPlayer;

            ECSController.ServiceEvents.Emit(new EventDeleteRoutine());
        }

        /// <summary>
        /// update 이후 system update(ecs) 동작
        /// </summary>
        public void Update()
        {
            DestroyUpdate();
            CreateUpdate();
            RefUpdate();
        }

        /// <summary>
        /// 참조 업데이트
        /// </summary>
        private void RefUpdate()
        {
            if (isRefUpdate == false) return;
            for (var i = 0; i < aspects.Count; i++)
            {
                switch(aspects[i])
                {
                    case FarmerAspect:
                        aspects[i] = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<FarmerAspect>(aspects[i].Entity);
                        player = aspects[i];
                        break;
                    case AnimalAspect:
                        aspects[i] = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<AnimalAspect>(aspects[i].Entity);
                        break;
                    case NpcAspect:
                        aspects[i] = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<NpcAspect>(aspects[i].Entity);
                        break;
                    case CropsAspect:
                        aspects[i] = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<CropsAspect>(aspects[i].Entity);
                        break;
                    case HoedirtAspect:
                        aspects[i] = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<HoedirtAspect>(aspects[i].Entity);
                        break;
                    case ShopObjectAspect:
                        aspects[i] = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<ShopObjectAspect>(aspects[i].Entity);
                        break;
                    case MapObjectAspect:
                        aspects[i] = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<MapObjectAspect>(aspects[i].Entity);
                        break;
                }

                aspects[i].OnRef();
                ECSController.ServiceEvents.Emit(new EventRefUpdate(aspects[i]));
            }

            isRefUpdate = false;
        }

        /// <summary>
        /// 생성 요청 처리 부분
        /// </summary>
        private void CreateUpdate()
        {
            if (addBuilderList.Count <= 0) return;

            for (int i = 0; i < addBuilderList.Count; i++)
            {
                var entity = addBuilderList[i].Build();
                addBuilderList[i].GetObjectAspect().OnInitialize();

                aspects.Add(addBuilderList[i].GetObjectAspect());

                if (addBuilderList[i].GetObjectAspect() is FarmerAspect)
                    player = addBuilderList[i].GetObjectAspect();

                ECSController.ServiceEvents.Emit(new EventCreateEntity(addBuilderList[i].GetObjectAspect()));
                isRefUpdate = true;
            }

            addBuilderList.Clear();
        }

        /// <summary>
        /// 삭제 요청 처리 부분
        /// </summary>
        private void DestroyUpdate()
        {
            foreach (var aspect in aspects)
            {
                if (aspect.DeleteReservation)
                {
                    removeList.Add(aspect);
                    aspect.OnDestroy();
                    ECSController.ServiceEvents.Emit(new EventDeleteEntity(aspect));
                }
            }

            foreach (var aspect in removeList)
            {
                World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(aspect.Entity);
                aspects.Remove(aspect);
                isRefUpdate = true;
            }

            removeList.Clear();
        }

        #region Event

        private void Bind()
        {
            ECSController.ServiceEvents.On<EventAddEntity>(OnObjectAdd);
            ECSController.ServiceEvents.On<EventDeleteRoutine>(OnEventDeleteRoutine);

            Root.SceneLoader.onSceneChangeStart += OnSceenChangeStart;
            Root.SceneLoader.TileMapManager.onMapChangeEnd += OnMapChange;
        }

        private void UnBind()
        {
            ECSController.ServiceEvents.Off<EventAddEntity>(OnObjectAdd);
            ECSController.ServiceEvents.Off<EventDeleteRoutine>(OnEventDeleteRoutine);

            Root.SceneLoader.onSceneChangeStart -= OnSceenChangeStart;
            Root.SceneLoader.TileMapManager.onMapChangeEnd -= OnMapChange;

        }

        private void OnObjectAdd(EventAddEntity e)
        {
            addBuilderList.Add(e.EntityBuilder);
        }

        private void OnEventDeleteRoutine(EventDeleteRoutine e)
        {
            if (e.isAll)
            {
                foreach (var aspect in aspects)
                    aspect.DeleteReservation = true;
            }
            else if (e.isRemove)
            {
                foreach (var aspect in aspects)
                {
                    if (aspect.Position.x == e.posX && aspect.Position.y == e.posY)
                    {
                        aspect.DeleteReservation = true;
                        break;
                    }
                }
            }
        }

        private void OnSceenChangeStart(SceneKind sceneKind)
        {
            if (sceneKind == SceneKind.Ingame)
            {
                var farmer = new WATP.ECS.FarmerAspectBuilder()
                    .SetPos(new float3(5, 5, 0));

                WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(farmer));
            }
            else
            {
                Clear(true);
            }
        }

        private void OnMapChange(string map)
        {
            Clear(false);
            ObjectLoad(map);

            if (player == null) return;
            NpcLoad(map);
            AnimalLoad(map);
            FarmLoad(map);
        }

        private void NpcLoad(string mapName)
        {
            var list = Root.GameDataManager.TableData.GetNPCPosTableDatas();

            foreach (var item in list)
            {
                if (item.Map == mapName)
                {
                    var tableData = Root.GameDataManager.TableData.GetNPCPosTableData(item.Id);
                    var npc = new WATP.ECS.NpcAspectBuilder()
                        .SetPos(new float3(item.PosX, item.PosY, 0))
                        .SetIsMove(tableData != null && tableData.Move == 1)
                        .SetId(item.Id);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(npc));
                }
            }
        }

        private void AnimalLoad(string mapName)
        {
            if (mapName == "Coop")
            {
                var list = Root.State.GetAnimals(1);
                int i = 0;
                foreach (var item in list)
                {
                    var animal = new WATP.ECS.AnimalAspectBuilder()
                        .SetPos(new float3((i + 3), (i + 3), 0))
                        .SetId(item.id)
                        .SetDataUid(item.animalUid);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(animal));
                    i++;
                }
            }
            else if (mapName == "Barn")
            {
                //stress test 용도
                for (int i = 0; i < 5; i++)
                {
                    for (int x = 0; x < 2; x++)
                    {
                        for (int y = 0; y < 50; y++)
                        {
                            var animal = new WATP.ECS.AnimalAspectBuilder()
                            .SetPos(new float3(5.5f + (10 * i) + (5 * x), (y * 2) + 0.5f, 0))
                            .SetId(100002 + i);

                            WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(animal));
                        }
                    }
                }
            }
        }

        private void ObjectLoad(string mapName)
        {
            if (mapName == "Shop")
            {
                var shopObj = new WATP.ECS.ShopObjectAspectBuilder()
                .SetPos(new float3(6.5f, 12.5f, 0));

                WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(shopObj));
            }

            if (Root.State.objectLoadMaps.Find(data => data == mapName) == null)
            {
                var list = Root.SceneLoader.TileMapManager.SaveObjectInfos;

                foreach (var data in list)
                    Root.State.AddObject(mapName, data.objectId, data.posX, data.posY, Root.GameDataManager.TableData.GetObjectTableData(data.objectId).Hp);
            }

            var objectList = Root.State.GetObjects(mapName);
            foreach (var data in objectList)
            {
                var mapObject = new WATP.ECS.MapObjectAspectBuilder()
                    .SetPos(new float3(data.posX, data.posY, 0))
                    .SetId(data.id)
                    .SetHp(data.hp);

                WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(mapObject));
            }
        }

        private void FarmLoad(string mapName)
        {
            var hoedirtList = Root.State.GetHoedirts(mapName);
            if (hoedirtList != null)
            {
                foreach (var item in hoedirtList)
                {
                    var hoedirt = new WATP.ECS.HoedirtAspectBuilder()
                            .SetPos(new float3(item.posX, item.posY, 0));

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(hoedirt));
                }

                if (hoedirtList.Count > 0)
                    Root.State.HoedirtUpdate();
            }

            var cropsList = Root.State.GetCrops(mapName);
            if (cropsList != null)
            {
                foreach (var item in cropsList)
                {
                    var crops = new WATP.ECS.CropsAspectBuilder()
                            .SetPos(new float3(item.posX, item.posY, 0))
                            .SetId(item.id)
                            .SetDay(item.day);

                    WATP.ECS.ECSController.ServiceEvents.Emit(new WATP.ECS.EventAddEntity(crops));
                }
            }
        }
    }
    #endregion
}