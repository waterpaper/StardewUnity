using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// 동물 클래스
    /// 원하는 기능을 컴포넌트를 추가하여 구현한다.
    /// 초기화가 필요한 경우에만 클래스에서 처리한다.
    /// </summary>
    [System.Serializable]
    public class AnimalEntity : Entity, IMoveComponent, IColliderComponent, ISightComponent, ITargetableComponent, IDataComponent, IPhysicsComponent, ICellTargetComponent, IFsmComponent, ICellAddObjectComponent
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

        protected AnimalEntity(Vector3 pos, int id, int datauid)
        {
            moveComponent = new MoveComponent();
            colliderComponent = new ColliderComponent();
            sightComponent = new SightComponent();
            targetableComponent = new TargetableComponent();
            dataComponent = new DataComponent();
            physicsComponent = new PhysicsComponent();
            cellTargetComponent = new CellTargetComponent();

            transformComponent.position = pos;
            transformComponent.rotation = Vector3.down;
            moveComponent.speed = 2;

            dataComponent.id = id;
            dataComponent.datauid = datauid;

            var tableData = Root.GameDataManager.TableData.GetAnimalTableData(id);
            if (tableData == null) return;

            colliderComponent.areaWidth = tableData.Width;
            colliderComponent.areaHeight = tableData.Height;
            cellTargetComponent.refreshTime = Random.Range(3, 10);
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

        public class AnimalEntityBuilder : IEntityBuilder
        {
            int id;
            int datauid;
            Vector3 pos = Vector3.zero;

            // 나머지 선택 멤버는 메서드로 설정
            public AnimalEntityBuilder()
            {
            }

            public AnimalEntityBuilder SetPos(Vector3 pos)
            {
                this.pos = pos;
                return this;
            }

            public AnimalEntityBuilder SetId(int id)
            {
                this.id = id;
                return this;
            }

            public AnimalEntityBuilder SetDataUid(int uid)
            {
                this.datauid = uid;
                return this;
            }

            public IEntity Build()
            {
                return new AnimalEntity(pos, id, datauid).Builder<AnimalEntity>();
            }
        }
        #endregion
    }
}
