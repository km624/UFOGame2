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
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: {resourcePath}");
            return default;
        }

        string[] lines = csvFile.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
        {
            Debug.LogWarning("CSV에 데이터가 없습니다.");
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
                Debug.LogError($"필드 {field.Name} 값 변환 오류: '{token}' → {field.FieldType.Name} / {ex.Message}");
            }
        }

        return obj;
    }
}
