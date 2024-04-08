namespace WATP.Structure
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if (object.ReferenceEquals(instance, null))
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }
}