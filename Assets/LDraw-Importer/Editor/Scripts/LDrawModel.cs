using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LDraw
{
    public class LDrawModel
    {
        private const string FileFormatVersion = "1.0.2";

        #region factory

        public static LDrawModel Create(string name, string path)
        {
            var model = new LDrawModel();
            model.Init(name, path);
            return model;
        }

        #endregion

        #region fields and properties

        private string _Name;
        private Material _Material;
        private bool _IsGenerateBackFace = true;
        private List<LDrawCommand> _Commands;
        private List<string> _SubModels;


        #endregion

        #region service methods

        private void Init(string name, string path)
        {
            _Material = new Material(Shader.Find("Diffuse"));
            _Name = name;
            _Commands = new List<LDrawCommand>();
            int counter = 0;
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    RegexOptions options = RegexOptions.None;
                    Regex regex = new Regex("[ ]{2,}", options);
                    line = regex.Replace(line, " ").Trim();

                    if (!String.IsNullOrEmpty(line))
                    {
                        var command = LDrawCommand.DeserializeCommand(line, this);
                        if (command != null)
                            _Commands.Add(command);
                    }
                }

                counter++;
            }
        }

        public void CreateMeshGameObject(Matrix4x4 trs, Transform parent = null)
        {
            if (_Commands.Count == 0) return;
            GameObject go = new GameObject(_Name);

            var triangles = new List<int>();
            var verts = new List<Vector3>();

            for (int i = 0; i < _Commands.Count; i++)
            {
                var sfCommand = _Commands[i] as LDrawSubFile;
                if (sfCommand == null)
                    _Commands[i].PrepareMeshData(triangles, verts);
                else
                {
                    sfCommand.GetModelGameObject(go.transform);
                }
            }

            if (verts.Count > 0)
            {
                var visualGO = new GameObject("mesh");

                visualGO.transform.SetParent(go.transform);
                var mf = visualGO.AddComponent<MeshFilter>();

                mf.sharedMesh = PrepareMesh(verts, triangles);
                var mr = visualGO.AddComponent<MeshRenderer>();

                mr.sharedMaterial = _Material;
            }
        
            go.transform.position = trs.ExtractPosition();
            go.transform.rotation = trs.ExtractRotation();
            go.transform.localScale = trs.ExtractScale();

            go.transform.SetParent(parent);
        }

        private Mesh PrepareMesh(List<Vector3> verts, List<int> triangles)
        {
            var frontVertsCount = verts.Count;
            Mesh mesh = new Mesh();
      
            mesh.name = _Name;

            if (_IsGenerateBackFace)
            {
                verts.AddRange(verts);
                int[] tris = new int[triangles.Count];
                triangles.CopyTo(tris);
                for (int i = 0; i < tris.Length; i += 3)
                {
                    int temp = tris[i];
                    tris[i] = tris[i + 1];
                    tris[i + 1] = temp;
                }

                for (int i = 0; i < tris.Length; i++)
                {
                    tris[i] = tris[i] + frontVertsCount;
                }
                triangles.AddRange(tris);
            }
            
            mesh.SetVertices(verts);
            mesh.SetTriangles(triangles, 0);
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        #endregion

        private LDrawModel()
        {
            
        }
    }
}