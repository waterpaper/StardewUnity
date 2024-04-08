using System.Collections.Generic;
using UnityEngine;

namespace WATP.ECS
{
    public class PhysicsCollisionService : IService
    {
        public List<IColliderComponent> colliderComponents = new();

        public void Add(IEntity entity)
        {
            if (entity is not IColliderComponent) return;

            colliderComponents.Add(entity as IColliderComponent);
        }

        public void Remove(IEntity entity)
        {
            if (entity is not IColliderComponent) return;

            colliderComponents.Remove(entity as IColliderComponent);
        }

        public void Clear()
        {
            colliderComponents.Clear();
        }

        public void Update(double frameTime)
        {
            Vector3 afterPos = Vector3.zero;
            IColliderComponent collisionObj = null;

            foreach (var colliderEntity in colliderComponents)
            {
                if (colliderEntity.PhysicsComponent.isEnable == false) continue;
                if (colliderEntity.ColliderComponent.isEnable == false) continue;

                if (colliderEntity.PhysicsComponent.velocity == Vector3.zero) continue;

                afterPos = colliderEntity.TransformComponent.position + colliderEntity.PhysicsComponent.velocity;
                afterPos = new Vector2(((int)(afterPos.x * 1000)) / 1000.0f, ((int)(afterPos.y * 1000)) / 1000.0f);
                bool isMove = true;

                // 맵 체크
                isMove = colliderEntity.ColliderComponent.colliderType == ColliderType.Square ? IsMove_ColliderRect(afterPos, colliderEntity.ColliderComponent.areaWidth, colliderEntity.ColliderComponent.areaHeight)
                    : IsMove_ColliderCircle(afterPos, colliderEntity.ColliderComponent.areaWidth);

                // 오브젝트 체크
                if (isMove)
                {
                    foreach (var collision in colliderComponents)
                    {
                        if (collision.PhysicsComponent.isEnable == false) continue;
                        if (collision.ColliderComponent.isEnable == false) continue;
                        if (colliderEntity == collision) continue;

                        isMove = !IsCollision(afterPos, colliderEntity.ColliderComponent.colliderType,
                            colliderEntity.ColliderComponent.areaWidth, colliderEntity.ColliderComponent.areaHeight, collision);
                        if (!isMove)
                        {
                            collisionObj = collision;
                            break;
                        }
                    }
                }

                if (isMove == false)
                    colliderEntity.PhysicsComponent.velocity = Vector3.zero;
            }
        }


        public bool IsMove_ColliderCircle(Vector3 pos, float range)
        {
            var cell1 = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x + range - 0.01f), (int)pos.y);
            var cell2 = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x - range), (int)pos.y);
            var cell3 = Root.SceneLoader.TileMapManager.GetCell((int)pos.x, (int)(pos.y + range - 0.01f));
            var cell4 = Root.SceneLoader.TileMapManager.GetCell((int)pos.x, (int)(pos.y - range));

            if (cell1 == null || cell1.Block || cell2 == null || cell2.Block || cell3 == null || cell3.Block || cell4 == null || cell4.Block)
                return false;

            return true;
        }

        public bool IsMove_ColliderRect(Vector3 pos, float width, float height)
        {
            float halfWidth = width / 2;
            float halfHeight = height / 2;

            var cell1 = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x + halfWidth - 0.01f), (int)(pos.y + halfHeight - 0.01f));
            var cell2 = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x + halfWidth - 0.01f), (int)(pos.y - halfHeight + 0.01f));
            var cell3 = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x - halfWidth + 0.01f), (int)(pos.y + halfHeight - 0.01f));
            var cell4 = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x - halfWidth + 0.01f), (int)(pos.y - halfHeight + 0.01f));

            if (cell1 == null || cell1.Block || cell2 == null || cell2.Block || cell3 == null || cell3.Block || cell4 == null || cell4.Block)
                return false;

            return true;
        }

        public bool IsCollision(Vector2 movePos, ColliderType type, float width, float height, IColliderComponent collision)
        {
            return type switch
            {
                ColliderType.Circle => IsCollision_Circle(movePos, width, height, collision),
                ColliderType.Square => IsCollision_Box(movePos, width, height, collision),
                _ => false,
            };
        }

        bool IsCollision_Circle(Vector2 movePos, float width, float height, IColliderComponent collision)
        {
            float range = width;
            float helfWidth = 0;
            float helfHeight = 0;

            if (collision.ColliderComponent.colliderType == ColliderType.Circle)
            {
                if (range == -1 || Vector3.Distance(movePos, collision.TransformComponent.position) - range < collision.ColliderComponent.areaWidth)
                    return true;
            }
            else if (collision.ColliderComponent.colliderType == ColliderType.Square)
            {
                helfWidth = collision.ColliderComponent.areaWidth / 2;
                helfHeight = collision.ColliderComponent.areaHeight / 2;

                float circleDistanceX = movePos.x - collision.TransformComponent.position.x;
                float circleDistanceY = movePos.y - collision.TransformComponent.position.y;

                if (circleDistanceX > (helfWidth + range)) { return false; }
                if (circleDistanceY > (helfHeight + range)) { return false; }

                if (circleDistanceX <= (helfWidth)) { return true; }
                if (circleDistanceY <= (helfHeight)) { return true; }

                float cornerDistance = (circleDistanceX - helfWidth) * 2 +
                                        (circleDistanceY - helfHeight) * 2;

                return (cornerDistance <= (range * 2));
            }

            return false;
        }


        bool IsCollision_Box(Vector2 movePos, float width, float height, IColliderComponent collision)
        {
            float helfWidth = width / 2;
            float helfHeight = height / 2;

            if (collision.ColliderComponent.colliderType == ColliderType.Circle)
            {
                float circleDistanceX = movePos.x - collision.TransformComponent.position.x;
                float circleDistanceY = movePos.y - collision.TransformComponent.position.y;

                if (circleDistanceX > (helfWidth + collision.ColliderComponent.areaWidth)) { return false; }
                if (circleDistanceY > (helfHeight + collision.ColliderComponent.areaWidth)) { return false; }

                if (circleDistanceX <= (helfWidth)) { return true; }
                if (circleDistanceY <= (helfHeight)) { return true; }

                float cornerDistance = (circleDistanceX - helfWidth) * 2 +
                                     (circleDistanceY - helfHeight) * 2;

                if (cornerDistance <= (collision.ColliderComponent.areaWidth * 2))
                    return true;
                else
                    return false;

            }
            else if (collision.ColliderComponent.colliderType == ColliderType.Square)
            {
                float targetHelfWidth = collision.ColliderComponent.areaWidth / 2;
                float targetHelfHeight = collision.ColliderComponent.areaHeight / 2;

                if (movePos.x - helfWidth < collision.TransformComponent.position.x + targetHelfWidth
                    && movePos.x + helfWidth > collision.TransformComponent.position.x - targetHelfWidth
                    && movePos.y + helfHeight > collision.TransformComponent.position.y - targetHelfHeight
                    && movePos.y - helfHeight < collision.TransformComponent.position.y + targetHelfHeight)
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
