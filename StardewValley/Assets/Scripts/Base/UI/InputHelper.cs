using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WATP.UI
{
    public class TouchBeginEvent : IGameEvent
    {
        public Vector3 mousePosition;
    }
    public class TouchMoveEvent : IGameEvent
    {
        public Vector3 mousePosition;
    }
    public class TouchEndEvent : IGameEvent
    {
        public Vector3 mousePosition;
    }
    public class BackButtonEvent : IGameEvent
    {
    }

    public class InputHelper
    {
        private EventSystem eventSystem;
        private Vector3 mousePosition;
        private bool isBlock = false;

        public void Initialize()
        {
            var obj = GameObject.Find("EventSystem");
            if(obj == null)
            {
                obj = new GameObject("EventSystem");
                obj.AddComponent<EventSystem>();
                obj.AddComponent<StandaloneInputModule>();
                obj.AddComponent<DontDestroyLoadObject>();
                eventSystem = obj.GetComponent<EventSystem>();
            }
            else
            {
                eventSystem = obj.GetComponent<EventSystem>();
            }
        }

        public void Destroy()
        {
            eventSystem = null;
        }

        public void Update()
        {
            TouchEvent();
            BackButtonEvent();
        }

        private void TouchEvent()
        {
            if (isBlock) return;

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WebGLPlayer)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    mousePosition = Input.mousePosition;
                    InputController.ServiceEvents.Emit(new TouchBeginEvent { mousePosition = mousePosition });
                    return;
                }

                if (Input.GetMouseButton(0))
                {
                    if (mousePosition != Input.mousePosition)
                    {
                        InputController.ServiceEvents.Emit(new TouchMoveEvent { mousePosition = Input.mousePosition });
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    InputController.ServiceEvents.Emit(new TouchEndEvent { mousePosition = Input.mousePosition });
                }

                mousePosition = Input.mousePosition;
            }
            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    mousePosition = touch.position;

                    if (touch.phase == TouchPhase.Began)
                    {
                        InputController.ServiceEvents.Emit(new TouchBeginEvent { mousePosition = mousePosition });
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        InputController.ServiceEvents.Emit(new TouchMoveEvent { mousePosition = mousePosition });
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        InputController.ServiceEvents.Emit(new TouchEndEvent { mousePosition = mousePosition });
                    }
                }
            }
        }

        private void BackButtonEvent()
        {
            if (Application.platform != RuntimePlatform.Android) return;
            if (Input.GetKeyDown(KeyCode.Escape) == false) return;

            InputController.ServiceEvents.Emit(new BackButtonEvent());
        }
    }
}