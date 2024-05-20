using System.Collections.Generic;
using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// 움직임 입력 컴포넌트
    /// 물리에 이동 속도를 추가한다.
    /// </summary>
    public class InputMoveService : IService
    {
        public List<ITransformComponent> transformComponents = new();
        public List<IMoveInputComponent> moveInputComponents = new();

        public void Add(IEntity entity)
        {
            if (entity is ITransformComponent)
            {
                transformComponents.Add(entity as ITransformComponent);
            }

            if (entity is not IMoveInputComponent) return;

            moveInputComponents.Add(entity as IMoveInputComponent);
        }

        public void Remove(IEntity entity)
        {
            if (entity is ITransformComponent)
            {
                transformComponents.Remove(entity as ITransformComponent);
            }

            if (entity is not IMoveInputComponent) return;

            moveInputComponents.Remove(entity as IMoveInputComponent);
        }

        public void Clear()
        {
            moveInputComponents.Clear();
            transformComponents.Clear();
        }

        public void Update(double frameTime)
        {
            EDirection direction = EDirection.DIRECTION_NULL;
            Vector3 velocity = Vector3.zero;

            foreach (var inputComponent in moveInputComponents)
            {
                if (inputComponent.MoveInputComponent.isEnable == false) continue;
                velocity = Vector3.zero;

                // Movement controls

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    direction = EDirection.DIRECTION_LEFT;
                    velocity += (float)frameTime * Vector3.left;
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    direction = EDirection.DIRECTION_RIGHT;
                    velocity += (float)frameTime * Vector3.right;
                }

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    direction = EDirection.DIRECTION_UP;
                    velocity += (float)frameTime * Vector3.up;
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    direction = EDirection.DIRECTION_DOWN;
                    velocity += (float)frameTime * Vector3.down;
                }

                if (direction != EDirection.DIRECTION_NULL && inputComponent is IPhysicsComponent && inputComponent is IMoveComponent)
                {
                    var physicsComponent = inputComponent as IPhysicsComponent;
                    var moveComponent = inputComponent as IMoveComponent;

                    physicsComponent.PhysicsComponent.velocity += moveComponent.MoveComponent.speed * velocity;

                    if (direction == EDirection.DIRECTION_LEFT)
                        physicsComponent.TransformComponent.rotation = Vector3.left;
                    else if (direction == EDirection.DIRECTION_RIGHT)
                        physicsComponent.TransformComponent.rotation = Vector3.right;
                    else if (direction == EDirection.DIRECTION_UP)
                        physicsComponent.TransformComponent.rotation = Vector3.up;
                    else if (direction == EDirection.DIRECTION_DOWN)
                        physicsComponent.TransformComponent.rotation = Vector3.down;

                    moveComponent.EventComponent?.onEvent("Direction");
                }
            }
        }
    }
}