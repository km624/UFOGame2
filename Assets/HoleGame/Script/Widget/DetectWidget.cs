using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class DetectWidget : MonoBehaviour
{
    [Header("Radar Settings")]
    private IDetctable player;
    [SerializeField]
    private float radarRange = 20.0f;

    [Header("UI Settings")]
    [SerializeField]
    private RectTransform radarUI; // 원형 UI 영역
    [SerializeField]
    private GameObject radarDotPrefab;
   /* [SerializeField]
    private Sprite RadorIcon;*/




    private List<IDetctable> targets = new();
    private Dictionary<IDetctable, GameObject> dotMap = new();

    private void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        UpdateRadarDots();
    }

    public void AddstandardTarget(IDetctable standard) 
    {
        player = standard;
    }

    public void AddTarget(IDetctable target)
    {
        if (targets.Contains(target)) return;


        //gameObject.SetActive(true);

        targets.Add(target);
        GameObject dot = Instantiate(radarDotPrefab, radarUI);
       
       // dot.GetComponent<Image>().sprite = target.RadarIcon;
        dotMap[target] = dot;
        gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases(); // 강제 갱신
       
    }

    public void RemoveTarget(IDetctable target)
    {
        if (!targets.Contains(target)) return;


        Destroy(dotMap[target]);
        dotMap.Remove(target);
        targets.Remove(target);

        if(targets.Count <= 0)
            gameObject.SetActive(false);

    }

    private void UpdateRadarDots()
    {
        foreach (var target in targets)
        {
            Vector3 offset = target.WorldPosition - player.WorldPosition;
            Vector2 flatOffset = new Vector2(offset.x, offset.z);
            float distance = flatOffset.magnitude;

            // 방향 계산 (normalized)
            Vector2 direction = flatOffset.normalized;

            float radarRadius = radarUI.rect.width / 2f;
            float scale = Mathf.Min(distance / radarRange, 1f); // 1보다 크면 클램핑

            // 클램핑 시 원 가장자리에 위치하게
            Vector2 clampedPos = direction * radarRadius * scale;

            RectTransform dotRT = dotMap[target].GetComponent<RectTransform>();
            dotRT.anchoredPosition = clampedPos;
        }
    }
}

