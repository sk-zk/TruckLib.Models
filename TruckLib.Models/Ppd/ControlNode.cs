using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Models.Ppd
{
    /// <summary>
    /// Represents a control node.
    /// </summary>
    public class ControlNode : IBinarySerializable
    {     
        public Vector3 Position { get; set; }

        public Vector3 Direction { get; set; }

        public int[] InputLines { get; set; } = new int[8];

        public int[] OutputLines { get; set; } = new int[8];

        /// <summary>
        /// The terrain points for this node, which are used to extrude
        /// terrain from the edge of this node.
        /// The key of the dictionary is the index of the model variant.
        /// </summary>
        public Dictionary<int, List<TerrainPoint>> TerrainPoints { get; set; } = [];

        internal uint TerrainPointIndex { get; set; }

        internal uint TerrainPointCount { get; set; }

        internal uint TerrainPointVariantIdx { get; set; }

        internal uint TerrainPointVariantCount { get; set; }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            TerrainPointIndex = r.ReadUInt32();
            TerrainPointCount = r.ReadUInt32();
            TerrainPointVariantIdx = r.ReadUInt32();
            TerrainPointVariantCount = r.ReadUInt32();

            Position = r.ReadVector3();
            Direction = r.ReadVector3();

            for (int i = 0; i < InputLines.Length; i++)
            {
                InputLines[i] = r.ReadInt32();
            }

            for (int i = 0; i < OutputLines.Length; i++)
            {
                OutputLines[i] = r.ReadInt32();
            }
        }

        public void Serialize(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}