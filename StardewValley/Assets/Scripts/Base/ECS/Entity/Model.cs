using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// 오브젝트의 기본이 되는 클래스 입니다.<br/><br/>
    /// 기본적으로 오브젝트의 데이터 저장, 관리만 담당합니다.<br/><br/>
    /// 복잡한 로직은 <see cref="IService"/>로 작성해야 합니다.<br/>
    /// </summary>
    [System.Serializable]
    public abstract class Model : IModel
    {
        [SerializeField]
        protected int uid;


        public int UID { get => uid; }

        public abstract void OnInitialize();
        
        public abstract void OnDestroy();

        public override string ToString()
        {
            return $"Model::UID({UID})";
        }
    }
}
