using System;
using System.Collections.Generic;
using System.IO;

namespace TruckLib.Models.Ppd
{
    // See https://modding.scssoft.com/wiki/Games/ETS2/Modding_guides/1.30#Prefabs

    public class NavNode : IBinarySerializable
    {
        public NavNodeType Type { get; set; }

        public ushort Index { get; set; }

        public List<NavNodeConnectionInfo> Connections { get; set; } = [];

        private const byte connectionsCount = 8;

        private static readonly NavNodeConnectionInfo empty = new()
        {
            TargetNodeIndex = ushort.MaxValue,
            Length = float.MaxValue,
        };

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Type = (NavNodeType)r.ReadByte();
            Index = r.ReadUInt16();

            var connectionCount = r.ReadByte();
            var connections = r.ReadObjectList<NavNodeConnectionInfo>(connectionsCount);
            connections = connections[0..connectionCount];

            Connections = connections;
        }

        public void Serialize(BinaryWriter w)
        {
            if (Connections.Count > connectionsCount)
            {
                throw new ArgumentOutOfRangeException("Connections.Count");
            }

            w.Write((byte)Type);
            w.Write(Index);

            w.Write((byte)Connections.Count);
            // used
            foreach (var connection in Connections)
            {
                connection.Serialize(w);
            }
            // unused
            for (int i = 0; i < connectionsCount - Connections.Count; i++)
            {
                empty.Serialize(w);
            }

        }
    }

    public class NavNodeConnectionInfo : IBinarySerializable
    {
        public ushort TargetNodeIndex { get; set; }

        public float Length { get; set; }

        public List<ushort> CurveIndices { get; set; } = [];

        private const byte curveIndicesCount = 8;

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            TargetNodeIndex = r.ReadUInt16();
            Length = r.ReadSingle();

            var curveCount = r.ReadByte();
            var curveIndices = r.ReadObjectList<ushort>(curveIndicesCount);
            curveIndices = curveIndices[0..curveCount];

            CurveIndices = curveIndices;
        }

        public void Serialize(BinaryWriter w)
        {
            if (CurveIndices.Count > curveIndicesCount)
            {
                throw new ArgumentOutOfRangeException("CurveIndices.Count");
            }

            w.Write(TargetNodeIndex);
            w.Write(Length);

            w.Write((byte)CurveIndices.Count);
            // used
            foreach (var idx in CurveIndices)
            {
                w.Write(idx);
            }
            // unused
            for (int i = 0; i < curveIndicesCount - CurveIndices.Count; i++)
            {
                w.Write(ushort.MaxValue);
            }
        }
    }
}
