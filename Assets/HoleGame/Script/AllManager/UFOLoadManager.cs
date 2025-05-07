using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;



public class UFOLoadManager : MonoBehaviour
{

    public static UFOLoadManager Instance;

    [SerializeField]
    private UFOAllData AllUfodataList;

    private Dictionary<string, UFOData> loadedUFODataDic = new Dictionary<string, UFOData>();
    public IReadOnlyDictionary<string, UFOData> ReadLoadedUFODataDic => loadedUFODataDic;
    //public List<UFOData> LoadedUFODataList { get; private set; } = new List<UFOData>();
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

    

   
    public void UnloadAllStageData()
    {
        foreach (var stage in loadedUFODataDic)
        {
            Addressables.Release(stage);
        }
        loadedUFODataDic.Clear();
        
    }

    public  void SetSelectUFODATA(string ufoname)
    {
        if(loadedUFODataDic.ContainsKey(ufoname))
        {
            selectUFOData = loadedUFODataDic[ufoname];

            GameManager.Instance.userData.SetCurrentUFO(ufoname);

            //GameManager.Instance.userDatasaveload.SavePlayerDataAsync()
        }
      
    }

    // 비동기 로드 함수
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
                loadedUFODataDic.Add(ufo.UFOName, ufo);
              
                Debug.Log(ufo.UFOName + "로딩 완료");
            }
            else
            {
                Debug.LogError("UFO  로딩 실패 :  " + ufodata.ToString());
            }

       
        }
    }
}
