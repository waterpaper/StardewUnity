using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{

    public readonly partial struct HoedirtAspect : IAspect, IWATPObjectAspect
    {
        #region Property

        public readonly Entity entity;
        readonly RefRW<TransformComponent> transformComponent;
        readonly RefRW<EventComponent> eventComponent;
        readonly RefRW<ColliderComponent> colliderComponent;
        readonly RefRW<PhysicsComponent> physicsComponent;
        readonly RefRW<HoedirtDataComponent> hoedirtDataComponent;
        readonly RefRW<DeleteComponent> deleteComponent;

        public Entity Entity
        {
            get => entity;
        }
        public int Index
        {
            get => entity.Index;
        }
        public bool DeleteReservation
        {
            get => deleteComponent.ValueRW.isDelate;
            set => deleteComponent.ValueRW.isDelate = value;
        }

        public float3 Position
        {
            get => transformComponent.ValueRO.position;
        }

        public HoedirtDataComponent HoedirtDataComponent
        {
            get => hoedirtDataComponent.ValueRO;
            set => hoedirtDataComponent.ValueRW = value;
        }
        public EventComponent EventComponent
        {
            get => eventComponent.ValueRO;
            set => eventComponent.ValueRW = value;
        }
        public bool HoedirtDataRight
        {
            set => hoedirtDataComponent.ValueRW.right = value;
        }
        public bool HoedirtDataLeft
        {
            set => hoedirtDataComponent.ValueRW.left = value;
        }
        public bool HoedirtDataUp
        {
            set => hoedirtDataComponent.ValueRW.up = value;
        }
        public bool HoedirtDataDown
        {
            set => hoedirtDataComponent.ValueRW.down = value;
        }

        public bool HoedirtDataWatering
        {
            set => hoedirtDataComponent.ValueRW.watering = value;
        }

        public DeleteComponent DeleteComponent
        {
            get => deleteComponent.ValueRO;
        }
        public void OnInitialize()
        {
            eventComponent.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Initialize });
        }

        public void OnDestroy()
        {
            eventComponent.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Destroy });
        }

        public void Init(float3 pos, bool isAdd)
        {
            transformComponent.ValueRW.position = pos;
            colliderComponent.ValueRW.isEnable = true;
            colliderComponent.ValueRW.colliderType = (int)ColliderType.Square;
            colliderComponent.ValueRW.areaWidth = 1;
            colliderComponent.ValueRW.areaHeight = 1;

            hoedirtDataComponent.ValueRW.add = isAdd;
            eventComponent.ValueRW.events.Capacity = 5;
            physicsComponent.ValueRW.isEnable = false;
        }
        public void OnRef()
        {
            eventComponent.ValueRW.events = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<EventBuffer>(entity);
        }

        #endregion
    }


    #region Builder

    public class HoedirtAspectBuilder : IEntityBuilder
    {
        bool isAdd = false;
        float3 pos = float3.zero;
        EventActionComponent eventActionComponent = new();
        HoedirtAspect hoedirtAspect;

        // 나머지 선택 멤버는 메서드로 설정
        public HoedirtAspectBuilder()
        {
        }

        public HoedirtAspectBuilder SetPos(float3 pos)
        {
            this.pos = pos;
            return this;
        }

        public HoedirtAspectBuilder SetisAdd(bool isAdd)
        {
            this.isAdd = isAdd;
            return this;
        }

        public EventActionComponent GetEventSystem()
        {
            return eventActionComponent;
        }

        public IWATPObjectAspect GetObjectAspect()
        {
            return hoedirtAspect;
        }

        public Entity Build()
        {
            var entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();

            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<TransformComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<EventComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<ColliderComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<PhysicsComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<HoedirtDataComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<DeleteComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentObject(entity, eventActionComponent);

            var eventComponent = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<EventComponent>(entity);
            eventComponent.events = World.DefaultGameObjectInjectionWorld.EntityManager.AddBuffer<EventBuffer>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, eventComponent);

            hoedirtAspect = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<HoedirtAspect>(entity);
            hoedirtAspect.Init(pos, isAdd);

            return entity;
        }
    }
    #endregion
}
