using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenrationObjects", menuName = "Scriptable Objects/GenrationObjects")]
public class GenerationObjects : ScriptableObject
{
    public List<FallingObject> objects = new List<FallingObject>();

    public FallingObject bomb = null;

    public float[] spawnWeights;

    private void OnValidate()
    {
        // objects 리스트가 null이 아니고, spawnWeights 배열의 길이가 리스트 개수와 다를 경우 처리
        if (objects != null)
        {
            int count = objects.Count;
            // spawnWeights가 null이거나 배열 길이가 맞지 않으면 새로운 배열로 교체 (기존 값은 보존)
            if (spawnWeights == null || spawnWeights.Length != count)
            {
                float[] newWeights = new float[count];
                for (int i = 0; i < count; i++)
                {
                    // 기존에 값이 있다면 그대로 유지, 없으면 기본값 1을 사용 (원하는 기본값으로 설정 가능)
                    newWeights[i] = (spawnWeights != null && i < spawnWeights.Length) ? spawnWeights[i] : 0.1f;
                }
                spawnWeights = newWeights;
#if UNITY_EDITOR
                // 에디터 로그로 알림 (선택 사항)
                Debug.Log($"spawnWeights 배열이 objects 리스트의 갯수({count})에 맞춰 업데이트되었습니다.");
#endif
            }
        }
    }
}
