using Unity.Entities;

namespace WATP.ECS
{
    public class ECSController
    {
        public static EventListeners ServiceEvents = new EventListeners();
    }

    //생성 요청
    public class EventAddEntity : IGameEvent
    {
        private IEntityBuilder entityBuilder;

        public IEntityBuilder EntityBuilder { get => entityBuilder; }

        public EventAddEntity(IEntityBuilder builder)
        {
            this.entityBuilder = builder;
        }
    }


    //생성된 엔티티
    public class EventCreateEntity : IGameEvent
    {
        private IWATPObjectAspect entity;

        public IWATPObjectAspect Entity { get => entity; }

        public EventCreateEntity(IWATPObjectAspect entity)
        {
            this.entity = entity;
        }
    }


    //제거된 엔티티
    public class EventDeleteEntity : IGameEvent
    {
        private IWATPObjectAspect entity;

        public IWATPObjectAspect Entity { get => entity; }

        public EventDeleteEntity(IWATPObjectAspect entity)
        {
            this.entity = entity;
        }
    }


    //엔티티 업데이트
    public class EventRefUpdate: IGameEvent
    {
        private IWATPObjectAspect entity;

        public IWATPObjectAspect Entity { get => entity; }

        public EventRefUpdate(IWATPObjectAspect entity)
        {
            this.entity = entity;
        }
    }

    //제거 루틴 요청(추가 가능)
    public class EventDeleteRoutine : IGameEvent
    {
        public bool isAll = false;
        public bool isRemove = false;
        public float posX;
        public float posY;
        public EventDeleteRoutine()
        {
        }
    }
}