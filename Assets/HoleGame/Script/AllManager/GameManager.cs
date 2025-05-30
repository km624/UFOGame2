//using System.Net.NetworkInformation;
using UnityEngine;
using System.Threading.Tasks;


public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    //진행도
    public float progress { get; private set; }

    public int SelectLevel { get; private set; }  = 0;

 
   // [SerializeField]
    //private StageManager stageManager;
    public  UserData userData { get; private set; }
    public IUserDataInterface userDatasaveload { get; private set; }

    public VibrationManager vibrationManager { get; private set; }

    public SoundManager soundManager { get; private set; }

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            vibrationManager = GetComponent<VibrationManager>();
            soundManager = GetComponent<SoundManager>();

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

        UFOLoadManager.Instance.SetSelectUFODATA(userData.SelectUFOName);
        progress = 0.6f;  // 100%

        AchievementManager.Instance.InitializeManager(userData);

        progress = 1.0f;

        Debug.Log("TEST Init 완료");
    }

    public async Task InitData2()
    {
        progress = 0f;

        // [1] 유저 데이터 로드
        await UFOLoadManager.Instance.LoadAppearanceData();
        progress = 0.3f;  
        await SimulateDelay(progress, 0.3f, 0.4f);

        //UFO 데이터 로드
        userData = await userDatasaveload.LoadPlayerDataAsync("Guest");
        UFOLoadManager.Instance.SetSelectUFODATA(userData.SelectUFOName);
        progress = 6.0f;
        await SimulateDelay(progress, 0.6f, 0.7f);

        //업적 로드
        AchievementManager.Instance.InitializeManager(userData);
        progress = 1.0f; 
        await SimulateDelay(progress, 1.0f, 1.0f);

        Debug.Log("InitData 완료! progress = 1.0f");
    }

    private async Task SimulateDelay(float startValue, float currentValue, float targetValue, float duration = 1.0f)
    {
        // duration 시간 동안, 현재 progress를 startValue → targetValue 로 서서히 증가
        float elapsed = 0f;
        float initial = currentValue;  // 시작 진행도

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            progress = Mathf.Lerp(initial, targetValue, t);
            await Task.Yield();  // 다음 프레임까지 대기
        }

        progress = targetValue; // 최종 값 고정
    }

    

    public async void SaveUserData()
    {
       await userDatasaveload.SavePlayerDataAsync(userData);
        Debug.Log("저장완료 : " + Time.time);
    }

  
}
