using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectPrefabsWindow : EditorWindow
{
    private List<GameObject> prefabList = new List<GameObject>(); // �ҷ��� �����յ�
    private Vector2 scrollPos;
    private GameObject selectedPrefab;

    // �������� �ִ� ���� ��� (������Ʈ�� �°� ����)
    private string prefabFolder = "Assets/HoleGame/Prefab/EarthObject";

    //[MenuItem("Tools/Prefab Selector")]
    public static void ShowWindow()
    {
        GetWindow<SelectPrefabsWindow>("Prefab Selector");
    }

    private void OnEnable()
    {
        LoadPrefabs();
    }

    // �������� ��� �������� �ҷ��ɴϴ�.
    private void LoadPrefabs()
    {
        prefabList.Clear();
        // "t:Prefab" ���͸� ����Ͽ� �ش� ������ ������ GUID���� �˻�
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { prefabFolder });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab != null)
                prefabList.Add(prefab);
        }
    }

    private void OnGUI()
    {
        GUIStyle labeltext = new GUIStyle(EditorStyles.boldLabel);
        labeltext.fontSize = 14;
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("������ ����", labeltext);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        // ��ũ�Ѻ並 ����Ͽ� ������ �̸����⸦ �׸���� ǥ��
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        int columns = 4;
        int count = prefabList.Count;
        int rows = Mathf.CeilToInt((float)count / columns);

        for (int row = 0; row < rows; row++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int col = 0; col < columns; col++)
            {
                int index = row * columns + col;
                if (index >= count) break;

                GameObject prefab = prefabList[index];
                // AssetPreview�� ����Ͽ� �������� �̸����� �ؽ�ó ��������
               
                Texture2D previewTex = AssetPreview.GetAssetPreview(prefab);
                if (previewTex == null)
                {
                    // �̸����� �̹����� ������ �⺻ ������ ���
                    GUIContent content = EditorGUIUtility.ObjectContent(prefab, typeof(GameObject));
                    previewTex = (Texture2D)content.image;
                }

                // �̸����� ��ư (Ŭ�� �� ����)
                if (GUILayout.Button(new GUIContent(previewTex), GUILayout.Width(64), GUILayout.Height(64)))
                {
                    selectedPrefab = prefab;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        if (selectedPrefab != null)
        {
            GUIStyle ImportButton = new GUIStyle(GUI.skin.button);
            ImportButton.fontSize = 20;

            GUILayout.Space(10);
            GUILayout.Label("������ ������: " + selectedPrefab.name, EditorStyles.boldLabel);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("������ ���� �Ϸ�",ImportButton))
            {
                // ���õ� �������� ��ġ ������ ���� (��: ���� ������ �Ŵ����� ����)
                MapEditorWindow.SetSelectedPrefab(selectedPrefab);
                Close(); // â �ݱ�
            }
        }
    }
}
