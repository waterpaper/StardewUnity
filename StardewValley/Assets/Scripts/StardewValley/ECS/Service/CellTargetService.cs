using System.Collections.Generic;
using UnityEngine;
using WATP.Map;

namespace WATP.ECS
{
    /// <summary>
    /// 길 찾기 목표 cell을 설정하는 서비스 로직
    /// 목표 cell까지의 길찾기 및 이동을 관리한다.
    /// </summary>
    public class CellTargetService : IService
    {
        public List<Cell> cells = new(); 
        public List<ICellTargetComponent> cellTargetComponents = new();

        public void Add(IEntity entity)
        {
            if (entity is not ICellTargetComponent) return;

            cellTargetComponents.Add(entity as ICellTargetComponent);
        }

        public void Remove(IEntity entity)
        {
            if (entity is not ICellTargetComponent) return;

            cellTargetComponents.Remove(entity as ICellTargetComponent);
        }

        public void Clear()
        {
            cellTargetComponents.Clear();
        }

        public void Update(double frameTime)
        {
            foreach (var cellTargetEntity in cellTargetComponents)
            {
                if (cellTargetEntity.CellTargetComponent.isEnable == false) continue;
                cells.Clear();

                cellTargetEntity.CellTargetComponent.refreshTimer += (float)frameTime;
                if (cellTargetEntity.CellTargetComponent.refreshTimer > cellTargetEntity.CellTargetComponent.refreshTime)
                {
                    cellTargetEntity.CellTargetComponent.refreshTimer -= cellTargetEntity.CellTargetComponent.refreshTime;
                    cellTargetEntity.CellTargetComponent.refreshTime = Random.Range(5, 20);
                    cellTargetEntity.CellTargetComponent.cellPath.Clear();
                    cellTargetEntity.CellTargetComponent.finish = false;

                    var mapSize = Root.SceneLoader.TileMapManager.TileSize;

                    float xPos = 0;
                    float yPos = 0;

                    xPos = Random.Range(0, mapSize.x);
                    yPos = Random.Range(0, mapSize.y);
                    if (Root.SceneLoader.TileMapManager.GetCell((int)xPos, (int)yPos).Block)
                    {
                        cellTargetEntity.CellTargetComponent.refreshTime = 3;
                        continue;
                    }

                    cells = Root.SceneLoader.TileMapManager.GetPath(new Vector2(cellTargetEntity.TransformComponent.position.x, cellTargetEntity.TransformComponent.position.y),
                        new Vector2(xPos, yPos));

                    foreach ( var cell in cells )
                        cellTargetEntity.CellTargetComponent.cellPath.Add(new Vector2(cell.Position.x, cell.Position.y));
                }

                if (cellTargetEntity.CellTargetComponent.finish)
                    continue;

                if(cellTargetEntity.CellTargetComponent.cellPath.Count == 0)
                {
                    cellTargetEntity.CellTargetComponent.finish = true;
                    cellTargetEntity.CellTargetComponent.refreshTime = 3;
                    continue;
                }

                if (Vector2.Distance(cellTargetEntity.TransformComponent.position, cellTargetEntity.CellTargetComponent.cellPath[0]) < 0.001f)
                {
                    cellTargetEntity.CellTargetComponent.cellPath.RemoveAt(0);
                    if (cellTargetEntity.CellTargetComponent.cellPath.Count == 0)
                    {
                        cellTargetEntity.CellTargetComponent.finish = true;
                        cellTargetEntity.MoveComponent.targetPos = Vector2.zero;
                        continue;
                    }
                }

                if (Root.SceneLoader.TileMapManager.GetCell((int)cellTargetEntity.CellTargetComponent.cellPath[0].x, (int)cellTargetEntity.CellTargetComponent.cellPath[0].y).ObjectBlock)
                    cellTargetEntity.CellTargetComponent.refreshTime -= 0.1f;
                
                cellTargetEntity.MoveComponent.targetPos = cellTargetEntity.CellTargetComponent.cellPath[0];
            }
        }
    }
}

