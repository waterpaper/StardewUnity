using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// ecs는 random시 job에서 이용가능하게 처리하려면 component를 추가한 entity가 하나 필요
    /// </summary>
    public struct RandomComponent : IComponentData
    {
        public Unity.Mathematics.Random Random;
    }
}