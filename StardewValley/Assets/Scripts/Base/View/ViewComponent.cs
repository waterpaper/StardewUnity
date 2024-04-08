using UnityEngine;

namespace WATP.View
{
    /// <summary>
    /// 컴포넌트 
    /// <see cref="OnDispose"/>로 반드시 해제해야 합니다.    
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
