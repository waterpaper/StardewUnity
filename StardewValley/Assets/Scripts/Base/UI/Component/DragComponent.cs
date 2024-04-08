using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WATP.UI
{
    public class DragComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Action<PointerEventData> beginAcion;
        private Action<PointerEventData> dragAcion;
        private Action<PointerEventData> endAcion;

        public Action<PointerEventData> onBegin
        {
            get { return beginAcion; }
            set { beginAcion = value; }
        }
        public Action<PointerEventData> onDrag
        {
            get { return dragAcion; }
            set { dragAcion = value; }
        }
        public Action<PointerEventData> onEnd
        {
            get { return endAcion; }
            set { endAcion = value; }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            beginAcion?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragAcion?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            endAcion?.Invoke(eventData);
        }
    }
}
