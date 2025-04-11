using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class SelectStatWindow : EditorWindow
{
    private List<EarthObjectStatData> dataList = new List<EarthObjectStatData>();
    private Vector2 scrollPos;
    private EarthObjectStatData selectedData;

    private bool bIsCompleted = false;

    private bool bIsModify = false;
    private string dataFolder = "Assets/HoleGame/StatData";
    

    //[MenuItem("Tools/select stat window")]
    public static void ShowWindow()
    {
        SelectStatWindow window = GetWindow<SelectStatWindow>("select stat window");
        window.minSize = new Vector2(425 , 600); 
        //window.maxSize = new Vector2(425 , 600); 
    }

    private void OnEnable()
    {
        LoadScriptableObjects();
    }

    private void LoadScriptableObjects()
    {
        dataList.Clear();
        // 폴더 내의 모든 MyData 에셋 찾기
        string[] guids = AssetDatabase.FindAssets("t:EarthObjectStatData", new string[] { dataFolder });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            EarthObjectStatData data = AssetDatabase.LoadAssetAtPath<EarthObjectStatData>(assetPath);
            if (data != null)
                dataList.Add(data);
        }
    }
    private void OnGUI()
    {
        //string assetpath = null;
        GUILayout.Label("ScriptableObject 목록", EditorStyles.boldLabel);
        GUILayout.Space(20);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));

        int columns = 5;
        int count = dataList.Count;
        int totalCells = count + 1;
        int rows = Mathf.CeilToInt((float)totalCells / columns);
        //int rows = Mathf.CeilToInt((float)count / columns);

        for (int row = 0; row < rows; row++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int col = 0; col < columns; col++)
            {
                int index = row * columns + col;
                if (index < count)
                {
                    //GUI.backgroundColor = dataList[index].bisdefaultStat ? Color.blue : Color.gray;
                    if (GUILayout.Button(dataList[index].name, GUILayout.Width(75), GUILayout.Height(75)))
                    {
                        bIsCompleted = false;
                        selectedData = dataList[index];
                       
                    }
                }
                else if (index == count) // 생성 버튼이 들어갈 셀
                {
                    GUI.backgroundColor = Color.gray;
                    if (GUILayout.Button("+ 생성", GUILayout.Width(75), GUILayout.Height(75)))
                    {
                        bIsCompleted = false;
                        string FirstDataName = "Custom";
                        CreateNewScriptableObject(FirstDataName);
                        LoadScriptableObjects(); // 목록 갱신
                    }
                }

            }
           
            EditorGUILayout.EndHorizontal();
        }
        

        EditorGUILayout.EndScrollView();

        GUILayout.Space(30);

        if (selectedData != null)
        {
            
            GUIStyle namestyle = EditorStyles.helpBox;
            namestyle.fontSize = 16;
            GUILayout.Label("데이터 이름 : " + selectedData.name, EditorStyles.helpBox, GUILayout.Height(30.0f));
            GUILayout.Space(10);

            //if (!selectedData.bisdefaultStat && !bIsModify)
            {
                GUILayout.Space(20);
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("커스텀 스텟 수정", GUILayout.Height(30)))
                {
                    bIsCompleted=false;
                    bIsModify = true;
                  
                }
                GUILayout.Space(20);
            }

            GUI.enabled = bIsModify;

            GUI.backgroundColor = Color.gray;
            selectedData.EXPCnt = EditorGUILayout.FloatField("경험치량", selectedData.EXPCnt);
            //selectedData.bRequired = EditorGUILayout.Toggle("클리어 목표", selectedData.bRequired);
            //selectedData.mass = EditorGUILayout.FloatField("질량", selectedData.mass);
           
            selectedData.bMovement = EditorGUILayout.Toggle("이동 여부", selectedData.bMovement);
            if (selectedData.bMovement)
            {
                selectedData.jumpDistance = EditorGUILayout.FloatField("점프(이동) 거리", selectedData.jumpDistance);
            }
            else
            {
                selectedData.squishAmount = EditorGUILayout.FloatField("바운스 정도", selectedData.squishAmount);
            }


            GUILayout.Space(20);
            if (bIsModify)
            {

                EditorGUILayout.BeginHorizontal();
               
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("수정 완료",  GUILayout.Height(40)))
                {
                    EditorUtility.SetDirty(selectedData);
                    AssetDatabase.SaveAssets();
                    bIsModify = false;
                    bIsCompleted = true;
                    LoadScriptableObjects();
                                                  
                }
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("삭제",  GUILayout.Height(40)))
                {
                    if (selectedData != null)
                    {
                        if (EditorUtility.DisplayDialog("삭제 확인", $"{selectedData.name}을(를) 삭제하시겠습니까?", "네", "아니오"))
                        {
                            string assetPath = AssetDatabase.GetAssetPath(selectedData);
                            AssetDatabase.DeleteAsset(assetPath);
                            selectedData = null;
                            bIsModify = false;
                            bIsCompleted = false;
                            LoadScriptableObjects();
                        }
                    }

                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                bIsCompleted = true;
            }


        }

        GUI.enabled = true;
        
        GUILayout.Space(20);
        if(bIsCompleted)
        {
            GUIStyle ImportButton = new GUIStyle(GUI.skin.button);
            ImportButton.fontSize = 20;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("완료", ImportButton))
            {
                MapEditorWindow.SetStatData(selectedData); 
                Close(); 
            }
        }
 
    }

   private void CreateNewScriptableObject(string name)
    {
        EarthObjectStatData newData = ScriptableObject.CreateInstance<EarthObjectStatData>();
        newData.name = name;
        string assetPath = $"{dataFolder}/{name}.asset";
        AssetDatabase.CreateAsset(newData, assetPath);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newData;
        
    }
}
