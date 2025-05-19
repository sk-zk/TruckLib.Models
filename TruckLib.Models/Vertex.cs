﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace TruckLib.Models
{
    [DebuggerDisplay("{Position.X} | {Position.Y} | {Position.Z}")]
    public class Vertex
    {
        public Vector3 Position { get; set; }

        public Vector3 Normal { get; set; } = Vector3.Zero;

        public Vector4? Tangent { get; set; }

        public Color Color { get; set; }

        public Color? SecondaryColor { get; set; }

        public List<Vector2> TextureCoordinates { get; set; }

        public byte[] BoneIndexes { get; set; }

        public byte[] BoneWeights { get; set; }

        private Vertex()
        {
        }

        public Vertex(Vector3 position)
        {
            Position = position;
        }

        public Vertex(float x, float y, float z) : this(new Vector3(x, y, z)) { }

        public Vertex(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }

        public Vertex Clone()
        {
            var cloned = (Vertex)MemberwiseClone();
            if (TextureCoordinates != null)
            {
                cloned.TextureCoordinates = new List<Vector2>(TextureCoordinates);
            }
            return cloned;
        }
    }
}
