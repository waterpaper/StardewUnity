using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// 잠 자리 위치 처리 
    /// </summary>
    [UpdateBefore(typeof(DeleteCheckSystem))]
    [UpdateAfter(typeof(WarpSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial class SleepSystem : SystemBase
    {
        bool isSleep;

        protected override void OnCreate()
        {
            RequireForUpdate<NormalSystemsCompTag>();
        }

        protected override void OnUpdate()
        {
            isSleep = false;

            Entities
               .WithAll<TransformComponent, SleepComponent>()
               .ForEach(
                   (ref TransformComponent transform, ref SleepComponent sleep) =>
                   {
                       if (IsSleepArea(transform.position))
                       {
                           if (sleep.isArea) return;
                           sleep.isArea = true;
                           isSleep = true;
                       }
                       else
                       {
                           if (sleep.isArea)
                               sleep.isArea = false;
                       }
                   }
               )
               .WithoutBurst().Run();

            //ui component setting
            if (isSleep)
            {
                Entities
               .WithAll<UISleepCheckComponent>()
               .WithEntityQueryOptions(EntityQueryOptions.IgnoreComponentEnabledState)
               .ForEach(
                   (Entity entity, ref UISleepCheckComponent component) =>
                   {
                       EntityManager.SetComponentEnabled<UISleepCheckComponent>(entity, true);
                   }
               )
               .WithoutBurst().Run();
            }
        }

        public bool IsSleepArea(float3 pos)
        {
            if (Root.SceneLoader.TileMapManager.MapName != "House") return false;

            return math.distance(pos, new float3(9, 4, 0)) < 1f;
        }
    }
}

