using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LDraw
{
    public partial class LDrawModel
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
        private Mesh[] _Meshes;

        public string Name
        {
            get { return _Name; }
        }

        public Mesh[] Meshes
        {
            get { return _Meshes; }
        }

        private List<LDrawCommand> _Commands;
        private List<string> _SubModels;


        #endregion

        #region service methods

        public void Init(string name, string path)
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

        public GameObject CreateMeshGameObject(Matrix4x4 hgsm, Transform parent = null)
        {
            if (_Commands.Count == 0)
                return null;
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

            PrepareMeshes(verts, triangles);

            for (int i = 0; i < _Meshes.Length; i++)
            {
                var visualGO = new GameObject("mesh " + i);

                visualGO.transform.SetParent(go.transform);
                var mf = visualGO.AddComponent<MeshFilter>();

                mf.sharedMesh = _Meshes[i];
                var mr = visualGO.AddComponent<MeshRenderer>();

                mr.sharedMaterial = _Material;
                visualGO.AddComponent<BoxCollider>();
            }

            ;
            go.transform.localPosition = hgsm.GetRow(3);
            go.transform.localRotation = Quaternion.LookRotation(hgsm.GetRow(2), hgsm.GetRow(1));
            go.transform.localScale = new Vector3(
                hgsm.GetRow(0).magnitude,
                hgsm.GetRow(1).magnitude,
                hgsm.GetRow(2).magnitude);

            go.transform.SetParent(parent);
            return go;
        }

        protected void PrepareMeshes(List<Vector3> verts, List<int> triangles)
        {
            _Meshes = new Mesh[2];
            _Meshes[0] = new Mesh();
            _Meshes[0].name = _Name;
            _Meshes[0].SetVertices(verts);
            _Meshes[0].SetTriangles(triangles, 0);
            _Meshes[0].RecalculateNormals();
            _Meshes[0].RecalculateBounds();

            _Meshes[1] = Mesh.Instantiate(_Meshes[0]);
            Vector3[] normals = _Meshes[1].normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            _Meshes[1].normals = normals;

            for (int m = 0; m < _Meshes[1].subMeshCount; m++)
            {
                int[] tris = _Meshes[1].GetTriangles(m);
                for (int i = 0; i < tris.Length; i += 3)
                {
                    int temp = tris[i + 0];
                    tris[i + 0] = tris[i + 1];
                    tris[i + 1] = temp;
                }

                _Meshes[1].SetTriangles(tris, m);
            }
        }

        #endregion

    }
}