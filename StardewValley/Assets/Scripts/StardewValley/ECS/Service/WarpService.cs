using System.Collections.Generic;
using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// ���� ó���� ���� ���� ����
    /// �ش��ϴ� ���� warp ������ ������ ������ �� �̵��� ó���Ѵ�.
    /// </summary>
    public class WarpService : IService
    {
        public List<IWarpComponent> warpComponents = new();

        public void Add(IEntity entity)
        {
            if (entity is not IWarpComponent) return;

            warpComponents.Add(entity as IWarpComponent);
        }

        public void Remove(IEntity entity)
        {
            if (entity is not IWarpComponent) return;

            warpComponents.Remove(entity as IWarpComponent);
        }

        public void Clear()
        {
            warpComponents.Clear();
        }

        public void Update(double frameTime)
        {
            foreach (var warpComponent in warpComponents)
            {
                if(IsCell_Warp(warpComponent.TransformComponent.position))
                {
                    var portalInfo = Root.SceneLoader.TileMapManager.GetPortalCell((int)(warpComponent.TransformComponent.position.x), (int)(warpComponent.TransformComponent.position.y));
                    if (portalInfo == null) return;

                    if (warpComponent is IPlayerComponent && (warpComponent as IPlayerComponent).PlayerComponent.value)
                        Root.SceneLoader.TileMapManager.MapSetting(portalInfo.portalName);

                    warpComponent.TransformComponent.position = new Vector3(portalInfo.portalNextPosX, portalInfo.portalNextPosY, 0);
                }
            }
        }



        public bool IsCell_Warp(Vector3 pos)
        {
            var cell = Root.SceneLoader.TileMapManager.GetCell((int)(pos.x), (int)(pos.y));

            if (cell == null)
                return false;

            return cell.GetCellType() == Map.CellKind.Portal;
        }
    }
}