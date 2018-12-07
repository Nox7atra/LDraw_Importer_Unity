using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LDrawConfig : ScriptableObject
{
    [SerializeField] private string _BlueprintsPath;
    [SerializeField] private string _ColorConfigPath;
    [SerializeField] private float _Scale;
    private Dictionary<string, string> _PartsPaths;
    
    public Matrix4x4 ScaleMatrix
    {
        get { return Matrix4x4.Scale(new Vector3(_Scale, _Scale, _Scale)); }
    }

    public string GetPartPath(string name)
    {
        InitParts();
        Debug.Log(name);
        return _PartsPaths[name.ToLower()];
    }

    private void InitParts()
    {
        _PartsPaths = new Dictionary<string, string>();
        var files = Directory.GetFiles(_BlueprintsPath, "*.*", SearchOption.AllDirectories);
        
        foreach (var file in files)
        {
            if (!file.Contains(".meta"))
            {
                if(!_PartsPaths.ContainsKey(Path.GetFileNameWithoutExtension(file)))
                    _PartsPaths.Add(Path.GetFileNameWithoutExtension(file), file);
            }
        }
    }
    private static LDrawConfig _Instance;
    public static LDrawConfig Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = AssetDatabase.LoadAssetAtPath<LDrawConfig>(ConfigPath);
            }

            return _Instance;
        }
    }
    private const string ConfigPath = "Assets/LDraw-Importer/Editor/Config.asset";
}
