using System;
using UnityEngine;

namespace WATP.UI
{
    /// <summary>
    /// mono에 추가하여 event를 처리하는 클래스 (animation event 관련)
    /// 이벤트를 등록하여 해당 이벤트 발생시 사용하는 식으로 사용한다.
    /// </summary>
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