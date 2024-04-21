using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// 프로젝트(Map object)에서 관리하는 기본 단위 aspect
    /// 인게임 MapObject에서 관리가 필요하지 않으면 상속할 필요는 없다.
    /// 생성, 제거, 재참조등을 관리한다.
    /// </summary>
    public interface IWATPObjectAspect : IAspect
    {
        public Entity Entity
        {
            get;
        }

        public int Index
        {
            get;
        }

        public float3 Position
        {
            get;
        }

        public bool DeleteReservation
        {
            get;
            set;
        }

        public void OnInitialize();

        public void OnDestroy();

        public void OnRef();
    }
}