using System.Collections.Generic;

namespace WATP.ECS
{
    /// <summary>
    /// 제거 기능을 처리하기 위한 클래스
    /// 딜레이된 delete 혹은 제거가 필요한 entity의 인자를 바꿔서 manager에서 삭제를 처리할 수 있게 도와준다.
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

