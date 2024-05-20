using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// 맵 상호 작용 오브젝트 클래스(크래프팅)
    /// 원하는 기능을 컴포넌트를 추가하여 구현한다.
    /// 초기화가 필요한 경우에만 클래스에서 처리한다.
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

            // 나머지 선택 멤버는 메서드로 설정
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
