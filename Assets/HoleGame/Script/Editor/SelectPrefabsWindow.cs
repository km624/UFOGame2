using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectPrefabsWindow : EditorWindow
{
    private List<GameObject> prefabList = new List<GameObject>(); // 불러온 프리팹들
    private Vector2 scrollPos;
    private GameObject selectedPrefab;

    // 프리팹이 있는 폴더 경로 (프로젝트에 맞게 수정)
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

    // 폴더에서 모든 프리팹을 불러옵니다.
    private void LoadPrefabs()
    {
        prefabList.Clear();
        // "t:Prefab" 필터를 사용하여 해당 폴더의 프리팹 GUID들을 검색
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
        GUILayout.Label("프리팹 선택", labeltext);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        // 스크롤뷰를 사용하여 프리팹 미리보기를 그리드로 표시
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
                // AssetPreview를 사용하여 프리팹의 미리보기 텍스처 가져오기
               
                Texture2D previewTex = AssetPreview.GetAssetPreview(prefab);
                if (previewTex == null)
                {
                    // 미리보기 이미지가 없으면 기본 아이콘 사용
                    GUIContent content = EditorGUIUtility.ObjectContent(prefab, typeof(GameObject));
                    previewTex = (Texture2D)content.image;
                }

                // 미리보기 버튼 (클릭 시 선택)
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
            GUILayout.Label("선택한 프리팹: " + selectedPrefab.name, EditorStyles.boldLabel);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("프리팹 선택 완료",ImportButton))
            {
                // 선택된 프리팹을 배치 도구에 전달 (예: 정적 변수나 매니저를 통해)
                MapEditorWindow.SetSelectedPrefab(selectedPrefab);
                Close(); // 창 닫기
            }
        }
    }
}
