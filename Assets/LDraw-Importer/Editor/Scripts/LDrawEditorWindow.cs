using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LDraw;
using UnityEditor;
using UnityEngine;

namespace LDraw
{
    public class LDrawEditorWindow : EditorWindow
    {
        [MenuItem("Window/LDrawImporter/Open Importer")]
        public static void Create()
        {
            var window = GetWindow<LDrawEditorWindow>("LDrawImporter");
            window.position = new Rect(100, 100, 400, 400);
            window.Show();
        }

        private string _CurrentPart;

        private void OnGUI()
        {
            _CurrentPart = EditorGUILayout.TextField("Name", _CurrentPart);
            GenerateModelButton();
        }

        private void GenerateModelButton()
        {
            if (GUILayout.Button("Generate"))
            {
                var model = LDrawModel.Create(_CurrentPart, LDrawConfig.Instance.GetModelPath(_CurrentPart));
                var go = model.CreateMeshGameObject(LDrawConfig.Instance.ScaleMatrix);
                go.transform.LocalReflect(Vector3.up);
            }
        }

        private const string PathToModels = "Assets/LDraw-Importer/Editor/base-parts/";
    }
}