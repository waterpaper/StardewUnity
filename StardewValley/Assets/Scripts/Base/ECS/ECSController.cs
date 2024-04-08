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
        private IEntity entity;

        public IEntity Entity { get => entity; }

        public EventCreateEntity(IEntity entity)
        {
            this.entity = entity;
        }
    }


    //���ŵ� ��ƼƼ
    public class EventDeleteEntity : IGameEvent
    {
        private IEntity entity;

        public IEntity Entity { get => entity; }

        public EventDeleteEntity(IEntity entity)
        {
            this.entity = entity;
        }
    }

    //���� ��ƾ ��û
    public class EventCreateRoutine : IGameEvent
    {
        public EventCreateRoutine()
        {
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