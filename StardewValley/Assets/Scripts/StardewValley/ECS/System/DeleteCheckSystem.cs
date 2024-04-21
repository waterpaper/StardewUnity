using Unity.Burst;
using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// ���� ó���� �ʿ��� component�� �����͸� �����Ѵ�.
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    [UpdateBefore(typeof(EventSystem))]
    [UpdateAfter(typeof(SleepSystem))]
    public partial struct DeleteCheckSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DeleteComponent>();
            state.RequireForUpdate<NormalSystemsCompTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var tag in SystemAPI.Query<RefRO<PauseSystemsCompTag>>())
                return;
            float frameTime = SystemAPI.Time.DeltaTime;
            foreach (var delete in SystemAPI.Query<RefRW<DeleteComponent>>())
            {
                if (delete.ValueRO.isDelate) continue;
                if (delete.ValueRO.isEnable == false) continue;

                if (delete.ValueRO.isTimer == false)
                {
                    delete.ValueRW.isDelate = true;
                    continue;
                }

                delete.ValueRW.timer += (float)frameTime;
                if (delete.ValueRO.deleteTime > delete.ValueRO.timer) continue;
                delete.ValueRW.isDelate = true;
            }
        }
    }
}

