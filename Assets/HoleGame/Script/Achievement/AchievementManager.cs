using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }
    [SerializeField] private List<AchievementData> achievementDataList;
    
    private UserData userData;

    // ���� ��ųʸ� + ���൵ ����
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

        Debug.Log("���� �ʱ�ȭ �Ϸ�  : " + progressDict.Count);
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
        //Debug.Log($"[Achievement Sync] ���� ����� ���� ��: {userData.Achievements.Count}");

        // 1) ���� ������ �������� �����
        foreach (var saved in userData.Achievements)
        {
            progressDict[saved.Id] = saved;
            //Debug.Log($"[Sync] ���� ������ ���� ����ȭ: {saved.Id}");
        }

        // 2) ���� �߰��� ������ ������ progressDict + userData.Achievements ���� �߰�
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

                //Debug.Log($"[Sync] �� ���� �߰���: {data.Id}");
            }
        }

       // Debug.Log($"[Achievement Sync] ���� ���� ��: {userData.Achievements.Count}");
    }

    public void ReportProgress(AchieveEnum type, string id, int amount)
    {
        // 1) Ÿ�Ժ� ��ųʸ����� �ش� ����Ʈ ��������
        if (!achievementDict.TryGetValue(type, out var subDict))
        {
            Debug.LogWarning($"[Achievement] ReportProgress ����: �������� �ʴ� Ÿ�� '{type}'");
            return;
        }

        // 2) ID�� ���� ���� ��������
        if (!subDict.TryGetValue(id, out var data))
        {
            Debug.LogWarning($"[Achievement] ReportProgress ����: Ÿ�� '{type}'�� ID '{id}' ������ �����ϴ�.");
            return;
        }

        // 3) ���൵ ��������
        var prog = progressDict[data.Id];
        if (prog.IsCompleted)
        {
            Debug.Log($"[Achievement] ReportProgress ����: �̹� �Ϸ�� ���� '{id}'");
            return;
        }

        // 4) �� ����
        prog.CurrentValue += amount;
        Debug.Log($"[Achievement] '{id}' ���൵ ����: {prog.CurrentValue}/{data.TiersList[prog.CurrentTierIndex].TargetValue}");


        //�������� ��ư ������ üũ �� �޼� �� �ϱ� 
        //�Լ��� ���� ������ �ҵ� �����̶� ����

        // 5) Ƽ�� �޼� üũ (while������ �ٴܰ� �ڵ� ��ȯ)
        
       /* while (prog.CurrentTierIndex < data.TiersList.Count &&
               prog.CurrentValue >= data.TiersList[prog.CurrentTierIndex].TargetValue)
        {
            // ���� ���� (Tier�� Rewardcnt ���)
            int rewardCount = data.TiersList[prog.CurrentTierIndex].Rewardcnt;
            //GiveReward(rewardCount);
            Debug.Log($"[Achievement] '{id}' Ƽ�� {prog.CurrentTierIndex} �޼�! ���� x{rewardCount}");

            prog.CurrentTierIndex++;

            // ���� Ƽ����� ��� �޼������� �Ϸ� ó��
            if (prog.CurrentTierIndex >= data.TiersList.Count)
            {
                prog.IsCompleted = true;
                Debug.Log($"[Achievement] '{id}' ��� Ƽ�� �Ϸ�!");
                break;
            }
            else
            {
                Debug.Log($"[Achievement] '{id}' ���� Ƽ��� �̵�: ��ǥ {data.TiersList[prog.CurrentTierIndex].TargetValue}");
            }
        }
*/
    
    }

}
