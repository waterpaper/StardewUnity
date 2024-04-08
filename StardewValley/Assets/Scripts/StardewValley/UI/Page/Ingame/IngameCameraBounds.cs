using UnityEngine;
using WATP.ECS;

namespace WATP.UI
{
    public class IngameCameraBounds
    {
        private Vector3 beforePos;
        private Vector3 pos;
        private FarmerEntity player;
        private int sizeX;
        private int sizeY;

        public void Setting(FarmerEntity player)
        {
            this.player = player;
        }

        public void Dispose()
        {
            player = null;
        }

        public void Update()
        {
            if (player == null) return;
            if (beforePos == player.TransformComponent.position) return;

            pos.x = player.TransformComponent.position.x;
            pos.y = player.TransformComponent.position.y;
            pos.z = Camera.main.transform.position.z;

            beforePos = player.TransformComponent.position;
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
