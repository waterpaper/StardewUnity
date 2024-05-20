using System;
using UnityEngine;

namespace WATP.UI
{
    /// <summary>
    /// mono�� �߰��Ͽ� event�� ó���ϴ� Ŭ���� (animation event ����)
    /// �̺�Ʈ�� ����Ͽ� �ش� �̺�Ʈ �߻��� ����ϴ� ������ ����Ѵ�.
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