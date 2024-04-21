using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace WATP.ECS
{
    public readonly partial struct PhysicsCollisionAspect : IAspect
    {
        public readonly Entity entity;
        [NativeDisableUnsafePtrRestriction]
        public readonly RefRW<TransformComponent> transformComponent;
        [NativeDisableUnsafePtrRestriction]
        public readonly RefRW<ColliderComponent> colliderComponent;
        [NativeDisableUnsafePtrRestriction]
        public readonly RefRW<PhysicsComponent> physicsComponent;
    }
}