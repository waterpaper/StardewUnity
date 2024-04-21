using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// 각 UI를 open 시켜주는 component
    /// open 이후 disable 처리해준다.
    /// </summary>
    public struct UIDialogComponent : IComponentData, IEnableableComponent
    {
        public int dataId;
        public int dialogType;
    }

    public struct UIShopComponent : IComponentData, IEnableableComponent
    {
    }

    public struct UISleepCheckComponent : IComponentData, IEnableableComponent
    {
    }
}