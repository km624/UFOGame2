using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor.PackageManager.UI;
using System.Collections.Generic;
using Codice.Client.BaseCommands;


public class MapEditorWindow : EditorWindow
{
    // 배치할 프리팹 (Inspector에서 선택)
    public static GameObject prefab;

    private GameObject parentObject;
    private List<FallingObject> fallingObjectList = new List<FallingObject>();
    private Dictionary<ShapeEnum, int> ShapeCnt = new Dictionary<ShapeEnum, int>();
    private Dictionary<ShapeEnum, int> ShapeNotRequiredCnt = new Dictionary<ShapeEnum, int>();
    private Vector2 scrollPos;

    // 배치 모드 활성화 여부
    private bool placementMode = false;

    // Scene View에 표시할 프리뷰 인스턴스
    private GameObject previewInstance;

    private static EarthObjectStatData statData =null;


    // 배치 시작점을 저장할 변수
    public Vector3 placementOrigin = Vector3.zero;

    /*//어느 높이에 생성할지 ( 기본값 y = 0)
    Plane plane = new Plane(Vector3.up, Vector3.zero);*/

    private float planeHeight = 0f;

    // 에디터 윈도우를 열기 위한 메뉴 아이템
    [MenuItem("Tools/Map Editor")]
    public static void ShowWindow()
    {
        MapEditorWindow window = GetWindow<MapEditorWindow>("Map Editor");
        window.minSize = new Vector2(425, 800);
       
    }

    private void OnEnable()
    {
        
        parentObject = GameObject.Find("Objects");
        if (parentObject == null)
        {
            Debug.LogWarning("씬에서 'Objects' 오브젝트를 찾을 수 없습니다.");
        }
        FindAllObject();
        Undo.undoRedoPerformed += OnUndoRedo;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        Undo.undoRedoPerformed -= OnUndoRedo;
        DestroyPreview();
        prefab = null;
        statData =null;
    }

    void FindAllObject()
    {
        ShapeCnt.Clear();
        fallingObjectList.Clear();
        ShapeNotRequiredCnt.Clear();
        if (parentObject == null) return;

        fallingObjectList = new List<FallingObject>(parentObject.GetComponentsInChildren<FallingObject>(true));

    }

    private void OnGUI()
    {
       
        GUI.enabled = false;
        if (fallingObjectList.Count > 0)
        {
            
            // Enum 이름을 Label로 표시
            if (ShapeCnt.Count > 0)
            {
                EditorGUILayout.LabelField("목표 도형", EditorStyles.boldLabel);
                foreach (var kvp in ShapeCnt)
                {
                    EditorGUILayout.BeginHorizontal();

                    // Enum 이름을 Label로 표시
                    EditorGUILayout.LabelField(kvp.Key.ToString(), GUILayout.Width(200));

                    // 개수를 IntField로 표시
                    int newCount = EditorGUILayout.IntField(kvp.Value, GUILayout.Width(60));

                    // 값 변경이 감지되면 딕셔너리 업데이트
                    if (newCount != kvp.Value)
                    {
                        ShapeCnt[kvp.Key] = newCount;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            if (ShapeNotRequiredCnt.Count > 0)
            {
                EditorGUILayout.LabelField("비 목표 도형", EditorStyles.boldLabel);
                foreach (var kvp in ShapeNotRequiredCnt)
                {
                    EditorGUILayout.BeginHorizontal();

                    // Enum 이름을 Label로 표시
                    EditorGUILayout.LabelField(kvp.Key.ToString(), GUILayout.Width(200));

                    // 개수를 IntField로 표시
                    int newCount = EditorGUILayout.IntField(kvp.Value, GUILayout.Width(60));

                    // 값 변경이 감지되면 딕셔너리 업데이트
                    if (newCount != kvp.Value)
                    {
                        ShapeCnt[kvp.Key] = newCount;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.LabelField("전체", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("전체 오브젝트 수", GUILayout.Width(200));

            // 개수를 IntField로 표시
            EditorGUILayout.IntField(fallingObjectList.Count, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();
        }
        GUI.enabled = true;
        //EditorGUILayout.EndScrollView();
        // 버튼 스타일 생성: 고정 크기와 폰트 사이즈 설정
        GUIStyle ImportButton = new GUIStyle(GUI.skin.button);
        ImportButton.fontSize = 16;

        GUILayout.Space(10);
        if (prefab == null)
        {
            GUIStyle LargeLabelBold = new GUIStyle(EditorStyles.boldLabel);
            LargeLabelBold.fontSize = 16;
            LargeLabelBold.clipping = TextClipping.Overflow;
            LargeLabelBold.normal.textColor = Color.red;
            LargeLabelBold.hover.textColor = Color.red;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("-프리팹을 선택하세요-", LargeLabelBold, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();  
        }
        GUILayout.Space(10);

        // 다른 툴을 띄우는 버튼
        if (GUILayout.Button("프리팹 선택 툴 열기", ImportButton))
        {
            // SecondaryToolWindow는 다른 에디터 윈도우 클래스입니다.
            GetWindow<SelectPrefabsWindow>("Prefab Selector");
        }
        GUILayout.Space(20);
        if (prefab != null)
        {
    
            // 프리팹 선택 필드
            prefab = (GameObject)EditorGUILayout.ObjectField("배치할 프리팹", prefab, typeof(GameObject), false);
            GUILayout.Space(20);
            Texture2D previewTex = AssetPreview.GetAssetPreview(prefab);
            if (previewTex == null)
            {
                // 미리보기 이미지가 없으면 기본 아이콘 사용
                GUIContent content = EditorGUIUtility.ObjectContent(prefab, typeof(GameObject));
                previewTex = (Texture2D)content.image;
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box(new GUIContent(previewTex), GUILayout.Width(100), GUILayout.Height(100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            if (GUILayout.Button("프리팹 스텟 선택", ImportButton))
            {
                
                GetWindow<SelectStatWindow>("Stat Selector");
            }
           
        }

        GUILayout.Space(20);
        // 프리팹이 선택되었을 때 배치 모드 토글 버튼 표시
        if(prefab != null && statData != null)
        {
            GUIStyle namestyle = EditorStyles.helpBox;
            namestyle.fontSize = 16;
            GUILayout.Label("적용된 스텟 : " + statData.name, EditorStyles.helpBox, GUILayout.Height(30.0f));
            GUILayout.Space(10);

            GUI.enabled = false;
            EditorGUILayout.FloatField("경험치량", statData.EXPCnt);
            EditorGUILayout.Toggle("클리어 목표", statData.bRequired);
            EditorGUILayout.FloatField("질량", statData.mass);
            EditorGUILayout.Toggle("이동 여부", statData.bMovement);
            if (statData.bMovement)
            {
                EditorGUILayout.FloatField("점프(이동) 거리", statData.jumpDistance);
            }
            else
            {
                EditorGUILayout.FloatField("바운스 정도", statData.squishAmount);
            }
            GUI.enabled = true;
        }

        GUILayout.Space(10);

        if (prefab != null && statData!=null)
        {
           
            GUIStyle helpStyle = new GUIStyle(EditorStyles.helpBox);
            helpStyle.fontSize = 13;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("프리팹 위치 변경\n\n키보드 Z : 위쪽  키보드 X : 아래쪽\n키보드 Q : 왼쪽  키보드 E : 오른쪽", helpStyle);
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUIStyle CarefulStyle = new GUIStyle(EditorStyles.boldLabel);
            CarefulStyle.fontSize = 18;
            CarefulStyle.normal.textColor = Color.red;
            CarefulStyle.hover.textColor = Color.red;
           
       
            GUILayout.Label("- 화면에 우클릭으로 먼저 터치!!! -", CarefulStyle);

            GUILayout.Space(20);
            GUI.backgroundColor = placementMode? Color.red : Color.green;
            if (GUILayout.Button(placementMode ? "프리팹 배치 종료" : "프리팹 배치 시작", ImportButton))
            {
                placementMode = !placementMode;
                
                if (placementMode)
                {
                    
                    CreatePreview();
                   

                }
                else
                {
                    
                    DestroyPreview();
                   
                }
            }
            GUILayout.Space(10);
            GUI.enabled = false;
           
            GUI.backgroundColor= Color.white;
            

        }
    }

    public static void SetSelectedPrefab(GameObject selectprefab)
    {
        if (selectprefab == null) return;

        prefab = selectprefab;

    }

    public static void SetStatData(EarthObjectStatData data)
    {
        if(data == null) return;
        statData = data;
    }
    // 프리뷰 인스턴스 생성
    private void CreatePreview()
    {
        if (prefab != null)
        {
            previewInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (previewInstance != null)
            {
               
                previewInstance.hideFlags = HideFlags.HideAndDontSave;
                // 필요에 따라 Collider나 스크립트를 비활성화하여 편집 중 간섭을 막을 수 있음
            }
        }
    }

    // 프리뷰 인스턴스 삭제
    private void DestroyPreview()
    {
        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
            previewInstance = null;
            
        }
    }

    // Scene View에서 마우스 입력을 처리하는 콜백
    private void OnSceneGUI(SceneView sceneView)
    {
        if (placementMode && previewInstance != null)
        {

            Event e = Event.current;
            SceneView.FocusWindowIfItsOpen(typeof(SceneView));

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
           HandleUtility.AddDefaultControl(controlID);

            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Alpha1)
                {
                    planeHeight += 0.1f;
                    e.Use();
                    Repaint();
                }
                else if (e.keyCode == KeyCode.Alpha2)
                {
                    planeHeight -= 0.1f;
                    e.Use();
                    Repaint();
                }
            }

            Plane plane = new Plane(Vector3.up, new Vector3(0, planeHeight, 0));
            // Scene View의 마우스 위치로부터 Ray 생성
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            
           
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                float gridSize = 1f;
                hitPoint.x = Mathf.Round(hitPoint.x / gridSize) * gridSize;
                hitPoint.z = Mathf.Round(hitPoint.z / gridSize) * gridSize;
                previewInstance.transform.position = hitPoint;
            }

            float rotationAmount = 10f; 
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Q)
                {
                    
                   
                    previewInstance.transform.Rotate(Vector3.up, -rotationAmount, Space.World);
                    e.Use(); 
                }
                else if (e.keyCode == KeyCode.E)
                {
                    
                    previewInstance.transform.Rotate(Vector3.up, rotationAmount, Space.World);
                    e.Use();
                }
                else if (e.keyCode == KeyCode.Escape)
                {
                    placementMode = !placementMode;
                    DestroyPreview();
                }
               
            }

            // 좌클릭 이벤트 감지 (e.button == 0)
            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                PlacePrefabAt(previewInstance.transform.position, previewInstance.transform.rotation);
                e.Use(); // 이벤트 소비 (다른 곳에 전달되지 않도록)
            }
        }
        SceneView.RepaintAll();
    }

    // 실제로 프리팹을 배치하는 함수
    private void PlacePrefabAt(Vector3 position,Quaternion rotate)
    {
        if (prefab == null) return;

        // 에디터 환경에서는 PrefabUtility를 사용해서 생성 (Undo 지원)
        GameObject placedObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        FallingObject fallingObj = placedObj.GetComponent<FallingObject>();
        if (fallingObj != null)
        {
            if(statData!=null)
                fallingObj.SetStatData(statData);
        }
        placedObj.transform.position = position;
        placedObj.transform.rotation = rotate;
        if (parentObject != null)
        {
            placedObj.transform.SetParent(parentObject.transform);
        }
        FindAllObject();
       
        Undo.RegisterCreatedObjectUndo(placedObj, "Place Prefab");
        Repaint();
    }

    private void PlacePrefabsInCircle(Vector3 center, float radius, int prefabCount)
    {
        for (int i = 0; i < prefabCount; i++)
        {
            // 각 프리팹의 각도 계산 (360도를 균등 분할)
            float angle = i * 360f / prefabCount;
            float rad = angle * Mathf.Deg2Rad;
            // 원의 바깥쪽 위치 계산
            Vector3 pos = center + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;

            // 프리팹 인스턴스화 (Undo 지원)
            GameObject placedObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            placedObj.transform.position = pos;

            // 중심에서 바깥 방향으로 향하도록 회전
            Vector3 direction = (pos - center).normalized;
            placedObj.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            // 부모 오브젝트가 있다면 설정 (예: parentObject)
            if (parentObject != null)
                placedObj.transform.SetParent(parentObject.transform);

            Undo.RegisterCreatedObjectUndo(placedObj, "Place Prefab in Circle");
        }
    }
    private void OnUndoRedo()
    {
       
        FindAllObject();
        Repaint();
    }


}