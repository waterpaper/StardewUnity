using UnityEngine;

namespace WATP.ECS
{

    [System.Serializable]
    public class CropsEntity : Entity, IColliderComponent, IPhysicsComponent, ICropsDataComponent, IInteractionComponent, IDelayDeleteComponent
    {
        #region Property

        private ColliderComponent colliderComponent;
        public ColliderComponent ColliderComponent { get => colliderComponent; }

        private PhysicsComponent physicsComponent;
        public PhysicsComponent PhysicsComponent { get => physicsComponent; }

        private CropsDataComponent cropsDataComponent;
        public CropsDataComponent CropsDataComponent { get => cropsDataComponent; }


        private DelayDeleteComponent delayDeleteComponent;
        public DelayDeleteComponent DelayDeleteComponent { get => delayDeleteComponent; }

        protected CropsEntity(Vector3 pos, int id, int day)
        {
            colliderComponent = new ColliderComponent();
            physicsComponent = new PhysicsComponent();
            cropsDataComponent = new CropsDataComponent();
            delayDeleteComponent = new DelayDeleteComponent();
            physicsComponent.isEnable = false;

            transformComponent.position = pos;
            cropsDataComponent.id = id;
            cropsDataComponent.day = day;

            delayDeleteComponent.deleteTime = 0.7f;
        }


        #endregion

        /// <summary>
        /// entity �ʱ�ȭ
        /// </summary>
        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        /// <summary>
        /// entity ����
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// <summary>
        /// ���� ���簡 �ȵ� �����Ͱ� ������ ���� �����ؾ� ��
        /// </summary>
        public override IEntity Clone()
        {
            FarmerEntity entity = (FarmerEntity)MemberwiseClone();
            return entity;
        }

        private void OnDeath()
        {
            DeleteReservation = true;
        }


        #region Builder

        public class CropsEntityBuilder : IEntityBuilder
        {
            Vector3 pos = Vector3.zero;
            int id;
            int day;

            // ������ ���� ����� �޼���� ����
            public CropsEntityBuilder()
            {
            }

            public CropsEntityBuilder SetPos(Vector3 pos)
            {
                this.pos = pos;
                return this;
            }

            public CropsEntityBuilder SetId(int id)
            {
                this.id = id;
                return this;
            }

            public CropsEntityBuilder SetDay(int day)
            {
                this.day = day;
                return this;
            }
            public IEntity Build()
            {
                return new CropsEntity(pos, id, day).Builder<CropsEntity>();
            }
        }
        #endregion
    }
}
