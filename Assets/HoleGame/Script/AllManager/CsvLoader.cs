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
}
