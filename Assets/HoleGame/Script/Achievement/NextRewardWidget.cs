using Lean.Gui;
using TMPro;
using UnityEngine;

public class NextRewardWidget : MonoBehaviour
{
    [SerializeField] private TMP_Text RewardCompleteText;
    [SerializeField] private LeanWindow window;
    
    public void SetNextRewardWidget(int cleartier)
    {
        RewardCompleteText.text = $"���� {cleartier}�ܰ� �Ϸ�";
        
        window.TurnOn();
    }
}
