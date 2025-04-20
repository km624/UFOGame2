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

    //�б� ����
    public  IReadOnlyList<StageData> GetstageDataList => stageDataList.allStageData;




    public async Task LoadStage(int stageIndex)
    {
        // Ȥ�� ���� ���������� ������ ����
        if (currentStageInstance != null)
        {
            Destroy(currentStageInstance);
            currentStageInstance = null;
        }

        if(stageIndex == currentstageindex)
        {
            Debug.Log("�̸̹� �ε�������");
            return;
        }

        //�ε� �ʱ�ȭ
        UnloadStage();

        // stageDataList���� �ش� �ε����� StageData ��������
        if (stageIndex < 0 || stageIndex >= stageDataList.allStageData.Count)
        {
            Debug.LogError("Invalid stage index: " + stageIndex);
            return;
        }
        currentstageindex = stageIndex;
        StageData stageData = stageDataList.allStageData[currentstageindex];

        // Addressables�� ������ �ε�
        Stagehandle =
            stageData.stagePrefab.LoadAssetAsync<GameObject>();
       
        await Stagehandle.Task;

        if (Stagehandle.Status == AsyncOperationStatus.Succeeded)
        {
           
             prefab = Stagehandle.Result;
            //currentStageInstance = Instantiate(prefab);
           // Debug.Log("level ����");
           Debug.Log(currentstageindex + "�ε� �Ϸ�");
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
            Stagehandle = default; // Handle �ʱ�ȭ
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
        Debug.Log("��ε� �Ϸ�");
    }

    // ��: ���������� Ŭ�������� ��
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
            // �� ���� �ð��̸� ���� �� ���� ����
            if (clearTime > userData.stageClearTimes[index])
            {
                userData.stageClearTimes[index] = clearTime;
            }

            if(star > userData.stageStars[index]) 
            { 
                userData.stageStars[index] = star; 
            }
           // Debug.Log("������ ����");
        }

        // Ŭ���� ������ ����
        if(userData.currentClearIndex < index )
        userData.currentClearIndex = index;*/
       /* if(GameManager.Instance != null)
        {
            await GameManager.Instance.userDatasaveload.SavePlayerDataAsync(userData);
        }
        else
        {
            Debug.Log("���� ����");
        }*/


}

   

