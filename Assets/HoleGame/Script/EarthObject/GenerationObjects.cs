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
        // objects ����Ʈ�� null�� �ƴϰ�, spawnWeights �迭�� ���̰� ����Ʈ ������ �ٸ� ��� ó��
        if (objects != null)
        {
            int count = objects.Count;
            // spawnWeights�� null�̰ų� �迭 ���̰� ���� ������ ���ο� �迭�� ��ü (���� ���� ����)
            if (spawnWeights == null || spawnWeights.Length != count)
            {
                float[] newWeights = new float[count];
                for (int i = 0; i < count; i++)
                {
                    // ������ ���� �ִٸ� �״�� ����, ������ �⺻�� 1�� ��� (���ϴ� �⺻������ ���� ����)
                    newWeights[i] = (spawnWeights != null && i < spawnWeights.Length) ? spawnWeights[i] : 0.1f;
                }
                spawnWeights = newWeights;
#if UNITY_EDITOR
                // ������ �α׷� �˸� (���� ����)
                Debug.Log($"spawnWeights �迭�� objects ����Ʈ�� ����({count})�� ���� ������Ʈ�Ǿ����ϴ�.");
#endif
            }
        }
    }
}
