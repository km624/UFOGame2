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

        Debug.Log("���������� �ε�");
        return data;
    }

    public async Task SavePlayerDataAsync(UserData data)
    {
        string path = GetSavePath(data.userId);

        // ����Ƽ �⺻ JsonUtility ���
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
               // Debug.Log("������ ���� ����: " + path);
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("������ ���� �� ���� �߻�: " + ex.Message);
        }
     
    }
}