using Lean.Gui;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LastRewardWidget : MonoBehaviour
{

    private PointRewardEnum RewardType;

    [SerializeField] private RectTransform myrecttransform;

    [SerializeField] private LeanWindow Rewardleanwindow;

    [SerializeField] private GameObject RenderCamera;

    [SerializeField] private MeshFilter UFOmeshFilter;
    
    [SerializeField] private MeshRenderer UFOrenderer;

    [SerializeField] private Image SkillImage;

    private List<GameObject> AddObjectInstanceList = new List<GameObject>();

    public void Start()
    {
        RenderCamera.SetActive(false);

    }

    public void SetLastRewardWidget(PointRewardEnum rewardtype, string ufoname , int colorindex)
    {
       
       
        foreach(GameObject obj in AddObjectInstanceList)
        {
            Destroy(obj);  
        }
        AddObjectInstanceList.Clear();

        UFOData rewardufodata = null;
        if (UFOLoadManager.Instance.ReadLoadedUFODataDic.ContainsKey(ufoname))
        {
            rewardufodata = UFOLoadManager.Instance.ReadLoadedUFODataDic[ufoname];
        }
        else
        {
            Debug.Log($"UFO DATA 없음 {ufoname}");
            return;
        }


        Material[] mats = null;
        Texture baseMap = null;

        if (rewardufodata != null)
        {

            UFOmeshFilter.mesh = rewardufodata.UFOMesh;

            var colorSet = rewardufodata.UFOColorDataList[colorindex];
            int count = colorSet.Materials.Count;
            mats = new Material[count];

            baseMap = colorSet.Materials[0].GetTexture("_BaseMap");

            for (int i = 0; i < count; i++)
            {
                mats[i] = new Material(colorSet.Materials[i]); // 복사본
            }

        }

        // 머터리얼 배열 통째로 적용
        UFOrenderer.materials = mats;

        CreateAddObject(rewardufodata, baseMap);
       
        //스킬 이미지 세팅
        SkillEnum skilltype = rewardufodata.Skilltype;
        Sprite skillicon = SkillIconManager.Instance.GetSkillIconSprite(skilltype);

        SkillImage.sprite = skillicon;

        RenderCamera.SetActive(true);


        //myrecttransform.localScale = new Vector3(0,0,1);
        transform.localScale = new Vector3(0.1f, 0.1f, 1);
        Rewardleanwindow.TurnOn();

    }


    private void CreateAddObject(UFOData currentUFOdata, Texture currentBaseMap)
    {
        // 새 인스턴스 생성
        foreach (var addobject in currentUFOdata.AddUFObject)
        {
            GameObject addobjectInstance = Instantiate(addobject, UFOmeshFilter.transform);

            if (addobjectInstance != null)
            {
                MeshRenderer renderer = addobjectInstance.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material[] originalMaterials = renderer.sharedMaterials;
                    Material[] newMaterials = new Material[originalMaterials.Length];

                    for (int i = 0; i < originalMaterials.Length; i++)
                    {
                        // 원본 머터리얼 복사
                        newMaterials[i] = new Material(originalMaterials[i]);

                        // 언락 상태 → BaseMap(Texture) 교체
                        if (newMaterials[i].HasProperty("_BaseMap"))
                        {
                            newMaterials[i].SetTexture("_BaseMap", currentBaseMap);
                        }


                    }

                    // 새 머터리얼 배열 적용
                    renderer.materials = newMaterials;
                }

                AddObjectInstanceList.Add(addobjectInstance);
            }
        }
    }
    


}
