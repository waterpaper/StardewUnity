using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// ���� ��� ó�� �ý���
    /// ������� ����� ��� ���� ����� �ݿ��Ͽ� component�� �����Ѵ�.
    /// </summary>
    [UpdateBefore(typeof(CellAddObjectSystem))]
    [UpdateAfter(typeof(FsmSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial struct PhysicsSystem : ISystem
    {
        float3 afterPos;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsComponent>();
            state.RequireForUpdate<NormalSystemsCompTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (tag, physics) in SystemAPI.Query<RefRO<PauseSystemsCompTag>, RefRW<PhysicsComponent>>())
            {
                physics.ValueRW.velocity = float3.zero;
                return;
            }

            afterPos = float3.zero;

            var job = new PhysicsJob();
            var handle = job.ScheduleParallel(state.Dependency);
            handle.Complete();
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public partial struct PhysicsJob : IJobEntity
    {
        float3 afterPos;
        // Execute() is called when the job runs.
        public void Execute(ref TransformComponent transform, ref PhysicsComponent physics)
        {
            if (physics.isEnable == false)
            {
                physics.velocity = float3.zero;
                return;
            }
            if (math.all(physics.velocity == float3.zero)) return;

            afterPos = transform.position + physics.velocity;
            afterPos = new float3(((int)(afterPos.x * 1000)) / 1000.0f, ((int)(afterPos.y * 1000)) / 1000.0f, 0);

            transform.position = afterPos;
            physics.velocity = float3.zero;
        }
    }
}
