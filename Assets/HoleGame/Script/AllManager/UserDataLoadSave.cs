using System;
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
           
            return new UserData(userId);
        }

       
        string json = await Task.Run(() => File.ReadAllText(path));

      
        UserData data = JsonUtility.FromJson<UserData>(json);

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
                //Debug.Log(data.stageClearTimes[0]);
               // Debug.Log("데이터 저장 성공: " + path);
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("데이터 저장 중 오류 발생: " + ex.Message);
        }
     
    }
}