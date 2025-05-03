using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }
    [SerializeField] private List<AchievementData> achievementDataList;
    
    private UserData userData;

    // 이중 딕셔너리 + 진행도 사전
    private Dictionary<AchieveEnum, Dictionary<string, AchievementData>> achievementDict;
    private Dictionary<string, UserAchieveData> progressDict;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

  
    }

    public void InitializeManager(UserData user)
    {
        userData = user;
        InitializeDictionaries();
        SyncProgressWithUserData();

        Debug.Log("업적 초기화 완료  : " + progressDict.Count);
    }

    private void InitializeDictionaries()
    {
        achievementDict = new Dictionary<AchieveEnum, Dictionary<string, AchievementData>>();
        progressDict = new Dictionary<string, UserAchieveData>();

        foreach (var data in achievementDataList)
        {
            if (!achievementDict.TryGetValue(data.AchieveType, out var sub))
            {
                sub = new Dictionary<string, AchievementData>();
                achievementDict[data.AchieveType] = sub;
            }
            sub[data.Id] = data;

        }

    }

    private void SyncProgressWithUserData()
    {
        //Debug.Log($"[Achievement Sync] 유저 저장된 업적 수: {userData.Achievements.Count}");

        // 1) 유저 데이터 기준으로 덮어쓰기
        foreach (var saved in userData.Achievements)
        {
            progressDict[saved.Id] = saved;
            //Debug.Log($"[Sync] 유저 데이터 업적 동기화: {saved.Id}");
        }

        // 2) 새로 추가된 업적이 있으면 progressDict + userData.Achievements 에도 추가
        foreach (var data in achievementDataList)
        {
            if (!progressDict.ContainsKey(data.Id))
            {
                var newData = new UserAchieveData
                {
                    Id = data.Id,
                    CurrentValue = 0,
                    CurrentTierIndex = 0,
                    IsCompleted = false
                };

                progressDict[data.Id] = newData;
                userData.Achievements.Add(newData);

                //Debug.Log($"[Sync] 새 업적 추가됨: {data.Id}");
            }
        }

       // Debug.Log($"[Achievement Sync] 최종 업적 수: {userData.Achievements.Count}");
    }

    public void ReportProgress(AchieveEnum type, string id, int amount)
    {
        // 1) 타입별 딕셔너리에서 해당 리스트 가져오기
        if (!achievementDict.TryGetValue(type, out var subDict))
        {
            Debug.LogWarning($"[Achievement] ReportProgress 실패: 존재하지 않는 타입 '{type}'");
            return;
        }

        // 2) ID로 업적 정의 가져오기
        if (!subDict.TryGetValue(id, out var data))
        {
            Debug.LogWarning($"[Achievement] ReportProgress 실패: 타입 '{type}'에 ID '{id}' 업적이 없습니다.");
            return;
        }

        // 3) 진행도 가져오기
        var prog = progressDict[data.Id];
        if (prog.IsCompleted)
        {
            Debug.Log($"[Achievement] ReportProgress 무시: 이미 완료된 업적 '{id}'");
            return;
        }

        // 4) 값 누적
        prog.CurrentValue += amount;
        Debug.Log($"[Achievement] '{id}' 진행도 갱신: {prog.CurrentValue}/{data.TiersList[prog.CurrentTierIndex].TargetValue}");


        //위젯에서 버튼 누르면 체크 후 달성 후 하기 
        //함수로 따로 빼놔야 할듯 위젯이랑 연동

        // 5) 티어 달성 체크 (while문으로 다단계 자동 전환)
        
       /* while (prog.CurrentTierIndex < data.TiersList.Count &&
               prog.CurrentValue >= data.TiersList[prog.CurrentTierIndex].TargetValue)
        {
            // 보상 지급 (Tier별 Rewardcnt 사용)
            int rewardCount = data.TiersList[prog.CurrentTierIndex].Rewardcnt;
            //GiveReward(rewardCount);
            Debug.Log($"[Achievement] '{id}' 티어 {prog.CurrentTierIndex} 달성! 보상 x{rewardCount}");

            prog.CurrentTierIndex++;

            // 최종 티어까지 모두 달성했으면 완료 처리
            if (prog.CurrentTierIndex >= data.TiersList.Count)
            {
                prog.IsCompleted = true;
                Debug.Log($"[Achievement] '{id}' 모든 티어 완료!");
                break;
            }
            else
            {
                Debug.Log($"[Achievement] '{id}' 다음 티어로 이동: 목표 {data.TiersList[prog.CurrentTierIndex].TargetValue}");
            }
        }
*/
    
    }

}
