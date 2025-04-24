
using UnityEngine;
using UnityEngine.UI;

public class SettingWidget : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button vibrationButton;
    [SerializeField] private Button bgmButton;
    [SerializeField] private Button sfxButton;

    [Header("컬러")]
    [SerializeField] private Color onColor = Color.white;
    [SerializeField] private Color offColor = Color.gray;

    UserSettingData userSettingData;
    public void Start()
    {
        if(GameManager.Instance != null)
        {
            userSettingData =new UserSettingData();
            if(GameManager.Instance.userData != null)
                userSettingData = GameManager.Instance.userData.userSettingData;

            Initialize(userSettingData);
        }
    }
    public void Initialize(UserSettingData userSettings)
    {
        userSettingData = userSettings;

        UpdateAllButtonStates();

        vibrationButton.onClick.AddListener(ToggleVibration);
        bgmButton.onClick.AddListener(ToggleBgm);
        sfxButton.onClick.AddListener(ToggleSfx);
    }

    private void UpdateAllButtonStates()
    {
        SetButtonColor(vibrationButton, userSettingData.bIsVibration);
        SetButtonColor(bgmButton, userSettingData.bIsBgm);
        SetButtonColor(sfxButton, userSettingData.bIsSfx);
    }

    private void SetButtonColor(Button button, bool isOn)
    {
        if (button.TryGetComponent(out Image img))
        {
            img.color = isOn ? onColor : offColor;
        }
    }

    private void ToggleVibration()
    {
        userSettingData.bIsVibration = !userSettingData.bIsVibration;
        SetButtonColor(vibrationButton, userSettingData.bIsVibration);
        if(GameManager.Instance!=null)
        {
            GameManager.Instance.vibrationManager.OnVibration(userSettingData.bIsVibration);
        }
    }

    private void ToggleBgm()
    {
        userSettingData.bIsBgm = !userSettingData.bIsBgm;
        SetButtonColor(bgmButton, userSettingData.bIsBgm);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.soundManager.SetBgmMute(!userSettingData.bIsBgm);   
        }
    }

    private void ToggleSfx()
    {
        userSettingData.bIsSfx = !userSettingData.bIsSfx;
        SetButtonColor(sfxButton, userSettingData.bIsSfx);
        GameManager.Instance.soundManager.SetSfxMute(!userSettingData.bIsSfx);
    }

    public void SaveUserdata()
    {
        if(GameManager.Instance.userData!=null)
        {
            GameManager.Instance.SaveUserData();
        }
    }
}
