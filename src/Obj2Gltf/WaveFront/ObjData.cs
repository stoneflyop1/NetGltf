using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Obj2Gltf.WaveFront
{
    /// <summary>
    /// A struct which represent `[start, end)` range.
    /// </summary>
    public struct Range
    {
        public Range(uint s) : this(s, Invalid) { }
        public Range(uint s, uint e)
        {
            Start = s;
            End = e;
        }
        public uint Start;
        public uint End;

        public override string ToString()
        {
            return $"{Start}, {End}";
        }

        public const uint Invalid = uint.MaxValue;
    }

    public class Group
    {
        private Group() { }

        public List<Range> Points { get; } = new List<Range>();

        public List<Range> Lines { get; } = new List<Range>();

        public List<Range> Polygons { get; } = new List<Range>();

        public override string ToString()
        {
            return $"Points: {Points.Count}; Lines: {Lines.Count}; Polygons: {Polygons.Count}";
        }

        public static Group Create(Counter counter)
        {
            var g = new Group();
            g.Start(counter);
            return g;
        }

        public void Start(Counter counter)
        {
            Points.Add(new Range(counter.PntCount));
            Lines.Add(new Range(counter.LineCount));
            Polygons.Add(new Range(counter.PolyCount));
        }

        public bool End(Counter counter)
        {
            EndList(Points, counter.PntCount);
            EndList(Lines, counter.LineCount);
            EndList(Polygons, counter.PolyCount);

            return Points.Count == 0 && Lines.Count == 0 && Polygons.Count == 0;
        }

        private static void EndList(List<Range> ranges, uint end)
        {
            var last = ranges[ranges.Count - 1];
            Trace.Assert(last.End == Range.Invalid);
            if (last.Start != end)
            {
                last.End = end;
                ranges[ranges.Count - 1] = last;
            }
            else
            {
                ranges.RemoveAt(ranges.Count - 1);
            }
        }
    }
    public struct PolygonVertex : IEquatable<PolygonVertex>
    {
        public PolygonVertex(uint v) : this(v, null, null) { }
        public PolygonVertex(uint v, uint? vn) : this(v, null, vn) { }
        public PolygonVertex(uint v, uint? vt, uint? vn)
        {
            V = v;
            VT = vt;
            VN = vn;
        }
        public uint V;
        public uint? VT;
        public uint? VN;

        public override string ToString()
        {
            return $"{V}/{VT}/{VN}";
        }

        public bool Equals(PolygonVertex other)
        {
            var v = V;
            var vt = VT ?? 0;
            var vn = VN ?? 0;
            return v == other.V && vt == (other.VT ?? 0) && vn == (other.VN ?? 0);
        }

        public override bool Equals(object obj)
        {
            if (obj is PolygonVertex)
            {
                return Equals((PolygonVertex)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var v = (int)V;
            var vt = (int)(VT ?? 0);
            var vn = (int)(VN ?? 0);
            return v ^ vt ^ vn;
        }
    }
    public class Polygon
    {
        public List<PolygonVertex> Vertices { get; } = new List<PolygonVertex>();
    }

    public struct LineVertex
    {
        public LineVertex(uint v) : this(v, null) { }
        public LineVertex(uint v, uint? vt)
        {
            V = v;
            VT = vt;
        }
        public uint V;
        public uint? VT;

        public override string ToString()
        {
            return $"{V}/{VT}";
        }
    }

    public class LineSegments
    {
        public List<LineVertex> Vertices { get; } = new List<LineVertex>();
    }
}
