
using UnityEngine;

namespace WATP
{
    public enum LogicState
    {
        None,
        Init,
        Loading,
        Normal,
        Parse,
        Last
    }

    public partial class GameState
    {
        public SubjectData<LogicState> logicState;
    }
}