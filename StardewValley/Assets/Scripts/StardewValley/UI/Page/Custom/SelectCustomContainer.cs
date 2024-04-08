using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class SelectCustomContainer : UIElement
    {
        private Button leftBtn;
        private Button rightBtn;
        private Text selectedIndexTxt;

        private Slider rSlider;
        private Text rTxt;
        private Slider gSlider;
        private Text gTxt;
        private Slider bSlider;
        private Text bTxt;

        private Action<int> buttonAction;
        private Action<int, int, int> colorChangeAction;

        public int SelectedIndex { get; private set; } = 1;
        public int MaxIndex { get; private set; } = 1;
        public int ColorR { get; private set; } = 255;
        public int ColorG { get; private set; } = 255;
        public int ColorB { get; private set; } = 255;



        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            leftBtn = rectTransform.RecursiveFindChild("Bt_Left").GetComponent<Button>();
            rightBtn = rectTransform.RecursiveFindChild("Bt_Right").GetComponent<Button>();
            selectedIndexTxt = rectTransform.RecursiveFindChild("Txt_Value").GetComponent<Text>();

            rSlider = rectTransform.RecursiveFindChild("ProgressBar", "R").GetComponent<Slider>();
            rTxt = rectTransform.RecursiveFindChild("Txt_Value", "R").GetComponent<Text>();

            gSlider = rectTransform.RecursiveFindChild("ProgressBar", "G").GetComponent<Slider>();
            gTxt = rectTransform.RecursiveFindChild("Txt_Value", "G").GetComponent<Text>();

            bSlider = rectTransform.RecursiveFindChild("ProgressBar", "B").GetComponent<Slider>();
            bTxt = rectTransform.RecursiveFindChild("Txt_Value", "B").GetComponent<Text>();

            Bind();


            selectedIndexTxt.text = SelectedIndex.ToString();
            rTxt.text = ColorB.ToString();
            gTxt.text = ColorB.ToString();
            bTxt.text = ColorB.ToString();
        }

        public override void Dispose()
        {
            UnBind();

            base.Dispose();
        }

        public void Injection(int maxIndex, Action<int> buttonAction, Action<int, int, int> colorChangeAction)
        {
            this.MaxIndex = maxIndex;
            this.buttonAction = buttonAction;
            this.colorChangeAction = colorChangeAction;
        }

        void Bind()
        {
            leftBtn?.onClick.AddListener(OnLeftButton);
            rightBtn?.onClick.AddListener(OnRightButton);

            rSlider?.onValueChanged.AddListener(OnRColorChange);
            gSlider?.onValueChanged.AddListener(OnGColorChange);
            bSlider?.onValueChanged.AddListener(OnBColorChange);
        }

        void UnBind()
        {
            leftBtn?.onClick.RemoveAllListeners();
            rightBtn?.onClick.RemoveAllListeners();

            rSlider?.onValueChanged.RemoveAllListeners();
            gSlider?.onValueChanged.RemoveAllListeners();
            bSlider?.onValueChanged.RemoveAllListeners();
        }


        #region Event Handlers


        private void OnLeftButton()
        {
            if (SelectedIndex <= 1) return;

            SelectedIndex--;
            buttonAction?.Invoke(SelectedIndex);

            selectedIndexTxt.text = SelectedIndex.ToString();
        }

        private void OnRightButton()
        {
            if (SelectedIndex >= MaxIndex) return;

            SelectedIndex++;
            buttonAction?.Invoke(SelectedIndex);

            selectedIndexTxt.text = SelectedIndex.ToString();
        }

        private void OnRColorChange(float value)
        {
            ColorR = (int)(value * 255);

            colorChangeAction?.Invoke(ColorR, ColorG, ColorB);

            rTxt.text = ColorR.ToString();
        }

        private void OnGColorChange(float value)
        {
            ColorG = (int)(value * 255);
            colorChangeAction?.Invoke(ColorR, ColorG, ColorB);

            gTxt.text = ColorG.ToString();
        }

        private void OnBColorChange(float value)
        {
            ColorB = (int)(value * 255);
            colorChangeAction?.Invoke(ColorR, ColorG, ColorB);

            bTxt.text = ColorB.ToString();
        }

        #endregion
    }
}