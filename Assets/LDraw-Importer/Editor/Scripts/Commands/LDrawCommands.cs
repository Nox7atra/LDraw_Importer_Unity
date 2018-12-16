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
        protected int _ColorCode;
        protected LDrawModel _Parent;
        public static LDrawCommand DeserializeCommand(string line, LDrawModel parent)
        {
            LDrawCommand command = null;
            int type;
            var args = line.Split(' ');
            if (Int32.TryParse(args[0], out type))
            {
                var commandType = (CommandType)type;
             
                switch (commandType)
                {
                    case CommandType.SubFile:
                        command = new LDrawSubFile();
                        command._ColorCode = Int32.Parse(args[1]);
                        break;
                    case CommandType.Triangle:
                        command = new LDrawTriangle();
                        command._ColorCode = Int32.Parse(args[1]);
                        break;
                    case CommandType.Quad:
                        command = new LDrawQuad();
                        command._ColorCode = Int32.Parse(args[1]);
                        break;
                }
            }

            if (command != null)
            {
                command._Parent = parent;
                command.Deserialize(line);
            }

            return command;
        }
        
        protected Vector3[] _Verts;
        public abstract void PrepareMeshData(List<int> triangles, List<Vector3> verts);
        public abstract void Deserialize(string serialized);

    }
}
