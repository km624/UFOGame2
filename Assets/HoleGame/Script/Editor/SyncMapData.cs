using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.AddressableAssets;

public class SyncMapData : EditorWindow
{
    // 폴더 경로 설정 (예: "Assets/Stages")
    private string stageDataFolder = "Assets/HoleGame/StageData";
    private string PrefabDataFolder = "Assets/HoleGame/Prefab/Stage";

    [MenuItem("Tools/Stage Sync Window")]
    public static void OpenWindow()
    {
        GetWindow<SyncMapData>("Stage Sync");
    }

    private void OnGUI()
    {
        GUILayout.Label("Stage Sync Tool", EditorStyles.boldLabel);

        // 폴더 입력 필드
       // stageDataFolder = EditorGUILayout.TextField("Stage Data Folder", stageDataFolder);

        if (GUILayout.Button("Sync All StageData & Prefabs"))
        {
            SyncAllStages(stageDataFolder);
        }
    }

    private void SyncAllStages(string folderPath)
    {
        // 1) folderPath에서 모든 StageData(.asset) 찾기
        string[] stageDataGuids = AssetDatabase.FindAssets("t:StageData", new[] { folderPath });
        if (stageDataGuids == null || stageDataGuids.Length == 0)
        {
            Debug.LogWarning($"No StageData found in folder: {folderPath}");
            return;
        }

        int syncCount = 0;
        foreach (string guid in stageDataGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            StageData stageData = AssetDatabase.LoadAssetAtPath<StageData>(assetPath);
            if (stageData == null) continue;

            // 2) StageData 이름에서 숫자 추출 (예: "Stage 3" → 3)
            //    이름 규칙: "Stage " 다음에 숫자가 온다고 가정
            if (!TryParseStageIndex(stageData.name, out int stageIndex))
            {
                Debug.LogWarning($"Could not parse stage index from {stageData.name}");
                continue;
            }

            // 3) 동일 인덱스의 프리팹 이름 예: "StagePrefab 3"
            //    (혹은 굳이 검색하지 않고, Addressable을 수동 할당할 수도 있음)
            string prefabName = $"StagePrefab {stageIndex}";

            // 4) 프리팹 검색
            string prefabGuid = FindPrefabByName(prefabName);
            if (string.IsNullOrEmpty(prefabGuid))
            {
                Debug.LogWarning($"Prefab '{prefabName}' not found for StageData '{stageData.name}'");
                continue;
            }

            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);

            // 5) 임시로 프리팹 Instantiate → ShapeEnum 카운트
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"Failed to load prefab at path: {prefabPath}");
                continue;
            }

            GameObject tempInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            //Debug.Log(tempInstance.gameObject.ToString());

            // 스캔
            var shapeObjects = tempInstance.GetComponentsInChildren<FallingObject>(true);

            // Dictionary 리셋
            stageData.RequiredShapeCnt.Clear();
            

            //프리팹 할당
            stageData.stagePrefab = new AssetReferenceGameObject(prefabGuid);
            // 임시 오브젝트 제거
            DestroyImmediate(tempInstance);

           /* Debug.Log(stageData.RequiredShapeCnt.Count);
            Debug.Log(stageData.RequiredShapeCnt[ShapeEnum.baseball]);*/
            EditorUtility.SetDirty(stageData);
            syncCount++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Stage Sync Complete. Updated {syncCount} StageData assets.");
    }

    /// <summary>
    /// "Stage 3" → 3 을 파싱
    /// </summary>
    private bool TryParseStageIndex(string stageName, out int stageIndex)
    {
        stageIndex = -1;
        // 예: stageName = "Stage 3"
        // 분리해서 끝에 숫자만 파싱
        // 간단하게 Split 써볼 수 있음
        // 예외 상황 처리는 자유
        string[] parts = stageName.Split(' ');
        if (parts.Length < 2) return false;

        return int.TryParse(parts[1], out stageIndex);
    }

    /// <summary>
    /// 해당 이름을 가진 prefab 파일을 찾고, 첫 번째 결과 GUID를 리턴
    /// </summary>
    private string FindPrefabByName(string prefabName)
    {
        // 모든 프리팹 검색 (비효율적일 수 있으니, 최적화 필요하면 folder 제한)
        string[] prefabGuids = AssetDatabase.FindAssets($"t:Prefab {prefabName}", new[] { PrefabDataFolder });
        //string[] stageDataGuids = AssetDatabase.FindAssets("t:StageData", new[] { folderPath });
        foreach (var guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // 파일명 체크
            if (Path.GetFileNameWithoutExtension(path) == prefabName)
            {
                return guid; // 첫 번째 일치
            }
        }
        return null;
    }
}