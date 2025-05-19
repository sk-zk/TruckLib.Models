﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TruckLib.Models
{
    [DebuggerDisplay("({A} {B} {C})")]
    public struct Triangle : IBinarySerializable
    {
        public ushort A { get; set; }
        public ushort B { get; set; }
        public ushort C { get; set; }

        public Triangle(ushort a, ushort b, ushort c)
        {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// Inverts the order of vertices from CW to CCW or vice versa.
        /// </summary>
        public void InvertOrder()
        {
            (C, A) = (A, C);
        }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            A = r.ReadUInt16();
            B = r.ReadUInt16();
            C = r.ReadUInt16();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(A);
            w.Write(B);
            w.Write(C);
        }

    }
}
