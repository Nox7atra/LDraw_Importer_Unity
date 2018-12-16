using System.Collections;
using System.Collections.Generic;
using LDraw;
using UnityEditor;
using UnityEngine;

namespace LDraw
{
    [CustomEditor(typeof(LDrawConfig))]
    public class LDrawConfigCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Update blueprints"))
            {
                var config = target as LDrawConfig;
                config.InitParts();
            }
        }
    }

}

