using WATP.ECS;

namespace WATP.View
{
    public interface IGridView
    {
        void SetGridView(bool view);
    }

    /// <summary>
    /// <see cref="View"/>�� �������̽� �Դϴ�.
    /// </summary>
    public interface IView
    {
        int UID { get; }

        bool IsShow { get; }
        /// <summary> ���̰� �� �� �ֳ�? </summary>
        bool CanShow { get; }
        bool IsAlreadyDisposed { get; }

        /// <summary>
        /// ���̰� �ϱ� <br/>
        /// </summary>
        void Show();
        /// <summary>
        /// ����� <br/>
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