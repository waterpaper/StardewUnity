using Cysharp.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// 워프 위치 처리 
    /// </summary>
    [UpdateBefore(typeof(SleepSystem))]
    [UpdateAfter(typeof(CellAddObjectSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial class WarpSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<NormalSystemsCompTag>();
        }
        protected override void OnUpdate()
        {
            Entities
               .WithAll<TransformComponent, SleepComponent>()
               .ForEach(
                   (ref TransformComponent transform, ref PlayerComponent player, ref WarpComponent warp) =>
                   {
                       if (warp.isEnable == false || player.value == false) { return; }

                       if (IsCell_Warp(transform.position) == false) return;

                       var portalInfo = Root.SceneLoader.TileMapManager.GetPortalCell((int)(transform.position.x), (int)(transform.position.y));
                       if (portalInfo == null) return;

                       Root.SceneLoader.TileMapManager.MapSetting(portalInfo.portalName).Forget();
                       transform.position = new float3(portalInfo.portalNextPosX, portalInfo.portalNextPosY, 0);
                   }
               )
               .WithoutBurst().Run();
        }

        public bool IsCell_Warp(float3 pos)
        {
            var cell = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x), (int)(pos.y));

            if (cell == null)
                return false;

            return cell.GetCellType() == Map.CellKind.Portal;
        }
    }
}

