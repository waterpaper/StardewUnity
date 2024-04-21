using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// 각 entity의 빌더 인터페이스 입니다.
    /// </summary>
    public interface IEntityBuilder
    {
        /// <summary>
        /// entity의 aspect를 리턴합니다.
        /// aspect를 기준으로 관리합니다.
        /// </summary>
        IWATPObjectAspect GetObjectAspect();

        /// <summary>
        /// entity을 생성합니다.
        /// </summary>
        Entity Build();
    }
}