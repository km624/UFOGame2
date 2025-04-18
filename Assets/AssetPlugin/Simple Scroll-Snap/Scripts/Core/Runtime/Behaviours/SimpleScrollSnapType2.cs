// Simple Scroll-Snap - https://assetstore.unity.com/packages/tools/gui/simple-scroll-snap-140884
// Copyright (c) Daniel Lochner


using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DanielLochner.Assets.SimpleScrollSnap
{
    [AddComponentMenu("UI/Simple Scroll-Snap")]
    [RequireComponent(typeof(ScrollRect))]
    public class SimpleScrollSnapType2 : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        #region Fields
        // Movement and Layout Settings
        [SerializeField] private MovementType movementType = MovementType.Fixed;
        [SerializeField] private MovementAxis movementAxis = MovementAxis.Horizontal;
        [SerializeField] private bool useAutomaticLayout = true;
        [SerializeField] private float automaticLayoutSpacing = 0.25f;
        [SerializeField] private SizeControl sizeControl = SizeControl.Fit;
        [SerializeField] private Vector2 size = new Vector2(400, 250);
        [SerializeField] private Margins automaticLayoutMargins = new Margins(0);
        [SerializeField] private bool useInfiniteScrolling = false;
        [SerializeField] private bool useRecycleScrolling = false;
        [SerializeField] private float infiniteScrollingSpacing = 0.25f;
        [SerializeField] private bool useOcclusionCulling = false;
        [SerializeField] private int startingPanel = 0;

        // Navigation Settings
        [SerializeField] private bool useSwipeGestures = true;
        [SerializeField] private float minimumSwipeSpeed = 0f;
        [SerializeField] private Button previousButton = null;
        [SerializeField] private Button nextButton = null;
        [SerializeField] private ToggleGroup pagination = null;
        [SerializeField] private bool useToggleNavigation = true;

        // Snap Settings
        [SerializeField] private SnapTarget snapTarget = SnapTarget.Next;
        [SerializeField] private float snapSpeed = 10f;
        [SerializeField] private float thresholdSpeedToSnap = -1f;
        [SerializeField] private bool useHardSnapping = true;
        [SerializeField] private bool useUnscaledTime = false;
        
        // Events
        [SerializeField] private UnityEvent<GameObject, float> onTransitionEffects = new UnityEvent<GameObject, float>();
        [SerializeField] private UnityEvent<int> onPanelSelecting = new UnityEvent<int>();
        [SerializeField] private UnityEvent<int> onPanelSelected = new UnityEvent<int>();
        [SerializeField] private UnityEvent<int, int> onPanelCentering = new UnityEvent<int, int>();
        [SerializeField] private UnityEvent<int, int> onPanelCentered = new UnityEvent<int, int>();

        private ScrollRect scrollRect;
        private Vector2 contentSize, prevAnchoredPosition, velocity;
        private Direction releaseDirection;
        private float releaseSpeed;
        private bool isDragging, isPressing, isSelected = true;
        #endregion

        #region Properties
        public MovementType MovementType
        {
            get => movementType;
            set => movementType = value;
        }
        public MovementAxis MovementAxis
        {
            get => movementAxis;
            set => movementAxis = value;
        }
        public bool UseAutomaticLayout
        {
            get => useAutomaticLayout;
            set => useAutomaticLayout = value;
        }
        public SizeControl SizeControl
        {
            get => sizeControl;
            set => sizeControl = value;
        }
        public Vector2 Size
        {
            get => size;
            set => size = value;
        }
        public float AutomaticLayoutSpacing
        {
            get => automaticLayoutSpacing;
            set => automaticLayoutSpacing = value;
        }
        public Margins AutomaticLayoutMargins
        {
            get => automaticLayoutMargins;
            set => automaticLayoutMargins = value;
        }
        public bool UseInfiniteScrolling
        {
            get => useInfiniteScrolling;
            set => useInfiniteScrolling = value;
        }
        public bool UseRecycleScrolling
        {
            get => useRecycleScrolling;
            
            set => useRecycleScrolling = value;
        }
        
        public float InfiniteScrollingSpacing
        {
            get => infiniteScrollingSpacing;
            set => infiniteScrollingSpacing = value;
        }
        public bool UseOcclusionCulling
        {
            get => useOcclusionCulling;
            set => useOcclusionCulling = value;
        }
        public int StartingPanel
        {
            get => startingPanel;
            set => startingPanel = value;
        }
        public bool UseSwipeGestures
        {
            get => useSwipeGestures;
            set => useSwipeGestures = value;
        }
        public float MinimumSwipeSpeed
        {
            get => minimumSwipeSpeed;
            set => minimumSwipeSpeed = value;
        }
        public Button PreviousButton
        {
            get => previousButton;
            set => previousButton = value;
        }
        public Button NextButton
        {
            get => nextButton;
            set => nextButton = value;
        }
        public ToggleGroup Pagination
        {
            get => pagination;
            set => pagination = value;
        }
        public bool ToggleNavigation
        {
            get => useToggleNavigation;
            set => useToggleNavigation = value;
        }
        public SnapTarget SnapTarget
        {
            get => snapTarget;
            set => snapTarget = value;
        }
        public float SnapSpeed
        {
            get => snapSpeed;
            set => snapSpeed = value;
        }
        public float ThresholdSpeedToSnap
        {
            get => thresholdSpeedToSnap;
            set => thresholdSpeedToSnap = value;
        }
        public bool UseHardSnapping
        {
            get => useHardSnapping;
            set => useHardSnapping = value;
        }
        public bool UseUnscaledTime
        {
            get => useUnscaledTime;
            set => useUnscaledTime = value;
        }
        public UnityEvent<GameObject, float> OnTransitionEffects
        {
            get => onTransitionEffects;
        }
        public UnityEvent<int> OnPanelSelecting
        {
            get => onPanelSelecting;
        }
        public UnityEvent<int> OnPanelSelected
        {
            get => onPanelSelected;
        }
        public UnityEvent<int, int> OnPanelCentering
        {
            get => onPanelCentering;
        }
        public UnityEvent<int, int> OnPanelCentered
        {
            get => onPanelCentered;
        }

        public RectTransform Content
        {
            get => ScrollRect.content;
        }
        public RectTransform Viewport
        {
            get => ScrollRect.viewport;
        }
        public RectTransform RectTransform
        {
            get => transform as RectTransform;
        }
        public ScrollRect ScrollRect
        {
            get
            {
                if (scrollRect == null)
                {
                    scrollRect = GetComponent<ScrollRect>();
                }
                return scrollRect;
            }
        }
        public int NumberOfPanels
        {
            get => Content.childCount;
        }
        private bool ValidConfig
        {
            get
            {
                bool valid = true;

                if (pagination != null)
                {
                    int numberOfToggles = pagination.transform.childCount;
                    if (numberOfToggles != NumberOfPanels)
                    {
                        Debug.LogError("<b>[SimpleScrollSnap]</b> The number of Toggles should be equivalent to the number of Panels. There are currently " + numberOfToggles + " Toggles and " + NumberOfPanels + " Panels. If you are adding Panels dynamically during runtime, please update your pagination to reflect the number of Panels you will have before adding.", gameObject);
                        valid = false;
                    }
                }
                if (snapSpeed < 0)
                {
                    Debug.LogError("<b>[SimpleScrollSnap]</b> Snapping speed cannot be negative.", gameObject);
                    valid = false;
                }

                return valid;
            }
        }
        public Vector2 Velocity
        {
            get => velocity;
            set
            {
                ScrollRect.velocity = velocity = value;
                isSelected = false;
            }
        }

        public RectTransform[] Panels
        {
            get;
            private set;
        }
        public Toggle[] Toggles
        {
            get;
            private set;
        }
        public int SelectedPanel
        {
            get;
            private set;
        }
        public int CenteredPanel
        {
            get;
            private set;
        }
        #endregion

        #region Methods
        /*private void Start()
        {
            if (ValidConfig)
            {
                Setup();
            }
            else
            {
                throw new Exception("Invalid configuration.");
            }
        }
        private void Update()
        {
            if (NumberOfPanels == 0) return;

            HandleOcclusionCulling();
            HandleSelectingAndSnapping();
            HandleInfiniteScrolling();
            HandleRecycleScrolling();
            HandleTransitionEffects();
            HandleSwipeGestures();

            GetVelocity();
        }
        
        public void SetuseRecycle(bool active)
        {
            useRecycleScrolling = active;
            useInfiniteScrolling = !active;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            isPressing = true;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            isPressing = false;
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging && onPanelSelecting.GetPersistentEventCount() > 0)
            {
                onPanelSelecting.Invoke(GetNearestPanel());
            }
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (useHardSnapping)
            {
                ScrollRect.inertia = true;
            }

            isSelected = false;
            isDragging = true;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            switch (movementAxis)
            {
                case MovementAxis.Horizontal:
                    releaseDirection = (Velocity.x > 0) ? Direction.Right : Direction.Left;
                    break;
                case MovementAxis.Vertical:
                    releaseDirection = (Velocity.y > 0) ? Direction.Up : Direction.Down;
                    break;
            }
            releaseSpeed = Velocity.magnitude;
        }

        
        private void Setup()
        {
            if (NumberOfPanels == 0) return;

            // 스크롤-Rect 설정
            ScrollRect.horizontal = (movementType == MovementType.Free) || (movementAxis == MovementAxis.Horizontal);
            ScrollRect.vertical = (movementType == MovementType.Free) || (movementAxis == MovementAxis.Vertical);

            // 혹시 SizeControl.Fit이면, 뷰포트 크기를 패널 크기로
            if (sizeControl == SizeControl.Fit)
            {
                size = Viewport.rect.size;
            }

            // 패널 배열 준비
            Panels = new RectTransform[NumberOfPanels];

            // [중요 변경] "왼쪽 정렬 + 수직 중앙"을 위해 anchor/pivot 강제 세팅
            // ----------------------------------------------------------
            for (int i = 0; i < NumberOfPanels; i++)
            {
                Panels[i] = Content.GetChild(i) as RectTransform;

                // movementType이 Fixed이면서 useAutomaticLayout이 true일 때, 패널 배치
                if (movementType == MovementType.Fixed && useAutomaticLayout)
                {
                    // (1) 각 패널의 anchor/pivot을 "왼쪽(0) + 수직 중앙(0.5)"에 맞춤
                    Panels[i].anchorMin = new Vector2(0f, 0.5f);
                    Panels[i].anchorMax = new Vector2(0f, 0.5f);
                    Panels[i].pivot = new Vector2(0f, 0.5f);

                    // (2) 패널 크기 = (size - 마진) / etc.
                    Panels[i].sizeDelta = size - new Vector2(
                        automaticLayoutMargins.Left + automaticLayoutMargins.Right,
                        automaticLayoutMargins.Top + automaticLayoutMargins.Bottom
                    );

                    // (3) i번째 패널의 위치(anchoredPosition) 계산
                    //     가로로 나란히 배치: (i * (패널너비 + spacing))
                    float panelPosX = i * ((automaticLayoutSpacing + 1f) * size.x);
                    // 수직축은 0(중앙), pivot이 0.5라서 anchoredPosition.y = 0이면 중앙에 위치
                    float panelPosY = 0f;

                    Panels[i].anchoredPosition = new Vector2(panelPosX, panelPosY);
                }
            }

            // Content 설정
            // ----------------------------------------------------------
            if (movementType == MovementType.Fixed)
            {
                // 자동 레이아웃
                if (useAutomaticLayout)
                {
                    // (1) Content도 "왼쪽 + 수직 중앙" 기준으로
                    Content.anchorMin = new Vector2(0f, 0.5f);
                    Content.anchorMax = new Vector2(0f, 0.5f);
                    Content.pivot = new Vector2(0f, 0.5f);

                    // (2) 가장 왼쪽(첫 번째) 패널과 오른쪽(마지막) 패널 확인
                    //     여기서는 별도로 min, max를 쓰지 않고 sizeDelta만 계산
                    float contentWidth = NumberOfPanels * ((automaticLayoutSpacing + 1f) * size.x) - (size.x * automaticLayoutSpacing);
                    float contentHeight = size.y; // Horizontal 모드이므로 높이는 size.y 그대로
                    Content.sizeDelta = new Vector2(contentWidth, contentHeight);
                }

                // 무한 스크롤(Infinite), 리사이클(Recycle) 스크롤 옵션
                if (useInfiniteScrolling || useRecycleScrolling)
                {
                    ScrollRect.movementType = ScrollRect.MovementType.Unrestricted;
                    contentSize = Content.rect.size + (size * infiniteScrollingSpacing);
                    HandleInfiniteScrolling(true);
                    HandleRecycleScrolling(true);
                }

                // 오클루전 컬링
                if (useOcclusionCulling)
                {
                    HandleOcclusionCulling(true);
                }
            }
            else
            {
                // movementType이 Free면 자동배치, 무한/리사이클 스크롤, 오클루전 사용 안 함
                useAutomaticLayout = false;
                useInfiniteScrolling = false;
                useRecycleScrolling = false;
                useOcclusionCulling = false;
            }

            // [중요 변경] 시작 패널의 위치: "뷰포트 왼쪽" 정렬
            // ----------------------------------------------------------
            //  - 원본은 뷰포트 절반을 offset으로 줘서 항상 중앙에 오도록 했으나,
            //    여기서는 xOffset=0, yOffset=0 (수직 정중앙은 위에서 anchor로 해결).
            float xOffset = 0f;
            float yOffset = 0f;
            Vector2 offset = new Vector2(xOffset, yOffset);

            // Panels[startingPanel].anchoredPosition를 기준으로 Content 위치를 결정
            // 패널과 Content의 pivot이 모두 (0, 0.5f)이므로, 실제로는
            // "왼쪽 + 수직 중앙" 정렬 효과가 남
            prevAnchoredPosition = Content.anchoredPosition
                = -Panels[startingPanel].anchoredPosition + offset;

            // 선택 패널 초기화
            SelectedPanel = startingPanel;
            CenteredPanel = startingPanel;

            // 이전 / 다음 버튼
            if (previousButton != null)
            {
                previousButton.onClick.AddListenerOnce(GoToPreviousPanel);
            }
            if (nextButton != null)
            {
                nextButton.onClick.AddListenerOnce(GoToNextPanel);
            }

            // 페이지네이션 (토글)
            if (pagination != null && NumberOfPanels != 0)
            {
                Toggles = pagination.GetComponentsInChildren<Toggle>();
                Toggles[startingPanel].SetIsOnWithoutNotify(true);
                for (int i = 0; i < Toggles.Length; i++)
                {
                    int panelNumber = i;
                    Toggles[i].onValueChanged.AddListenerOnce(delegate (bool isOn) {
                        if (isOn && useToggleNavigation)
                        {
                            GoToPanel(panelNumber);
                        }
                    });
                }
            }
        }

        private void HandleSelectingAndSnapping()
        {
            if (isSelected)
            {
                if (!((isDragging || isPressing) && useSwipeGestures))
                {
                    SnapToPanel();
                }
            }
            else if (!isDragging && (ScrollRect.velocity.magnitude <= thresholdSpeedToSnap || thresholdSpeedToSnap == -1f))
            {
                SelectPanel();
            }
        }
        private void HandleOcclusionCulling(bool forceUpdate = false)
        {
            if (useOcclusionCulling && (Velocity.magnitude > 0f || forceUpdate))
            {
                for (int i = 0; i < NumberOfPanels; i++)
                {
                    switch (movementAxis)
                    {
                        case MovementAxis.Horizontal:
                            Panels[i].gameObject.SetActive(Mathf.Abs(GetDisplacementFromCenter(i).x) <= Viewport.rect.width  / 2f + size.x);
                            break;
                        case MovementAxis.Vertical:
                            Panels[i].gameObject.SetActive(Mathf.Abs(GetDisplacementFromCenter(i).y) <= Viewport.rect.height / 2f + size.y);
                            break;
                    }
                }
            }
        }
        private void HandleInfiniteScrolling(bool forceUpdate = false)
        {
            if (useInfiniteScrolling && (Velocity.magnitude > 0 || forceUpdate))
            {
                switch (movementAxis)
                {
                    case MovementAxis.Horizontal:
                        for (int i = 0; i < NumberOfPanels; i++)
                        {
                            Vector2 offset = new Vector2(contentSize.x, 0);
                            if (GetDisplacementFromCenter(i).x > Content.rect.width /  2f)
                            {
                                Panels[i].anchoredPosition -= offset;
                            }
                            else
                            if (GetDisplacementFromCenter(i).x < Content.rect.width / -2f)
                            {
                                Panels[i].anchoredPosition += offset;
                            }
                        }
                        break;
                    case MovementAxis.Vertical:
                        for (int i = 0; i < NumberOfPanels; i++)
                        {
                            Vector2 offset = new Vector2(0, contentSize.y);
                            if (GetDisplacementFromCenter(i).y > Content.rect.height /  2f)
                            {
                                Panels[i].anchoredPosition -= offset;
                            }
                            else
                            if (GetDisplacementFromCenter(i).y < Content.rect.height / -2f)
                            {
                                Panels[i].anchoredPosition += offset;
                            }
                        }
                        break;
                }
            }
        }

        private void HandleRecycleScrolling(bool forceUpdate = false)
        {
            if (useRecycleScrolling && (Velocity.magnitude > 0 || forceUpdate))
            {
                switch (movementAxis)
                {
                    case MovementAxis.Horizontal:
                        for (int i = 0; i < NumberOfPanels; i++)
                        {
                            Vector2 offset = new Vector2(contentSize.x, 0);
                            if (GetDisplacementFromCenter(i).x > Content.rect.width / 2f)
                            {
                                Panels[i].anchoredPosition -= offset;
                            }
                            else
                            if (GetDisplacementFromCenter(i).x < Content.rect.width / -2f)
                            {
                                Panels[i].anchoredPosition += offset;
                            }
                        }
                        break;
                    case MovementAxis.Vertical:
                        for (int i = 0; i < NumberOfPanels; i++)
                        {
                            Vector2 offset = new Vector2(0, contentSize.y);
                            if (GetDisplacementFromCenter(i).y > Content.rect.height / 2f)
                            {
                                Panels[i].anchoredPosition -= offset;
                            }
                            else
                            if (GetDisplacementFromCenter(i).y < Content.rect.height / -2f)
                            {
                                Panels[i].anchoredPosition += offset;
                            }
                        }
                        break;
                }
            }
        }
        private void HandleSwipeGestures()
        {
            if (useSwipeGestures)
            {
                ScrollRect.horizontal = (movementType == MovementType.Free) || (movementAxis == MovementAxis.Horizontal);
                ScrollRect.vertical   = (movementType == MovementType.Free) || (movementAxis == MovementAxis.Vertical);
            }
            else
            {
                ScrollRect.horizontal = ScrollRect.vertical = !isDragging;
            }
        }
        private void HandleTransitionEffects()
        {
            if (onTransitionEffects.GetPersistentEventCount() == 0) return;

            for (int i = 0; i < NumberOfPanels; i++)
            {
                Vector2 displacement = GetDisplacementFromCenter(i);
                float d = (movementType == MovementType.Free) ? displacement.magnitude : ((movementAxis == MovementAxis.Horizontal) ? displacement.x : displacement.y);
                onTransitionEffects.Invoke(Panels[i].gameObject, d);
            }
        }

        private void SelectPanel()
        {
            int nearestPanel = GetNearestPanel();
            Vector2 displacementFromCenter = GetDisplacementFromCenter(nearestPanel);

            if (snapTarget == SnapTarget.Nearest || releaseSpeed <= minimumSwipeSpeed)
            {
                GoToPanel(nearestPanel);
            }
            else 
            if (snapTarget == SnapTarget.Previous)
            {
                if ((releaseDirection == Direction.Right && displacementFromCenter.x < 0f) || (releaseDirection == Direction.Up   && displacementFromCenter.y < 0f))
                {
                    GoToNextPanel();
                }
                else 
                if ((releaseDirection == Direction.Left  && displacementFromCenter.x > 0f) || (releaseDirection == Direction.Down && displacementFromCenter.y > 0f))
                {
                    GoToPreviousPanel();
                }
                else
                {
                    GoToPanel(nearestPanel);
                }
            }
            else 
            if (snapTarget == SnapTarget.Next)
            {
                if ((releaseDirection == Direction.Right && displacementFromCenter.x > 0f) || (releaseDirection == Direction.Up   && displacementFromCenter.y > 0f))
                {
                    GoToPreviousPanel();
                }
                else 
                if ((releaseDirection == Direction.Left  && displacementFromCenter.x < 0f) || (releaseDirection == Direction.Down && displacementFromCenter.y < 0f))
                {
                    GoToNextPanel();
                }
                else
                {
                    GoToPanel(nearestPanel);
                }
            }
        }
        private void SnapToPanel()
        {
            // (수정) 뷰포트 절반 대신 0을 사용하면, '왼쪽' 기준
            float xOffset = 0f;
            float yOffset = 0f;
            Vector2 offset = new Vector2(xOffset, yOffset);

            Vector2 targetPosition = -Panels[CenteredPanel].anchoredPosition + offset;
            Content.anchoredPosition = Vector2.Lerp(
                Content.anchoredPosition,
                targetPosition,
                (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * snapSpeed
            );

            if (SelectedPanel != CenteredPanel)
            {
                // 중심 판넬 갱신 로직은 그대로
                if (GetDisplacementFromCenter(CenteredPanel).magnitude < (Viewport.rect.width / 10f))
                {
                    onPanelCentered.Invoke(CenteredPanel, SelectedPanel);
                    SelectedPanel = CenteredPanel;
                }
            }
            else
            {
                onPanelCentering.Invoke(CenteredPanel, SelectedPanel);
            }
        }

        public void GoToPanel(int panelNumber)
        {
            CenteredPanel = panelNumber;
            isSelected = true;
            onPanelSelected.Invoke(SelectedPanel);

            if (pagination != null)
            {
                Toggles[panelNumber].isOn = true;
            }
            if (useHardSnapping)
            {
                ScrollRect.inertia = false;
            }
        }
        public void GoToPreviousPanel()
        {
            int nearestPanel = GetNearestPanel();
            if (nearestPanel != 0)
            {
                GoToPanel(nearestPanel - 1);
            }
            else
            {
                if (useInfiniteScrolling)
                {
                    GoToPanel(NumberOfPanels - 1);
                }
                else
                {
                    GoToPanel(nearestPanel);
                }
            }
        }
        public void GoToNextPanel()
        {
            int nearestPanel = GetNearestPanel();
            if (nearestPanel != (NumberOfPanels - 1))
            {
                GoToPanel(nearestPanel + 1);
            }
            else
            {
                if (useInfiniteScrolling)
                {
                    GoToPanel(0);
                }
                else
                {
                    GoToPanel(nearestPanel);
                }
            }
        }

        public void AddToFront(GameObject panel)
        {
            Add(panel, 0);
        }
        public void AddToBack(GameObject panel)
        {
            Add(panel, NumberOfPanels);
        }
        public void Add(GameObject panel, int index)
        {
            if (NumberOfPanels != 0 && (index < 0 || index > NumberOfPanels))
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> Index must be an integer from 0 to " + NumberOfPanels + ".", gameObject);
                return;
            }
            else if (!useAutomaticLayout)
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> \"Automatic Layout\" must be enabled for content to be dynamically added during runtime.");
                return;
            }

            panel = Instantiate(panel, Content, false);
            panel.transform.SetSiblingIndex(index);

            if (ValidConfig)
            {
                if (CenteredPanel <= index)
                {
                    startingPanel = CenteredPanel;
                }
                else
                {
                    startingPanel = CenteredPanel + 1;
                }
                Setup();
            }
        }

        public void AddInstantaiteBack(GameObject panel)
        {
          
            if (!useAutomaticLayout)
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> \"Automatic Layout\" must be enabled for content to be dynamically added during runtime.");
                return;
            }

           
            panel.transform.SetSiblingIndex(NumberOfPanels);

            if (ValidConfig)
            {
                if (CenteredPanel <= NumberOfPanels)
                {
                    startingPanel = CenteredPanel;
                }
                else
                {
                    startingPanel = CenteredPanel + 1;
                }
                Setup();
            }
        }

        public void SetStartingPanel(int index)
        {
            startingPanel = index;
            Setup(); 
        }
        public void RemoveFromFront()
        {
            Remove(0);
        }
        public void RemoveFromBack()
        {
            if (NumberOfPanels > 0)
            {
                Remove(NumberOfPanels - 1);
            }
            else
            {
                Remove(0);
            }
        }
        public void Remove(int index)
        {
            if (NumberOfPanels == 0)
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> There are no panels to remove.", gameObject);
                return;
            }
            else if (index < 0 || index > (NumberOfPanels - 1))
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> Index must be an integer from 0 to " + (NumberOfPanels - 1) + ".", gameObject);
                return;
            }
            else if (!useAutomaticLayout)
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> \"Automatic Layout\" must be enabled for content to be dynamically removed during runtime.");
                return;
            }

            DestroyImmediate(Panels[index].gameObject);

            if (ValidConfig)
            {
                if (CenteredPanel == index)
                {
                    if (index == NumberOfPanels)
                    {
                        startingPanel = CenteredPanel - 1;
                    }
                    else
                    {
                        startingPanel = CenteredPanel;
                    }
                }
                else if (CenteredPanel < index)
                {
                    startingPanel = CenteredPanel;
                }
                else
                {
                    startingPanel = CenteredPanel - 1;
                }
                Setup();
            }
        }

        private Vector2 GetDisplacementFromCenter(int index)
        {
            // 1) x축은 "왼쪽"을 기준 → 따라서 뷰포트 절반(width/2)은 빼지 않음
            float x = Panels[index].anchoredPosition.x + Content.anchoredPosition.x;

            // 2) y축은 "뷰포트 중앙"을 기준 → 기존 로직대로 절반을 빼줌
            float y = Panels[index].anchoredPosition.y + Content.anchoredPosition.y
                      - (Viewport.rect.height * (0.5f - Content.anchorMin.y));

            return new Vector2(x, y);
        }
        private int GetNearestPanel()
        {
            float[] distances = new float[NumberOfPanels];
            for (int i = 0; i < Panels.Length; i++)
            {
                distances[i] = GetDisplacementFromCenter(i).magnitude;
            }

            int nearestPanel = 0;
            float minDistance = Mathf.Min(distances);
            for (int i = 0; i < Panels.Length; i++)
            {
                if (minDistance == distances[i])
                {
                    nearestPanel = i;
                    break;
                }
            }
            return nearestPanel;
        }
        private void GetVelocity()
        {
            Vector2 displacement = Content.anchoredPosition - prevAnchoredPosition;
            float time = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            velocity = displacement / time;
            prevAnchoredPosition = Content.anchoredPosition;
        }*/

        private void Start()
        {
            if (ValidConfig)
            {
                Setup();
            }
            else
            {
                throw new Exception("Invalid configuration.");
            }
        }
        private void Update()
        {
            if (NumberOfPanels == 0) return;

            HandleOcclusionCulling();
            HandleSelectingAndSnapping();
            HandleInfiniteScrolling();
            HandleRecycleScrolling();
            HandleTransitionEffects();
            HandleSwipeGestures();

            GetVelocity();
        }

        public void SetuseRecycle(bool active)
        {
            useRecycleScrolling = active;
            useInfiniteScrolling = !active;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            isPressing = true;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            isPressing = false;
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging && onPanelSelecting.GetPersistentEventCount() > 0)
            {
                onPanelSelecting.Invoke(GetNearestPanel());
            }
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (useHardSnapping)
            {
                ScrollRect.inertia = true;
            }

            isSelected = false;
            isDragging = true;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            switch (movementAxis)
            {
                case MovementAxis.Horizontal:
                    releaseDirection = (Velocity.x > 0) ? Direction.Right : Direction.Left;
                    break;
                case MovementAxis.Vertical:
                    releaseDirection = (Velocity.y > 0) ? Direction.Up : Direction.Down;
                    break;
            }
            releaseSpeed = Velocity.magnitude;
        }


        private void Setup()
        {
            if (NumberOfPanels == 0) return;

            // 1) ScrollRect - 축 설정
            ScrollRect.horizontal = (movementType == MovementType.Free) || (movementAxis == MovementAxis.Horizontal);
            ScrollRect.vertical = (movementType == MovementType.Free) || (movementAxis == MovementAxis.Vertical);

            // 2) SizeControl.Fit이면, 뷰포트 크기를 패널 사이즈로
            if (sizeControl == SizeControl.Fit)
            {
                size = Viewport.rect.size;
            }

            // 3) 패널 배열 초기화
            Panels = new RectTransform[NumberOfPanels];

            // 4) 만약 MovementType == Fixed && useAutomaticLayout이면 “자동 배치”
            if (movementType == MovementType.Fixed && useAutomaticLayout)
            {
                // ---- (A) Horizontal : "왼쪽 + 수직 중앙" 배치 ----
                if (movementAxis == MovementAxis.Horizontal)
                {
                    // Content (부모) 세팅: anchor/pivot = (0, 0.5)
                    Content.anchorMin = new Vector2(0f, 0.5f);
                    Content.anchorMax = new Vector2(0f, 0.5f);
                    Content.pivot = new Vector2(0f, 0.5f);

                    // 각 패널: anchor/pivot = (0, 0.5)
                    for (int i = 0; i < NumberOfPanels; i++)
                    {
                        Panels[i] = Content.GetChild(i) as RectTransform;

                        Panels[i].anchorMin = new Vector2(0f, 0.5f);
                        Panels[i].anchorMax = new Vector2(0f, 0.5f);
                        Panels[i].pivot = new Vector2(0f, 0.5f);

                        // 패널 크기 (마진 고려)
                        Panels[i].sizeDelta = size - new Vector2(
                            automaticLayoutMargins.Left + automaticLayoutMargins.Right,
                            automaticLayoutMargins.Top + automaticLayoutMargins.Bottom
                        );

                        // i번째 패널 수평 위치: (i * (패널너비 + spacing))
                        float panelPosX = i * ((automaticLayoutSpacing + 1f) * size.x);
                        // 수직은 0 (pivot.y=0.5f 이므로 "수직 중앙")
                        float panelPosY = 0f;

                        Panels[i].anchoredPosition = new Vector2(panelPosX, panelPosY);
                    }

                    // Content의 sizeDelta도 패널 전체 크기에 맞춰 조정
                    //  → (패널 개수 * 패널너비) + 간격
                    float contentWidth = NumberOfPanels * ((automaticLayoutSpacing + 1f) * size.x)
                                          - (size.x * automaticLayoutSpacing);
                    float contentHeight = size.y;
                    Content.sizeDelta = new Vector2(contentWidth, contentHeight);

                    // 시작 패널을 왼쪽에 두기 위해 offset=0
                    Vector2 offset = Vector2.zero;
                    prevAnchoredPosition = Content.anchoredPosition
                        = -Panels[startingPanel].anchoredPosition + offset;
                }
                // ---- (B) Vertical : "위쪽 + 수평 중앙" 배치 ----
                else // movementAxis == MovementAxis.Vertical
                {
                    // Content (부모) 세팅: anchor/pivot = (0.5f, 1f)
                    //  → 가로: 중앙(0.5), 세로: 맨 위(1)
                    Content.anchorMin = new Vector2(0.5f, 1f);
                    Content.anchorMax = new Vector2(0.5f, 1f);
                    Content.pivot = new Vector2(0.5f, 1f);

                    // 각 패널: anchor/pivot = (0.5f, 1f)
                    for (int i = 0; i < NumberOfPanels; i++)
                    {
                        Panels[i] = Content.GetChild(i) as RectTransform;

                        Panels[i].anchorMin = new Vector2(0.5f, 1f);
                        Panels[i].anchorMax = new Vector2(0.5f, 1f);
                        Panels[i].pivot = new Vector2(0.5f, 1f);

                        // 패널 크기 (마진 고려)
                        Panels[i].sizeDelta = size - new Vector2(
                            automaticLayoutMargins.Left + automaticLayoutMargins.Right,
                            automaticLayoutMargins.Top + automaticLayoutMargins.Bottom
                        );

                        // i번째 패널 수직 위치: 위에서 아래로
                        //  → 첫 패널이 맨 위, i가 증가하면 아래로 내려가므로 -연산
                        float panelPosX = 0f;
                        float panelPosY = -i * ((automaticLayoutSpacing + 1f) * size.y);

                        Panels[i].anchoredPosition = new Vector2(panelPosX, panelPosY);
                    }

                    // Content의 sizeDelta도 패널 전체 크기에 맞춰 조정
                    float contentWidth = size.x;
                    float contentHeight = NumberOfPanels * ((automaticLayoutSpacing + 1f) * size.y)
                                          - (size.y * automaticLayoutSpacing);
                    Content.sizeDelta = new Vector2(contentWidth, contentHeight);

                    // 시작 패널을 맨 위에 두기 위해 offset=0
                    // anchor/pivot=(0.5,1)이므로 Y=0이 "상단"
                    Vector2 offset = Vector2.zero;
                    prevAnchoredPosition = Content.anchoredPosition
                        = -Panels[startingPanel].anchoredPosition + offset;
                }
            }
            else
            {
                // movementType이 Free이거나, useAutomaticLayout==false인 경우
                // 원본에서처럼 자동배치 로직을 쓰지 않도록:
                useAutomaticLayout = false;
                useInfiniteScrolling = false;
                useRecycleScrolling = false;
                useOcclusionCulling = false;

                // 패널 목록만 채워두고, 별도 Layout은 건너뜀.
                for (int i = 0; i < NumberOfPanels; i++)
                {
                    Panels[i] = Content.GetChild(i) as RectTransform;
                }

                // (원하시는 경우, 여기서 별도 커스텀 배치하거나 그대로 둠)

                // 시작 패널 (기본 로직: 가운데 정렬 or 직접 수정)
                // 필요하다면 아래 offset=0 등으로 조정 가능
                // prevAnchoredPosition = Content.anchoredPosition
                //     = -Panels[startingPanel].anchoredPosition;
            }

            // 5) 무한 스크롤 / 리사이클 스크롤
            if (useInfiniteScrolling || useRecycleScrolling)
            {
                ScrollRect.movementType = ScrollRect.MovementType.Unrestricted;
                contentSize = Content.rect.size + (size * infiniteScrollingSpacing);
                HandleInfiniteScrolling(true);
                HandleRecycleScrolling(true);
            }

            // 6) 오클루전 컬링
            if (useOcclusionCulling)
            {
                HandleOcclusionCulling(true);
            }

            // 7) SelectedPanel, CenteredPanel 초기화
            SelectedPanel = startingPanel;
            CenteredPanel = startingPanel;

            // 8) 이전/다음 버튼
            if (previousButton != null)
            {
                previousButton.onClick.AddListenerOnce(GoToPreviousPanel);
            }
            if (nextButton != null)
            {
                nextButton.onClick.AddListenerOnce(GoToNextPanel);
            }

            // 9) 페이지네이션 (Toggles)
            if (pagination != null && NumberOfPanels != 0)
            {
                Toggles = pagination.GetComponentsInChildren<Toggle>();
                if (startingPanel < Toggles.Length)
                {
                    Toggles[startingPanel].SetIsOnWithoutNotify(true);
                }
                for (int i = 0; i < Toggles.Length; i++)
                {
                    int panelNumber = i;
                    Toggles[i].onValueChanged.AddListenerOnce(delegate (bool isOn) {
                        if (isOn && useToggleNavigation)
                        {
                            GoToPanel(panelNumber);
                        }
                    });
                }
            }
        }

        private void HandleSelectingAndSnapping()
        {
            if (isSelected)
            {
                if (!((isDragging || isPressing) && useSwipeGestures))
                {
                    SnapToPanel();
                }
            }
            else if (!isDragging && (ScrollRect.velocity.magnitude <= thresholdSpeedToSnap || thresholdSpeedToSnap == -1f))
            {
                SelectPanel();
            }
        }
        private void HandleOcclusionCulling(bool forceUpdate = false)
        {
            if (useOcclusionCulling && (Velocity.magnitude > 0f || forceUpdate))
            {
                for (int i = 0; i < NumberOfPanels; i++)
                {
                    switch (movementAxis)
                    {
                        case MovementAxis.Horizontal:
                            Panels[i].gameObject.SetActive(Mathf.Abs(GetDisplacementFromCenter(i).x) <= Viewport.rect.width / 2f + size.x);
                            break;
                        case MovementAxis.Vertical:
                            Panels[i].gameObject.SetActive(Mathf.Abs(GetDisplacementFromCenter(i).y) <= Viewport.rect.height / 2f + size.y);
                            break;
                    }
                }
            }
        }
        private void HandleInfiniteScrolling(bool forceUpdate = false)
        {
            if (useInfiniteScrolling && (Velocity.magnitude > 0 || forceUpdate))
            {
                switch (movementAxis)
                {
                    case MovementAxis.Horizontal:
                        for (int i = 0; i < NumberOfPanels; i++)
                        {
                            Vector2 offset = new Vector2(contentSize.x, 0);
                            if (GetDisplacementFromCenter(i).x > Content.rect.width / 2f)
                            {
                                Panels[i].anchoredPosition -= offset;
                            }
                            else
                            if (GetDisplacementFromCenter(i).x < Content.rect.width / -2f)
                            {
                                Panels[i].anchoredPosition += offset;
                            }
                        }
                        break;
                    case MovementAxis.Vertical:
                        for (int i = 0; i < NumberOfPanels; i++)
                        {
                            Vector2 offset = new Vector2(0, contentSize.y);
                            if (GetDisplacementFromCenter(i).y > Content.rect.height / 2f)
                            {
                                Panels[i].anchoredPosition -= offset;
                            }
                            else
                            if (GetDisplacementFromCenter(i).y < Content.rect.height / -2f)
                            {
                                Panels[i].anchoredPosition += offset;
                            }
                        }
                        break;
                }
            }
        }

        private void HandleRecycleScrolling(bool forceUpdate = false)
        {
            if (useRecycleScrolling && (Velocity.magnitude > 0 || forceUpdate))
            {
                switch (movementAxis)
                {
                    case MovementAxis.Horizontal:
                        for (int i = 0; i < NumberOfPanels; i++)
                        {
                            Vector2 offset = new Vector2(contentSize.x, 0);
                            if (GetDisplacementFromCenter(i).x > Content.rect.width / 2f)
                            {
                                Panels[i].anchoredPosition -= offset;
                            }
                            else
                            if (GetDisplacementFromCenter(i).x < Content.rect.width / -2f)
                            {
                                Panels[i].anchoredPosition += offset;
                            }
                        }
                        break;
                    case MovementAxis.Vertical:
                        for (int i = 0; i < NumberOfPanels; i++)
                        {
                            Vector2 offset = new Vector2(0, contentSize.y);
                            if (GetDisplacementFromCenter(i).y > Content.rect.height / 2f)
                            {
                                Panels[i].anchoredPosition -= offset;
                            }
                            else
                            if (GetDisplacementFromCenter(i).y < Content.rect.height / -2f)
                            {
                                Panels[i].anchoredPosition += offset;
                            }
                        }
                        break;
                }
            }
        }
        private void HandleSwipeGestures()
        {
            if (useSwipeGestures)
            {
                ScrollRect.horizontal = (movementType == MovementType.Free) || (movementAxis == MovementAxis.Horizontal);
                ScrollRect.vertical = (movementType == MovementType.Free) || (movementAxis == MovementAxis.Vertical);
            }
            else
            {
                ScrollRect.horizontal = ScrollRect.vertical = !isDragging;
            }
        }
        private void HandleTransitionEffects()
        {
            if (onTransitionEffects.GetPersistentEventCount() == 0) return;

            for (int i = 0; i < NumberOfPanels; i++)
            {
                Vector2 displacement = GetDisplacementFromCenter(i);
                float d = (movementType == MovementType.Free) ? displacement.magnitude : ((movementAxis == MovementAxis.Horizontal) ? displacement.x : displacement.y);
                onTransitionEffects.Invoke(Panels[i].gameObject, d);
            }
        }

        private void SelectPanel()
        {
            int nearestPanel = GetNearestPanel();
            Vector2 displacementFromCenter = GetDisplacementFromCenter(nearestPanel);

            if (snapTarget == SnapTarget.Nearest || releaseSpeed <= minimumSwipeSpeed)
            {
                GoToPanel(nearestPanel);
            }
            else
            if (snapTarget == SnapTarget.Previous)
            {
                if ((releaseDirection == Direction.Right && displacementFromCenter.x < 0f) || (releaseDirection == Direction.Up && displacementFromCenter.y < 0f))
                {
                    GoToNextPanel();
                }
                else
                if ((releaseDirection == Direction.Left && displacementFromCenter.x > 0f) || (releaseDirection == Direction.Down && displacementFromCenter.y > 0f))
                {
                    GoToPreviousPanel();
                }
                else
                {
                    GoToPanel(nearestPanel);
                }
            }
            else
            if (snapTarget == SnapTarget.Next)
            {
                if ((releaseDirection == Direction.Right && displacementFromCenter.x > 0f) || (releaseDirection == Direction.Up && displacementFromCenter.y > 0f))
                {
                    GoToPreviousPanel();
                }
                else
                if ((releaseDirection == Direction.Left && displacementFromCenter.x < 0f) || (releaseDirection == Direction.Down && displacementFromCenter.y < 0f))
                {
                    GoToNextPanel();
                }
                else
                {
                    GoToPanel(nearestPanel);
                }
            }
        }
        private void SnapToPanel()
        {
            // (수정) 뷰포트 절반 대신 0을 사용하면, '왼쪽' 기준
            float xOffset = 0f;
            float yOffset = 0f;
            Vector2 offset = new Vector2(xOffset, yOffset);

            Vector2 targetPosition = - Panels[CenteredPanel].anchoredPosition + offset;
            Content.anchoredPosition = Vector2.Lerp(
                Content.anchoredPosition,
                targetPosition,
                (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * snapSpeed
            );

            if (SelectedPanel != CenteredPanel)
            {
                // 중심 판넬 갱신 로직은 그대로
                if (GetDisplacementFromCenter(CenteredPanel).magnitude < (Viewport.rect.width / 10f))
                {
                    onPanelCentered.Invoke(CenteredPanel, SelectedPanel);
                    SelectedPanel = CenteredPanel;
                }
            }
            else
            {
                onPanelCentering.Invoke(CenteredPanel, SelectedPanel);
            }
        }

        public void GoToPanel(int panelNumber)
        {
            CenteredPanel = panelNumber;
            isSelected = true;
            onPanelSelected.Invoke(SelectedPanel);

            if (pagination != null)
            {
                Toggles[panelNumber].isOn = true;
            }
            if (useHardSnapping)
            {
                ScrollRect.inertia = false;
            }
        }
        public void GoToPreviousPanel()
        {
            int nearestPanel = GetNearestPanel();
            if (nearestPanel != 0)
            {
                GoToPanel(nearestPanel - 1);
            }
            else
            {
                if (useInfiniteScrolling)
                {
                    GoToPanel(NumberOfPanels - 1);
                }
                else
                {
                    GoToPanel(nearestPanel);
                }
            }
        }
        public void GoToNextPanel()
        {
            int nearestPanel = GetNearestPanel();
            if (nearestPanel != (NumberOfPanels - 1))
            {
                GoToPanel(nearestPanel + 1);
            }
            else
            {
                if (useInfiniteScrolling)
                {
                    GoToPanel(0);
                }
                else
                {
                    GoToPanel(nearestPanel);
                }
            }
        }

        public void AddToFront(GameObject panel)
        {
            Add(panel, 0);
        }
        public void AddToBack(GameObject panel)
        {
            Add(panel, NumberOfPanels);
        }
        public void Add(GameObject panel, int index)
        {
            if (NumberOfPanels != 0 && (index < 0 || index > NumberOfPanels))
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> Index must be an integer from 0 to " + NumberOfPanels + ".", gameObject);
                return;
            }
            else if (!useAutomaticLayout)
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> \"Automatic Layout\" must be enabled for content to be dynamically added during runtime.");
                return;
            }

            panel = Instantiate(panel, Content, false);
            panel.transform.SetSiblingIndex(index);

            if (ValidConfig)
            {
                if (CenteredPanel <= index)
                {
                    startingPanel = CenteredPanel;
                }
                else
                {
                    startingPanel = CenteredPanel + 1;
                }
                Setup();
            }
        }

        public void AddInstantaiteBack(GameObject panel)
        {

            if (!useAutomaticLayout)
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> \"Automatic Layout\" must be enabled for content to be dynamically added during runtime.");
                return;
            }


            panel.transform.SetSiblingIndex(NumberOfPanels);

            if (ValidConfig)
            {
                if (CenteredPanel <= NumberOfPanels)
                {
                    startingPanel = CenteredPanel;
                }
                else
                {
                    startingPanel = CenteredPanel + 1;
                }
               // Setup();
            }
        }

        public void SetStartingPanel(int index)
        {
           
            startingPanel = index;
            Setup();
        }

        public void RemoveFromFront()
        {
            Remove(0);
        }
        public void RemoveFromBack()
        {
            if (NumberOfPanels > 0)
            {
                Remove(NumberOfPanels - 1);
            }
            else
            {
                Remove(0);
            }
        }
        public void Remove(int index)
        {
            if (NumberOfPanels == 0)
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> There are no panels to remove.", gameObject);
                return;
            }
            else if (index < 0 || index > (NumberOfPanels - 1))
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> Index must be an integer from 0 to " + (NumberOfPanels - 1) + ".", gameObject);
                return;
            }
            else if (!useAutomaticLayout)
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> \"Automatic Layout\" must be enabled for content to be dynamically removed during runtime.");
                return;
            }

            DestroyImmediate(Panels[index].gameObject);

            if (ValidConfig)
            {
                if (CenteredPanel == index)
                {
                    if (index == NumberOfPanels)
                    {
                        startingPanel = CenteredPanel - 1;
                    }
                    else
                    {
                        startingPanel = CenteredPanel;
                    }
                }
                else if (CenteredPanel < index)
                {
                    startingPanel = CenteredPanel;
                }
                else
                {
                    startingPanel = CenteredPanel - 1;
                }
                Setup();
            }
        }

        private Vector2 GetDisplacementFromCenter(int index)
        {
            Vector2 disp = Vector2.zero;

            if (movementAxis == MovementAxis.Horizontal)
            {
                // 수평 스크롤:
                //  - x축은 "왼쪽 = 0"
                //  - y축은 "뷰포트 중앙 = 0"
                float x = Panels[index].anchoredPosition.x + Content.anchoredPosition.x;
                float y = Panels[index].anchoredPosition.y + Content.anchoredPosition.y
                          // 뷰포트 세로길이 절반
                          - (Viewport.rect.height * (0.5f - Content.anchorMin.y));

                disp = new Vector2(x, y);
            }
            else
            {
                // 수직 스크롤:
                //  - x축은 "뷰포트 중앙 = 0"
                //  - y축은 "위쪽 = 0"
                float x = Panels[index].anchoredPosition.x + Content.anchoredPosition.x
                          // 뷰포트 가로길이 절반
                          - (Viewport.rect.width * (0.5f - Content.anchorMin.x));
                float y = Panels[index].anchoredPosition.y + Content.anchoredPosition.y;

                disp = new Vector2(x, y);
            }

            return disp;
        }
        private int GetNearestPanel()
        {
            float[] distances = new float[NumberOfPanels];
            for (int i = 0; i < Panels.Length; i++)
            {
                distances[i] = GetDisplacementFromCenter(i).magnitude;
            }

            int nearestPanel = 0;
            float minDistance = Mathf.Min(distances);
            for (int i = 0; i < Panels.Length; i++)
            {
                if (minDistance == distances[i])
                {
                    nearestPanel = i;
                    break;
                }
            }
            return nearestPanel;
        }
        private void GetVelocity()
        {
            Vector2 displacement = Content.anchoredPosition - prevAnchoredPosition;
            float time = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            velocity = displacement / time;
            prevAnchoredPosition = Content.anchoredPosition;
        }
        #endregion
    }
}