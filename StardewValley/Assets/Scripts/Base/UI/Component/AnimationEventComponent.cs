using System;
using UnityEngine;

namespace WATP.UI
{
    public class AnimationEventComponent : MonoBehaviour
    {
        private Action<string> animEvent;
        public Action<string> onAnimEvent
        {
            get { return animEvent; }
            set { animEvent = value; }
        }

        public void AnimEvent(string message)
        {
            animEvent?.Invoke(message);
        }

    }
}