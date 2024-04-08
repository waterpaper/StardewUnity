using System.Collections.Generic;
using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// 움직임 컴포넌트
    /// </summary>
    public class MoveService : IService
    {
        public List<IMoveComponent> moveComponents = new();

        public void Add(IEntity entity)
        {
            if (entity is not IMoveComponent) return;

            moveComponents.Add(entity as IMoveComponent);
        }

        public void Remove(IEntity entity)
        {
            if (entity is not IMoveComponent) return;

            moveComponents.Remove(entity as IMoveComponent);
        }

        public void Clear()
        {
            moveComponents.Clear();
        }

        public void Update(double frameTime)
        { 
            foreach (var moveEntity in moveComponents)
            {
                if (moveEntity.MoveComponent.isEnable == false || moveEntity.MoveComponent.targetPos == Vector3.zero) continue;

                if (moveEntity.MoveComponent.targetPos.y > moveEntity.TransformComponent.position.y)
                {
                    moveEntity.TransformComponent.rotation = Vector3.up;
                }
                else if (moveEntity.MoveComponent.targetPos.y < moveEntity.TransformComponent.position.y)
                {
                    moveEntity.TransformComponent.rotation = Vector3.down;
                }
                else if (moveEntity.MoveComponent.targetPos.x > moveEntity.TransformComponent.position.x)
                {
                    moveEntity.TransformComponent.rotation = Vector3.right;
                }
                else if (moveEntity.MoveComponent.targetPos.x < moveEntity.TransformComponent.position.x)
                {
                    moveEntity.TransformComponent.rotation = Vector3.left;
                }
                else
                    continue;

                var power = moveEntity.MoveComponent.speed * (float)frameTime;
                var dis = Vector2.Distance(moveEntity.MoveComponent.targetPos, moveEntity.TransformComponent.position);
                if (power > dis)
                    power = dis;

                moveEntity.PhysicsComponent.velocity += moveEntity.TransformComponent.rotation * power;
                moveEntity.EventComponent?.onEvent("Direction");
            }
        }
    }
}

