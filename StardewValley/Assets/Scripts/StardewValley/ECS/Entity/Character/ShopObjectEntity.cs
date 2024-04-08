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
            ShopObjectEntity entity = (ShopObjectEntity)MemberwiseClone();
            return entity;
        }

        #region Builder

        public class ShopObjectEntityBuilder : IEntityBuilder
        {
            int id;
            Vector3 pos = Vector3.zero;

            // ������ ���� ����� �޼���� ����
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
