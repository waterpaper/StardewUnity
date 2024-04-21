using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using WATP.Map;

namespace WATP.ECS
{
    /// <summary>
    /// component의 길 찾기 로직을 수행한다.
    /// tilemap을 참조하며 길을 찾을 수 없으면 path에 추가되지 않는다.
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    [UpdateBefore(typeof(MoveSystem))]
    [UpdateAfter(typeof(HoedirtAddSystem))]
    public partial class CellTargetSystem : SystemBase
    {
        List<Cell> cells = new();

        protected override void OnCreate()
        {
            RequireForUpdate<NormalSystemsCompTag>();
        }
        protected override void OnUpdate()
        {
            if (Root.State.logicState.Value != LogicState.Normal) return;

            float frameTime = SystemAPI.Time.DeltaTime;
            var random = SystemAPI.GetSingletonRW<RandomComponent>();

            Entities
                .WithAll<MoveComponent, CellTargetComponent, TransformComponent>()
                .ForEach(
                    (ref MoveComponent move, ref CellTargetComponent cellTarget, ref TransformComponent transform, ref DeleteComponent deleteComponent) =>
                    {
                        if (cellTarget.isEnable == false) return;
                        if (deleteComponent.isDelate == true) return;
                        cells.Clear();

                        cellTarget.refreshTimer += (float)frameTime;
                        if (cellTarget.refreshTimer > cellTarget.refreshTime)
                        {
                            cellTarget.refreshTimer -= cellTarget.refreshTime;
                            cellTarget.refreshTime = random.ValueRW.Random.NextFloat(5, 20);
                            cellTarget.cellPaths.Clear();
                            cellTarget.finish = false;

                            var mapSize = Root.SceneLoader.TileMapManager.TileSize;

                            float xPos = 0;
                            float yPos = 0;

                            xPos = random.ValueRW.Random.NextFloat(0, mapSize.x);
                            yPos = random.ValueRW.Random.NextFloat(0, mapSize.y);
                            if (Root.SceneLoader.TileMapManager.GetCell((int)xPos, (int)yPos).Block)
                            {
                                cellTarget.refreshTime = 3;
                                return;
                            }

                            cells = Root.SceneLoader.TileMapManager.GetPath(new float2(transform.position.x, transform.position.y),
                                new float2(xPos, yPos));

                            foreach (var cell in cells)
                                cellTarget.cellPaths.Add(new CellPath() { value = new float2(cell.Position.x, cell.Position.y) });
                        }

                        if (cellTarget.finish)
                            return;

                        if (cellTarget.cellPaths.Length == 0)
                        {
                            cellTarget.finish = true;
                            cellTarget.refreshTime = 3;
                            return;
                        }

                        if (math.distance(transform.position, new float3(cellTarget.cellPaths[0].value.x, cellTarget.cellPaths[0].value.y, 0)) < 0.001f)
                        {
                            cellTarget.cellPaths.RemoveAt(0);
                            if (cellTarget.cellPaths.Length == 0)
                            {
                                cellTarget.finish = true;
                                move.targetPos = float3.zero;
                                return;
                            }
                        }

                        if (Root.SceneLoader.TileMapManager.GetCell((int)cellTarget.cellPaths[0].value.x, (int)cellTarget.cellPaths[0].value.y).ObjectBlock)
                            cellTarget.refreshTime -= 0.1f;

                        move.targetPos = new float3(cellTarget.cellPaths[0].value.x, cellTarget.cellPaths[0].value.y, 0);
                    }
                )
                .WithoutBurst().Run();
        }
    }
}

