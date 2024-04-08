namespace WATP.View
{
    public class ViewController
    {
        public static EventListeners ServiceEvents = new EventListeners();
    }

    //积己等 view
    public class ViewCreateEvent : IGameEvent
    {
        private IView view;

        public IView View { get => view; }

        public ViewCreateEvent(IView view)
        {
            this.view = view;
        }
    }


    //力芭等 view
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