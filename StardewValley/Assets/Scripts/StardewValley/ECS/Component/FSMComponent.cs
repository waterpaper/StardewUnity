using System.Collections.Generic;

namespace WATP.ECS
{
    public delegate bool FsmTransitionCondition();
    public interface IState
    {
        public string Name { get; }

        public void Enter(IFsmComponent entity, float frameTime);
        public void Stay(IFsmComponent entity, float frameTime);
        public void Exit(IFsmComponent entity, float frameTime);
    }

    internal class StateTransitions
    {
        public readonly List<(string target, FsmTransitionCondition condition)> Transitions;

        public StateTransitions()
        {
            Transitions = new();
        }
    }

    public interface IFsmComponent : IStateComponent
    {
    }

    public class FsmComponent : StateComponent
    {
        public string NextState { get; set; } = "default";
        public bool IsChanged { get; set; } = false;
        internal IReadOnlyDictionary<string, IState> States { get; set; }
        internal IReadOnlyDictionary<string, StateTransitions> StateMap { get; set; }

        public class FsmComponentBuilder
        {
            private readonly Dictionary<string, IState> states = new();
            private readonly Dictionary<string, StateTransitions> stateMap = new();

            private string currentBuildState = string.Empty;
            internal string DefaultState { get; private set; } = string.Empty;

            /// <summary>
            /// </summary>
            public FsmComponentBuilder SetDefaultState(string state)
            {
                if (state != string.Empty)
                    DefaultState = state;

                return this;
            }

            /// <summary>
            /// 없으면 생성, 있으면 <see cref="AddTransition"/>를 위해 가져옵니다.
            /// </summary>
            public FsmComponentBuilder AddState(IState state)
            {
                states.Add(state.Name, state);
                stateMap.Add(state.Name, new StateTransitions());

                currentBuildState = state.Name;
                if (DefaultState == string.Empty)
                    DefaultState = state.Name;

                return this;
            }

            /// <summary>
            /// 상태 전이 조건을 추가합니다.
            /// </summary>
            /// <param name="from">전이 조건을 검사할 상태 입니다.</param>
            /// <param name="to">전이 조건이 맞으면 변경될 상태 입니다.</param>
            /// <param name="predicate">전이 조건</param>
            /// <returns></returns>
            public FsmComponentBuilder AddTransition(string from, string to, FsmTransitionCondition condition)
            {
                stateMap[from].Transitions.Add((to, condition));

                return this;
            }

            /// <summary>
            /// 상태 전이 조건을 추가합니다.<br/>
            /// <see cref="State"/>로 지정한 상태에서만 사용 가능합니다.
            /// </summary>
            /// <param name="to">전이 조건이 맞으면 변경될 상태 입니다.</param>
            /// <param name="predicate">전이 조건</param>
            /// <returns></returns>
            public FsmComponentBuilder AddTransition(string to, FsmTransitionCondition condition)
            {
                if (currentBuildState != null)
                {
                    stateMap[currentBuildState].Transitions.Add((to, condition));
                }

                return this;
            }

            /// <summary>
            /// FSM을 생성합니다.
            /// </summary>
            /// <returns></returns>
            public FsmComponent Build(IFsmComponent entity)
            {
                FsmComponent component = new()
                {
                    States = states,
                    StateMap = stateMap,
                    State = DefaultState,
                    NextState = DefaultState
                };

                component.States[DefaultState].Enter(entity, 0);
                return component;
            }
        }

    }
}