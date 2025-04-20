using System.Collections.Generic;
using System;
using UnityEngine;

public static class CsvLoader
{
    public static List<T> LoadCSV<T>(string resourcePath) where T : new()
    {
        TextAsset csvFile = Resources.Load<TextAsset>(resourcePath);
        string[] lines = csvFile.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var props = typeof(T).GetFields();
        List<T> list = new();

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            string[] tokens = lines[i].Split(',');
            if (tokens.Length == 0) continue;

            T obj = new T();
            for (int j = 0; j < props.Length && j < tokens.Length; j++)
            {
                var field = props[j];
                var val = Convert.ChangeType(tokens[j], field.FieldType);
                field.SetValue(obj, val);
            }
            list.Add(obj);
        }

        return list;
    }

    public static T LoadSingleCSV<T>(string resourcePath) where T : new()
    {
        TextAsset csvFile = Resources.Load<TextAsset>(resourcePath);
        if (csvFile == null)
        {
            Debug.LogError($"CSV ������ ã�� �� �����ϴ�: {resourcePath}");
            return default;
        }

        string[] lines = csvFile.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
        {
            Debug.LogWarning("CSV�� �����Ͱ� �����ϴ�.");
            return default;
        }

        var props = typeof(T).GetFields();

        string[] tokens = lines[1].Split(',');

        T obj = new T();

        for (int i = 0; i < props.Length && i < tokens.Length; i++)
        {
            var field = props[i];
            string token = tokens[i].Trim();

            try
            {
                object val = Convert.ChangeType(token, field.FieldType);
                field.SetValue(obj, val);
            }
            catch (Exception ex)
            {
                Debug.LogError($"�ʵ� {field.Name} �� ��ȯ ����: '{token}' �� {field.FieldType.Name} / {ex.Message}");
            }
        }

        return obj;
    }
}
