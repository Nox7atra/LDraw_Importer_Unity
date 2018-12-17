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
        /// FileFormatVersion 1.0.2;

        #region factory

        public static LDrawModel Create(string name, string path)
        {
            if (_models.ContainsKey(name)) return _models[name];
            var model = new LDrawModel();
            model.Init(name, path);
          
            return model;
        }

        #endregion

        #region fields and properties

        private string _Name;
        private List<LDrawCommand> _Commands;
        private List<string> _SubModels;
        private static Dictionary<string, LDrawModel> _models = new Dictionary<string, LDrawModel>();
        
        public string Name
        {
            get { return _Name; }
        }
        #endregion

        #region service methods

        private void Init(string name, string serialized)
        {
            _Name = name;
            _Commands = new List<LDrawCommand>();
            using (StringReader reader = new StringReader(serialized))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Regex regex = new Regex("[ ]{2,}", RegexOptions.None);
                    line = regex.Replace(line, " ").Trim();

                    if (!String.IsNullOrEmpty(line))
                    {
                        var command = LDrawCommand.DeserializeCommand(line, this);
                        if (command != null)
                            _Commands.Add(command);
                    }
                }
            }

            if (!_models.ContainsKey(name))
            {
                _models.Add(name, this);
            }
        }

        public GameObject CreateMeshGameObject(Matrix4x4 trs, Material mat = null, Transform parent = null)
        {
            if (_Commands.Count == 0) return null;
            GameObject go = new GameObject(_Name);
        
            var triangles = new List<int>();
            var verts = new List<Vector3>();
        
            for (int i = 0; i < _Commands.Count; i++)
            {
                var sfCommand = _Commands[i] as LDrawSubFile;
                if (sfCommand == null)
                {
                    _Commands[i].PrepareMeshData(triangles, verts);
                }
                else
                {
                    sfCommand.GetModelGameObject(go.transform);
                }
            }
        
            if (mat != null)
            {
                var childMrs = go.transform.GetComponentsInChildren<MeshRenderer>();
                foreach (var meshRenderer in childMrs)
                {
                    meshRenderer.material = mat;
                }
            }
        
            if (verts.Count > 0)
            {
                var visualGO = new GameObject("mesh");
                visualGO.transform.SetParent(go.transform);
                var mf = visualGO.AddComponent<MeshFilter>();
        
                mf.sharedMesh = PrepareMesh(verts, triangles);
                var mr = visualGO.AddComponent<MeshRenderer>();
                if (mat != null)
                {
                    mr.sharedMaterial = mat;
                  
                }
            }
            
            go.transform.ApplyLocalTRS(trs);
        
            go.transform.SetParent(parent);
            return go;
        }
        private Mesh PrepareMesh(List<Vector3> verts, List<int> triangles)
        {  
            
            Mesh mesh = LDrawConfig.Instance.GetMesh(_Name);
            if (mesh != null) return mesh;
            
          
            mesh = new Mesh();
      
            mesh.name = _Name;
            var frontVertsCount = verts.Count;
            //backface
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
            //end backface
            
            mesh.SetVertices(verts);
            mesh.SetTriangles(triangles, 0);
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            LDrawConfig.Instance.SaveMesh(mesh);
            return mesh;
        }
  
        #endregion

        private LDrawModel()
        {
            
        }
    }
}