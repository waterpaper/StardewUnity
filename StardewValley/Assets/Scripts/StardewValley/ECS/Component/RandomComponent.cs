using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// ecs�� random�� job���� �̿밡���ϰ� ó���Ϸ��� component�� �߰��� entity�� �ϳ� �ʿ�
    /// </summary>
    public struct RandomComponent : IComponentData
    {
        public Unity.Mathematics.Random Random;
    }
}