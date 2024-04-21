using WATP.ECS;

namespace WATP.View
{
    public interface IGridView
    {
        void SetGridView(bool view);
    }

    /// <summary>
    /// <see cref="View"/>의 인터페이스 입니다.
    /// </summary>
    public interface IView
    {
        int UID { get; }

        bool IsShow { get; }
        /// <summary> 보이게 할 수 있나? </summary>
        bool CanShow { get; }
        bool IsAlreadyDisposed { get; }

        /// <summary>
        /// 보이게 하기 <br/>
        /// </summary>
        void Show();
        /// <summary>
        /// 숨기기 <br/>
        /// </summary>
        void Hide();


        void Render();
        void ReRef(IWATPObjectAspect aspect);
        void Dispose();

        void SetMultiply(float multiply);

        void SetContinue(float multiply);

        void SetPause();
    }
}