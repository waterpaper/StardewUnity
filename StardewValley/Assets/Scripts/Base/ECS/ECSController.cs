using Unity.Entities;

namespace WATP.ECS
{
    public class ECSController
    {
        public static EventListeners ServiceEvents = new EventListeners();
    }

    //���� ��û
    public class EventAddEntity : IGameEvent
    {
        private IEntityBuilder entityBuilder;

        public IEntityBuilder EntityBuilder { get => entityBuilder; }

        public EventAddEntity(IEntityBuilder builder)
        {
            this.entityBuilder = builder;
        }
    }


    //������ ��ƼƼ
    public class EventCreateEntity : IGameEvent
    {
        private IWATPObjectAspect entity;

        public IWATPObjectAspect Entity { get => entity; }

        public EventCreateEntity(IWATPObjectAspect entity)
        {
            this.entity = entity;
        }
    }


    //���ŵ� ��ƼƼ
    public class EventDeleteEntity : IGameEvent
    {
        private IWATPObjectAspect entity;

        public IWATPObjectAspect Entity { get => entity; }

        public EventDeleteEntity(IWATPObjectAspect entity)
        {
            this.entity = entity;
        }
    }


    //��ƼƼ ������Ʈ
    public class EventRefUpdate: IGameEvent
    {
        private IWATPObjectAspect entity;

        public IWATPObjectAspect Entity { get => entity; }

        public EventRefUpdate(IWATPObjectAspect entity)
        {
            this.entity = entity;
        }
    }

    //���� ��ƾ ��û(�߰� ����)
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