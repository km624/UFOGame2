using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MyNamespace
{
    /// <summary>
    /// 별도 스크립트. SimpleScrollSnap에 붙이거나, 같은 GameObject에 붙여도 됨.
    /// </summary>
    public class RecycleScroll : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("SimpleScrollSnap 컴포넌트를 Drag & Drop")]
        public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap scrollSnap;

        [Header("Stage Info")]
        [Tooltip("전체 스테이지(데이터) 개수")]
        public int totalStages = 100;

        [Tooltip("실제로 만들어둘 물리적 패널 수 (Content 하위)")]
        public int maxVisiblePanels = 5;

        // 현재 어떤 스테이지부터 시작해서 maxVisiblePanels개의 실제 패널을 표시 중인지
        //private int startStageIndex = 0;

        // 각 실제 패널(RectTransform)마다 대응되는 스테이지 인덱스
        private Dictionary<RectTransform, int> panelStageMap = new Dictionary<RectTransform, int>();

        public bool bSetup =false;

        // 간단한 예시로: 패널 텍스트만 교체한다고 가정
        // (실제로는 이미지를 바꾸거나, 다른 UI 요소를 갱신해야 할 수도 있음)
        void Start()
        {
          /*  if (!scrollSnap)
            {
                Debug.LogError("SimpleScrollSnap 참조가 필요합니다.");
                return;
            }

            // 초기화
            InitializePanels();*/
        }

        void Update()
        {
            // 스크롤 상태가 바뀔 때 패널 재활용 로직을 실행
            if(bSetup)
                HandleRecycling();
        }

        /// <summary>
        /// 초기 패널 설정: 
        /// - SimpleScrollSnap.Content 하위에 있는 패널들 중 maxVisiblePanels 개만 실제로 둔다고 가정.
        /// - 나머지는 직접 삭제하거나, 'useAutomaticLayout = false' 상태에서 관리해야 함.
        /// </summary>
        public void InitializePanels()
        {
            int realPanelCount = scrollSnap.NumberOfPanels;

            //Debug.Log(realPanelCount);
            if (realPanelCount < maxVisiblePanels)
            {
                Debug.LogWarning($"[SimpleScrollSnapRecycler] 실제 패널 수({realPanelCount})가 maxVisiblePanels({maxVisiblePanels})보다 적습니다.");
                maxVisiblePanels = realPanelCount;
            }

            // 처음에는 startStageIndex = 0 ~ maxVisiblePanels-1 까지 표시
            for (int i = 0; i < realPanelCount; i++)
            {
                Debug.Log(i);
                RectTransform panel = scrollSnap.Panels[i];
                
                if (i < maxVisiblePanels)
                {
                    // 예: 패널 i에 스테이지 i 연결
                    panelStageMap[panel] = i;
                    UpdatePanelContent(panel, i);
                }
                else
                {
                    // 만약 실제로 Content 안에 패널이 더 많다면, 
                    // 여기서는 그냥 SetActive(false)하거나 'useAutomaticLayout' 등을 꺼서 위치를 숨김 처리
                    panel.gameObject.SetActive(false);
                }
            }

           
        }

        /// <summary>
        /// 패널이 화면 밖으로 벗어났으면, 반대편으로 재활용하여 새 스테이지 보여주기
        /// (수평 스크롤 가정)
        /// </summary>
        private void HandleRecycling()
        {
            // Viewport 범위를 구한다
            float viewportLeft = scrollSnap.Viewport.rect.xMin + scrollSnap.Viewport.position.x;
            float viewportRight = scrollSnap.Viewport.rect.xMax + scrollSnap.Viewport.position.x;

            // 모든 패널(활성화된) 검사
            for (int i = 0; i < maxVisiblePanels; i++)
            {
                RectTransform panel = scrollSnap.Panels[i];

                if (!panel.gameObject.activeSelf)
                    continue; // 비활성화된 패널은 무시

                Vector3 worldPos = panel.transform.position;
                float panelLeft = worldPos.x - (panel.rect.width * panel.pivot.x);
                float panelRight = worldPos.x + (panel.rect.width * (1f - panel.pivot.x));

                // 만약 패널이 왼쪽 화면 밖으로 완전히 벗어났다면 -> 오른쪽으로 재활용
                if (panelRight < viewportLeft)
                {
                    RecycleToRight(panel);
                }
                // 혹은 패널이 오른쪽 화면 밖으로 벗어났다면 -> 왼쪽으로 재활용
                else if (panelLeft > viewportRight)
                {
                    RecycleToLeft(panel);
                }
            }
        }

       
        private void RecycleToRight(RectTransform panel)
        {
            int oldStageIndex = panelStageMap[panel];

            // 마지막 스테이지라면 재활용하지 않음.
            if (oldStageIndex >= totalStages - maxVisiblePanels)
                return;

            int newStageIndex = oldStageIndex + maxVisiblePanels; // 단순히 +5

            panelStageMap[panel] = newStageIndex;
            UpdatePanelContent(panel, newStageIndex);

            float panelWidth = panel.rect.width;
            panel.anchoredPosition += new Vector2(panelWidth * maxVisiblePanels, 0f);
        }

        /// <summary>
        /// 화면 오른쪽 밖으로 나간 패널을 가장 왼쪽 위치로 재활용
        /// </summary>
        private void RecycleToLeft(RectTransform panel)
        {
            int oldStageIndex = panelStageMap[panel];

            if (oldStageIndex <=0)
                return;

            int newStageIndex = oldStageIndex - maxVisiblePanels; // 단순히 -5

          
            panelStageMap[panel] = newStageIndex;
            UpdatePanelContent(panel, newStageIndex);

            float panelWidth = panel.rect.width;
            panel.anchoredPosition -= new Vector2(panelWidth * maxVisiblePanels, 0f);
        }

        /// <summary>
        /// 패널에 표시되는 UI (텍스트, 이미지 등)를 'stageIndex'에 맞춰 갱신
        /// </summary>
        private void UpdatePanelContent(RectTransform panel, int stageIndex)
        {
            // 여기서는 간단히 패널 이름으로 표시한다고 예시
            // 실제로는 자식 Text, Image 등을 찾아서 데이터 갱신하면 됨.
            panel.gameObject.name = $"StagePanel_{stageIndex}";

            // 예시) 자식에 Text가 있다면:
            Text txt = panel.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = $"Stage {stageIndex}";
            }
        }
    }
}