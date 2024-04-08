using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class DialogPopup : PopupWidget
    {
        public static string DefaultPrefabPath { get => "DialogPopup"; }

        public Image icon;
        public Text diName;
        public Text conversation;

        private int npcId;

        protected override void OnInit()
        {
            icon = rectTransform.RecursiveFindChild("Img_Icon").GetComponent<Image>();
            diName = rectTransform.RecursiveFindChild("Txt_Name").GetComponent<Text>();
            conversation = rectTransform.RecursiveFindChild("Txt_Dialog").GetComponent<Text>();
            closeButton = rectTransform.RecursiveFindChild("Bt_Close").GetComponent<Button>();

            Root.State.logicState.Value = LogicState.Parse;
            Bind();
        }

        protected override void OnUpdate()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                ClosePopup();
            }
        }


        protected override void OnDestroy()
        {
            UnBind();
            var tablaData = Root.GameDataManager.TableData.GetNPCTableData(npcId);

            AssetLoader.Unload<Sprite>($"Address/Sprite/Portraits/{tablaData.Name_En}.png[{tablaData.Name_En}_{0}]", icon.sprite);
            Root.State.logicState.Value = LogicState.Normal;
            base.OnDestroy();
        }

        protected override void PopupOptionSetting(PopupOption popupOption)
        {
            popupOption.isBlur = false;
            popupOption.isBlockerAni = false;
            popupOption.blockerColor = new Color(0, 0, 0, 0);
            popupOption.isOutClickClose = false;
        }

        private void Bind()
        {
            closeButton.onClick.AddListener(ClosePopup);
        }

        private void UnBind()
        {
            closeButton.onClick.RemoveAllListeners();
        }

        public void Setting(int npcId, int type)
        {
            var tablaData = Root.GameDataManager.TableData.GetNPCTableData(npcId);
            this.npcId = npcId;
            diName.text = tablaData.Name;

            icon.sprite = AssetLoader.Load<Sprite>($"Address/Sprite/Portraits/{tablaData.Name_En}.png[{tablaData.Name_En}_{0}]");

            var convarstionTable = Root.GameDataManager.TableData.GetNpcConversationTableData(npcId);
            if (type == 1)
            {
                conversation.text = convarstionTable.Conversation;
            }
            else if(type == 2)
            {
                conversation.text = convarstionTable.Gift;
            }
            else if(type == 3)
            {
                conversation.text = convarstionTable.FavorGift;
            }
        }
    }
}
