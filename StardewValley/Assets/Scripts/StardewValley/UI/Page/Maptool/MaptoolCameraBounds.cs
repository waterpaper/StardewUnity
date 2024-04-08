using UnityEngine;

namespace WATP.UI
{
    public class MaptoolCameraBounds
    {
        private Vector3 pos;
        private int sizeX;
        private int sizeY;


        public void Update()
        {
            pos = Camera.main.transform.position;
            if (Input.GetKey(KeyCode.DownArrow))
            {
                pos.y += (Time.deltaTime * -5);
                if (!ConfirmBounds(pos, 1))
                    pos.y = 10;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                pos.y += (Time.deltaTime * 5);
                if (!ConfirmBounds(pos, 2))
                    pos.y = sizeY - 10 < 10 ? 10 : sizeY - 10;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                pos.x += (Time.deltaTime * -5);
                if (!ConfirmBounds(pos, 3))
                    pos.x = 15;

            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                pos.x += (Time.deltaTime * 5);
                if (!ConfirmBounds(pos, 4))
                    pos.x = sizeX - 8 < 15 ? 15 : sizeX -8;
            }

            Camera.main.transform.position = pos;
        }

        public void Setting(int x, int y)
        {
            sizeX = x;
            sizeY = y;
            Camera.main.transform.position = new Vector3(15, 10, Camera.main.transform.position.z);
        }

        public bool ConfirmBounds(Vector3 vector, int direction)
        {
            if(vector.x > sizeX - 8 && direction == 4)
                return false;

            if (vector.x < 15 && direction == 3)
                return false;

            if (vector.y > sizeY - 10 && direction == 2)
                return false;

            if (vector.y < 10 && direction == 1)
                return false;

            return true;
        }

    }
}
