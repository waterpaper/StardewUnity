using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// input �� ��ȣ�ۿ� Ű �Է½� ó���Ǿ� �ϴ� entity�� ���� component
    /// </summary>
    public struct InteractionInputComponent : IComponentData
    {
        /// <summary> ���ɿ��� </summary>
        public bool isEnable;
    }
}