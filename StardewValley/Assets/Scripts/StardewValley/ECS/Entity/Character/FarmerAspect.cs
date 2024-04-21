using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// farmer의 필요한 component를 정의한 aspect
    /// 각 component의 정보 참조가 가능하며
    /// Builder를 통해 필요 정보를 미리 받아 생성처리한다.
    /// </summary>
    public readonly partial struct FarmerAspect : IAspect, IWATPObjectAspect
    {
        #region Property
        public readonly Entity entity;

        readonly RefRW<TransformComponent> transformComponent;
        readonly RefRW<EventComponent> eventComponent;
        readonly RefRW<PlayerComponent> playerComponent;
        readonly RefRW<MoveComponent> moveComponent;
        readonly RefRW<ColliderComponent> colliderComponent;
        readonly RefRW<SleepComponent> sleepComponent;
        readonly RefRW<TargetableComponent> targetableComponent;
        readonly RefRW<MoveInputComponent> moveInputComponent;
        readonly RefRW<PhysicsComponent> physicsComponent;
        readonly RefRW<WarpComponent> warpComponent;
        readonly RefRW<UsingInputComponent> usingInputComponent;
        readonly RefRW<InteractionInputComponent> interactionInputComponent;
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
            get => deleteComponent.ValueRO.isDelate;
            set => deleteComponent.ValueRW.isDelate = value;
        }

        public float3 Position
        {
            get => transformComponent.ValueRO.position;
        }
        public float3 Rotation
        {
            get => transformComponent.ValueRO.rotation;
        }
        public float Speed
        {
            get => moveComponent.ValueRO.speed;
        }

        public float3 TargetPostion
        {
            get => usingInputComponent.ValueRO.targetPos;
        }

        public bool MoveInputComponentEnable
        {
            get => moveInputComponent.ValueRO.isEnable;
            set => moveInputComponent.ValueRW.isEnable = value;
        }

        public bool InteractionInputComponentEnable
        {
            get => interactionInputComponent.ValueRO.isEnable;
            set => interactionInputComponent.ValueRW.isEnable = value;
        }

        public EventComponent EventComponent
        {
            get => eventComponent.ValueRO;
            set => eventComponent.ValueRW = value;
        }

        public void Init(float3 pos)
        {
            transformComponent.ValueRW.position = pos;
            playerComponent.ValueRW.value = true;
            moveComponent.ValueRW.speed = 3;
            moveComponent.ValueRW.isEnable = true;
            colliderComponent.ValueRW.isEnable = true;
            colliderComponent.ValueRW.colliderType = (int)ColliderType.Square;
            colliderComponent.ValueRW.areaWidth = 1;
            colliderComponent.ValueRW.areaHeight = 1;
            targetableComponent.ValueRW.isEnable = true;
            moveInputComponent.ValueRW.isEnable = true;
            physicsComponent.ValueRW.isEnable = true;
            warpComponent.ValueRW.isEnable = true;
            usingInputComponent.ValueRW.isEnable = true;
            interactionInputComponent.ValueRW.isEnable = true;

            deleteComponent.ValueRW.isEnable = false;
            eventComponent.ValueRW.events.Capacity = 5;
        }

        public void OnInitialize()
        {
            eventComponent.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Initialize });
        }

        public void OnDestroy()
        {
            eventComponent.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Destroy });
        }

        public void OnRef()
        {
            eventComponent.ValueRW.events = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<EventBuffer>(entity);
        }
        #endregion
    }

    #region Builder

    public class FarmerAspectBuilder : IEntityBuilder
    {
        float3 pos = float3.zero;
        EventActionComponent eventActionComponent = new EventActionComponent();
        FarmerAspect farmerAspect;

        // 나머지 선택 멤버는 메서드로 설정
        public FarmerAspectBuilder()
        {
        }

        public FarmerAspectBuilder SetPos(float3 pos)
        {
            this.pos = pos;
            return this;
        }

        public EventActionComponent GetEventSystem()
        {
            return eventActionComponent;
        }

        public IWATPObjectAspect GetObjectAspect()
        {
            return farmerAspect;
        }

        public Entity Build()
        {
            var entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<TransformComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<EventComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<PlayerComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<MoveComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<ColliderComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<SleepComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<TargetableComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<MoveInputComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<PhysicsComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<WarpComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<UsingInputComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<InteractionInputComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<DeleteComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentObject(entity, eventActionComponent);

            ///fsm 정의
            var actionState = new EFarmerState_Action();
            var fsmCom = new FsmComponent.FsmComponentBuilder()
                         // Idle -> ??
                         .AddState(new EState_Idle())
                         .AddTransition(StateType.Move, (ref Entity entity) => {
                             var com = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<PhysicsComponent>(entity);
                             return !math.all(com.velocity == float3.zero);
                         })
                         .AddTransition(StateType.Action, (ref Entity entity) => {
                             var com = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<UsingInputComponent>(entity);
                             return com.isAction && com.actionTimer == 0;
                         })
                         // Move -> ??
                         .AddState(new EState_Run())
                         .AddTransition(StateType.Idle, (ref Entity entity) => {
                             var com = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<PhysicsComponent>(entity);
                             return math.all(com.velocity == float3.zero);
                         })
                         .AddTransition(StateType.Action, (ref Entity entity) => {
                             var com = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<UsingInputComponent>(entity);
                             return com.isAction && com.actionTimer == 0;
                         })
                         .AddState(actionState)
                         .AddTransition(StateType.Idle, (ref Entity entity) => {
                             return actionState.Timer > 0.5f;
                         })

                         .AddState(new EFarmerState_Dead())
                         .SetDefaultState(StateType.Idle)
                         .Build(entity);

            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentObject(entity, fsmCom);

            var eventComponent = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<EventComponent>(entity);
            eventComponent.events = World.DefaultGameObjectInjectionWorld.EntityManager.AddBuffer<EventBuffer>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, eventComponent);

            farmerAspect = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<FarmerAspect>(entity);
            farmerAspect.Init(pos);

            return entity;
        }
    }

    #endregion
}
