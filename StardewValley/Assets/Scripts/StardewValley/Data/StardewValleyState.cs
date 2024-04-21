using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Entities;
using WATP.Data;
using WATP.ECS;
using WATP.Player;

namespace WATP
{
    /// <summary>
    /// 전체 게임 정보를 가지고 있는 클래스
    /// 플레이어 및 진행 정보를 가지고 있다.
    /// </summary>
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

        private Entity normalEntity, pauseEntity, mapUpdateEntity;


        public void Init()
        {
            var entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<RandomComponent>(entity);

            var random = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<RandomComponent>(entity);
            random.Random.state = 777777;
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, random);

            normalEntity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(NormalSystemsCompTag));
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<NormalSystemsCompTag>(normalEntity, false);

            pauseEntity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(PauseSystemsCompTag));
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<PauseSystemsCompTag>(pauseEntity, false);

            mapUpdateEntity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(MapUpdateOptionComponent));

            GameInit();
            logicState.onChange += OnStateChange;
        }

        public void Dispose()
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(normalEntity);
            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(pauseEntity);
            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(mapUpdateEntity);
            logicState.onChange -= OnStateChange;
        }


        public void GameInit()
        {
            player = new();
            inventory.Init();
            npcInfos.Clear();
            animalInfos.Clear();
            objectLoadMaps.Clear();
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
                MonthUpdateSetting();
            else
                day.Value = day.Value + 1;

            time.Value = 360;
            timer = 0;
            player.actingPower.Value = player.actingPowerMax.Value;
            player.hp.Value = player.maxHp.Value;

            var component = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<MapUpdateOptionComponent>(mapUpdateEntity);
            component.isDay = true;
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(mapUpdateEntity, component);
        }

        public void MonthUpdateSetting()
        {
            dayOfWeek.Value = DayOfWeek.Monday;
            day.Value = 1;
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


        private void OnStateChange(LogicState state)
        {
            if (LogicState.Normal != state)
            {
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<PauseSystemsCompTag>(pauseEntity, true);
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<NormalSystemsCompTag>(normalEntity, false);
            }
            else
            {
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<PauseSystemsCompTag>(pauseEntity, false);
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<NormalSystemsCompTag>(normalEntity, true);
            }
        }

    }
}