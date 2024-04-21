using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// Hoedirt의 업데이트 이벤트를 처리한다.
    /// </summary>
    [UpdateBefore(typeof(CellTargetSystem))]
    [UpdateAfter(typeof(DayUpdateSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial struct HoedirtAddSystem : ISystem
    {
        NativeList<HoedirtAddData> aspects;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<HoedirtDataComponent>();
            aspects = new(1000, Allocator.Persistent);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            bool isUpdate = false;
            foreach (var tag in SystemAPI.Query<RefRO<PauseSystemsCompTag>>())
                return;

            foreach (var com in SystemAPI.Query<RefRO<MapUpdateOptionComponent>>())
                isUpdate = com.ValueRO.isHoedirt;

            if (!isUpdate) return;

            aspects.Clear();

            foreach (var (transform, hoedirt) in SystemAPI.Query<RefRO<TransformComponent>, RefRO<HoedirtDataComponent>>())
            {
                aspects.Add(new HoedirtAddData() { transformComponent = transform.ValueRO, hoedirtDataComnponent = hoedirt.ValueRO});
            }

            var job = new HoedirtAddJob()
            {
                frameTime = SystemAPI.Time.DeltaTime,
                otherDatas = aspects.AsArray()
            };
            var handle = job.ScheduleParallel(state.Dependency);
            handle.Complete();

            foreach (var (hoedirt, eventComponent) in SystemAPI.Query<RefRO<HoedirtDataComponent>, RefRO<EventComponent>>())
            {
                eventComponent.ValueRO.events.Add(new EventBuffer { value = hoedirt.ValueRO.watering ? (int)EventKind.Watering : (int)EventKind.Normal });
            }

            foreach (var com in SystemAPI.Query<RefRW<MapUpdateOptionComponent>>())
                com.ValueRW.isHoedirt = false;
        }

        [BurstCompile(CompileSynchronously = true)]
        public void OnDestroy(ref SystemState state)
        {
            aspects.Clear();
            aspects.Dispose();
        }
    }

    public struct HoedirtAddData
    {
        [ReadOnly] public TransformComponent transformComponent;
        [ReadOnly] public HoedirtDataComponent hoedirtDataComnponent;
    }

    [BurstCompile(CompileSynchronously = true)]
    public partial struct HoedirtAddJob : IJobEntity
    {
        [ReadOnly] public NativeArray<HoedirtAddData> otherDatas;
        public float frameTime;
        // Execute() is called when the job runs.
        public void Execute(Entity entity, ref TransformComponent transform, ref HoedirtDataComponent hoedirtData)
        {
            foreach (var otherData in otherDatas)
            {
                if (transform.position.x - 1 == otherData.transformComponent.position.x && transform.position.y == otherData.transformComponent.position.y)
                {
                    hoedirtData.left = true;
                }
                else if (transform.position.x + 1 == otherData.transformComponent.position.x && transform.position.y == otherData.transformComponent.position.y)
                {
                    hoedirtData.right = true;
                }
                else if (transform.position.y - 1 == otherData.transformComponent.position.y && transform.position.x == otherData.transformComponent.position.x)
                {
                    hoedirtData.down = true;
                }
                else if (transform.position.y + 1 == otherData.transformComponent.position.y && transform.position.x == otherData.transformComponent.position.x)
                {
                    hoedirtData.up = true;
                }
            }
        }
    }

}

