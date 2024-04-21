using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// �� UI�� open �����ִ� component
    /// open ���� disable ó�����ش�.
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