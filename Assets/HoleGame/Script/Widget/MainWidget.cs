
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
            if (GameManager.Instance.userData == null)
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

            ufoAllWidget.InitializedUFOAllWidget();
        }
    }


   /* public void SetUFOModel(int selectnum ,int prevnum)
    {
        if (UFOLoadManager.Instance != null)
        {
            if (UFOLoadManager.Instance.LoadedUFODataList.Count == 0) return;
            UFOData ufodata  = UFOLoadManager.Instance.LoadedUFODataList[selectnum];
            if(ufodata != null)
            {
                MeshFilter meshFilter = SelectUFOModel.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    meshFilter.mesh = ufodata.UFOMesh;
                }
                MeshRenderer meshRenderer = SelectUFOModel.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    if (ufodata.UFOMaterials != null && ufodata.UFOMaterials.Count > 0)
                    {

                        meshRenderer.materials = ufodata.UFOMaterials.ToArray();
                    }
                }

                UFOLoadManager.Instance.SetSelectUFODATA(selectnum);
            }
        }
    }*/
   
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
