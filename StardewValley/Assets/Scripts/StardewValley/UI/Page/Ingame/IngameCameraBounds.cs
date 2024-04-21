using Unity.Mathematics;
using UnityEngine;
using WATP.ECS;

namespace WATP.UI
{
    public class IngameCameraBounds
    {
        private float3 beforePos;
        private float3 pos;
        private MapObjectManager mapObjectManager;
        private bool isSetting;
        private int sizeX;
        private int sizeY;

        public void Setting()
        {
            this.mapObjectManager = StardewValleyRoot.MapObjectManager;
            isSetting = true;
        }

        public void Dispose()
        {
            mapObjectManager = null;
            isSetting = false;
        }

        public void Update()
        {
            if (!isSetting || Root.State.logicState.Value == LogicState.Parse) return;
            var playerPostion = mapObjectManager.PlayerPosision;
            if (math.all(beforePos == playerPostion)) return;

            pos.x = playerPostion.x;
            pos.y = playerPostion.y;
            pos.z = Camera.main.transform.position.z;

            beforePos = playerPostion;
            Camera.main.transform.position = ConfirmBounds(pos);
        }

        public Vector3 ConfirmBounds(Vector3 vector)
        {
            var size = Root.SceneLoader.TileMapManager.TileSize;
            sizeX = (int)size.x;
            sizeY = (int)size.y;

            if ((sizeX < 30) || (sizeY < 20))
            {
                if (sizeX < 30)
                    vector.x = sizeX / 2;

                if (sizeY < 20)
                    vector.y = sizeY / 2;

                return vector;
            }

            if (vector.x > sizeX - 15)
                vector.x = sizeX - 15;

            if (vector.x < 15)
                vector.x = 15;

            if (vector.y > sizeY - 10)
                vector.y = sizeY - 10;

            if (vector.y < 10)
                vector.y = 10;

            return vector;
        }

    }
}
