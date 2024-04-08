using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class PopupOption
    {
        // canvas order
        public int orderIndex = 0;
        // 블러 사용 여부
        public bool isBlur = false;
        // 아웃 클릭시 종료 여부
        public bool isOutClickClose = true;
        // 전체 클릭 가능 여부
        public bool isInteraction = true;

        //alpha tween 여부
        public bool isAlphaAni = true;
        //tween start value
        public float alphaStart = 0;
        //tween time
        public float alphaAniTime = 0.2f;

        //size tween 여부
        public bool isSizeAni = true;
        //size start value
        public Vector2 sizeStart = new Vector2(0.6f, 0.6f);
        //size time
        public float sizeAniTime = 0.2f;

        //blocker tween 여부
        public bool isBlockerAni = true;
        //blocker start value
        public Color blockerStartColor = new Color(0, 0, 0, 0.0f);
        //blocker image color
        public Color blockerColor = new Color(0, 0, 0, 0.6f);
        //blocker tween time
        public float blockerAlphaAniTime = 0.2f;

        //기본 tween 사용 여부
        public bool DefaultAni
        {
            set
            {
                isAlphaAni = value;
                isSizeAni = value;
                isBlockerAni = value;
            }
        }
    }
}
