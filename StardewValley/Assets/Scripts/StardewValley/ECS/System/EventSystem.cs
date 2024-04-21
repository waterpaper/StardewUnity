using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// 현재까지 추가된 내부 event를 발생시킨다.
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    [UpdateAfter(typeof(DeleteCheckSystem))]
    public partial class EventSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<NormalSystemsCompTag>();
        }
        protected override void OnUpdate()
        {
            Entities
                .WithAll<EventActionComponent, EventComponent>()
                .ForEach(
                    (EventActionComponent eventActionComponent, ref EventComponent eventComponent) =>
                    {
                        if(eventComponent.events.Length != 0)
                            foreach(var eventData in eventComponent.events)
                            {
                                eventActionComponent.OnEvent?.Invoke(eventData.value);
                            }

                        eventComponent.events.Clear();
                    }
                )
                .WithoutBurst().Run();
        }
    }
}

