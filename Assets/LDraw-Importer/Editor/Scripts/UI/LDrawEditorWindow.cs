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
        private ModelType _CurrentType;
        private void OnGUI()
        {
            GUILayout.Label("This is LDraw model importer for file format v1.0.2");
            _CurrentType = (ModelType) EditorGUILayout.EnumPopup("Blueprint Type", _CurrentType);
          
            _CurrentPart = EditorGUILayout.TextField("Name", _CurrentPart);
            GenerateModelButton();
        }

        private void GenerateModelButton()
        {
            if (GUILayout.Button("Generate"))
            {
                // good test 949ac01
                var model = LDrawModel.Create(_CurrentPart, LDrawConfig.Instance.GetSerializedPart(_CurrentPart));
                var go = model.CreateMeshGameObject(LDrawConfig.Instance.ScaleMatrix);
                go.transform.LocalReflect(Vector3.up);
            }
        }
        private enum ModelType
        {
            DefaultParts,
            Models
        }
        private const string PathToModels = "Assets/LDraw-Importer/Editor/base-parts/";
    }
}