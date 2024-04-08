using UnityEngine;
using WATP.UI;

namespace WATP.ECS
{

    [System.Serializable]
    public class NpcEntity : Entity, IMoveComponent, IColliderComponent, ISightComponent, ITargetableComponent, IDataComponent, IPhysicsComponent, IConversationComponent, IInteractionComponent, ICellTargetComponent, IFsmComponent
    {
        #region Property

        private MoveComponent moveComponent;
        public MoveComponent MoveComponent { get => moveComponent; }

        private ColliderComponent colliderComponent;
        public ColliderComponent ColliderComponent { get => colliderComponent; }

        private SightComponent sightComponent;
        public SightComponent SightComponent { get => sightComponent; }

        private TargetableComponent targetableComponent;
        public TargetableComponent TargetableComponent { get => targetableComponent; }

        private DataComponent dataComponent;
        public DataComponent DataComponent { get => dataComponent; }

        private PhysicsComponent physicsComponent;
        public PhysicsComponent PhysicsComponent { get => physicsComponent; }

        private CellTargetComponent cellTargetComponent;
        public CellTargetComponent CellTargetComponent { get => cellTargetComponent; }

        public FsmComponent fsmComponent;
        public StateComponent StateComponent { get => fsmComponent; }

        protected NpcEntity(Vector3 pos, int id)
        {
            moveComponent = new MoveComponent();
            colliderComponent = new ColliderComponent();
            sightComponent = new SightComponent();
            targetableComponent = new TargetableComponent();
            dataComponent = new DataComponent();
            physicsComponent = new PhysicsComponent();
            cellTargetComponent = new CellTargetComponent();

            transformComponent.position = pos;
            moveComponent.speed = 2;

            var tableData = Root.GameDataManager.TableData.GetNPCPosTableData(id);
            if (tableData == null || tableData.Move == 0)
                moveComponent.isEnable = false;

            dataComponent.id = id;
            cellTargetComponent.refreshTime = Random.Range(10, 20);
            StateSetting();
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


        #region Builder

        public class NpcEntityBuilder : IEntityBuilder
        {
            int id;
            Vector3 pos = Vector3.zero;

            // 나머지 선택 멤버는 메서드로 설정
            public NpcEntityBuilder()
            {
            }

            public NpcEntityBuilder SetPos(Vector3 pos)
            {
                this.pos = pos;
                return this;
            }

            public NpcEntityBuilder SetId(int id)
            {
                this.id = id;
                return this;
            }

            public IEntity Build()
            {
                return new NpcEntity(pos, id).Builder<NpcEntity>();
            }
        }
        #endregion
    }
}
