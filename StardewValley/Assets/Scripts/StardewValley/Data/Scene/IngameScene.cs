using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using Unity.Collections;
using Unity.Entities;
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

        Entity uiShopEntity, uiDialogEntity, uiSleepEntity;

        public override void Init()
        {
            cameraBounds = new IngameCameraBounds();
            Bind();
            uiShopEntity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(UIShopComponent));
            uiDialogEntity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(UIDialogComponent));
            uiSleepEntity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(UISleepCheckComponent));

            if (World.DefaultGameObjectInjectionWorld.EntityManager.IsComponentEnabled<UIShopComponent>(uiShopEntity))
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<UIShopComponent>(uiShopEntity, false);
            if (World.DefaultGameObjectInjectionWorld.EntityManager.IsComponentEnabled<UIDialogComponent>(uiDialogEntity))
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<UIDialogComponent>(uiDialogEntity, false);
            if (World.DefaultGameObjectInjectionWorld.EntityManager.IsComponentEnabled<UISleepCheckComponent>(uiSleepEntity))
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<UISleepCheckComponent>(uiSleepEntity, false);
        }

        public override void Complete()
        {
            Root.State.logicState.Value = LogicState.Normal;
            Root.SoundManager.PlaySound(SoundTrack.BGM, "farm", true);
        }

        public async override UniTask Load(CancellationTokenSource cancellationToken)
        {
            await Root.SceneLoader.TileMapManager.MapSetting("House");
            if(cancellationToken.IsCancellationRequested) return;
            await OpenIngamePage();
        }

        public async override UniTask Unload(CancellationTokenSource cancellationToken)
        {
            Root.SceneLoader.TileMapManager.Clear();
            UnBind();
            cameraBounds.Dispose();
            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(uiShopEntity);
            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(uiDialogEntity);
            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(uiSleepEntity);
            await UniTask.Yield(cancellationToken: cancellationToken.Token);
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
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                if (Root.State.logicState.Value != LogicState.Normal) return;
                Root.State.MonthUpdateSetting();
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


            if (World.DefaultGameObjectInjectionWorld.EntityManager.IsComponentEnabled<UIShopComponent>(uiShopEntity))
            {
                OpenShopPage().Forget();
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<UIShopComponent>(uiShopEntity, false);
            }

            if (World.DefaultGameObjectInjectionWorld.EntityManager.IsComponentEnabled<UIDialogComponent>(uiDialogEntity))
            {
                var com = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<UIDialogComponent>(uiDialogEntity);
                OpenDialogPopup(com.dataId, com.dialogType).Forget();
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<UIDialogComponent>(uiDialogEntity, false);
            }

            if (World.DefaultGameObjectInjectionWorld.EntityManager.IsComponentEnabled<UISleepCheckComponent>(uiSleepEntity))
            {
                OpenSleepCheckPopup().Forget();
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentEnabled<UISleepCheckComponent>(uiSleepEntity, false);
            }
        }

        private async UniTask OpenIngamePage()
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

        private async UniTaskVoid OpenDialogPopup(int id, int type)
        {
            var dialogPopup = new DialogPopup();
            dialogPopup = await Root.UIManager.Widgets.CreateAsync<DialogPopup>(dialogPopup, DialogPopup.DefaultPrefabPath);
            dialogPopup.Setting(id, type);
        }

        private async UniTaskVoid OpenShopPage()
        {
            var shoppage = new ShopPage();
            shoppage = await Root.UIManager.Widgets.CreateAsync<ShopPage>(shoppage, ShopPage.DefaultPrefabPath, null, true);
        }

        private async UniTaskVoid OpenSleepCheckPopup()
        {
            var sleepCheckPopup = new SleepCheckPopup();
            sleepCheckPopup = await Root.UIManager.Widgets.CreateAsync<SleepCheckPopup>(sleepCheckPopup, SleepCheckPopup.DefaultPrefabPath);
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
            if (e.Entity is not FarmerAspect) return;

            cameraBounds.Setting();
        }
    }
}