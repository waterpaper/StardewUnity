using System;
using UnityEngine;

namespace WATP.UI
{
    public class CreatePanel : UIElement
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
