using UnityEngine;

namespace WATP.ECS
{
    
    [System.Serializable]
    public class FarmerEntity : Entity, IPlayerComponent, IMoveComponent, IColliderComponent, ISleepComponent, ITargetableComponent, IPhysicsComponent, IMoveInputComponent, IInteractionInputComponent, IUsingInputComponent, IWarpComponent, IFsmComponent
    {
        #region Property

        private PlayerComponent playerComponent;
        public PlayerComponent PlayerComponent { get => playerComponent; }


        private MoveComponent moveComponent;
        public MoveComponent MoveComponent { get => moveComponent; }


        private ColliderComponent colliderComponent;
        public ColliderComponent ColliderComponent { get => colliderComponent; }


        private SleepComponent sleepComponent;
        public SleepComponent SleepComponent { get => sleepComponent; }


        private TargetableComponent targetableComponent;
        public TargetableComponent TargetableComponent { get => targetableComponent; }


        public PhysicsComponent physicsComponent;
        public PhysicsComponent PhysicsComponent { get => physicsComponent; }


        public WarpComponent warpComponent;
        public WarpComponent WarpComponent { get => warpComponent; }

        public FsmComponent fsmComponent;
        public StateComponent StateComponent { get => fsmComponent; }

        public UsingInputComponent usingInputComponent;
        public UsingInputComponent UsingInputComponent { get => usingInputComponent; }


        public InteractionInputComponent interactionInputComponent;
        public InteractionInputComponent InteractionInputComponent { get => interactionInputComponent; }


        public MoveInputComponent moveInputComponent;
        public MoveInputComponent MoveInputComponent { get => moveInputComponent; }

        protected FarmerEntity(Vector3 pos)
        {
            playerComponent = new PlayerComponent();
            playerComponent.value = true;

            moveComponent = new MoveComponent();
            moveComponent.speed = 3;

            colliderComponent = new ColliderComponent();
            colliderComponent.colliderType = ColliderType.Square;
            colliderComponent.areaWidth = 1;
            colliderComponent.areaHeight =1;

            sleepComponent = new SleepComponent();
            targetableComponent = new TargetableComponent();
            physicsComponent = new PhysicsComponent();
            warpComponent = new WarpComponent();
            usingInputComponent =new UsingInputComponent();
            interactionInputComponent = new InteractionInputComponent();
            moveInputComponent = new MoveInputComponent();

            StateSetting();

            this.transformComponent.position = pos;
        }


        #endregion

        /// <summary>
        /// entity 초기화
        /// </summary>
        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        /// <summary>
        /// entity 해제
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// <summary>
        /// 추후 복사가 안된 데이터가 있으면 직접 복사해야 함
        /// </summary>
        public override IEntity Clone()
        {
            FarmerEntity entity = (FarmerEntity)MemberwiseClone();
            return entity;
        }

        private void StateSetting()
        {
            var actionState = new EFarmerState_Action();
            fsmComponent = new FsmComponent.FsmComponentBuilder()
                  // Idle -> ??
                  .AddState(new EState_Idle())
                  .AddTransition(StateType.Move, () => PhysicsComponent.velocity != Vector3.zero)
                  .AddTransition(StateType.Action, () => usingInputComponent.isAction && usingInputComponent.actionTimer == 0)
                  // Move -> ??
                  .AddState(new EState_Run())
                  .AddTransition(StateType.Idle, () => PhysicsComponent.velocity == Vector3.zero)
                  .AddTransition(StateType.Action, () => usingInputComponent.isAction && usingInputComponent.actionTimer == 0)

                  .AddState(actionState)
                  .AddTransition(StateType.Idle, () => actionState.Timer > 0.5f)

                  .AddState(new EFarmerState_Dead())
                  .SetDefaultState(StateType.Idle)
                  .Build(this);
        }

        private void OnDeath()
        {
            DeleteReservation = true;
        }


        #region Builder

        public class FarmerEntityBuilder : IEntityBuilder
        {
            Vector3 pos = Vector3.zero;

            // 나머지 선택 멤버는 메서드로 설정
            public FarmerEntityBuilder()
            {
            }

            public FarmerEntityBuilder SetPos(Vector3 pos)
            {
                this.pos = pos;
                return this;
            }

            public IEntity Build()
            {
                return new FarmerEntity(pos).Builder<FarmerEntity>();
            }
        }
        #endregion
    }
}
