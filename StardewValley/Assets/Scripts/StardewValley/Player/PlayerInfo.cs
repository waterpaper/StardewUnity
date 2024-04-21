using UnityEngine;

namespace WATP.Player
{
    /// <summary>
    /// player ���� ������
    /// </summary>
    public class PlayerInfo
    {
        public SubjectData<string> name;                             //�̸�
        public SubjectData<string> farmName;                         //�����̸�

        public SubjectData<int> hairIndex;                           //�Ӹ� �ε���
        public SubjectData<int> clothsIndex;                         //�� �ε���
        public SubjectData<Vector3> hairColor;                       //�Ӹ� ��
        public SubjectData<Vector3> clothsColor;                     //�� ��


        public SubjectData<int> hp;                                  //ü��
        public SubjectData<int> maxHp;                               //ü���ִ�
        public SubjectData<int> actingPower;                         //�ൿ��
        public SubjectData<int> actingPowerMax;                      //�ൿ���ִ�
        public SubjectData<int> money;                               //���� �ݾ�
    }
}

