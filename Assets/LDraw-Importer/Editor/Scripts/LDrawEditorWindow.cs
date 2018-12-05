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
    }

    private void GenerateModelButton()
    {
        if (GUILayout.Button("Generate"))
        {
            LDrawModel.Create(_CurrentPart, LDrawConfig.Instance.GetPartPath(_CurrentPart));
        }
    }
    private const string PathToModels = "Assets/LDraw-Importer/Editor/base-parts/";
}
