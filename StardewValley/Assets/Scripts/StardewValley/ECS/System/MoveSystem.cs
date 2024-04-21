using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{

    /// <summary>
    /// 움직임 컴포넌트
    /// 목표 위치로의 이동을 PhysicsComponent에 추가한다.
    /// </summary>
    [UpdateBefore(typeof(InputMoveSystem))]
    [UpdateAfter(typeof(CellTargetSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial struct MoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MoveComponent>();
            state.RequireForUpdate<NormalSystemsCompTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var tag in SystemAPI.Query<RefRO<PauseSystemsCompTag>>())
                return;
            var job = new MoveJob()
            {
                frameTime = SystemAPI.Time.DeltaTime
            };
            var handle = job.ScheduleParallel(state.Dependency);
            handle.Complete();

            foreach (var (move, eventComponent) in SystemAPI.Query<RefRO<MoveComponent>, RefRW<EventComponent>>())
            {
                if(move.ValueRO.isMoveTurn)
                    eventComponent.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Direction });
            }
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public partial struct MoveJob : IJobEntity
    {
        public float frameTime;
        // Execute() is called when the job runs.
        public void Execute(Entity entity, ref TransformComponent transform, ref MoveComponent move, ref PhysicsComponent physics)
        {
            move.isMoveTurn = false;
            if (move.isEnable == false || math.all(move.targetPos == float3.zero)) return;

            if (move.targetPos.y > transform.position.y)
            {
                transform.rotation = new float3(0, 1, 0);
            }
            else if (move.targetPos.y < transform.position.y)
            {
                transform.rotation = new float3(0, -1, 0);
            }
            else if (move.targetPos.x > transform.position.x)
            {
                transform.rotation = new float3(1, 0, 0);
            }
            else if (move.targetPos.x < transform.position.x)
            {
                transform.rotation = new float3(-1, 0, 0);
            }
            else
                return;

            var power = move.speed * frameTime;
            var dis = math.distance(move.targetPos, transform.position);
            if (power > dis)
                power = dis;

            physics.velocity += transform.rotation * power;
            move.isMoveTurn = true;
        }
    }
}

