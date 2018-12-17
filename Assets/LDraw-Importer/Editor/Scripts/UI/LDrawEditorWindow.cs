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

        private string[] _ModelNames;
        private string _CurrentPart;
        private int _CurrentIndex;
        private GeneratingType _CurrentType;

        private void OnEnable()
        {
            _ModelNames = LDrawConfig.Instance.ModelFileNames;
        }

        private void OnGUI()
        {
            GUILayout.Label("This is LDraw model importer for file format v1.0.2");
            if (GUILayout.Button("Update blueprints"))
            {
                LDrawConfig.Instance.InitParts();
                _ModelNames = LDrawConfig.Instance.ModelFileNames;
            }
            _CurrentType = (GeneratingType) EditorGUILayout.EnumPopup("Blueprint Type", _CurrentType);
            switch (_CurrentType)
            {
                    case GeneratingType.ByName:
                        _CurrentPart = EditorGUILayout.TextField("Name", _CurrentPart);
                        break;
                    case GeneratingType.Models:
                        _CurrentIndex = EditorGUILayout.Popup("Models", _CurrentIndex, _ModelNames);
                        break;
            }
      
            GenerateModelButton();
        }

        private void GenerateModelButton()
        {
            if (GUILayout.Button("Generate"))
            {
                _CurrentPart = _CurrentType == GeneratingType.ByName ? _CurrentPart 
                    : LDrawConfig.Instance.GetModelByFileName(_ModelNames[_CurrentIndex]); 
                // good test 949ac01
                var model = LDrawModel.Create(_CurrentPart, LDrawConfig.Instance.GetSerializedPart(_CurrentPart));
                var go = model.CreateMeshGameObject(LDrawConfig.Instance.ScaleMatrix);
                go.transform.LocalReflect(Vector3.up);
            }
        }
        private enum GeneratingType
        {
            ByName,
            Models
        }
        private const string PathToModels = "Assets/LDraw-Importer/Editor/base-parts/";
    }
}