using System.Collections.Generic;

namespace WATP.ECS
{
    /// <summary>
    /// 상태 처리 로직
    /// </summary>
    public class StateService : IService
    {
        public List<IStateComponent> stateComponents = new();

        public void Add(IEntity entity)
        {
            if (entity is not IStateComponent) return;

            stateComponents.Add(entity as IStateComponent);
        }

        public void Remove(IEntity entity)
        {
            if (entity is not IStateComponent) return;

            stateComponents.Remove(entity as IStateComponent);
        }

        public void Clear()
        {
            stateComponents.Clear();
        }

        public void Update(double frameTime)
        {
            foreach (var stateComponent in stateComponents)
            {
                if (stateComponent is IFsmComponent)
                {
                    FSMUpdate(stateComponent as IFsmComponent, (float)frameTime);
                }
            }
        }


        private void FSMUpdate(IFsmComponent fsmEntity, float frameTime)
        {
            var component = fsmEntity.StateComponent as FsmComponent;
            if (!component.StateMap.TryGetValue(component.State, out var stateData))
                return;

            if (component.NextState != component.State)
            {
                component.State = component.NextState;
                component.States[component.NextState].Enter(fsmEntity, (float)frameTime);
            }

            var nextState = TryTransition(stateData);
            if (nextState == null)
            {
                if (component.State != null)
                    component.States[component.State].Stay(fsmEntity, (float)frameTime);
                return;
            }


            if (component.State != null)
                component.States[component.State].Exit(fsmEntity, (float)frameTime);

            component.IsChanged = true;
            component.NextState = nextState;
        }


        private string TryTransition(StateTransitions stateTransitions)
        {
            string nextState = null;
            foreach (var changer in stateTransitions.Transitions)
            {
                if (!changer.condition())
                    continue;

                nextState = changer.target;
                break;
            }

            return nextState;
        }
    }
}