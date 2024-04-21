using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    public readonly partial struct NpcAspect : IAspect, IWATPObjectAspect
    {
        #region Property
        public readonly Entity entity;

        readonly RefRW<TransformComponent> transformComponent;
        readonly RefRW<EventComponent> eventComponent;
        readonly RefRW<MoveComponent> moveComponent;
        readonly RefRW<ColliderComponent> colliderComponent;
        readonly RefRW<SightComponent> sightComponent;
        readonly RefRW<TargetableComponent> targetableComponent;
        readonly RefRW<DataComponent> dataComponent;
        readonly RefRW<PhysicsComponent> physicsComponent;
        readonly RefRW<CellTargetComponent> cellTargetComponent;
        readonly RefRW<ConversationComponent> conversationComponent;
        readonly RefRW<InteractionComponent> interactionComponent;
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
        public DataComponent DataComponent
        {
            get => dataComponent.ValueRO;
        }

        public float3 Rotation
        {
            get => transformComponent.ValueRO.rotation;
        }
        public float Speed
        {
            get => moveComponent.ValueRO.speed;
        }
        public DeleteComponent DeleteComponent
        {
            get => deleteComponent.ValueRO;
        }

        public void Init(float3 pos, int id, bool isMove)
        {
            transformComponent.ValueRW.position = pos;
            moveComponent.ValueRW.speed = 2;
            moveComponent.ValueRW.isEnable = isMove;
            colliderComponent.ValueRW.isEnable = true;
            colliderComponent.ValueRW.colliderType = (int)ColliderType.Square;
            colliderComponent.ValueRW.areaWidth = 1;
            colliderComponent.ValueRW.areaHeight = 1;
            targetableComponent.ValueRW.isEnable = true;
            physicsComponent.ValueRW.isEnable = true;
            sightComponent.ValueRW.value = 10;
            cellTargetComponent.ValueRW.isEnable = true;
            cellTargetComponent.ValueRW.refreshTime = UnityEngine.Random.Range(5.0f, 10.0f);

            dataComponent.ValueRW.id = id;
            dataComponent.ValueRW.dataUid = id;
            OnRef();
            eventComponent.ValueRW.events.Capacity = 5;
            cellTargetComponent.ValueRW.cellPaths.Capacity = 200;
        }

        public void OnInitialize()
        {
            eventComponent.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Initialize });
        }

        public void OnDestroy()
        {
            eventComponent.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Destroy });
            cellTargetComponent.ValueRW.cellPaths.Clear();
        }

        public void OnRef()
        {
            eventComponent.ValueRW.events = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<EventBuffer>(entity);
            cellTargetComponent.ValueRW.cellPaths = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<CellPath>(entity);
        }

    }
    #endregion
    /*

            private void StateSetting()
            {
                fsmComponent = new FsmComponent.FsmComponentBuilder()
                      // Idle -> ??
                      .AddState(new EState_Idle())
                      .AddTransition(StateType.Move, () => PhysicsComponent.velocity != Vector3.zero)
                      // Move -> ??
                      .AddState(new EState_Run())
                      .AddTransition(StateType.Idle, () => PhysicsComponent.velocity == Vector3.zero)

                      .AddState(new EFarmerState_Dead())
                      .SetDefaultState(StateType.Idle)
                      .Build(this);
            }


            private void OnDeath()
            {
                DeleteReservation = true;
            }
    */

    #region Builder

    public class NpcAspectBuilder : IEntityBuilder
    {
        int id;
        bool isMove =false;
        float3 pos = float3.zero;
        EventActionComponent eventActionComponent = new();
        NpcAspect npcAspect;

        // 나머지 선택 멤버는 메서드로 설정
        public NpcAspectBuilder()
        {
        }

        public NpcAspectBuilder SetPos(float3 pos)
        {
            this.pos = pos;
            return this;
        }

        public NpcAspectBuilder SetId(int id)
        {
            this.id = id;
            return this;
        }

        public NpcAspectBuilder SetIsMove(bool value)
        {
            this.isMove = value;
            return this;
        }
        public EventActionComponent GetEventSystem()
        {
            return eventActionComponent;
        }

        public IWATPObjectAspect GetObjectAspect()
        {
            return npcAspect;
        }

        public Entity Build()
        {
            var entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<TransformComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<EventComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<MoveComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<ColliderComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<SightComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<TargetableComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<DataComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<PhysicsComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<CellTargetComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<ConversationComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<InteractionComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<DeleteComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentObject(entity, eventActionComponent);

            var fsmCom = new FsmComponent.FsmComponentBuilder()
                        // Idle -> ??
                        .AddState(new EState_Idle())
                        .AddTransition(StateType.Move, (ref Entity entity) => {
                            var com = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<PhysicsComponent>(entity);
                            return !math.all(com.velocity == float3.zero);
                        })
                        // Move -> ??
                        .AddState(new EState_Run())
                        .AddTransition(StateType.Idle, (ref Entity entity) => {
                            var com = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<PhysicsComponent>(entity);
                            return math.all(com.velocity == float3.zero);
                        })
                        .AddState(new EFarmerState_Dead())
                        .SetDefaultState(StateType.Idle)
                        .Build(entity);

            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentObject(entity, fsmCom);

            var eventComponent = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<EventComponent>(entity);
            eventComponent.events = World.DefaultGameObjectInjectionWorld.EntityManager.AddBuffer<EventBuffer>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, eventComponent);

            var cellTargetComponent = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<CellTargetComponent>(entity);
            cellTargetComponent.cellPaths = World.DefaultGameObjectInjectionWorld.EntityManager.AddBuffer<CellPath>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, cellTargetComponent);

            npcAspect = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<NpcAspect>(entity);
            npcAspect.Init(pos, id, isMove);
            return entity;
        }
    }
    #endregion

}
