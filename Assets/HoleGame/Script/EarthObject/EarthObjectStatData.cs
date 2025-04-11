using UnityEngine;

[CreateAssetMenu(fileName = "EarthObjectStatData", menuName = "Scriptable Objects/EarthObjectStatData")]
public class EarthObjectStatData : ScriptableObject
{
    
    //[Header("�⺻������")]
   // public bool bisdefaultStat = false;
    [Header("����ġ��")]
    public float EXPCnt = 5.0f;
    [Header("�߰� �ð�")]
    public float TimeCnt = 0.5f;
   // [Header("Ŭ���� ��ǥ")]
    //public bool bRequired = false;
    [Header("�̵��ϴ���")]
    public bool bMovement = false;
    [Header("�����Ÿ�")]
    public float jumpDistance = 1.0f;
    [Header("��׷����� ����")]
    public float squishAmount = 0.2f;

   //[Header("����")]
    public float mass = 1.0f;


}
