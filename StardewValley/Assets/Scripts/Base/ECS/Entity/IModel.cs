namespace WATP.ECS
{
    /// <summary>
    /// <see cref="Model"/>의 인터페이스 입니다.
    /// </summary>
    public interface IModel
    {
        /// <summary> Unique한 값 </summary>
        int UID { get; }

        /// <summary>
        /// Model 초기화 될 시 실행<br/>
        /// </summary>
        void OnInitialize();
        
        /// <summary>
        /// 삭제를 기준으로 실행
        /// </summary>
        void OnDestroy();
    }
}