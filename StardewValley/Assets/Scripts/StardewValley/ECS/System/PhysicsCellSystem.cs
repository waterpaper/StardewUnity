using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// 물리 충돌 처리 시스템(cell 기반)
    /// colliderComponent를 가진 component 중 cell과 충돌하는 component의 결과를 초기화한다.
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    [UpdateBefore(typeof(PhysicsCollisionSystem))]
    [UpdateAfter(typeof(InputUsingSystem))]
    public partial class PhysicsCellSystem : SystemBase
    {
        float3 afterPos;
        protected override void OnCreate()
        {
            RequireForUpdate<NormalSystemsCompTag>();
        }
        protected override void OnUpdate()
        {
            if (Root.State.logicState.Value != LogicState.Normal) return;
            float frameTime = SystemAPI.Time.DeltaTime;

            Entities
                .WithAll<PhysicsCollisionAspect>()
                .ForEach(
                    (PhysicsCollisionAspect aspect) =>
                    {
                        if (aspect.physicsComponent.ValueRO.isEnable == false) return;
                        if (aspect.colliderComponent.ValueRO.isEnable == false) return;

                        if (math.all(aspect.physicsComponent.ValueRO.velocity == float3.zero)) return;

                        afterPos = aspect.transformComponent.ValueRO.position + aspect.physicsComponent.ValueRO.velocity;
                        afterPos = new float3(((int)(afterPos.x * 1000)) / 1000.0f, ((int)(afterPos.y * 1000)) / 1000.0f, 0);

                        // 맵 체크
                        if ((ColliderType)aspect.colliderComponent.ValueRO.colliderType == ColliderType.Square ? !IsMove_ColliderRect(afterPos, aspect.colliderComponent.ValueRO.areaWidth, aspect.colliderComponent.ValueRO.areaHeight)
                            : !IsMove_ColliderCircle(afterPos, aspect.colliderComponent.ValueRO.areaWidth))
                        {
                            aspect.physicsComponent.ValueRW.velocity = float3.zero;
                        }
                    }
                )
                .WithoutBurst().Run();
        }

        public bool IsMove_ColliderCircle(float3 pos, float range)
        {
            var cell1 = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x + range - 0.01f), (int)pos.y);
            var cell2 = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x - range), (int)pos.y);
            var cell3 = Root.SceneLoader.TileMapManager.GetCell((int)pos.x, (int)(pos.y + range - 0.01f));
            var cell4 = Root.SceneLoader.TileMapManager.GetCell((int)pos.x, (int)(pos.y - range));

            if (cell1 == null || cell1.Block || cell2 == null || cell2.Block || cell3 == null || cell3.Block || cell4 == null || cell4.Block)
                return false;

            return true;
        }

        public bool IsMove_ColliderRect(float3 pos, float width, float height)
        {
            float halfWidth = width / 2;
            float halfHeight = height / 2;

            var cell1 = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x + halfWidth - 0.01f), (int)(pos.y + halfHeight - 0.01f));
            var cell2 = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x + halfWidth - 0.01f), (int)(pos.y - halfHeight + 0.01f));
            var cell3 = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x - halfWidth + 0.01f), (int)(pos.y + halfHeight - 0.01f));
            var cell4 = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x - halfWidth + 0.01f), (int)(pos.y - halfHeight + 0.01f));

            if (cell1 == null || cell1.Block || cell2 == null || cell2.Block || cell3 == null || cell3.Block || cell4 == null || cell4.Block)
                return false;

            return true;
        }

    }
}