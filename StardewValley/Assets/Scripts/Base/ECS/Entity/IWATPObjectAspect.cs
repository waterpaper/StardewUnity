using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// ������Ʈ(Map object)���� �����ϴ� �⺻ ���� aspect
    /// �ΰ��� MapObject���� ������ �ʿ����� ������ ����� �ʿ�� ����.
    /// ����, ����, ���������� �����Ѵ�.
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