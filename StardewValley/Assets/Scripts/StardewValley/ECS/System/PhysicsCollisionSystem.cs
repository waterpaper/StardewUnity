using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// 물리 충돌 처리 시스템
    /// colliderComponent를 가진 component 중 물리처리 이후 충돌하는 component의 결과를 초기화한다.
    /// </summary>
    [UpdateBefore(typeof(FsmSystem))]
    [UpdateAfter(typeof(PhysicsCellSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial struct PhysicsCollisionSystem : ISystem
    {
        float3 afterPos;
        bool isCollision;
        ColliderComponent collision;
        NativeList<PhysicsCollisionData> aspects;

        [BurstCompile(CompileSynchronously = true)]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsComponent>();
            state.RequireForUpdate<NormalSystemsCompTag>();
            aspects = new(1000, Allocator.Persistent);
        }

        [BurstCompile(CompileSynchronously = true)]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var tag in SystemAPI.Query <RefRO<PauseSystemsCompTag>>())
                return;

            afterPos = float3.zero;

            aspects.Clear();

            foreach (var aspect in SystemAPI.Query<PhysicsCollisionAspect>())
            {
                aspects.Add(new PhysicsCollisionData() { index = aspect.entity.Index, colliderComponent = aspect.colliderComponent.ValueRO, physicsComponent = aspect.physicsComponent.ValueRO, transformComponent = aspect.transformComponent.ValueRO });
            }

            var job = new PhysicsCollisionJob()
            {
                data = aspects.AsArray()
            };

            var handle = job.ScheduleParallel(state.Dependency);
            handle.Complete();
        }

        [BurstCompile(CompileSynchronously = true)]
        public void OnDestroy(ref SystemState state)
        {
            aspects.Clear();
            aspects.Dispose();
        }
    }


    public struct PhysicsCollisionData
    {
        [ReadOnly] public int index;
        [ReadOnly] public ColliderComponent colliderComponent;
        [ReadOnly] public PhysicsComponent physicsComponent;
        [ReadOnly] public TransformComponent transformComponent;
    }

    [BurstCompile(CompileSynchronously = true)]
    public partial struct PhysicsCollisionJob : IJobEntity
    {
        [ReadOnly] public NativeArray<PhysicsCollisionData> data;
        float3 afterPos;
        // Execute() is called when the job runs.
        public void Execute(PhysicsCollisionAspect aspect)
        {
            if (aspect.physicsComponent.ValueRO.isEnable == false) return;
            if (aspect.colliderComponent.ValueRO.isEnable == false) return;

            if (math.all(aspect.physicsComponent.ValueRO.velocity == float3.zero)) return;

            afterPos = aspect.transformComponent.ValueRO.position + aspect.physicsComponent.ValueRO.velocity;
            afterPos = new float3(((int)(afterPos.x * 1000)) / 1000.0f, ((int)(afterPos.y * 1000)) / 1000.0f, 0);

            foreach (var otherAspect in data)
            {
                if (otherAspect.physicsComponent.isEnable == false) continue;
                if (otherAspect.colliderComponent.isEnable == false) continue;
                if (aspect.entity.Index == otherAspect.index) continue;

                if (IsCollision(afterPos, aspect.colliderComponent.ValueRO.colliderType, aspect.colliderComponent.ValueRO.areaWidth, aspect.colliderComponent.ValueRO.areaHeight,
                     otherAspect.transformComponent.position, otherAspect.colliderComponent.colliderType, otherAspect.colliderComponent.areaWidth, otherAspect.colliderComponent.areaHeight))
                {
                    aspect.physicsComponent.ValueRW.velocity = float3.zero;
                    return;
                }
            }
        }

        public bool IsCollision(float3 movePos, int type, float width, float height, float3 otherPos, int otherType, float otherWidth, float otherHeight)
        {
            return type switch
            {
                (int)ColliderType.Circle => IsCollision_Circle(movePos, width, height, otherPos, otherType, otherWidth, otherHeight),
                (int)ColliderType.Square => IsCollision_Box(movePos, width, height, otherPos, otherType, otherWidth, otherHeight),
                _ => false,
            };
        }

        bool IsCollision_Circle(float3 movePos, float width, float height, float3 otherPos, int otherType, float otherWidth, float ohterHeight)
        {
            float range = width;
            float helfWidth = 0;
            float helfHeight = 0;

            if (otherType == (int)ColliderType.Circle)
            {
                if (range == -1 || math.distance(movePos, otherPos) - range < otherWidth)
                    return true;
            }
            else if (otherType == (int)ColliderType.Square)
            {
                helfWidth = otherWidth / 2;
                helfHeight = ohterHeight / 2;

                float circleDistanceX = movePos.x - otherPos.x;
                float circleDistanceY = movePos.y - otherPos.y;

                if (circleDistanceX > (helfWidth + range)) { return false; }
                if (circleDistanceY > (helfHeight + range)) { return false; }

                if (circleDistanceX <= (helfWidth)) { return true; }
                if (circleDistanceY <= (helfHeight)) { return true; }

                float cornerDistance = (circleDistanceX - helfWidth) * 2 +
                                        (circleDistanceY - helfHeight) * 2;

                return (cornerDistance <= (range * 2));
            }

            return false;
        }


        bool IsCollision_Box(float3 movePos, float width, float height, float3 otherPos, int otherType, float otherWidth, float ohterHeight)
        {
            float helfWidth = width / 2;
            float helfHeight = height / 2;

            if (otherType == (int)ColliderType.Circle)
            {
                float circleDistanceX = movePos.x - otherPos.x;
                float circleDistanceY = movePos.y - otherPos.y;

                if (circleDistanceX > (helfWidth + otherWidth)) { return false; }
                if (circleDistanceY > (helfHeight + ohterHeight)) { return false; }

                if (circleDistanceX <= (helfWidth)) { return true; }
                if (circleDistanceY <= (helfHeight)) { return true; }

                float cornerDistance = (circleDistanceX - helfWidth) * 2 +
                                     (circleDistanceY - helfHeight) * 2;

                if (cornerDistance <= (otherWidth * 2))
                    return true;
                else
                    return false;

            }
            else if (otherType == (int)ColliderType.Square)
            {
                float targetHelfWidth = otherWidth / 2;
                float targetHelfHeight = ohterHeight / 2;

                if (movePos.x - helfWidth < otherPos.x + targetHelfWidth
                    && movePos.x + helfWidth > otherPos.x - targetHelfWidth
                    && movePos.y + helfHeight > otherPos.y - targetHelfHeight
                    && movePos.y - helfHeight < otherPos.y + targetHelfHeight)
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
