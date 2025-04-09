using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor.PackageManager.UI;
using System.Collections.Generic;
using Codice.Client.BaseCommands;


public class MapEditorWindow : EditorWindow
{
    // ��ġ�� ������ (Inspector���� ����)
    public static GameObject prefab;

    private GameObject parentObject;
    private List<FallingObject> fallingObjectList = new List<FallingObject>();
    private Dictionary<ShapeEnum, int> ShapeCnt = new Dictionary<ShapeEnum, int>();
    private Dictionary<ShapeEnum, int> ShapeNotRequiredCnt = new Dictionary<ShapeEnum, int>();
    private Vector2 scrollPos;

    // ��ġ ��� Ȱ��ȭ ����
    private bool placementMode = false;

    // Scene View�� ǥ���� ������ �ν��Ͻ�
    private GameObject previewInstance;

    private static EarthObjectStatData statData =null;


    // ��ġ �������� ������ ����
    public Vector3 placementOrigin = Vector3.zero;

    /*//��� ���̿� �������� ( �⺻�� y = 0)
    Plane plane = new Plane(Vector3.up, Vector3.zero);*/

    private float planeHeight = 0f;

    // ������ �����츦 ���� ���� �޴� ������
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
            Debug.LogWarning("������ 'Objects' ������Ʈ�� ã�� �� �����ϴ�.");
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
            
            // Enum �̸��� Label�� ǥ��
            if (ShapeCnt.Count > 0)
            {
                EditorGUILayout.LabelField("��ǥ ����", EditorStyles.boldLabel);
                foreach (var kvp in ShapeCnt)
                {
                    EditorGUILayout.BeginHorizontal();

                    // Enum �̸��� Label�� ǥ��
                    EditorGUILayout.LabelField(kvp.Key.ToString(), GUILayout.Width(200));

                    // ������ IntField�� ǥ��
                    int newCount = EditorGUILayout.IntField(kvp.Value, GUILayout.Width(60));

                    // �� ������ �����Ǹ� ��ųʸ� ������Ʈ
                    if (newCount != kvp.Value)
                    {
                        ShapeCnt[kvp.Key] = newCount;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            if (ShapeNotRequiredCnt.Count > 0)
            {
                EditorGUILayout.LabelField("�� ��ǥ ����", EditorStyles.boldLabel);
                foreach (var kvp in ShapeNotRequiredCnt)
                {
                    EditorGUILayout.BeginHorizontal();

                    // Enum �̸��� Label�� ǥ��
                    EditorGUILayout.LabelField(kvp.Key.ToString(), GUILayout.Width(200));

                    // ������ IntField�� ǥ��
                    int newCount = EditorGUILayout.IntField(kvp.Value, GUILayout.Width(60));

                    // �� ������ �����Ǹ� ��ųʸ� ������Ʈ
                    if (newCount != kvp.Value)
                    {
                        ShapeCnt[kvp.Key] = newCount;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.LabelField("��ü", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("��ü ������Ʈ ��", GUILayout.Width(200));

            // ������ IntField�� ǥ��
            EditorGUILayout.IntField(fallingObjectList.Count, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();
        }
        GUI.enabled = true;
        //EditorGUILayout.EndScrollView();
        // ��ư ��Ÿ�� ����: ���� ũ��� ��Ʈ ������ ����
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
            GUILayout.Label("-�������� �����ϼ���-", LargeLabelBold, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();  
        }
        GUILayout.Space(10);

        // �ٸ� ���� ���� ��ư
        if (GUILayout.Button("������ ���� �� ����", ImportButton))
        {
            // SecondaryToolWindow�� �ٸ� ������ ������ Ŭ�����Դϴ�.
            GetWindow<SelectPrefabsWindow>("Prefab Selector");
        }
        GUILayout.Space(20);
        if (prefab != null)
        {
    
            // ������ ���� �ʵ�
            prefab = (GameObject)EditorGUILayout.ObjectField("��ġ�� ������", prefab, typeof(GameObject), false);
            GUILayout.Space(20);
            Texture2D previewTex = AssetPreview.GetAssetPreview(prefab);
            if (previewTex == null)
            {
                // �̸����� �̹����� ������ �⺻ ������ ���
                GUIContent content = EditorGUIUtility.ObjectContent(prefab, typeof(GameObject));
                previewTex = (Texture2D)content.image;
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box(new GUIContent(previewTex), GUILayout.Width(100), GUILayout.Height(100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            if (GUILayout.Button("������ ���� ����", ImportButton))
            {
                
                GetWindow<SelectStatWindow>("Stat Selector");
            }
           
        }

        GUILayout.Space(20);
        // �������� ���õǾ��� �� ��ġ ��� ��� ��ư ǥ��
        if(prefab != null && statData != null)
        {
            GUIStyle namestyle = EditorStyles.helpBox;
            namestyle.fontSize = 16;
            GUILayout.Label("����� ���� : " + statData.name, EditorStyles.helpBox, GUILayout.Height(30.0f));
            GUILayout.Space(10);

            GUI.enabled = false;
            EditorGUILayout.FloatField("����ġ��", statData.EXPCnt);
            EditorGUILayout.Toggle("Ŭ���� ��ǥ", statData.bRequired);
            EditorGUILayout.FloatField("����", statData.mass);
            EditorGUILayout.Toggle("�̵� ����", statData.bMovement);
            if (statData.bMovement)
            {
                EditorGUILayout.FloatField("����(�̵�) �Ÿ�", statData.jumpDistance);
            }
            else
            {
                EditorGUILayout.FloatField("�ٿ ����", statData.squishAmount);
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
            GUILayout.Label("������ ��ġ ����\n\nŰ���� Z : ����  Ű���� X : �Ʒ���\nŰ���� Q : ����  Ű���� E : ������", helpStyle);
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUIStyle CarefulStyle = new GUIStyle(EditorStyles.boldLabel);
            CarefulStyle.fontSize = 18;
            CarefulStyle.normal.textColor = Color.red;
            CarefulStyle.hover.textColor = Color.red;
           
       
            GUILayout.Label("- ȭ�鿡 ��Ŭ������ ���� ��ġ!!! -", CarefulStyle);

            GUILayout.Space(20);
            GUI.backgroundColor = placementMode? Color.red : Color.green;
            if (GUILayout.Button(placementMode ? "������ ��ġ ����" : "������ ��ġ ����", ImportButton))
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
    // ������ �ν��Ͻ� ����
    private void CreatePreview()
    {
        if (prefab != null)
        {
            previewInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (previewInstance != null)
            {
               
                previewInstance.hideFlags = HideFlags.HideAndDontSave;
                // �ʿ信 ���� Collider�� ��ũ��Ʈ�� ��Ȱ��ȭ�Ͽ� ���� �� ������ ���� �� ����
            }
        }
    }

    // ������ �ν��Ͻ� ����
    private void DestroyPreview()
    {
        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
            previewInstance = null;
            
        }
    }

    // Scene View���� ���콺 �Է��� ó���ϴ� �ݹ�
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
            // Scene View�� ���콺 ��ġ�κ��� Ray ����
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

            // ��Ŭ�� �̺�Ʈ ���� (e.button == 0)
            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                PlacePrefabAt(previewInstance.transform.position, previewInstance.transform.rotation);
                e.Use(); // �̺�Ʈ �Һ� (�ٸ� ���� ���޵��� �ʵ���)
            }
        }
        SceneView.RepaintAll();
    }

    // ������ �������� ��ġ�ϴ� �Լ�
    private void PlacePrefabAt(Vector3 position,Quaternion rotate)
    {
        if (prefab == null) return;

        // ������ ȯ�濡���� PrefabUtility�� ����ؼ� ���� (Undo ����)
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
            // �� �������� ���� ��� (360���� �յ� ����)
            float angle = i * 360f / prefabCount;
            float rad = angle * Mathf.Deg2Rad;
            // ���� �ٱ��� ��ġ ���
            Vector3 pos = center + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;

            // ������ �ν��Ͻ�ȭ (Undo ����)
            GameObject placedObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            placedObj.transform.position = pos;

            // �߽ɿ��� �ٱ� �������� ���ϵ��� ȸ��
            Vector3 direction = (pos - center).normalized;
            placedObj.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            // �θ� ������Ʈ�� �ִٸ� ���� (��: parentObject)
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