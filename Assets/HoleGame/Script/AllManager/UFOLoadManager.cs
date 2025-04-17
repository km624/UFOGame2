using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;



public class UFOLoadManager : MonoBehaviour
{

    public static UFOLoadManager Instance;

    [SerializeField]
    private UFOAllData AllUfodataList;

    // ĳ��: Ű�� appearanceID, ���� �ε�� SpaceshipAppearanceData
    //private Dictionary<string, UFOData> appearanceCache = new Dictionary<string, UFOData>();

    public UFOData selectUFOData { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
           
            Destroy(gameObject);
        }
    }

    public List<UFOData> LoadedUFODataList { get; private set; } = new List<UFOData>();

   
    public void UnloadAllStageData()
    {
        foreach (var stage in LoadedUFODataList)
        {
            Addressables.Release(stage);
        }
        LoadedUFODataList.Clear();
        
    }

    public  void SetSelectUFODATA(int selectnum)
    {
        if(selectnum < LoadedUFODataList.Count)
        {
            selectUFOData = LoadedUFODataList[selectnum];
            GameManager.Instance.userData.CurrentUFO = selectnum;

            //GameManager.Instance.userDatasaveload.SavePlayerDataAsync()
        }
      
    }

    // �񵿱� �ε� �Լ�
    public async Task LoadAppearanceData()
    {
        if (AllUfodataList == null) return;

        foreach(var ufodata in AllUfodataList.UFOAllDataList)
        {
             AsyncOperationHandle<UFOData> ufohandle = ufodata.LoadAssetAsync<UFOData>();
            await ufohandle.Task;

            if (ufohandle.Status == AsyncOperationStatus.Succeeded)
            {
                UFOData ufo = ufohandle.Result;
                LoadedUFODataList.Add(ufo);
              
                Debug.Log(ufo.UFOName + "�ε� �Ϸ�");
            }
            else
            {
                Debug.LogError("UFO  �ε� ���� :  " + ufodata.ToString());
            }

       
        }
    }
}
