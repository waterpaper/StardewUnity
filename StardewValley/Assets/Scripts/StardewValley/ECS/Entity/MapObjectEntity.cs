using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// �� ��ȣ �ۿ� ������Ʈ Ŭ����(ũ������)
    /// ���ϴ� ����� ������Ʈ�� �߰��Ͽ� �����Ѵ�.
    /// �ʱ�ȭ�� �ʿ��� ��쿡�� Ŭ�������� ó���Ѵ�.
    /// </summary>
    [System.Serializable]
    public class MapObjectEntity : Entity, IColliderComponent, IPhysicsComponent, IMapObjectDataComponent, IDelayDeleteComponent
    {
        #region Property

        private ColliderComponent colliderComponent;
        public ColliderComponent ColliderComponent { get => colliderComponent; }


        private PhysicsComponent physicsComponent;
        public PhysicsComponent PhysicsComponent { get => physicsComponent; }


        private MapObjectDataComponent mapObjectDataComponent;
        public MapObjectDataComponent MapObjectDataComponent { get => mapObjectDataComponent; }


        private DelayDeleteComponent delayDeleteComponent;
        public DelayDeleteComponent DelayDeleteComponent { get => delayDeleteComponent; }

        protected MapObjectEntity(Vector3 pos, int id, int hp)
        {
            colliderComponent = new ColliderComponent();
            physicsComponent = new PhysicsComponent();
            mapObjectDataComponent = new MapObjectDataComponent();
            delayDeleteComponent = new DelayDeleteComponent();

            transformComponent.position = pos;
            delayDeleteComponent.deleteTime = 0.5f;
            mapObjectDataComponent.id = id;
            mapObjectDataComponent.hp = hp;
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

        public class MapObjectEntityBuilder : IEntityBuilder
        {
            int id = 0;
            int hp = 0;
            Vector3 pos = Vector3.zero;

            // ������ ���� ����� �޼���� ����
            public MapObjectEntityBuilder()
            {
            }

            public MapObjectEntityBuilder SetPos(Vector3 pos)
            {
                this.pos = pos;
                return this;
            }
            public MapObjectEntityBuilder SetId(int id)
            {
                this.id = id;
                return this;
            }
            public MapObjectEntityBuilder SetHp(int hp)
            {
                this.hp = hp;
                return this;
            }


            public IEntity Build()
            {
                return new MapObjectEntity(pos, id, hp).Builder<MapObjectEntity>();
            }
        }
        #endregion
    }
}
