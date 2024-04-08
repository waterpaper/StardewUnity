using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{

    public class ProgressBarSlice : ProgressBar
    {
        private void Awake()
        {
            Setting();
        }

        public override void Progress(float amount)
        {
            if (_foreImage == null)
                Setting();

            _current = amount;
            _foreImage.rectTransform.anchorMax = new Vector2(amount, _foreImage.rectTransform.anchorMax.y);
            _foreImage.rectTransform.rect.Set(0, 0, 0, 0);
        }

        public void Setting()
        {
            if (_backImage == null)
            {
                var backImg = transform.RecursiveFindChild("BackGround");
                if (backImg != null) _backImage = backImg.GetComponent<Image>();

            }
            if (_foreImage == null)
            {
                var foreImg = transform.RecursiveFindChild("ForeGround");
                if (foreImg != null) _foreImage = foreImg.GetComponent<Image>();

            }
        }
    }
}