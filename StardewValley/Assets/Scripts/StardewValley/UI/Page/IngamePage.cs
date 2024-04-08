
namespace WATP.UI
{
    public class IngamePage : PageWidget
    {
        IngameInfoContainer infoContainer;
        IngameToolbarContainer toolbarContainer;
        IngameGaugeBarContainer gaugeBarContainer;

        IngameCameraBounds cameraBounds;

        IngameGetItemContainer getItemContainer;

        public static string DefaultPrefabPath { get => "IngamePage"; }

        protected override void OnInit()
        {
            infoContainer = new IngameInfoContainer();
            infoContainer.Initialize(rectTransform.RecursiveFindChild("Img_GameInfo"));

            toolbarContainer = new IngameToolbarContainer();
            toolbarContainer.Initialize(rectTransform.RecursiveFindChild("ToolBar"));

            gaugeBarContainer = new IngameGaugeBarContainer();
            gaugeBarContainer.Initialize(rectTransform.RecursiveFindChild("GaugeBar"));

            cameraBounds = new IngameCameraBounds();

            getItemContainer = new IngameGetItemContainer();
            getItemContainer.Initialize(rectTransform.RecursiveFindChild("GetItemPanel"));
            Bind();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            cameraBounds.Update();
            getItemContainer.Update();
        }

        protected override void OnDestroy()
        {
            infoContainer.Dispose();
            toolbarContainer.Dispose();
            gaugeBarContainer.Dispose();
            getItemContainer.Dispose();
            UnBind();
        }


        #region event

        private void Bind()
        {
        }

        private void UnBind()
        {
        }

        #endregion

    }
}
