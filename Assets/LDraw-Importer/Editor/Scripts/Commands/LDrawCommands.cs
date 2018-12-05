using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace LDraw
{
    
    public enum CommandType
    {
        SubFile = 1,
        Triangle = 3,
        Quad = 4
    }

    public abstract class LDrawCommand
    {
        protected LDrawModel _Parent;
        public static LDrawCommand DeserializeCommand(string serialized, LDrawModel parent)
        {
            LDrawCommand command = null;
            int type;
            if (Int32.TryParse(serialized[0].ToString(), out type))
            {
                var commandType = (CommandType)type;
                Debug.Log("Deserializing " + commandType);
                switch (commandType)
                {
                    case CommandType.SubFile:
                        command = new LDrawSubFile();
                        break;
                    case CommandType.Triangle:
                        command = new LDrawTriangle();
                        break;
                    case CommandType.Quad:
                        command = new LDrawQuad();
                        break;
                }
            }

            if (command != null)
            {
                command._Parent = parent;
                command.Deserialize(serialized);
            }

            return command;
        }
        
        protected Vector3[] _Verts;
        public abstract void PrepareMeshData(List<int> triangles, List<Vector3> verts);
        public abstract void Deserialize(string serialized);

    }
}
