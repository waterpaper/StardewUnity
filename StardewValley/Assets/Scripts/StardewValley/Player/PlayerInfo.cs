using UnityEngine;

namespace WATP.Player
{
    /// <summary>
    /// player 정보 데이터
    /// </summary>
    public class PlayerInfo
    {
        public SubjectData<string> name;                             //이름
        public SubjectData<string> farmName;                         //농장이름

        public SubjectData<int> hairIndex;                           //머리 인덱스
        public SubjectData<int> clothsIndex;                         //옷 인덱스
        public SubjectData<Vector3> hairColor;                       //머리 색
        public SubjectData<Vector3> clothsColor;                     //옷 색


        public SubjectData<int> hp;                                  //체력
        public SubjectData<int> maxHp;                               //체력최대
        public SubjectData<int> actingPower;                         //행동력
        public SubjectData<int> actingPowerMax;                      //행동력최대
        public SubjectData<int> money;                               //현재 금액
    }
}

