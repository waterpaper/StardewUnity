using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{

    public class ProgressBarFill : ProgressBar
    {
        private void Awake()
        {
            Setting();
        }

        public override void Progress(float amount)
        {
            _current = amount;
            _foreImage.fillAmount = amount;
        }

        public virtual void Setting()
        {
            if(_backImage == null)
            {
                var backImg = transform.RecursiveFindChild("BackGround");
                if(backImg != null) _backImage = backImg.GetComponent<Image>();

            }
            if (_foreImage == null)
            {
                var foreImg = transform.RecursiveFindChild("ForeGround");
                if (foreImg != null) _foreImage = foreImg.GetComponent<Image>();

            }
        }

    }
}