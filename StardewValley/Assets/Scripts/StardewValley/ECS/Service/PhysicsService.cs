using System.Collections.Generic;
using UnityEngine;

namespace WATP.ECS
{
    public class PhysicsService : IService
    {
        public List<IPhysicsComponent> physicsComponents = new();

        public void Add(IEntity entity)
        {
            if (entity is not IPhysicsComponent) return;

            physicsComponents.Add(entity as IPhysicsComponent);
        }

        public void Remove(IEntity entity)
        {
            if (entity is not IPhysicsComponent) return;

            physicsComponents.Remove(entity as IPhysicsComponent);
        }

        public void Clear()
        {
            physicsComponents.Clear();
        }

        public void Update(double frameTime)
        {
            Vector3 afterPos = Vector3.zero;

            foreach (var physicsEntity in physicsComponents)
            {
                if (physicsEntity.PhysicsComponent.isEnable == false)
                {
                    physicsEntity.PhysicsComponent.velocity = Vector3.zero;
                    continue;
                }
                if (physicsEntity.PhysicsComponent.velocity == Vector3.zero) continue;

                afterPos = physicsEntity.TransformComponent.position + physicsEntity.PhysicsComponent.velocity;
                afterPos = new Vector2(((int)(afterPos.x * 1000)) / 1000.0f, ((int)(afterPos.y * 1000)) / 1000.0f);

                physicsEntity.TransformComponent.position = afterPos;
                physicsEntity.PhysicsComponent.velocity = Vector3.zero;
                continue;
            }
        }
    }
}
