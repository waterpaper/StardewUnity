using UnityEngine;

namespace WATP.UI
{
    public class LikeabilityPanel : UIElement
    {
        private NpcLikePanel likePanel;
        private RectTransform contents;


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            likePanel = new NpcLikePanel();
            likePanel.Initialize(rectTransform.RecursiveFindChild("NpcLikeAbilty"));
            contents = rectTransform.RecursiveFindChild("Content");


            var list = Root.State.npcInfos;
            list.Sort((a, b) =>
            {
                if (a.likeAbility < b.likeAbility)
                    return -1;
                else
                    return 1;
            });

            for (int i = 0; i < list.Count; i++)
            {
                if(i == 0)
                {
                    likePanel.Setting(list[i].id, list[i].likeAbility);
                }
                else
                {
                    var newObj = GameObject.Instantiate(likePanel.RectTransform.gameObject, contents);
                    var newlikePanel = new NpcLikePanel();
                    newlikePanel.Initialize(newObj.GetComponent<RectTransform>());
                    newlikePanel.Setting(list[i].id, list[i].likeAbility / 10);
                }
            }

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
