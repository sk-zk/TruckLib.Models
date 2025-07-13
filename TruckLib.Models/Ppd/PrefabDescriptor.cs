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
    /// Represents a prefab descriptor (.ppd) file.
    /// </summary>
    public class PrefabDescriptor : IBinarySerializable
    {
        public uint Version { get; private set; }

        public List<ControlNode> Nodes { get; set; } = [];

        public List<NavCurve> NavCurves { get; set; } = [];

        public List<Sign> Signs { get; set; } = [];

        public List<Semaphore> Semaphores { get; set; } = [];

        public List<SpawnPoint> SpawnPoints { get; set; } = [];

        public List<MapPoint> MapPoints { get; set; } = [];

        public List<TriggerPoint> TriggerPoints { get; set; } = [];

        public List<Intersection> Intersections { get; set; } = [];

        public List<NavNode> NavNodes { get; set; } = [];

        /// <summary>
        /// Reads a ppd file from disk.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The prefab descriptor.</returns>
        /// <exception cref="NotSupportedException">Thrown if the descriptor version
        /// is not supported.</exception>
        public static PrefabDescriptor Open(string path)
        {
            using var ppdStream = File.OpenRead(path);
            return Load(ppdStream);
        }

        /// <summary>
        /// Reads a ppd file from disk.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="fs">The file system to load the file from.</param>
        /// <returns>The prefab descriptor.</returns>
        /// <exception cref="NotSupportedException">Thrown if the descriptor version
        /// is not supported.</exception>
        public static PrefabDescriptor Open(string path, IFileSystem fs)
        {
            using var ppdStream = fs.Open(path);
            return Load(ppdStream);
        }

        /// <summary>
        /// Reads a ppd file from memory.
        /// </summary>
        /// <param name="ppdBuffer">The buffer containing the file.</param>
        /// <returns>The prefab descriptor.</returns>
        /// <exception cref="NotSupportedException">Thrown if the descriptor version
        /// is not supported.</exception>
        public static PrefabDescriptor Load(byte[] ppdBuffer)
        {
            using var ms = new MemoryStream(ppdBuffer);
            return Load(ms);
        }

        /// <summary>
        /// Reads a ppd file from memory.
        /// </summary>
        /// <param name="ppdStream">The stream containing the file.</param>
        /// <returns>The prefab descriptor.</returns>
        /// <exception cref="NotSupportedException">Thrown if the descriptor version
        /// is not supported.</exception>
        public static PrefabDescriptor Load(Stream ppdStream)
        {
            var ppd = new PrefabDescriptor();
            using var r = new BinaryReader(ppdStream);
            ppd.Deserialize(r);
            return ppd;
        }

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">Thrown if the descriptor version
        /// is not supported.</exception>
        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Version = r.ReadUInt32();
            switch (Version)
            {
                case 0x15:
                    Deserialize15(r);
                    break;
                case 0x16:
                    Deserialize16(r);
                    break;
                case 0x17:
                    Deserialize17(r);
                    break;
                case 0x18:
                    Deserialize18(r);
                    break;
                case 0x19:
                    Deserialize19(r);
                    break;
                default:
                    throw new UnsupportedVersionException($"Version {Version} is not supported.");
            }
        }

        private void Deserialize15(BinaryReader r)
        {
            const int version = 0x15;
            var nodeCount = r.ReadUInt32();
            var navCurveCount = r.ReadUInt32();
            var signCount = r.ReadUInt32();
            var semaphoreCount = r.ReadUInt32();
            var spawnPointCount = r.ReadUInt32();
            var terrainPointCount = r.ReadUInt32();
            var terrainPointVariantCount = r.ReadUInt32();
            var mapPointCount = r.ReadUInt32();
            var triggerPointCount = r.ReadUInt32();
            var intersectionCount = r.ReadUInt32();

            // offsets; we can probably ignore this
            for (int i = 0; i < 11; i++)
                r.ReadUInt32();

            Nodes = r.ReadObjectList<ControlNode>(nodeCount);
            NavCurves = r.ReadObjectList<NavCurve>(navCurveCount, version);
            Signs = r.ReadObjectList<Sign>(signCount);
            Semaphores = r.ReadObjectList<Semaphore>(semaphoreCount, version);
            SpawnPoints = r.ReadObjectList<SpawnPoint>(spawnPointCount, version);
            var terrainPointPositions = r.ReadObjectList<Vector3>(terrainPointCount);
            var terrainPointNormals = r.ReadObjectList<Vector3>(terrainPointCount);
            var terrainPointVariants = r.ReadObjectList<TerrainPointVariant>(terrainPointVariantCount);
            MapPoints = r.ReadObjectList<MapPoint>(mapPointCount);
            TriggerPoints = r.ReadObjectList<TriggerPoint>(triggerPointCount);
            Intersections = r.ReadObjectList<Intersection>(intersectionCount);

            SetTerrainPoints(terrainPointPositions, terrainPointNormals, terrainPointVariants);
        }

        private void Deserialize16(BinaryReader r)
        {
            const int version = 0x16;
            var nodeCount = r.ReadUInt32();
            var navCurveCount = r.ReadUInt32();
            var signCount = r.ReadUInt32();
            var semaphoreCount = r.ReadUInt32();
            var spawnPointCount = r.ReadUInt32();
            var terrainPointCount = r.ReadUInt32();
            var terrainPointVariantCount = r.ReadUInt32();
            var mapPointCount = r.ReadUInt32();
            var triggerPointCount = r.ReadUInt32();
            var intersectionCount = r.ReadUInt32();
            var navNodeCount = r.ReadUInt32();

            // offsets; we can probably ignore this
            for (int i = 0; i < 12; i++)
                r.ReadUInt32();

            Nodes = r.ReadObjectList<ControlNode>(nodeCount);
            NavCurves = r.ReadObjectList<NavCurve>(navCurveCount, version);
            Signs = r.ReadObjectList<Sign>(signCount);
            Semaphores = r.ReadObjectList<Semaphore>(semaphoreCount, version);
            SpawnPoints = r.ReadObjectList<SpawnPoint>(spawnPointCount, version);
            var terrainPointPositions = r.ReadObjectList<Vector3>(terrainPointCount);
            var terrainPointNormals = r.ReadObjectList<Vector3>(terrainPointCount);
            var terrainPointVariants = r.ReadObjectList<TerrainPointVariant>(terrainPointVariantCount);
            TriggerPoints = r.ReadObjectList<TriggerPoint>(triggerPointCount);
            Intersections = r.ReadObjectList<Intersection>(intersectionCount);
            NavNodes = r.ReadObjectList<NavNode>(navNodeCount);

            SetTerrainPoints(terrainPointPositions, terrainPointNormals, terrainPointVariants);
        }

        private void Deserialize17(BinaryReader r)
        {
            const int version = 0x17;
            var nodeCount = r.ReadUInt32();
            var navCurveCount = r.ReadUInt32();
            var signCount = r.ReadUInt32();
            var semaphoreCount = r.ReadUInt32();
            var spawnPointCount = r.ReadUInt32();
            var terrainPointCount = r.ReadUInt32();
            var terrainPointVariantCount = r.ReadUInt32();
            var mapPointCount = r.ReadUInt32();
            var triggerPointCount = r.ReadUInt32();
            var intersectionCount = r.ReadUInt32();
            var navNodeCount = r.ReadUInt32();

            // offsets; we can probably ignore this
            for (int i = 0; i < 12; i++)
                r.ReadUInt32();

            Nodes = r.ReadObjectList<ControlNode>(nodeCount);
            NavCurves = r.ReadObjectList<NavCurve>(navCurveCount, version);
            Signs = r.ReadObjectList<Sign>(signCount);
            Semaphores = r.ReadObjectList<Semaphore>(semaphoreCount, version);
            SpawnPoints = r.ReadObjectList<SpawnPoint>(spawnPointCount, version);
            var terrainPointPositions = r.ReadObjectList<Vector3>(terrainPointCount);
            var terrainPointNormals = r.ReadObjectList<Vector3>(terrainPointCount);
            var terrainPointVariants = r.ReadObjectList<TerrainPointVariant>(terrainPointVariantCount);
            MapPoints = r.ReadObjectList<MapPoint>(mapPointCount);
            TriggerPoints = r.ReadObjectList<TriggerPoint>(triggerPointCount);
            Intersections = r.ReadObjectList<Intersection>(intersectionCount);
            NavNodes = r.ReadObjectList<NavNode>(navNodeCount);

            SetTerrainPoints(terrainPointPositions, terrainPointNormals, terrainPointVariants);
        }

        private void Deserialize18(BinaryReader r)
        {
            const int version = 0x18;
            var nodeCount = r.ReadUInt32();
            var navCurveCount = r.ReadUInt32();
            var signCount = r.ReadUInt32();
            var semaphoreCount = r.ReadUInt32();
            var spawnPointCount = r.ReadUInt32();
            var terrainPointCount = r.ReadUInt32();
            var terrainPointVariantCount = r.ReadUInt32();
            var mapPointCount = r.ReadUInt32();
            var triggerPointCount = r.ReadUInt32();
            var intersectionCount = r.ReadUInt32();
            var navNodeCount = r.ReadUInt32();

            // offsets; we can probably ignore this
            for (int i = 0; i < 12; i++)
                r.ReadUInt32();

            Nodes = r.ReadObjectList<ControlNode>(nodeCount);
            NavCurves = r.ReadObjectList<NavCurve>(navCurveCount, version);
            Signs = r.ReadObjectList<Sign>(signCount);
            Semaphores = r.ReadObjectList<Semaphore>(semaphoreCount, version);
            SpawnPoints = r.ReadObjectList<SpawnPoint>(spawnPointCount, version);
            var terrainPointPositions = r.ReadObjectList<Vector3>(terrainPointCount);
            var terrainPointNormals = r.ReadObjectList<Vector3>(terrainPointCount);
            var terrainPointVariants = r.ReadObjectList<TerrainPointVariant>(terrainPointVariantCount);
            MapPoints = r.ReadObjectList<MapPoint>(mapPointCount);
            TriggerPoints = r.ReadObjectList<TriggerPoint>(triggerPointCount);
            Intersections = r.ReadObjectList<Intersection>(intersectionCount);
            NavNodes = r.ReadObjectList<NavNode>(navNodeCount);

            SetTerrainPoints(terrainPointPositions, terrainPointNormals, terrainPointVariants);
        }

        private void Deserialize19(BinaryReader r)
        {
            const int version = 0x19;
            var nodeCount = r.ReadUInt32();
            var navCurveCount = r.ReadUInt32();
            var signCount = r.ReadUInt32();
            var semaphoreCount = r.ReadUInt32();
            var spawnPointCount = r.ReadUInt32();
            var terrainPointCount = r.ReadUInt32();
            var terrainPointVariantCount = r.ReadUInt32();
            var mapPointCount = r.ReadUInt32();
            var triggerPointCount = r.ReadUInt32();
            var intersectionCount = r.ReadUInt32();
            var navNodeCount = r.ReadUInt32();

            // offsets; we can probably ignore this
            for (int i = 0; i < 12; i++)
                r.ReadUInt32();

            Nodes = r.ReadObjectList<ControlNode>(nodeCount);
            NavCurves = r.ReadObjectList<NavCurve>(navCurveCount, version);
            Signs = r.ReadObjectList<Sign>(signCount);
            Semaphores = r.ReadObjectList<Semaphore>(semaphoreCount, version);
            SpawnPoints = r.ReadObjectList<SpawnPoint>(spawnPointCount, version);
            var terrainPointPositions = r.ReadObjectList<Vector3>(terrainPointCount);
            var terrainPointNormals = r.ReadObjectList<Vector3>(terrainPointCount);
            var terrainPointVariants = r.ReadObjectList<TerrainPointVariant>(terrainPointVariantCount);
            MapPoints = r.ReadObjectList<MapPoint>(mapPointCount);
            TriggerPoints = r.ReadObjectList<TriggerPoint>(triggerPointCount);
            Intersections = r.ReadObjectList<Intersection>(intersectionCount);
            NavNodes = r.ReadObjectList<NavNode>(navNodeCount);

            SetTerrainPoints(terrainPointPositions, terrainPointNormals, terrainPointVariants);
        }

        /// <summary>
        /// Parses the terrain point data into a more user-friendly format.
        /// </summary>
        /// <param name="terrainPointPositions">The raw, flat list of terrain point positions.</param>
        /// <param name="terrainPointNormals">The raw, flat list of terrain point normals.</param>
        /// <param name="terrainPointVariants">The raw, flat list of terrain point variant mappings.</param>
        private void SetTerrainPoints(List<Vector3> terrainPointPositions, List<Vector3> terrainPointNormals, 
            List<TerrainPointVariant> terrainPointVariants)
        {
            for (int nodeIdx = 0; nodeIdx < Nodes.Count; nodeIdx++)
            {
                var node = Nodes[nodeIdx];

                var start = (int)node.TerrainPointIndex;
                var count = (int)node.TerrainPointCount;
                var points = terrainPointPositions[start..(start + count)];
                var normals = terrainPointNormals[start..(start + count)];

                var mappingStart = (int)node.TerrainPointVariantIdx;
                var mappingsCount = (int)node.TerrainPointVariantCount;

                if (mappingsCount > 0)
                {
                    var mappings = terrainPointVariants[mappingStart..(mappingStart + mappingsCount)];

                    for (int mapIdx = 0; mapIdx < mappings.Count; mapIdx++)
                    {
                        var list = new List<TerrainPoint>((int)mappings[mapIdx].Length);
                        node.TerrainPoints[mapIdx] = list;

                        var first = (int)mappings[mapIdx].Start;
                        var last = mappings[mapIdx].Start + mappings[mapIdx].Length;
                        for (int pointIdx = first; pointIdx < last; pointIdx++)
                        {
                            list.Add(new TerrainPoint(points[pointIdx], normals[pointIdx]));
                        }
                    }
                }
                else
                {
                    var list = new List<TerrainPoint>(count);
                    node.TerrainPoints[0] = list;
                    for (int pointIdx = 0; pointIdx < count; pointIdx++)
                    {
                        list.Add(new TerrainPoint(points[pointIdx], normals[pointIdx]));
                    }
                }
            }
        }

        public void Serialize(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
