using System.Collections.Generic;

namespace WATP.ECS
{
    /// <summary>
    /// service interface로 원하는 기능을 처리한다.(보통 component별 기능 구현)
    /// 원하는 compoenent를 가진 entity를 미리 캐싱해 두기 위해 add, remove, clear 구현
    /// 기능은 update에서 구현
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// 원하는 compoenent를 가진 entity를 미리 캐싱하기 위한 용도
        /// </summary>
        void Add(IEntity entity);

        /// <summary>
        /// 미리 캐싱된 entity를 삭제하기 위한 용도
        /// </summary>
        void Remove(IEntity entity);

        /// <summary>
        /// 미리 캐싱된 entity를 전체 삭제하기 위한 용도
        /// </summary>
        void Clear();

        void Update(double frameTime);
    }
}