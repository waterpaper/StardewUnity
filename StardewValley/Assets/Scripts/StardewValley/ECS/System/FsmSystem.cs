using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// fsm을 처리한다.
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    [UpdateBefore(typeof(PhysicsSystem))]
    [UpdateAfter(typeof(PhysicsCollisionSystem))]
    public partial class FsmSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<NormalSystemsCompTag>();
        }
        protected override void OnUpdate()
        {
            float frameTime = SystemAPI.Time.DeltaTime;

            Entities
                .WithAll<FsmComponent>()
                .ForEach(
                    (Entity entity, FsmComponent fsmComponent) =>
                    {
                        FSMUpdate(entity, fsmComponent, (float)frameTime);
                    }
                )
                .WithoutBurst().Run();
        }


        private void FSMUpdate(Entity entity, FsmComponent fsmComponent, float frameTime)
        {
            if (!fsmComponent.StateMap.TryGetValue(fsmComponent.State, out var stateData))
                return;

            if (fsmComponent.NextState != fsmComponent.State)
            {
                fsmComponent.State = fsmComponent.NextState;
                fsmComponent.States[fsmComponent.NextState].Enter(entity, (float)frameTime);
            }

            var nextState = TryTransition(ref entity, stateData);
            if (nextState == null)
            {
                if (fsmComponent.State != null)
                    fsmComponent.States[fsmComponent.State].Stay(ref entity, (float)frameTime);
                return;
            }


            if (fsmComponent.State != null)
                fsmComponent.States[fsmComponent.State].Exit(ref entity, (float)frameTime);

            fsmComponent.IsChanged = true;
            fsmComponent.NextState = nextState;
        }


        private string TryTransition(ref Entity entity, StateTransitions stateTransitions)
        {
            string nextState = null;
            foreach (var changer in stateTransitions.Transitions)
            {
                if (!changer.condition(ref entity))
                    continue;

                nextState = changer.target;
                break;
            }

            return nextState;
        }
    }
}