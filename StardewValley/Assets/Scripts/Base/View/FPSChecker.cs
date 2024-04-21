using UnityEngine;

namespace WATP
{
    public class FpsCheck : MonoBehaviour
    {
        float posX, posY;

        float deltaTime = 0.0f;
        float totalTime = 0.0f;


        private void Start()
        {
            SetPos();

        }
        private void OnDestroy()
        {
        }

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            totalTime += Time.deltaTime;
        }

        void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            //Rect rect = new Rect(0, 0, w, h * 2 / 100);
            Rect rect = new Rect(posX, posY, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100 + 3;
            style.normal.textColor = new Color(1f, 0.0f, 0.0f, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;

            string text = string.Format("{0:0.0} ms ({1:0.} fps)\nAppVersion: {2}", msec, fps, Application.version);

            GUI.Label(rect, text, style);
        }

        void SetPos()
        {
            int setWidth = 1200; // 사용자 설정 너비
            int setHeight = 800; // 사용자 설정 높이
            int deviceWidth = Screen.width;
            int deviceHeight = Screen.height;

            int safeDeviceWidth = (int)Screen.safeArea.width;
            int safeDeviceHeight = (int)Screen.safeArea.height;
            Rect safeArea = Screen.safeArea;

            Rect rect;
            if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
            {
                float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
                rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
            }
            else // 게임의 해상도 비가 더 큰 경우
            {
                float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
                rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
            }

            var x = rect.x * deviceWidth;
            var w = rect.width * deviceWidth;
            var y = rect.y * deviceHeight;
            var h = rect.height * deviceHeight;

            if (safeArea.x > x || safeArea.y > y || x + w > safeArea.x + safeArea.width || y + h > safeArea.y + safeArea.height)
            {
                Debug.Log("Out of safe Area");
                float xR = safeArea.x / deviceWidth;
                float yR = safeArea.y / deviceHeight;
                if ((float)setWidth / setHeight < (float)safeDeviceWidth / safeDeviceHeight) // 기기의 해상도 비가 더 큰 경우
                {
                    var rateTemp = (float)safeDeviceWidth / deviceWidth;
                    float newWidth = ((float)setWidth / setHeight) / ((float)safeDeviceWidth / safeDeviceHeight); // 새로운 너비

                    rect = new Rect(xR + ((1f - newWidth) / 2f) * rateTemp, yR, newWidth * rateTemp, 1f - yR); // 새로운 Rect 적용
                }
                else // 게임의 해상도 비가 더 큰 경우
                {
                    var rateTemp = (float)safeDeviceHeight / deviceHeight;
                    float newHeight = ((float)safeDeviceWidth / safeDeviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
                    rect = new Rect(xR, yR + ((1f - newHeight) / 2f) * rateTemp, 1f - xR, newHeight * rateTemp); // 새로운 Rect 적용
                }
            }

            posX = rect.x * deviceWidth;
            posY = rect.y * deviceHeight;
        }
    }
}
