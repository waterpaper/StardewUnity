using UnityEngine;

namespace WATP.ECS
{

    [System.Serializable]
    public class HoedirtEntity : Entity, IColliderComponent, IPhysicsComponent, IHoedirtDataComponent
    {
        #region Property

        private ColliderComponent colliderComponent;
        public ColliderComponent ColliderComponent { get => colliderComponent; }

        private PhysicsComponent physicsComponent;
        public PhysicsComponent PhysicsComponent { get => physicsComponent; }

        private HoedirtDataComponent hoedirtDataComponent;
        public HoedirtDataComponent HoedirtDataComponent { get => hoedirtDataComponent; }

        protected HoedirtEntity(Vector3 pos, bool isAdd)
        {
            colliderComponent = new ColliderComponent();
            physicsComponent = new PhysicsComponent();
            hoedirtDataComponent = new HoedirtDataComponent();
            physicsComponent.isEnable = false;

            hoedirtDataComponent.add = isAdd;
            transformComponent.position = pos;
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

        public class HoedirtEntityBuilder : IEntityBuilder
        {
            bool isAdd = false;
            Vector3 pos = Vector3.zero;

            // ������ ���� ����� �޼���� ����
            public HoedirtEntityBuilder()
            {
            }

            public HoedirtEntityBuilder SetPos(Vector3 pos)
            {
                this.pos = pos;
                return this;
            }

            public HoedirtEntityBuilder SetisAdd(bool isAdd)
            {
                this.isAdd = isAdd;
                return this;
            }


            public IEntity Build()
            {
                return new HoedirtEntity(pos, isAdd).Builder<HoedirtEntity>();
            }
        }
        #endregion
    }
}
