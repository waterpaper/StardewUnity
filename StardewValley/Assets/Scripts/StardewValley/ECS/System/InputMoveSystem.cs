using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// input 이벤트중 움직임에 대한 이벤트를 처리한다.
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    [UpdateBefore(typeof(InputInteractionSystem))]
    [UpdateAfter(typeof(MoveSystem))]
    public partial struct InputMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MoveInputComponent>();
            state.RequireForUpdate<NormalSystemsCompTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var tag in SystemAPI.Query<RefRO<PauseSystemsCompTag>>())
                return;

            EDirection direction = EDirection.DIRECTION_NULL;
            float3 velocity = float3.zero;
            float frameTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, physics, eventComponent, move, moveInput) in SystemAPI.Query<RefRW<TransformComponent>, RefRW<PhysicsComponent>, RefRW<EventComponent>, RefRO<MoveComponent>, RefRO<MoveInputComponent>>())
            {
                if (moveInput.ValueRO.isEnable == false) continue;
                velocity = float3.zero;

                // Movement controls

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    direction = EDirection.DIRECTION_LEFT;
                    velocity += (float)frameTime * new float3(-1, 0, 0);
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    direction = EDirection.DIRECTION_RIGHT;
                    velocity += (float)frameTime * new float3(1, 0, 0);
                }

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    direction = EDirection.DIRECTION_UP;
                    velocity += (float)frameTime * new float3(0, 1, 0);
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    direction = EDirection.DIRECTION_DOWN;
                    velocity += (float)frameTime * new float3(0, -1, 0);
                }

                if (direction != EDirection.DIRECTION_NULL)
                {
                    physics.ValueRW.velocity += move.ValueRO.speed * velocity;

                    if (direction == EDirection.DIRECTION_LEFT)
                        transform.ValueRW.rotation = new float3(-1, 0, 0);
                    else if (direction == EDirection.DIRECTION_RIGHT)
                        transform.ValueRW.rotation = new float3(1, 0, 0);
                    else if (direction == EDirection.DIRECTION_UP)
                        transform.ValueRW.rotation = new float3(0, 1, 0);
                    else if (direction == EDirection.DIRECTION_DOWN)
                        transform.ValueRW.rotation = new float3(0, -1, 0);

                    eventComponent.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Direction });
                }
            }
        }
    }
}