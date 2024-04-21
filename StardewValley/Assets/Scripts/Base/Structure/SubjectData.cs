using System;


namespace WATP
{
    /// <summary>
    /// Observer pattern 구현을 위한 struct
    /// 필요한 경우 action에 등록해 사용한다.
    /// </summary>
    public struct SubjectData<T>
    {
        private T v;
        public T Value
        {
            get
            {
                return this.v;
            }
            set
            {
                this.v = value;
                this.onChange?.Invoke(value);
            }
        }
        public Action<T> onChange;
    }
}
