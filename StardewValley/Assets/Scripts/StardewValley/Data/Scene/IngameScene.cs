using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using WATP.ECS;
using WATP.UI;

namespace WATP
{
    /// <summary>
    /// ingame 씬
    /// ingame에 필요한 entity 및 class를 생성하고 설정을 도와준다.
    /// 옵션 이벤트 처리를 담당한다.
    /// </summary>
    public class IngameScene : GameSceneBase
    {
        MenuPopup menuPopup;
        IngameCameraBounds cameraBounds;

        public override void Init()
        {
            cameraBounds = new IngameCameraBounds();
            Bind();
        }

        public override void Complete()
        {
            Root.State.logicState.Value = LogicState.Normal;
            Root.SoundManager.PlaySound(SoundTrack.BGM, "farm", true);
        }

        public override IEnumerator Load()
        {
            Root.SceneLoader.TileMapManager.MapSetting("House");
            yield return OpenIngamePage();
        }

        public override IEnumerator Unload()
        {
            Root.SceneLoader.TileMapManager.Clear();
            UnBind();
            cameraBounds.Dispose();

            yield return null;
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (menuPopup == null)
                    OpenMenuPopup().Forget();
                else
                    CloseMenuPopup();
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                if (Root.State.logicState.Value != LogicState.Normal) return;

                Root.State.TodayUpdateSetting();

                Root.State.inventory.AddInventory(203, 1);
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                if (Root.State.logicState.Value != LogicState.Normal) return;
                Root.State.MonthUpdateSetting();
                Root.State.inventory.AddInventory(501, 2);
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                if (menuPopup == null)
                    OpenAdvicePopup().Forget();
                else
                    CloseMenuPopup();
            }


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Root.State.inventory.selectIndex.Value = Root.State.inventory.selectIndex.Value == 0 ? -1 : 0;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Root.State.inventory.selectIndex.Value = Root.State.inventory.selectIndex.Value == 1 ? -1 : 1;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Root.State.inventory.selectIndex.Value = Root.State.inventory.selectIndex.Value == 2 ? -1 : 2;
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Root.State.inventory.selectIndex.Value = Root.State.inventory.selectIndex.Value == 3 ? -1 : 3;
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Root.State.inventory.selectIndex.Value = Root.State.inventory.selectIndex.Value == 4 ? -1 : 4;
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Root.State.inventory.selectIndex.Value = Root.State.inventory.selectIndex.Value == 5 ? -1 : 5;
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                Root.State.inventory.selectIndex.Value = Root.State.inventory.selectIndex.Value == 6 ? -1 : 6;
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                Root.State.inventory.selectIndex.Value = Root.State.inventory.selectIndex.Value == 7 ? -1 : 7;
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Root.State.inventory.selectIndex.Value = Root.State.inventory.selectIndex.Value == 8 ? -1 : 8;
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Root.State.inventory.selectIndex.Value = Root.State.inventory.selectIndex.Value == 9 ? -1 : 9;
            }

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                Root.State.inventory.selectIndex.Value = Root.State.inventory.selectIndex.Value == 10 ? -1 : 10;
            }

            if (Input.GetKeyDown(KeyCode.KeypadEquals))
            {
                Root.State.inventory.selectIndex.Value = Root.State.inventory.selectIndex.Value == 11 ? -1 : 11;
            }

            cameraBounds.Update();
        }

        private async UniTaskVoid OpenIngamePage()
        {
            var ingamePage = new IngamePage();
            ingamePage = await Root.UIManager.Widgets.CreateAsync<IngamePage>(ingamePage, IngamePage.DefaultPrefabPath);
        }

        private async UniTaskVoid OpenMenuPopup()
        {
            menuPopup = new MenuPopup();
            menuPopup.onCloseEvent += () => { menuPopup = null; };
            menuPopup = await Root.UIManager.Widgets.CreateAsync<MenuPopup>(menuPopup, MenuPopup.DefaultPrefabPath);
        }

        private async UniTaskVoid OpenAdvicePopup()
        {
            menuPopup = new MenuPopup();
            menuPopup.onCloseEvent += () => { menuPopup = null; };
            menuPopup = await Root.UIManager.Widgets.CreateAsync<MenuPopup>(menuPopup, MenuPopup.DefaultPrefabPath);
            menuPopup.tabMenu.SelectTab(4);
        }

        private void CloseMenuPopup()
        {
            menuPopup.ClosePopup();
        }


        private void Bind()
        {
            ECSController.ServiceEvents.On<EventCreateEntity>(OnEntityCreate);
        }

        private void UnBind()
        {
            ECSController.ServiceEvents.Off<EventCreateEntity>(OnEntityCreate);
        }

        private void OnEntityCreate(EventCreateEntity e)
        {
            if (e.Entity is not FarmerEntity) return;

            cameraBounds.Setting(e.Entity as FarmerEntity);
        }
    }
}