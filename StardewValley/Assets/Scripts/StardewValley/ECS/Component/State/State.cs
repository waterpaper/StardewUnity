using Unity.Entities;

namespace WATP.ECS
{
    public static class StateType
    {
        public static string Idle = "Idle"; // 아무것도 아닌 상태(변경 가능 한)
        public static string Move = "Move";
        public static string Action = "Action";
        public static string ActionEnd = "ActionEnd";
        public static string Dead = "Dead";
    }

    public class EState_Idle : IState
    {
        public string Name { get; protected set; } = StateType.Idle;

        public void Enter(Entity entity, float frameTime)
        {
            if (World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<EventComponent>(entity) == false) return;
            var eventAspect = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<EventAspect>(entity);

            eventAspect.EventComponent.events.Add(new EventBuffer() { value = (int)EventKind.Idle });
        }

        public void Exit(ref Entity entity, float frameTime)
        {
        }

        public void Stay(ref Entity entity, float frameTime)
        { }
    }

    public class EState_Run : IState
    {
        public string Name { get; protected set; } = StateType.Move;

        public void Enter(Entity entity, float frameTime)
        {
        }

        public void Exit(ref Entity  entity, float frameTime)
        {
        }

        public void Stay(ref Entity entity, float frameTime)
        {
            if (World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<EventComponent>(entity) == false) return;
            var eventAspect = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<EventAspect>(entity);

            eventAspect.EventComponent.events.Add(new EventBuffer() { value = (int)EventKind.Move });
        }
    }


    public class EFarmerState_Action : IState
    {
        public float Timer { get; protected set; } = 0;
        public string Name { get; protected set; } = StateType.Action;

        public void Enter(Entity entity, float frameTime)
        {
            if (World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<PlayerComponent>(entity) == false) return;
            var farmerAspect = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<FarmerAspect>(entity);

            farmerAspect.MoveInputComponentEnable = true;
            farmerAspect.InteractionInputComponentEnable = true;
            Timer = 0;
            farmerAspect.EventComponent.events.Add(new EventBuffer() { value = (int)EventKind.Action });
        }

        public void Exit(ref Entity entity, float frameTime)
        {
            if (World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<PlayerComponent>(entity) == false) return;
            var farmerAspect = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<FarmerAspect>(entity);

            farmerAspect.MoveInputComponentEnable = true;
            farmerAspect.InteractionInputComponentEnable = true;
            Timer = 0;
            farmerAspect.EventComponent.events.Add(new EventBuffer() { value = (int)EventKind.ActionEnd });
        }

        public void Stay(ref Entity entity, float frameTime)
        {
            Timer += frameTime;
        }
    }


    public class EFarmerState_Dead : IState
    {
        public string Name { get; protected set; } = StateType.Dead;

        public void Enter(Entity entity, float frameTime)
        {
        }

        public void Exit(ref Entity entity, float frameTime)
        {
        }

        public void Stay(ref Entity entity, float frameTime)
        {
        }
    }
}