using UnityEngine;

[CreateAssetMenu(fileName = "EarthObjectStatData", menuName = "Scriptable Objects/EarthObjectStatData")]
public class EarthObjectStatData : ScriptableObject
{
    
    //[Header("기본값인지")]
   // public bool bisdefaultStat = false;
    [Header("경험치량")]
    public float EXPCnt = 5.0f;
    [Header("추가 시간")]
    public float TimeCnt = 0.5f;
   // [Header("클리어 목표")]
    //public bool bRequired = false;
    [Header("이동하는지")]
    public bool bMovement = false;
    [Header("점프거리")]
    public float jumpDistance = 1.0f;
    [Header("찌그러지는 정도")]
    public float squishAmount = 0.2f;

   //[Header("질량")]
    public float mass = 1.0f;


}
