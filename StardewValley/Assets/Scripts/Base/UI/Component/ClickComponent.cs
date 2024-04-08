using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WATP.UI
{
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
