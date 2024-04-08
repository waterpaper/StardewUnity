using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public abstract class ProgressBar : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        protected float _current;
        [SerializeField]
        protected Image _backImage;
        [SerializeField]
        protected Image _foreImage;
        protected RectTransform rectTrans;

        public float Amount { get => _current; }
        public Image BackImage { get => _backImage; }
        public Image ForeImage { get => _foreImage; }

        public virtual void Position(Vector2 pos)
        {
            if (rectTrans == null)
                rectTrans = transform.GetComponent<RectTransform>();
            rectTrans.anchoredPosition = pos;
        }

        public virtual void Position(Vector3 pos)
        {
            if (rectTrans == null)
                rectTrans = transform.GetComponent<RectTransform>();
            rectTrans.anchoredPosition3D = pos;
        }

        public abstract void Progress(float amount);
    }
}