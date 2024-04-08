using System;


namespace WATP
{
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
