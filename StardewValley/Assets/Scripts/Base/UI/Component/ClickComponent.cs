using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WATP.UI
{
    /// <summary>
    /// mono에 추가하여 event를 처리하는 클래스 (click 관련)
    /// 이벤트를 등록하여 해당 이벤트 발생시 사용하는 식으로 사용한다.
    /// </summary>
    public class ClickComponent : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private bool isPointerDown = false;
        private Action<PointerEventData> clickAcion;
        private Action<PointerEventData> downAcion;
        private Action<PointerEventData> upAcion;
        private Action<PointerEventData> moveAcion;
        private Action<PointerEventData> enterAcion;
        private Action<PointerEventData> exitAcion;

        public Action<PointerEventData> onClick
        {
            get { return clickAcion; }
            set { clickAcion = value; }
        }
        public Action<PointerEventData> onDown
        {
            get { return downAcion; }
            set { downAcion = value; }
        }
        public Action<PointerEventData> onUp
        {
            get { return upAcion; }
            set { upAcion = value; }
        }
        public Action<PointerEventData> onMove
        {
            get { return moveAcion; }
            set { moveAcion = value; }
        }
        public Action<PointerEventData> onEnter
        {
            get { return enterAcion; }
            set { enterAcion = value; }
        }
        public Action<PointerEventData> onExit
        {
            get { return exitAcion; }
            set { exitAcion = value; }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            clickAcion?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            downAcion?.Invoke(eventData);
            isPointerDown = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            enterAcion?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            exitAcion?.Invoke(eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (isPointerDown)
                moveAcion?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            upAcion?.Invoke(eventData);
            isPointerDown = false;
        }
    }
}
