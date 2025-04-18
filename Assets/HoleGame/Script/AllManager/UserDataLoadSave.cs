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

            //�⺻ UFO ���� ������ �߰� ����
            if (UFOLoadManager.Instance != null)
            {
                if(UFOLoadManager.Instance.LoadedUFODataList.Count ==0)
                {
                    Debug.Log("UFO ���� ����");
                }
                UFOData baseufodata = UFOLoadManager.Instance.LoadedUFODataList[0];

                UserUFOData baseuserufodata = new UserUFOData(baseufodata);
                newUserData.serialUFOList.AddUFO(baseuserufodata);
                return newUserData;
            }

        }

       
        string json = await Task.Run(() => File.ReadAllText(path));

      
        UserData data = JsonUtility.FromJson<UserData>(json);

        //UFO ���� ����ȭ
        data.InitializeUserData();

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