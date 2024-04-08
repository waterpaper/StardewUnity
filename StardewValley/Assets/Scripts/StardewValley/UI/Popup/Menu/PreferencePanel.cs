using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class PreferencePanel : UIElement
    {
        private Text bgmText;
        private Text sfxText;
        private Slider bgmSlider;
        private Slider sfxSlider;
        private Button titleBtn;
        private Button exitBtn;

        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            bgmText = rectTransform.RecursiveFindChild("Txt_Bgm").GetComponent<Text>();
            sfxText = rectTransform.RecursiveFindChild("Txt_Sfx").GetComponent<Text>();

            bgmSlider = rectTransform.RecursiveFindChild("Slider_Bgm").GetComponent<Slider>();
            sfxSlider = rectTransform.RecursiveFindChild("Slider_Sfx").GetComponent<Slider>();

            titleBtn = rectTransform.RecursiveFindChild("Bt_Title").GetComponent<Button>();
            exitBtn = rectTransform.RecursiveFindChild("Bt_Exit").GetComponent<Button>();

            bgmSlider.value = Root.GameDataManager.Preferences.Bgm;
            sfxSlider.value = Root.GameDataManager.Preferences.Sfx;
            Bind();
        }

        public override void Dispose()
        {
            UnBind();

            base.Dispose();
        }

        void Bind()
        {
            Root.GameDataManager.Preferences.OnBgmChange += OnBgmChange;
            Root.GameDataManager.Preferences.OnSfxChange += OnSfxChange;

            bgmSlider.onValueChanged.AddListener(OnBgmSliderChange);
            sfxSlider.onValueChanged.AddListener(OnSfxSliderChange);

            titleBtn.onClick.AddListener(OnTitleButton);
            exitBtn.onClick.AddListener(OnExitButton);
        }

        void UnBind()
        {
            Root.GameDataManager.Preferences.OnBgmChange -= OnBgmChange;
            Root.GameDataManager.Preferences.OnSfxChange -= OnSfxChange;

            bgmSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.RemoveAllListeners();

            titleBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.RemoveAllListeners();
        }


        void OnBgmChange(int value)
        {
            bgmText.text = value.ToString();
        }

        void OnSfxChange(int value)
        {
            sfxText.text = value.ToString();
        }

        void OnBgmSliderChange(float value)
        {
            Root.GameDataManager.Preferences.Bgm = (int)(value*100);
        }

        void OnSfxSliderChange(float value)
        {
            Root.GameDataManager.Preferences.Sfx = (int)(value * 100);
        }

        void OnTitleButton()
        {
            Root.SceneLoader.SceneLoad(SceneKind.Title);
        }

        void OnExitButton()
        {
            App.Quit();
        }
    }
}
