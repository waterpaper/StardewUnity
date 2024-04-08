using System.Collections.Generic;
using UnityEngine;
using WATP.Map;

namespace WATP.ECS
{
    /// <summary>
    /// 움직임 컴포넌트
    /// </summary>
    public class CellAddObjectService : IService
    {
        public List<Cell> cells = new();
        public List<ICellAddObjectComponent> cellAddObjectComponents = new();

        public void Add(IEntity entity)
        {
            if (entity is not ICellAddObjectComponent) return;

            cellAddObjectComponents.Add(entity as ICellAddObjectComponent);
        }

        public void Remove(IEntity entity)
        {
            if (entity is not ICellAddObjectComponent) return;

            cellAddObjectComponents.Remove(entity as ICellAddObjectComponent);
        }

        public void Clear()
        {
            cellAddObjectComponents.Clear();
        }

        public void Update(double frameTime)
        {
            Cell cellTemp;
            foreach (var cell in cells)
                cell.ObjectBlock = false;

            cells.Clear();
            foreach (var cellTargetEntity in cellAddObjectComponents)
            {
                cellTemp = Root.SceneLoader.TileMapManager.GetCell((int)cellTargetEntity.TransformComponent.position.x, (int)cellTargetEntity.TransformComponent.position.y);
                cellTemp.ObjectBlock = true;

                cells.Add(cellTemp);
            }
        }
    }
}

