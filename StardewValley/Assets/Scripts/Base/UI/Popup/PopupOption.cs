using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class PopupOption
    {
        // canvas order
        public int orderIndex = 0;
        // �� ��� ����
        public bool isBlur = false;
        // �ƿ� Ŭ���� ���� ����
        public bool isOutClickClose = true;
        // ��ü Ŭ�� ���� ����
        public bool isInteraction = true;

        //alpha tween ����
        public bool isAlphaAni = true;
        //tween start value
        public float alphaStart = 0;
        //tween time
        public float alphaAniTime = 0.2f;

        //size tween ����
        public bool isSizeAni = true;
        //size start value
        public Vector2 sizeStart = new Vector2(0.6f, 0.6f);
        //size time
        public float sizeAniTime = 0.2f;

        //blocker tween ����
        public bool isBlockerAni = true;
        //blocker start value
        public Color blockerStartColor = new Color(0, 0, 0, 0.0f);
        //blocker image color
        public Color blockerColor = new Color(0, 0, 0, 0.6f);
        //blocker tween time
        public float blockerAlphaAniTime = 0.2f;

        //�⺻ tween ��� ����
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
