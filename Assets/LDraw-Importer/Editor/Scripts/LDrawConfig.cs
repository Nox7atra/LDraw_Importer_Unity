using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace LDraw
{
    public class LDrawConfig : ScriptableObject
    {
        [SerializeField] private string _BasePartsPath;
        [SerializeField] private string _ModelsPath;
        [SerializeField] private string _ColorConfigPath;
        [SerializeField] private float _Scale;
        [SerializeField] private Material _DefaultMaterial;
        private Dictionary<string, string> _Parts;
        private Dictionary<string, string> _Models;
        
        private Dictionary<int, Color> _Colors;

        public Matrix4x4 ScaleMatrix
        {
            get { return Matrix4x4.Scale(new Vector3(_Scale, _Scale, _Scale)); }
        }

        public Material GetColoredMaterial(int code)
        {
            var material = new Material(_DefaultMaterial);
            material.color = _Colors[code];
            return material;
        }
        
        public string GetSerializedPart(string name)
        {
            InitParts();
            Debug.Log(name);
            return _Parts[name.ToLower()];
        }

     
        private void InitParts()
        {
            ParseColors();
            _Parts = new Dictionary<string, string>();
            var files = Directory.GetFiles(_BasePartsPath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (!file.Contains(".meta"))
                {
                    string fileName = file.Replace(_BasePartsPath, "").Split('.')[0];
                   
                    if (!_Parts.ContainsKey(fileName))
                        _Parts.Add(fileName, File.ReadAllText(file));
                }
            }
        }

        private void ParseColors()
        {
            _Colors = new Dictionary<int, Color>();
            using (StreamReader reader = new StreamReader(_ColorConfigPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Regex regex = new Regex("[ ]{2,}", RegexOptions.None);
                    line = regex.Replace(line, " ").Trim();
                    var args = line.Split(' ');
                    if (args.Length  > 1 && args[1] == "!COLOUR")
                    {
                        Color color;
                        if(ColorUtility.TryParseHtmlString(args[6], out color))
                            _Colors.Add(int.Parse(args[4]),color);
                    }
                }
            }
        }
        
        private void PrepareModels()
        {
            var files = Directory.GetFiles(_ModelsPath, "*.*", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    string line;
                    string filename = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Regex regex = new Regex("[ ]{2,}", RegexOptions.None);
                        line = regex.Replace(line, " ").Trim();
                        var args = line.Split(' ');
                        if (args.Length  > 1 && args[1] == "!File")
                        {
                            filename = GetFileName(args, 2);
                            _Models.Add(filename, String.Empty);
                        }

                        if (!string.IsNullOrEmpty(filename))
                        {
                            _Models[filename] += line + "\n";
                        }
                    } 
                }
                
            }
        }
        public static string GetFileName(string[] args, int filenamePos)
        {
            string name = string.Empty;
            for (int i = filenamePos; i < args.Length; i++)
            {
                name += args[i];
            }

            return Path.GetFileNameWithoutExtension(name);
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
        public const int DefaultMaterialCode = 16;
    }
}