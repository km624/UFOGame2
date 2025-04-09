using TMPro;
using UnityEngine;

public class UFOWidget : MonoBehaviour
{
    [SerializeField]
    GameObject UFOmodel;

    [SerializeField]
    TMP_Text UFONameText;
    [SerializeField]
    TMP_Text MoveSpeedtext;
    [SerializeField]
    TMP_Text LiftSpeedText;

    void Start()
    {
        
    }

    public void SetUfoWidget(UFOData data)
    {

        MeshFilter meshFilter = UFOmodel.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = data.UFOMesh;
        }
        MeshRenderer meshRenderer = UFOmodel.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            if (data.UFOMaterials != null && data.UFOMaterials.Count > 0)
            {

                meshRenderer.materials = data.UFOMaterials.ToArray();
            }
        }
        UFONameText.text = data.UFOName;
        MoveSpeedtext.text = "Move " + data.MoveSpeed.ToString();
        LiftSpeedText.text="Lift " +  data.LiftSpeed.ToString();
    }
    
}
