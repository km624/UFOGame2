//using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GameManager Instance { get; private set; }

    //���൵
    public float progress { get; private set; }

    public int SelectLevel { get; private set; }  = 0;

 
   // [SerializeField]
    //private StageManager stageManager;
    public  UserData userData { get; private set; }
    public IUserDataInterface userDatasaveload { get; private set; }

    public VibrationManager vibrationManager { get; private set; }

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
            vibrationManager = GetComponent<VibrationManager>();
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }

        userDatasaveload = new UserDataLoadSave();

       

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    private  void Start()
    {

    }
   

    public async Task InitData()
    {
        progress = 0f;


        await UFOLoadManager.Instance.LoadAppearanceData();
       

        progress = 0.3f;  // 30%


        
        userData = await userDatasaveload.LoadPlayerDataAsync("Guest");

        UFOLoadManager.Instance.SetSelectUFODATA(userData.CurrentUFO);
        progress = 1.0f;  // 100%

        //Debug.Log("InitData �Ϸ�! progress = 1.0f");

    }

    public async Task InitData2()
    {
        progress = 0f;

        // [1] ���� ������ �ε�
        await UFOLoadManager.Instance.LoadAppearanceData();
        
        progress = 0.3f;  // 30%
       
        // ����ũ ������
        await SimulateDelay(progress, 0.5f, 0.6f);

        userData = await userDatasaveload.LoadPlayerDataAsync("Guest");

        UFOLoadManager.Instance.SetSelectUFODATA(userData.CurrentUFO);
        progress = 1.0f; 

        //  ������ ����ũ ������
        await SimulateDelay(progress, 1.0f, 1.0f);

        Debug.Log("InitData �Ϸ�! progress = 1.0f");
    }

    private async Task SimulateDelay(float startValue, float currentValue, float targetValue, float duration = 1.0f)
    {
        // duration �ð� ����, ���� progress�� startValue �� targetValue �� ������ ����
        float elapsed = 0f;
        float initial = currentValue;  // ���� ���൵

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            progress = Mathf.Lerp(initial, targetValue, t);
            await Task.Yield();  // ���� �����ӱ��� ���
        }

        progress = targetValue; // ���� �� ����
    }

    

    public async void SaveUserData()
    {
       await userDatasaveload.SavePlayerDataAsync(userData);
        Debug.Log("����Ϸ� : " + Time.time);
    }

  
}
