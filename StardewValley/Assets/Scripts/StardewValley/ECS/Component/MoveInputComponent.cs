using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// input �� ������ �Է½� ó���Ǿ� �ϴ� entity�� ���� component
    /// </summary>
    public struct MoveInputComponent : IComponentData
    {
        /// <summary> ���ɿ��� </summary>
        public bool isEnable;
    }
}