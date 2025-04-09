using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.AddressableAssets;

public class SyncMapData : EditorWindow
{
    // ���� ��� ���� (��: "Assets/Stages")
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

        // ���� �Է� �ʵ�
       // stageDataFolder = EditorGUILayout.TextField("Stage Data Folder", stageDataFolder);

        if (GUILayout.Button("Sync All StageData & Prefabs"))
        {
            SyncAllStages(stageDataFolder);
        }
    }

    private void SyncAllStages(string folderPath)
    {
        // 1) folderPath���� ��� StageData(.asset) ã��
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

            // 2) StageData �̸����� ���� ���� (��: "Stage 3" �� 3)
            //    �̸� ��Ģ: "Stage " ������ ���ڰ� �´ٰ� ����
            if (!TryParseStageIndex(stageData.name, out int stageIndex))
            {
                Debug.LogWarning($"Could not parse stage index from {stageData.name}");
                continue;
            }

            // 3) ���� �ε����� ������ �̸� ��: "StagePrefab 3"
            //    (Ȥ�� ���� �˻����� �ʰ�, Addressable�� ���� �Ҵ��� ���� ����)
            string prefabName = $"StagePrefab {stageIndex}";

            // 4) ������ �˻�
            string prefabGuid = FindPrefabByName(prefabName);
            if (string.IsNullOrEmpty(prefabGuid))
            {
                Debug.LogWarning($"Prefab '{prefabName}' not found for StageData '{stageData.name}'");
                continue;
            }

            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);

            // 5) �ӽ÷� ������ Instantiate �� ShapeEnum ī��Ʈ
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"Failed to load prefab at path: {prefabPath}");
                continue;
            }

            GameObject tempInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            //Debug.Log(tempInstance.gameObject.ToString());

            // ��ĵ
            var shapeObjects = tempInstance.GetComponentsInChildren<FallingObject>(true);

            // Dictionary ����
            stageData.RequiredShapeCnt.Clear();
            

            //������ �Ҵ�
            stageData.stagePrefab = new AssetReferenceGameObject(prefabGuid);
            // �ӽ� ������Ʈ ����
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
    /// "Stage 3" �� 3 �� �Ľ�
    /// </summary>
    private bool TryParseStageIndex(string stageName, out int stageIndex)
    {
        stageIndex = -1;
        // ��: stageName = "Stage 3"
        // �и��ؼ� ���� ���ڸ� �Ľ�
        // �����ϰ� Split �Ẽ �� ����
        // ���� ��Ȳ ó���� ����
        string[] parts = stageName.Split(' ');
        if (parts.Length < 2) return false;

        return int.TryParse(parts[1], out stageIndex);
    }

    /// <summary>
    /// �ش� �̸��� ���� prefab ������ ã��, ù ��° ��� GUID�� ����
    /// </summary>
    private string FindPrefabByName(string prefabName)
    {
        // ��� ������ �˻� (��ȿ������ �� ������, ����ȭ �ʿ��ϸ� folder ����)
        string[] prefabGuids = AssetDatabase.FindAssets($"t:Prefab {prefabName}", new[] { PrefabDataFolder });
        //string[] stageDataGuids = AssetDatabase.FindAssets("t:StageData", new[] { folderPath });
        foreach (var guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // ���ϸ� üũ
            if (Path.GetFileNameWithoutExtension(path) == prefabName)
            {
                return guid; // ù ��° ��ġ
            }
        }
        return null;
    }
}