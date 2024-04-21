using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// day update시 정보 변화를 처리한다.
    /// </summary>
    [UpdateBefore(typeof(HoedirtAddSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial class DayUpdateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            bool isUpdate = false;
            foreach (var tag in SystemAPI.Query<RefRO<PauseSystemsCompTag>>())
                return;

            foreach (var com in SystemAPI.Query<RefRO<MapUpdateOptionComponent>>())
                isUpdate = com.ValueRO.isDay;

            if (!isUpdate) return;

            foreach (var (transform, hoedirt, eventComponent, deleteComponent) in SystemAPI.Query<RefRO<TransformComponent>, RefRW<HoedirtDataComponent>, RefRW<EventComponent>, RefRW<DeleteComponent>>())
            {
                var hoedirtData = Root.State.GetHoedirt(Root.SceneLoader.TileMapManager.MapName, transform.ValueRO.position.x, transform.ValueRO.position.y);
                if (hoedirtData != null)
                {
                    hoedirt.ValueRW.watering = hoedirtData.watering;
                    eventComponent.ValueRO.events.Add(new EventBuffer() { value = (int)EventKind.Normal });
                }
                else
                {
                    deleteComponent.ValueRW.isDelate = true;
                }
            }


            foreach (var (transform, crops, eventComponent, deleteComponent) in SystemAPI.Query<RefRO<TransformComponent>, RefRW<CropsDataComponent>, RefRW<EventComponent>, RefRW<DeleteComponent>>())
            {
                var hoedirtData = Root.State.GetCrops(Root.SceneLoader.TileMapManager.MapName, transform.ValueRO.position.x, transform.ValueRO.position.y);
                if (hoedirtData != null)
                {
                    crops.ValueRW.day = hoedirtData.day;
                    eventComponent.ValueRO.events.Add(new EventBuffer() { value = (int)EventKind.Day });
                }
                else
                {
                    deleteComponent.ValueRW.isDelate = true;
                }
            }

            foreach (var com in SystemAPI.Query<RefRW<MapUpdateOptionComponent>>())
                com.ValueRW.isDay = false;
        }
    }
}

