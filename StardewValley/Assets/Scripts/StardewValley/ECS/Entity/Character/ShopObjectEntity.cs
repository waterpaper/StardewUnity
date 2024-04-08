using UnityEngine;

namespace WATP.ECS
{

    [System.Serializable]
    public class ShopObjectEntity : Entity, IInteractionInputComponent, IShopObjectComponent
    {
        private InteractionInputComponent interactionInputComponent;
        public InteractionInputComponent InteractionInputComponent { get => interactionInputComponent; }
        #region Property

        protected ShopObjectEntity(Vector2 pos)
        {
            transformComponent.position = pos;
            interactionInputComponent = new InteractionInputComponent();
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
            ShopObjectEntity entity = (ShopObjectEntity)MemberwiseClone();
            return entity;
        }

        #region Builder

        public class ShopObjectEntityBuilder : IEntityBuilder
        {
            int id;
            Vector3 pos = Vector3.zero;

            // 나머지 선택 멤버는 메서드로 설정
            public ShopObjectEntityBuilder()
            {
            }

            public ShopObjectEntityBuilder SetPos(Vector3 pos)
            {
                this.pos = pos;
                return this;
            }

            public IEntity Build()
            {
                return new ShopObjectEntity(pos).Builder<ShopObjectEntity>();
            }
        }
        #endregion
    }
}
