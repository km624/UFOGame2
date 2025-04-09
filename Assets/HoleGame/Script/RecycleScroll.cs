using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MyNamespace
{
    /// <summary>
    /// ���� ��ũ��Ʈ. SimpleScrollSnap�� ���̰ų�, ���� GameObject�� �ٿ��� ��.
    /// </summary>
    public class RecycleScroll : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("SimpleScrollSnap ������Ʈ�� Drag & Drop")]
        public DanielLochner.Assets.SimpleScrollSnap.SimpleScrollSnap scrollSnap;

        [Header("Stage Info")]
        [Tooltip("��ü ��������(������) ����")]
        public int totalStages = 100;

        [Tooltip("������ ������ ������ �г� �� (Content ����)")]
        public int maxVisiblePanels = 5;

        // ���� � ������������ �����ؼ� maxVisiblePanels���� ���� �г��� ǥ�� ������
        //private int startStageIndex = 0;

        // �� ���� �г�(RectTransform)���� �����Ǵ� �������� �ε���
        private Dictionary<RectTransform, int> panelStageMap = new Dictionary<RectTransform, int>();

        public bool bSetup =false;

        // ������ ���÷�: �г� �ؽ�Ʈ�� ��ü�Ѵٰ� ����
        // (�����δ� �̹����� �ٲٰų�, �ٸ� UI ��Ҹ� �����ؾ� �� ���� ����)
        void Start()
        {
          /*  if (!scrollSnap)
            {
                Debug.LogError("SimpleScrollSnap ������ �ʿ��մϴ�.");
                return;
            }

            // �ʱ�ȭ
            InitializePanels();*/
        }

        void Update()
        {
            // ��ũ�� ���°� �ٲ� �� �г� ��Ȱ�� ������ ����
            if(bSetup)
                HandleRecycling();
        }

        /// <summary>
        /// �ʱ� �г� ����: 
        /// - SimpleScrollSnap.Content ������ �ִ� �гε� �� maxVisiblePanels ���� ������ �дٰ� ����.
        /// - �������� ���� �����ϰų�, 'useAutomaticLayout = false' ���¿��� �����ؾ� ��.
        /// </summary>
        public void InitializePanels()
        {
            int realPanelCount = scrollSnap.NumberOfPanels;

            //Debug.Log(realPanelCount);
            if (realPanelCount < maxVisiblePanels)
            {
                Debug.LogWarning($"[SimpleScrollSnapRecycler] ���� �г� ��({realPanelCount})�� maxVisiblePanels({maxVisiblePanels})���� �����ϴ�.");
                maxVisiblePanels = realPanelCount;
            }

            // ó������ startStageIndex = 0 ~ maxVisiblePanels-1 ���� ǥ��
            for (int i = 0; i < realPanelCount; i++)
            {
                Debug.Log(i);
                RectTransform panel = scrollSnap.Panels[i];
                
                if (i < maxVisiblePanels)
                {
                    // ��: �г� i�� �������� i ����
                    panelStageMap[panel] = i;
                    UpdatePanelContent(panel, i);
                }
                else
                {
                    // ���� ������ Content �ȿ� �г��� �� ���ٸ�, 
                    // ���⼭�� �׳� SetActive(false)�ϰų� 'useAutomaticLayout' ���� ���� ��ġ�� ���� ó��
                    panel.gameObject.SetActive(false);
                }
            }

           
        }

        /// <summary>
        /// �г��� ȭ�� ������ �������, �ݴ������� ��Ȱ���Ͽ� �� �������� �����ֱ�
        /// (���� ��ũ�� ����)
        /// </summary>
        private void HandleRecycling()
        {
            // Viewport ������ ���Ѵ�
            float viewportLeft = scrollSnap.Viewport.rect.xMin + scrollSnap.Viewport.position.x;
            float viewportRight = scrollSnap.Viewport.rect.xMax + scrollSnap.Viewport.position.x;

            // ��� �г�(Ȱ��ȭ��) �˻�
            for (int i = 0; i < maxVisiblePanels; i++)
            {
                RectTransform panel = scrollSnap.Panels[i];

                if (!panel.gameObject.activeSelf)
                    continue; // ��Ȱ��ȭ�� �г��� ����

                Vector3 worldPos = panel.transform.position;
                float panelLeft = worldPos.x - (panel.rect.width * panel.pivot.x);
                float panelRight = worldPos.x + (panel.rect.width * (1f - panel.pivot.x));

                // ���� �г��� ���� ȭ�� ������ ������ ����ٸ� -> ���������� ��Ȱ��
                if (panelRight < viewportLeft)
                {
                    RecycleToRight(panel);
                }
                // Ȥ�� �г��� ������ ȭ�� ������ ����ٸ� -> �������� ��Ȱ��
                else if (panelLeft > viewportRight)
                {
                    RecycleToLeft(panel);
                }
            }
        }

       
        private void RecycleToRight(RectTransform panel)
        {
            int oldStageIndex = panelStageMap[panel];

            // ������ ����������� ��Ȱ������ ����.
            if (oldStageIndex >= totalStages - maxVisiblePanels)
                return;

            int newStageIndex = oldStageIndex + maxVisiblePanels; // �ܼ��� +5

            panelStageMap[panel] = newStageIndex;
            UpdatePanelContent(panel, newStageIndex);

            float panelWidth = panel.rect.width;
            panel.anchoredPosition += new Vector2(panelWidth * maxVisiblePanels, 0f);
        }

        /// <summary>
        /// ȭ�� ������ ������ ���� �г��� ���� ���� ��ġ�� ��Ȱ��
        /// </summary>
        private void RecycleToLeft(RectTransform panel)
        {
            int oldStageIndex = panelStageMap[panel];

            if (oldStageIndex <=0)
                return;

            int newStageIndex = oldStageIndex - maxVisiblePanels; // �ܼ��� -5

          
            panelStageMap[panel] = newStageIndex;
            UpdatePanelContent(panel, newStageIndex);

            float panelWidth = panel.rect.width;
            panel.anchoredPosition -= new Vector2(panelWidth * maxVisiblePanels, 0f);
        }

        /// <summary>
        /// �гο� ǥ�õǴ� UI (�ؽ�Ʈ, �̹��� ��)�� 'stageIndex'�� ���� ����
        /// </summary>
        private void UpdatePanelContent(RectTransform panel, int stageIndex)
        {
            // ���⼭�� ������ �г� �̸����� ǥ���Ѵٰ� ����
            // �����δ� �ڽ� Text, Image ���� ã�Ƽ� ������ �����ϸ� ��.
            panel.gameObject.name = $"StagePanel_{stageIndex}";

            // ����) �ڽĿ� Text�� �ִٸ�:
            Text txt = panel.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = $"Stage {stageIndex}";
            }
        }
    }
}