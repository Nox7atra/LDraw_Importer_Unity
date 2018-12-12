using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LDraw
{
    public class LDrawConfig : ScriptableObject
    {
        [SerializeField] private string _BasePartsPath;
        [SerializeField] private string _ColorConfigPath;
        [SerializeField] private float _Scale;
        private Dictionary<string, string> _Parts;

        public Matrix4x4 ScaleMatrix
        {
            get { return Matrix4x4.Scale(new Vector3(_Scale, _Scale, _Scale)); }
        }

        public string GetModelPath(string name)
        {
            InitParts();
            Debug.Log(name);
            return _Parts[name.ToLower()];
        }

        private void InitParts()
        {
            _Parts = new Dictionary<string, string>();
            var files = Directory.GetFiles(_BasePartsPath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (!file.Contains(".meta"))
                {
                    string extention = Path.GetExtension(file);
                    string fileName = file.Replace(_BasePartsPath, "").Split('.')[0];
                    switch (extention)
                    {
                        default:
                            if (!_Parts.ContainsKey(fileName))
                                _Parts.Add(fileName, file);
                            break;
                        case ".mpd":
                            break;
                    }

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
}