namespace WATP.View
{
    public class ViewController
    {
        public static EventListeners ServiceEvents = new EventListeners();
    }

    //������ view
    public class ViewCreateEvent : IGameEvent
    {
        private IView view;

        public IView View { get => view; }

        public ViewCreateEvent(IView view)
        {
            this.view = view;
        }
    }


    //���ŵ� view
    public class ViewDeleteEvent : IGameEvent
    {
        private IView view;

        public IView View { get => view; }

        public ViewDeleteEvent(IView view)
        {
            this.view = view;
        }
    }
}