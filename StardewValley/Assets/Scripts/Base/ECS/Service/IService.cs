using System.Collections.Generic;

namespace WATP.ECS
{
    public interface IService
    {
        void Add(IEntity entity);

        void Remove(IEntity entity);

        void Clear();

        void Update(double frameTime);
    }
}