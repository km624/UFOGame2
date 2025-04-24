
using EasyTransition;
using Lean.Gui;

using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class MainWidget : MonoBehaviour
{
    public LeanSwitch leanSwitch;
    public TransitionSettings transition;

    public Button PlayButton;

    public UFOAllWidget ufoAllWidget;

    public TMP_Text StarCntText;
    void Start()
    {
       
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.userData != null)
            {
                SetStarCntText(GameManager.Instance.userData.StarCnt);

                ufoAllWidget.InitializedUFOAllWidget();
            }
            else
            {
                Debug.Log("Userdata����");
                
                TestLoadData();  
            }
          
        }
     

        int currentLevel = QualitySettings.GetQualityLevel();
        string levelName = QualitySettings.names[currentLevel];
       
        Debug.Log($"Quality Level: {levelName} ({currentLevel})");
    }

    public async void TestLoadData()
    {
        if (GameManager.Instance != null)
        {
            await GameManager.Instance.InitData();

            SetStarCntText(GameManager.Instance.userData.StarCnt);

            ufoAllWidget.InitializedUFOAllWidget();
        }
    }

   
   public void ChangeleanSwitch(int change,int select)
   {
        leanSwitch.State = change;
   }

    public void SetStarCntText(int cnt)
    {
        StarCntText.text= cnt.ToString();
    }

    public void CallBack_OnPurchased(bool bsucess , int currentcnt)
    {
        if(bsucess)
        {
            // �����ϴ� �ִϸ��̼� ����

            SetStarCntText(currentcnt);
        }
        else
        {
            //�� ���� ���� â ������
            Debug.Log("���� ����");
        }
        
    }

   public void PlayGame()
   {
      
       
        TransitionManager.Instance().Transition("GameScene", transition,0);
       


    }
 
   
}
