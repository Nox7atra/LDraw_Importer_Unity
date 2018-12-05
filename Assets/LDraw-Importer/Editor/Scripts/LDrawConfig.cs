using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LDrawConfig : ScriptableObject
{
    [SerializeField] private string _BlueprintsPath;
    [SerializeField] private string _ColorConfigPath;
    private Dictionary<string, string> _PartsPaths;

    public Dictionary<string, string> PartsPaths
    {
        get { return _PartsPaths; }
    }

    public string BlueprintsPath
    {
        get { return _BlueprintsPath; }
    }


    public string GetPartPath(string name)
    {
        InitParts();
        return _PartsPaths[name];
    }

    private void InitParts()
    {
        if(_PartsPaths != null) return;
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
    private const string ConfigPath = "/Assets/Editor/LDraw-Importer/Editor/Config.asset";
}
