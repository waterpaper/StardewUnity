using UnityEngine;

namespace WATP.View
{
    /// <summary>
    /// ������Ʈ 
    /// <see cref="OnDispose"/>�� �ݵ�� �����ؾ� �մϴ�.    
    /// /// </summary>
    public abstract class ViewComponent
    {
        public bool IsEnable { get; set; } = true;

        public IView view { get; private set; }

        public ViewComponent(IView e)
        {
            view = e;
        }

        public virtual void OnInitialize(Transform trs)
        {
        }

        public virtual void OnDispose()
        {
            IsEnable = false;
            view = null;
        }
    }
}
