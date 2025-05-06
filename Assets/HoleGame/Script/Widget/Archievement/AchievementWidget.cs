using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class AchievementWidget : MonoBehaviour
{
    private AllAchievementWidget allArchievementWidget;

    public AchieveEnum achiveType { get; private set; }

    public string achiveID { get; private set; }
    //public int currentreward { get; private set; }

    //public int currentTier { get; private set; }

    public int Progress { get; private set; }
    public int Target { get; private set; }

    public int Reward { get; private set; }

    public bool bisCleared { get; private set; }

    [SerializeField] private Image BackGroundImage;
    [SerializeField] private Color TierClearColor = Color.white;
    [SerializeField] private Color NormalColor = Color.white;

    [SerializeField] private Button RewardButton;
    [SerializeField] private TMP_Text RewardText;

    [SerializeField] private Image AchieveIcon;
    [SerializeField] private Sprite Clearimage;
    [SerializeField] private Sprite Notimage;


    [SerializeField] private TMP_Text AchieveTitle;

    [SerializeField] private GameObject progressPanel;
    [SerializeField] private TMP_Text AchieveProgress;
    [SerializeField] private TMP_Text AchieveTarget;

    [SerializeField] private Image ProgressBar;

    [SerializeField] private GameObject CompleteImage;

    [SerializeField] private RectTransform MoneyTransform;
    public void InitializeWidget(AllAchievementWidget allwidget, AchieveEnum type, string id, string title, int progress, int target,
        int currentReward, bool bcompleted)
    {
        allArchievementWidget = allwidget;
        achiveType = type;
        achiveID = id;

        AchieveTitle.text = title;

        Progress = progress;
        Target = target;
        Reward = currentReward;

        AchieveProgress.text = progress.ToString();
        AchieveTarget.text = target.ToString();

        ProgressBar.fillAmount = Progress / Target;

       
        RewardText.text = currentReward.ToString();


        bisCleared =  bcompleted;

        CheckRewardButton();
      
        CompleteImage.gameObject.SetActive(bisCleared);



    }



    public bool RenewalProgress(int progress)
    {
        Progress = progress;
        AchieveProgress.text = progress.ToString();
        ProgressBar.fillAmount = Progress / Target;
        return CheckRewardButton();
    }


    public void OnclickRewardButton()
    {
        allArchievementWidget.RewardAchievement(achiveType, achiveID, Reward , MoneyTransform);
    }

    public void NextTierTarget(int newtarget, int newreward)
    {
        Target = newtarget;
        AchieveTarget.text = Target.ToString();

        Reward = newreward;
        RewardText.text = Reward.ToString();

       
    }

    private bool CheckRewardButton()
    {
        bool reward = Progress >= Target;
        RewardButton.interactable = reward;
        if (reward)
        {
            BackGroundImage.color = TierClearColor;
            AchieveIcon.sprite = Clearimage;
           
            if (bisCleared)
                RewardButton.interactable = false;

        }
        else
        {

            BackGroundImage.color = NormalColor;
            AchieveIcon.sprite = Notimage;

            
        }
        return reward;

    }
}


 
