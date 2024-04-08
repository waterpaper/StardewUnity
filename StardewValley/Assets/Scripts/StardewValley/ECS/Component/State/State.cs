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

        public void Enter(IFsmComponent entity, float frameTime)
        {
            entity.EventComponent.onEvent?.Invoke(StateType.Idle);
        }

        public void Exit(IFsmComponent entity, float frameTime)
        {
        }

        public void Stay(IFsmComponent entity, float frameTime)
        { }
    }

    public class EState_Run : IState
    {
        public string Name { get; protected set; } = StateType.Move;

        public void Enter(IFsmComponent entity, float frameTime)
        {
        }

        public void Exit(IFsmComponent entity, float frameTime)
        {
        }

        public void Stay(IFsmComponent entity, float frameTime)
        {
            entity.EventComponent.onEvent?.Invoke(StateType.Move);
        }
    }


    public class EFarmerState_Action : IState
    {
        public float Timer { get; protected set; } = 0;
        public string Name { get; protected set; } = StateType.Action;

        public void Enter(IFsmComponent entity, float frameTime)
        {
            var farmer = entity as FarmerEntity;
            farmer.MoveInputComponent.isEnable = false;
            farmer.InteractionInputComponent.isEnable = false;
            Timer = 0;
            entity.EventComponent.onEvent?.Invoke(StateType.Action);
        }

        public void Exit(IFsmComponent entity, float frameTime)
        {
            var farmer = entity as FarmerEntity;
            farmer.MoveInputComponent.isEnable = true;
            farmer.InteractionInputComponent.isEnable = true;
            Timer = 0;
            entity.EventComponent.onEvent?.Invoke("ActionEnd");
        }

        public void Stay(IFsmComponent entity, float frameTime)
        {
            Timer += frameTime;
        }
    }


    public class EFarmerState_Dead : IState
    {
        public string Name { get; protected set; } = StateType.Dead;

        public void Enter(IFsmComponent entity, float frameTime)
        {
        }

        public void Exit(IFsmComponent entity, float frameTime)
        {
        }

        public void Stay(IFsmComponent entity, float frameTime)
        {
        }
    }
}