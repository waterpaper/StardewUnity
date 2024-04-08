using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class MapPanel : UIElement
    {
        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            Bind();
        }

        public override void Dispose()
        {
            UnBind();

            base.Dispose();
        }

        void Bind()
        {
        }

        void UnBind()
        {
        }
    }
}
