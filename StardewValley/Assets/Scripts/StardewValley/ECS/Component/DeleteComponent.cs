using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// map object �� system���� ���� ó���� �ʿ��� entity�� ���� component
    /// </summary>
    public struct DeleteComponent : IComponentData
    {
        public bool isEnable;
        public bool isDelate;
        public bool isTimer;
        public float timer;
        public float deleteTime;
    }

}