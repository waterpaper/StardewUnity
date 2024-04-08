using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WATP.Data;

namespace WATP.UI
{
    public class CustomPage : PageWidget
    {
        Button confirmButton;

        InputField nameInput;
        InputField farmInput;

        Image farmerBody, farmerHair, farmerPants, farmerShirts;

        SelectCustomContainer hairSelectContainer, clothsSelectContainer;

        int hairIndex = 1, clothsIndex = 1;
        int hairR = 255, hairG = 255, hairB = 255;
        int clothsR = 255, clothsG = 255, clothsB = 255;

        public static string DefaultPrefabPath { get => "CustomPage"; }

        protected override void OnInit()
        {
            closeButton = rectTransform.RecursiveFindChild("Bt_Back").GetComponent<Button>();
            confirmButton = rectTransform.RecursiveFindChild("Bt_Confirm").GetComponent<Button>();

            nameInput = rectTransform.RecursiveFindChild("TxtInput_FarmerName").GetComponent<InputField>();
            farmInput = rectTransform.RecursiveFindChild("TxtInput_FarmName").GetComponent<InputField>();

            farmerBody = rectTransform.RecursiveFindChild("Img_Body").GetComponent<Image>();
            farmerHair = rectTransform.RecursiveFindChild("Img_Hair").GetComponent<Image>();
            farmerPants = rectTransform.RecursiveFindChild("Img_Pants").GetComponent<Image>();
            farmerShirts = rectTransform.RecursiveFindChild("Img_Shirts").GetComponent<Image>();

            hairSelectContainer = new SelectCustomContainer();
            hairSelectContainer.Initialize(rectTransform.RecursiveFindChild("Hair"));
            hairSelectContainer.Injection(Data.Config.CUSTOM_HAIR_MAX, OnHairChangeButton, OnHairColorChangeButton);

            clothsSelectContainer = new SelectCustomContainer();
            clothsSelectContainer.Initialize(rectTransform.RecursiveFindChild("Shirt"));
            clothsSelectContainer.Injection(Data.Config.CUSTOM_CLOTHS_MAX, OnClothsChangeButton, OnClothsColorChangeButton);


            Bind();
        }

        protected override void OnDestroy()
        {
            hairSelectContainer.Dispose();
            clothsSelectContainer.Dispose();
            UnBind();
        }

        #region event

        private void Bind()
        {
            closeButton.onClick.AddListener(OnBackButton);
            confirmButton.onClick.AddListener(OnConfirmButton);
        }

        private void UnBind()
        {
            closeButton.onClick.RemoveAllListeners();
            confirmButton.onClick.RemoveAllListeners();
        }

        private void OnBackButton()
        {
        }

        private void OnConfirmButton()
        {
            Root.State.GameInit();
            Root.State.GameStartSetting();

            Root.State.player.name.Value = nameInput.text;
            Root.State.player.farmName.Value = farmInput.text;

            Root.State.player.hairIndex.Value = hairIndex;
            Root.State.player.clothsIndex.Value = clothsIndex;

            Root.State.player.hairColor.Value = new Vector3(hairR, hairG, hairB);
            Root.State.player.clothsColor.Value = new Vector3(clothsR, clothsG, clothsB);

            Root.SceneLoader.SceneLoad(SceneKind.Ingame);
        }

        private void OnHairChangeButton(int value)
        {
            hairIndex = value;
            farmerHair.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_HAIR_PATH, $"FarmerHairs_{hairIndex - 1}");
        }

        private void OnHairColorChangeButton(int r, int g, int b)
        {
            hairR = r;
            hairG = g;
            hairB = b;

            farmerHair.color = new Color(r / 255.0f, g / 255.0f, b / 255.0f);
        }

        private void OnClothsChangeButton(int value)
        {
            clothsIndex = value;
            farmerShirts.sprite = Root.GameDataManager.AtlasContainer.GetSheetSprite(Root.GameDataManager.AtlasContainer.FARMER_SHIRTS_PATH, $"FarmerShirts_{clothsIndex - 1}");
        }

        private void OnClothsColorChangeButton(int r, int g, int b)
        {
            clothsR = r;
            clothsG = g;
            clothsB = b;

            farmerPants.color = new Color(r / 255.0f, g / 255.0f, b / 255.0f);
        }

        #endregion

    }
}
