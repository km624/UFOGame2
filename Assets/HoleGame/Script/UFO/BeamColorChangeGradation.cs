using UnityEngine;

public class BeamColorChangeGradation : MonoBehaviour
{
    [Header("Shader Material")]
    [SerializeField]
    private Material beamMaterial; // ShaderGraph 만든 머터리얼 연결

    [Header("Color Properties")]
    [SerializeField]
    private Color[] colors = new Color[5]; // 미리 등록해둔 5개 컬러

    [Header("Shader Property Names")]
   
    private string[] colorPropertyNames = { "_Color1", "_Color2", "_Color3", "_Color4", "_Color5" };

   
    private string[] edgeStartPropertyNames = { "_Edge1Start", "_Edge2Start", "_Edge3Start", "_Edge4Start", "_Edge5Start" };

   
    private string[] edgeEndPropertyNames = { "_Edge1End", "_Edge2End", "_Edge3End", "_Edge4End", "_Edge5End" };

    [Header("Settings")]
    [SerializeField]
    private int maxObjectCnt = 5;

    private int maxLevel = 5;
    private int currentLevel = 1;

   

    // 컬러들을 머터리얼에 초기 셋팅
    private void InitializeColors()
    {
        for (int i = 0; i < colors.Length; i++)
        {
            beamMaterial.SetColor(colorPropertyNames[i], colors[i]);
        }
    }

    public void SetMaxLevel(int max)
    {
        maxLevel = max;
    }

    // 현재 레벨에 따라 구간 조정
    public void UpdateLevelSettings(int level)
    {
        currentLevel = Mathf.Clamp(level - (maxLevel - maxObjectCnt), 1, maxObjectCnt);

        int colorCount = Mathf.Clamp(currentLevel, 1, maxLevel); // 1~5 제한
        float stepSize = 1f / colorCount;

      /*  // Color 세팅
        for (int i = 0; i < colorCount; i++)
        {
            beamMaterial.SetColor($"_Color{i + 1}", colorSettings[i]);
        }
        beamMaterial.SetColor($"_Color6", colorSettings[0]); // 마지막 Color1 복제
*/
        // Edge 세팅
        // 초기화
        for (int i = 0; i < 5; i++)
        {
            beamMaterial.SetFloat($"_Edge{i + 1}Start", 1f);
            beamMaterial.SetFloat($"_Edge{i + 1}End", 1f);
        }

        if (level == 1)
        {
            beamMaterial.SetFloat("_Edge1Start", 1f);
            beamMaterial.SetFloat("_Edge1End", 1f);
        }
        else
        {
            // 기본 (Edge1 ~ Edge(colorCount-1)) 설정
            for (int i = 0; i < colorCount - 1; i++)
            {
                beamMaterial.SetFloat($"_Edge{i + 1}Start", stepSize * i);
                beamMaterial.SetFloat($"_Edge{i + 1}End", stepSize * (i + 1));
            }
            // 마지막은 Edge5에 할당 (항상)
            beamMaterial.SetFloat("_Edge5Start", stepSize * (colorCount - 1));
            beamMaterial.SetFloat("_Edge5End", 1f);
        }
    }
}
