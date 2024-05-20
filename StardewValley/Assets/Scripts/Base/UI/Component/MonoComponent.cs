using System;
using UnityEngine;

namespace WATP.UI
{
    /// <summary>
    /// mono에 추가하여 event를 처리하는 클래스 (mono 관련)
    /// 이벤트를 등록하여 해당 이벤트 발생시 사용하는 식으로 사용한다.
    /// </summary>
    public class MonoComponent : MonoBehaviour
    {
        private Action awakeAction;
        private Action startAction;
        private Action updateAction;
        private Action fixedUpdateAction;
        private Action enableAction;
        private Action disableAction;
        private Action destroyAction;

        public Action onAwake
        {
            get { return awakeAction; }
            set { awakeAction = value; }
        }
        public Action onStart
        {
            get { return startAction; }
            set { startAction = value; }
        }
        public Action onUpdate
        {
            get { return updateAction; }
            set { updateAction = value; }
        }
        public Action onFixedUpdate
        {
            get { return fixedUpdateAction; }
            set { fixedUpdateAction = value; }
        }
        public Action onEnable
        {
            get { return enableAction; }
            set { enableAction = value; }
        }
        public Action onDisable
        {
            get { return disableAction; }
            set { disableAction = value; }
        }
        public Action onDestroy
        {
            get { return destroyAction; }
            set { destroyAction = value; }
        }

        private void Awake()
        {
            awakeAction?.Invoke();
        }

        private void Start()
        {
            startAction?.Invoke();
        }

        private void Update()
        {
            updateAction?.Invoke();
        }

        private void FixedUpdate()
        {
            fixedUpdateAction?.Invoke();
        }

        private void OnEnable()
        {
            enableAction?.Invoke();
        }

        private void OnDisable()
        {
            disableAction?.Invoke();
        }

        private void OnDestroy()
        {
            destroyAction?.Invoke();
        }

    }
}
