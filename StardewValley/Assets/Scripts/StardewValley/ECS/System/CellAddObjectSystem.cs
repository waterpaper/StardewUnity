using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using WATP.Map;

namespace WATP.ECS
{
    /// <summary>
    /// cell�� ���� �߰��� ó���Ѵ�.
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    [UpdateBefore(typeof(WarpSystem))]
    [UpdateAfter(typeof(PhysicsSystem))]
    public partial class CellAddObjectSystem : SystemBase
    {
        public List<Cell> cells = new();
        public List<CellAddObjectComponent> cellAddObjectComponents = new();

        protected override void OnCreate()
        {
            RequireForUpdate<NormalSystemsCompTag>();
        }
        protected override void OnUpdate()
        {
            if (Root.State.logicState.Value != LogicState.Normal) return;
            float frameTime = SystemAPI.Time.DeltaTime;

            Cell cellTemp = null;
            foreach (var cell in cells)
                cell.ObjectBlock = false;

            cells.Clear();
            Entities
               .WithAll<CellAddObjectComponent, TransformComponent>()
               .ForEach(
                   (ref CellAddObjectComponent cellAddObject, ref TransformComponent transform) =>
                   {
                       cellTemp = Root.SceneLoader.TileMapManager.GetCell((int)transform.position.x, (int)transform.position.y);
                       cellTemp.ObjectBlock = true;

                       cells.Add(cellTemp);
                   }
               )
               .WithoutBurst().Run();
        }
    }
}

