using System;

namespace WATP.UI
{
    public class UIController
    {
        public static EventListeners ServiceEvents = new EventListeners();
    }

    public class InputController
    {
        public static EventListeners ServiceEvents = new EventListeners();
    }

    public class UIException : IGameEvent
    {
        public Exception exception;
    }
}