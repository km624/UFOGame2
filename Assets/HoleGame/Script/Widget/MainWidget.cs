
using EasyTransition;
using Lean.Gui;

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Splines;
using System.Collections.Generic;
using System.Runtime.CompilerServices;



public class MainWidget : MonoBehaviour
{
    //public LeanSwitch leanSwitch;
    public TransitionSettings transition;

    public Button PlayButton;

    public UFOAllWidget ufoAllWidget;

    public TMP_Text StarCntText;

    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform ufomodel;
    [SerializeField] private float moveDuration = 2.0f;


    [SerializeField] Vector3 UFOTargetScale = new Vector3(0.5f, 0.5f, 0.5f); // ���� �� ũ��
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
        UFOPathCalculate();

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

   
  /* public void ChangeleanSwitch(int change,int select)
   {
        leanSwitch.State = change;
   }*/

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

    private void UFOPathCalculate()
    {
        // Spline ����Ʈ �� DOTween ��η� ��ȯ
        var spline = splineContainer.Spline;
        int sampleCount = 30;
        List<Vector3> path = new();

        for (int i = 0; i <= sampleCount; i++)
        {
            float t = i / (float)sampleCount;

            Vector3 localPos = spline.EvaluatePosition(t);
            Vector3 worldPos = splineContainer.transform.TransformPoint(localPos);
            
            Debug.Log(worldPos);

            path.Add(worldPos);
        }

        

        BounceShape bounce = ufomodel.GetComponent<BounceShape>();   

        if(bounce !=null)
        {
            bounce.enabled = false;

        }

        Sequence seq = DOTween.Sequence();
        seq.Append(ufomodel.DOPath(path.ToArray(), moveDuration, PathType.CatmullRom)
              .SetEase(Ease.InOutSine));
              //.SetLookAt(0.01f)); // �ɼ�: ���� ���� �ϱ�

        seq.Join(ufomodel.DOScale(UFOTargetScale, moveDuration).SetEase(Ease.InOutSine));

    }


    public void PlayGame()
   {

        DOTween.KillAll();
        TransitionManager.Instance().Transition("GameScene", transition,0);
       


    }
 
   
}
