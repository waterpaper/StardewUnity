namespace WATP.ECS
{
    /// <summary>
    /// component interface로 component를 갖는 interface 구현으로 다중 상속 대신 구현
    /// component 조합 기반으로 entity를 만든다.
    /// entity는 내부에 정보를 가지지 않고 component에 정보를 가져 해당 조합으로 기능을 처리한다.
    /// (ex: entity - object, component - component)
    /// </summary>
    public interface IComponent
    {

    }
}