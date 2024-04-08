using System.Collections.Generic;

namespace WATP.ECS
{
    /// <summary>
    /// 움직임 컴포넌트
    /// </summary>
    public class DelayDeleteService : IService
    {
        public List<IDelayDeleteComponent> delayDeleteComponents = new();

        public void Add(IEntity entity)
        {
            if (entity is not IDelayDeleteComponent) return;

            delayDeleteComponents.Add(entity as IDelayDeleteComponent);
        }

        public void Remove(IEntity entity)
        {
            if (entity is not IDelayDeleteComponent) return;

            delayDeleteComponents.Remove(entity as IDelayDeleteComponent);
        }

        public void Clear()
        {
            delayDeleteComponents.Clear();
        }

        public void Update(double frameTime)
        {
            foreach (var delayDeleteEntity in delayDeleteComponents)
            {
                if (delayDeleteEntity.DelayDeleteComponent.isEnable == false) continue;

                delayDeleteEntity.DelayDeleteComponent.timer += (float)frameTime;
                if (delayDeleteEntity.DelayDeleteComponent.deleteTime > delayDeleteEntity.DelayDeleteComponent.timer) continue;
                delayDeleteEntity.DeleteReservation = true;
            }
        }
    }
}

