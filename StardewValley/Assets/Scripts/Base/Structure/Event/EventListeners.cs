using System;
using System.Collections.Generic;

namespace WATP
{
    /// <summary>
    /// Event driven을 이용하여 모듈간 통신 진행
    /// </summary>
    public class EventListeners
    {
        private readonly Dictionary<Type, List<Action<IGameEvent>>> listeners = new Dictionary<Type, List<Action<IGameEvent>>>();
        private readonly Dictionary<object, Action<IGameEvent>> actions = new Dictionary<object, Action<IGameEvent>>();

        public void On<T>(Action<T> action) where T : class, IGameEvent
        {
            var t = typeof(T);
            List<Action<IGameEvent>> events;
            if (!listeners.TryGetValue(t, out events))
            {
                events = new List<Action<IGameEvent>>();
                listeners.Add(t, events);
            }

            Action<IGameEvent> n = (IGameEvent e) => { action(e as T); };
            actions.Add(action, n);
            events.Add(n);
        }

        public void Off<T>(Action<T> action) where T : class, IGameEvent
        {
            var t = typeof(T);
            List<Action<IGameEvent>> events;
            if (!listeners.TryGetValue(t, out events))
            {
                throw new Exception();
            }

            Action<IGameEvent> n;
            if (!actions.TryGetValue(action, out n))
            {
                throw new Exception();
            }

            actions.Remove(action);

            if (!events.Remove(n))
            {
                throw new Exception();
            }
        }

        public void Emit<T>(T e) where T : class, IGameEvent
        {
            var t = typeof(T);
            List<Action<IGameEvent>> events;
            if (listeners.TryGetValue(t, out events))
            {
                foreach (Action<IGameEvent> action in events)
                {
                    action(e);
                }
            }
        }
    }
}