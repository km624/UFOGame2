using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class UserDataLoadSave : IUserDataInterface
{
    private string GetSavePath(string userId)
    {
        return Path.Combine(Application.persistentDataPath, userId + "_userdata.json");
    }

    public async Task<UserData> LoadPlayerDataAsync(string userId)
    {
        string path = GetSavePath(userId);
        if (!File.Exists(path))
        {
            UserData newUserData = new UserData(userId);

            //기본 UFO 유저 데이터 추가 로직
            if (UFOLoadManager.Instance != null)
            {
                if(UFOLoadManager.Instance.LoadedUFODataList.Count == 0)
                {
                    Debug.Log("UFO 세팅 못함");
                }
                UFOData baseufodata = UFOLoadManager.Instance.LoadedUFODataList[0];

                UserUFOData baseuserufodata = new UserUFOData(baseufodata);
                newUserData.serialUFOList.AddUFO(baseuserufodata);

                if(AchievementManager.Instance != null)
                {
                    int tier = AchievementManager.Instance.ReadPointRewardDataList[0].PointRewardDatas.Count;
                    newUserData.userAchievePointData.InitTierList(tier);
                }
                   

                return newUserData;
            }

        }

       
        string json = await Task.Run(() => File.ReadAllText(path));

      
        UserData data = JsonUtility.FromJson<UserData>(json);

        //UFO 외형 직렬화
        data.InitializeUserData();

        Debug.Log("기존데이터 로드");
        return data;
    }

    public async Task SavePlayerDataAsync(UserData data)
    {
        string path = GetSavePath(data.userId);

   
        // 유니티 기본 JsonUtility 사용
        string json = JsonUtility.ToJson(data, prettyPrint: true);
      
        try
        {
            await Task.Run(() =>
            {
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                   
                }
               
                //Debug.Log("데이터 저장 성공: " + path);
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("데이터 저장 중 오류 발생: " + ex.Message);
        }
     
    }
}