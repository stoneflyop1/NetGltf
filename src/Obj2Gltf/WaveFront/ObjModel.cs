using System;
using System.Collections.Generic;
using System.Text;

namespace Obj2Gltf.WaveFront
{
    /// <summary>
    /// http://paulbourke.net/dataformats/obj/
    /// </summary>
    public class ObjModel
    {
        /// <summary>
        /// o
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// mtllib
        /// </summary>
        public List<string> MaterialLibaries { get; } = new List<string>();
        /// <summary>
        /// v
        /// </summary>
        public List<Point> Positions { get; } = new List<Point>();
        /// <summary>
        /// vt
        /// </summary>
        public List<Uv> TextureCoords { get; } = new List<Uv>();
        /// <summary>
        /// vn
        /// </summary>
        public List<Normal> Normals { get; } = new List<Normal>();

        /// <summary>
        /// p
        /// </summary>
        public List<uint> Points { get; } = new List<uint>();
        /// <summary>
        /// l
        /// </summary>
        public List<LineSegments> Lines { get; } = new List<LineSegments>();
        /// <summary>
        /// fo | f
        /// </summary>
        public List<Polygon> Polygons { get; } = new List<Polygon>();
        /// <summary>
        /// o
        /// </summary>
        public Dictionary<string, Group> Objects {get; internal set;}
        /// <summary>
        /// g
        /// </summary>
        public Dictionary<string, Group> Groups { get; internal set; }
        /// <summary>
        /// usemtl
        /// </summary>
        public Dictionary<string, Group> Meshes { get; internal set; }
        /// <summary>
        /// get objects or groups which has more elements
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Group> GetGroups()
        {
            var gCount = 0;
            if (Groups.Count > 0) { 
                gCount = Groups.Count;
            }
            var oCount = 0;
            if (Objects.Count > 0) {
                oCount = Objects.Count;
            }
            if (gCount >= oCount) { 
                return Groups;
            }
            return Objects;
        }
        /// <summary>
        /// 
        /// </summary>
        public Counter CurrentCounter
        {
            get
            {
                return new Counter((uint)Points.Count, (uint)Lines.Count, (uint)Polygons.Count);
            }
        }

        internal GroupBuilder GetMapBuilder(string input)
        {
            var builder = new GroupBuilder(this);
            builder.Start(input);

            return builder;
        }
    }
    /// <summary>
    /// primitive counter
    /// </summary>
    public struct Counter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pntCount"></param>
        /// <param name="lineCount"></param>
        /// <param name="polyCount"></param>
        public Counter(uint pntCount, uint lineCount, uint polyCount)
        {
            PntCount = pntCount;
            LineCount = lineCount;
            PolyCount = polyCount;
        }
        /// <summary>
        /// 
        /// </summary>
        public uint PntCount;
        /// <summary>
        /// 
        /// </summary>
        public uint LineCount;
        /// <summary>
        /// 
        /// </summary>
        public uint PolyCount;
    }
}
