namespace WATP.ECS
{
    [System.Serializable]
    public class AdditionalComponent : IComponent
    {
        protected float hpRate;
        protected float attackRate;
        protected float attackSpeedRate;
        protected float moveSpeedRate;

        protected int hpValue;
        protected int attackValue;
        protected int attackSpeedValue;
        protected int moveSpeedValue;

        protected int notMoveCount;
        protected int stunCount;


        public float HpRate { get => hpRate; set { hpRate = value;  } }
        public float AttackRate { get => attackRate; set { attackRate = value;  } }
        public float AttackSpeedRate { get => attackSpeedRate; set { attackSpeedRate = value;  } }
        public float MoveSpeedRate { get => moveSpeedRate; set { moveSpeedRate = value;  } }


        public int HpValue { get => hpValue; set { hpValue = value; } }
        public int AttackValue { get => attackValue; set { attackValue = value; } }
        public int AttackSpeedValue { get => attackSpeedValue; set { attackSpeedValue = value; } }
        public int MoveSpeedValue { get => moveSpeedValue; set { moveSpeedValue = value; } }

        public int NotMoveCount { get => notMoveCount; set { notMoveCount = value; } }
        public int StunCount { get => stunCount; set { stunCount = value;  } }


        public AdditionalComponent() { }

        public AdditionalComponent(AdditionalComponent clone) 
        { 
            hpRate = clone.hpRate;
            attackRate = clone.attackRate;
            attackSpeedRate = clone.attackSpeedRate;
            moveSpeedRate = clone.moveSpeedRate;

            hpValue = clone.hpValue;
            attackValue = clone.attackValue;
            attackSpeedValue = clone.attackSpeedValue;
            moveSpeedValue = clone.moveSpeedValue;

            stunCount = clone.stunCount;
        }
    }
}