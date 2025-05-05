using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievePointRewardWidget : MonoBehaviour
{
    [SerializeField] private Button RewardButton;
    

    public int RewardTier { get; private set; }
    
    public int biscompleted { get; private set; }

    [SerializeField] private Image IconImage;
    [SerializeField] private Sprite OpenIcon;
    [SerializeField] private Sprite CloseIcon;
    [SerializeField] private TMP_Text PointText;

    public void InitRewardWidget()
    {

    }

    public void OnClickRewardButton()
    {

    }


    
}
