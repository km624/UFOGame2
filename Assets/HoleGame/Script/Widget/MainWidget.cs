
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

    public TMP_Text qualitytext;

    public GameObject SelectUFOModel;

    public UFOAllWidget uFOAllWidget;
    void Start()
    {
       
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.userData != null)
            {
             
            }
            else
            {
                GameManager.Instance.TestLoadData();
               
                Debug.Log("Userdata¾øÀ½");
            }

            
            
        }

        int currentLevel = QualitySettings.GetQualityLevel();
        string levelName = QualitySettings.names[currentLevel];
        qualitytext.text = levelName;
        //Debug.Log($"Quality Level: {levelName} ({currentLevel})");
    }

    public void SetUFOModel(int selectnum ,int prevnum)
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
    }
   
  

   public void ChangeleanSwitch(int change,int select)
   {
        leanSwitch.State = change;
   }

   public void PlayGame()
   {
      
       
        TransitionManager.Instance().Transition("GameScene", transition,0);
       


    }
 
   
}
