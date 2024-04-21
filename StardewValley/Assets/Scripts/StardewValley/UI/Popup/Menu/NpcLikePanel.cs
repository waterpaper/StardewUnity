using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class NpcLikePanel : UIElement
    {
        private int id;
        private Text nameText;
        private Image npcIcon;
        private Text pointText;
        private List<GameObject> likeObjs;


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            nameText = rectTransform.RecursiveFindChild("Txt_Name").GetComponent<Text>();
            npcIcon = rectTransform.RecursiveFindChild("Img_Npc").GetComponent<Image>();
            pointText = rectTransform.RecursiveFindChild("Txt_Point").GetComponent<Text>();

            var likeArea = rectTransform.RecursiveFindChild("LikeArea").transform;
            likeObjs = new();
            for (int i = 0; i < likeArea.childCount; i++)
                likeObjs.Add(likeArea.GetChild(i).gameObject);

            Bind();
        }

        public override void Dispose()
        {
            UnBind();
            var tablaData = Root.GameDataManager.TableData.GetNPCTableData(id);
            
            AssetLoader.Unload<Sprite>($"Address/Sprite/Characters/{tablaData.Name_En}.png[{tablaData.Name_En}_{0}]", npcIcon.sprite);
            base.Dispose();
        }
        void Bind()
        {
        }

        void UnBind()
        {
        }

        public void Setting(int id, int point)
        {
            var tablaData = Root.GameDataManager.TableData.GetNPCTableData(id);
            this.id = id;
            nameText.text = tablaData.Name;
            pointText.text = point.ToString();

            npcIcon.sprite = AssetLoader.Load<Sprite>($"Address/Sprite/Characters/{tablaData.Name_En}.png[{tablaData.Name_En}_{0}]");


            for (int i = 0; i < likeObjs.Count; i++)
            {
                if (point > i)
                {
                    likeObjs[i].transform.GetChild(0).gameObject.SetActive(true);
                    likeObjs[i].transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    likeObjs[i].transform.GetChild(0).gameObject.SetActive(false);
                    likeObjs[i].transform.GetChild(1).gameObject.SetActive(true);
                }
            }
        }
    }
}
