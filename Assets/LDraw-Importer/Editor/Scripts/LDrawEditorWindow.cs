using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LDraw;
using UnityEditor;
using UnityEngine;

namespace LDraw
{
    

}
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
    private void OnEnable()
    {
   
    }

    private void OnGUI()
    {
       
        _CurrentPart = GUILayout.TextField(_CurrentPart);
        GenerateModelButton();
        
        if (Event.current.Equals(Event.KeyboardEvent("#v")))
            _CurrentPart = EditorGUIUtility.systemCopyBuffer;
    }

    private void GenerateModelButton()
    {
        if (GUILayout.Button("Generate"))
        {
            var model = LDrawModel.Create(_CurrentPart, LDrawConfig.Instance.GetPartPath(_CurrentPart));
            model.CreateMeshGameObject(LDrawConfig.Instance.ScaleMatrix);
        }
    }
    private const string PathToModels = "Assets/LDraw-Importer/Editor/base-parts/";
}
