using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using System.Collections.Generic;



public class StageManager : MonoBehaviour
{
    /*[SerializeField]
    private AllStageData stageDataList;

    private int currentstageindex=-1;

    private AsyncOperationHandle<GameObject> Stagehandle;
    private GameObject currentStageInstance;
    private GameObject prefab;
    //private IUserDataInterface userDatasaveload;
    private UserData userData;



    public void SetUserData(UserData Data)
    {
        userData = Data;
    }

    //읽기 전용
    public  IReadOnlyList<StageData> GetstageDataList => stageDataList.allStageData;




    public async Task LoadStage(int stageIndex)
    {
        // 혹시 기존 스테이지가 있으면 제거
        if (currentStageInstance != null)
        {
            Destroy(currentStageInstance);
            currentStageInstance = null;
        }

        if(stageIndex == currentstageindex)
        {
            Debug.Log("이미맵 로딩되있음");
            return;
        }

        //로딩 초기화
        UnloadStage();

        // stageDataList에서 해당 인덱스의 StageData 가져오기
        if (stageIndex < 0 || stageIndex >= stageDataList.allStageData.Count)
        {
            Debug.LogError("Invalid stage index: " + stageIndex);
            return;
        }
        currentstageindex = stageIndex;
        StageData stageData = stageDataList.allStageData[currentstageindex];

        // Addressables로 프리팹 로드
        Stagehandle =
            stageData.stagePrefab.LoadAssetAsync<GameObject>();
       
        await Stagehandle.Task;

        if (Stagehandle.Status == AsyncOperationStatus.Succeeded)
        {
           
             prefab = Stagehandle.Result;
            //currentStageInstance = Instantiate(prefab);
           // Debug.Log("level 생성");
           Debug.Log(currentstageindex + "로딩 완료");
        }
        else
        {
            Debug.LogError("Failed to load stage prefab: " + stageData.stagePrefab);
        }
    }

    public void SapwnInstanceStage()
    {
        if (prefab != null)
        {
            currentStageInstance = Instantiate(prefab);
        }
    }

    public void UnloadStage()
    {
        if (Stagehandle.IsValid())
        {
            Addressables.Release(Stagehandle);
            Stagehandle = default; // Handle 초기화
        }

        if (prefab != null)
        {
            
            prefab = null;
        }

        if(currentStageInstance!=null)
        {
            //Destroy(currentStageInstance);
            currentStageInstance = null;
        }

        currentstageindex = -1;
        Debug.Log("언로드 완료");
    }

    // 예: 스테이지를 클리어했을 때
    public void OnStageClear(int stageindex ,int clearTime,int star)
    {*/
        /*int index = stageindex;

        if (index < 0 || index >= userData.stageClearTimes.Count)
        {
            userData.stageClearTimes.Add(clearTime);
            userData.stageStars.Add(star);
           

        }
        else
        {
            // 더 빠른 시간이면 갱신 등 로직 가능
            if (clearTime > userData.stageClearTimes[index])
            {
                userData.stageClearTimes[index] = clearTime;
            }

            if(star > userData.stageStars[index]) 
            { 
                userData.stageStars[index] = star; 
            }
           // Debug.Log("데이터 갱신");
        }

        // 클리어 데이터 저장
        if(userData.currentClearIndex < index )
        userData.currentClearIndex = index;*/
       /* if(GameManager.Instance != null)
        {
            await GameManager.Instance.userDatasaveload.SavePlayerDataAsync(userData);
        }
        else
        {
            Debug.Log("저장 에러");
        }*/


}

   

