using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using NetGltf.Json;

namespace Obj2Gltf
{
    // https://github.com/mapbox/earcut
    public class EarCut
    {
        public static List<int> Triangulate(IList<float> data)
        {
            var dim = 2;
            var hastHoles = false;
            var outerLen = data.Count;
            var outerNode = LinkedList(data, 0, outerLen, dim, true);
            var triangles = new List<int>();

            if (outerNode == null || outerNode.Next == outerNode.Prev)
            {
                return triangles;
            }

            float minX = 0, minY = 0, maxX = 0, maxY = 0, x = 0, y = 0, invSize = 0;

            if (data.Count > 80 * dim)
            {
                minX = maxX = data[0];
                minY = maxY = data[1];

                for (var i =  dim; i < outerLen; i += dim)
                {
                    x = data[i];
                    y = data[i + 1];
                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                }
                invSize = Math.Max(maxX - minX, maxY - minY);
                invSize = invSize != 0 ? 1.0f / invSize : 0;
            }

            EarCutLinked(outerNode, triangles, dim, minX, minY, invSize, 0);

            return triangles;
        }
        private static Node FilterPoints(Node start, Node end)
        {
            if (start == null) return start;
            if (end == null) end = start;
            var p = start;
            bool again;
            do
            {
                again = false;
                if (!p.Steiner && (p.Equals(p.Next) || Area(p.Prev, p, p.Next) == 0))
                {
                    RemoveNode(p);
                    p = end = p.Prev;
                    if (p == p.Next)
                    {
                        break;
                    }
                    again = true;
                }
                else
                {
                    p = p.Next;
                }
            }
            while (again || p != end);

            return end;
        }

        private static Node LinkedList(IList<float> data, int start, int end, int dim, bool clockWise)
        {
            Node last = null;
            if (clockWise == (SignedArea(data, start, end, dim) > 0))
            {
                for(var i = start; i < end; i+= dim)
                {
                    last = InsertNode(i, data[i], data[i + 1], last);
                }
            }
            else
            {
                for(var i = end - dim; i >= start; i -= dim)
                {
                    last = InsertNode(i, data[i], data[i + 1], last);
                }
            }
            if (last != null && last.Equals(last.Next))
            {
                RemoveNode(last);
                last = last.Next;
            }
            return last;
        }
        private static void EarCutLinked(Node ear, List<int> triangles, int dim, float minX, float minY, float invSize, int pass)
        {
            if (ear == null) return;

            if (pass == 0 && invSize != 0)
            {
                IndexCurve(ear, minX, minY, invSize);
            }

            var stop = ear;
            Node prev = null, next = null;

            while(ear.Prev != ear.Next)
            {
                prev = ear.Prev;
                next = ear.Next;

                if (invSize != 0 ? IsEarHashed(ear, minX, minY, invSize) : IsEar(ear))
                {
                    triangles.Add(prev.Index / dim);
                    triangles.Add(ear.Index / dim);
                    triangles.Add(next.Index / dim);

                    RemoveNode(ear);

                    ear = next.Next;
                    stop = next.Next;

                    continue;
                }

                ear = next;

                if (ear == stop)
                {
                    if (pass == 0)
                    {
                        EarCutLinked(FilterPoints(ear, null), triangles, dim, minX, minY, invSize, 1);
                    }
                    else if (pass == 1)
                    {
                        ear = CureLocalIntersections(FilterPoints(ear, null), triangles, dim);
                        EarCutLinked(ear, triangles, dim, minX, minY, invSize, 2);
                    }
                    else if (pass == 2)
                    {
                        SplitEarCut(ear, triangles, dim, minX, minY, invSize);
                    }

                    break;
                }
            }
        }

        private static bool IsValidDiagonal(Node a, Node b)
        {
            return a.Next.Index != b.Index && a.Prev.Index != b.Index && !IntersectsPolygon(a, b)
                && (LocallyInside(a, b) && LocallyInside(b, a) && MiddleInside(a, b)
                && (Area(a.Prev, a, b.Prev) != 0 || Area(a, b.Prev, b) != 0)
                || a.Equals(b) && Area(a.Prev, a, a.Next) > 0 && Area(b.Prev, b, b.Next) > 0);
        }

        private static bool IntersectsPolygon(Node a, Node b)
        {
            var p = a;
            do
            {
                if (p.Index != a.Index && p.Next.Index != a.Index 
                    && p.Index != b.Index && p.Next.Index != b.Index && Intersects(p, p.Next, a, b))
                {
                    return true;
                }
            }
            while (p != a);
            return false;
        }

        private static void SplitEarCut(Node start, List<int> triangles, int dim, float minX, float minY, float invSize)
        {
            var a = start;
            do
            {
                var b = a.Next.Next;
                while(b != a.Prev)
                {
                    if (a.Index != b.Index && IsValidDiagonal(a, b))
                    {
                        var c = SplitPolygon(a, b);

                        a = FilterPoints(a, a.Next);
                        c = FilterPoints(c, c.Next);

                        EarCutLinked(a, triangles, dim, minX, minY, invSize, 0);
                        EarCutLinked(c, triangles, dim, minX, minY, invSize, 0);
                        return;
                    }
                    b = b.Next;
                }
            }
            while (a != start);
        }

        private static Node CureLocalIntersections(Node start, List<int> triangles, int dim)
        {
            var p = start;
            do
            {
                var a = p.Prev;
                var b = p.Next.Next;

                if (!a.Equals(b) && Intersects(a, p, p.Next, b) && LocallyInside(a, b) && LocallyInside(b,a))
                {
                    triangles.Add(a.Index / dim);
                    triangles.Add(p.Index / dim);
                    triangles.Add(b.Index / dim);

                    RemoveNode(p);
                    RemoveNode(p.Next);

                    p = start = b;
                }
            }
            while (p != start);

            return FilterPoints(p, null);
        }

        private static void IndexCurve(Node start, float minX, float minY, float invSize)
        {
            var p = start;
            do
            {
                if (p.Z == null)
                {
                    p.Z = ZOrder(p.X, p.Y, minX, minY, invSize);
                }
                p.PrevZ = p.Prev;
                p.NextZ = p.Next;
                p = p.Next;
            }
            while (p != start);

            SortLinked(p);
        }

        private static Node SortLinked(Node list)
        {
            var inSize = 1;
            int numMerges = 0;
            int pSize = 0;
            int qSize = 0;
            Node p = null, q = null, tail = null, e = null;
            do
            {
                p = list;
                list = null;
                tail = null;
                numMerges = 0;

                while(p != null)
                {
                    numMerges++;
                    q = p;
                    pSize = 0;
                    for(var i = 0; i < inSize; i++)
                    {
                        pSize++;
                        q = q.NextZ;
                        if (q == null)
                        {
                            break;
                        }
                    }
                    qSize = inSize;
                }

                while (pSize > 0 || (qSize > 0 && q != null))
                {

                    if (pSize != 0 && (qSize == 0 || q == null || p.Z <= q.Z))
                    {
                        e = p;
                        p = p.NextZ;
                        pSize--;
                    }
                    else
                    {
                        e = q;
                        q = q.NextZ;
                        qSize--;
                    }

                    if (tail != null) tail.NextZ = e;
                    else list = e;

                    e.PrevZ = tail;
                    tail = e;
                }

            }
            while (numMerges > 1);

            return list;
        }
        private static bool IsEarHashed(Node ear, float minX, float minY, float invSize)
        {
            var a = ear.Prev;
            var b = ear;
            var c = ear.Next;

            if (Area(a, b, c) >= 0) return false;

            var minTX = a.X < b.X ? (a.X < c.X ? a.X : c.X) : (b.X < c.X ? b.X : c.X);
            var minTY = a.Y < b.Y ? (a.Y < c.Y ? a.Y : c.Y) : (b.Y < c.Y ? b.Y : c.Y);
            var maxTX = a.X > b.X ? (a.X > c.X ? a.X : c.X) : (b.X > c.X ? b.X : c.X);
            var maxTY = a.Y > b.Y ? (a.Y > c.Y ? a.Y : c.Y) : (b.Y > c.Y ? b.Y : c.Y);

            var minZ = ZOrder(minTX, minTY, minX, minY, invSize);
            var maxZ = ZOrder(maxTX, maxTY, minX, minY, invSize);

            var p = ear.PrevZ;
            var n = ear.NextZ;

            while (p != null && p.Z >= minZ && n != null && n.Z <= maxZ)
            {
                if (p != ear.Prev && p != ear.Next &&
                    PntInTriangle(a.X, a.Y, b.X, b.Y, c.X, c.Y, p.X, p.Y) &&
                    Area(p.Prev, p, p.Next) >= 0) return false;
                p = p.PrevZ;

                if (n != ear.Prev && n != ear.Next &&
                    PntInTriangle(a.X, a.Y, b.X, b.Y, c.X, c.Y, n.X, n.Y) &&
                    Area(n.Prev, n, n.Next) >= 0) return false;
                n = n.NextZ;
            }

            while (p != null && p.Z >= minZ)
            {
                if (p != ear.Prev && p != ear.Next &&
                    PntInTriangle(a.X, a.Y, b.X, b.Y, c.X, c.Y, p.X, p.Y) &&
                    Area(p.Prev, p, p.Next) >= 0) return false;
                p = p.PrevZ;
            }

            while (n != null && n.Z <= maxZ)
            {
                if (n != ear.Prev && n != ear.Next &&
                    PntInTriangle(a.X, a.Y, b.X, b.Y, c.X, c.Y, n.X, n.Y) &&
                    Area(n.Prev, n, n.Next) >= 0) return false;
                n = n.NextZ;
            }

            return true;

        }

        private static Node SplitPolygon(Node a, Node b)
        {
            var a2 = new Node(a.Index, a.X, a.Y);
            var b2 = new Node(b.Index, b.X, b.Y);
            var an = a.Next;
            var bp = b.Prev;

            a.Next = b;
            b.Prev = a;

            a2.Next = an;
            an.Prev = a2;

            b2.Next = a2;
            a2.Prev = b2;

            bp.Next = b2;
            b2.Prev = bp;

            return b2;
        }

        private static Node InsertNode(int index, float x, float y, Node last)
        {
            var p = new Node(index, x, y);

            if (last == null)
            {
                p.Prev = p;
                p.Next = p;
            }
            else
            {
                p.Next = last.Next;
                p.Prev = last;
                last.Next.Prev = p;
                last.Next = p;
            }

            return p;
        }

        private static void RemoveNode(Node p)
        {
            p.Next.Prev = p.Prev;
            p.Prev.Next = p.Next;

            if (p.PrevZ != null)
            {
                p.PrevZ.NextZ = p.NextZ;
            }
            if (p.NextZ != null)
            {
                p.NextZ.PrevZ = p.PrevZ;
            }
        }
        private static bool IsEar(Node ear)
        {
            var a = ear.Prev;
            var b = ear;
            var c = ear.Next;

            if (Area(a, b, c) >= 0) return false;

            var p = ear.Next.Next;

            while(p != ear.Prev)
            {
                if (PntInTriangle(a.X, a.Y, b.X, b.Y, c.X, c.Y, p.X, p.Y) 
                    && Area(p.Prev, p, p.Next) >= 0)
                {
                    return false;
                }
                p = p.Next;
            }
            return true;
        }

        private static bool LocallyInside(Node a, Node b)
        {
            return Area(a.Prev, a, a.Next) < 0 ?
                Area(a, b, a.Next) >= 0 && Area(a, a.Prev, b) >= 0 :
                Area(a, b, a.Prev) < 0 || Area(a, a.Next, b) < 0;
        }

        private static bool MiddleInside(Node a, Node b)
        {
            var p = a;
            var inside = false;
            var px = (a.X + b.X) / 2;
            var py = (a.Y + b.Y) / 2;

            do
            {
                if (((p.Y > py) != (p.Next.Y > py)) && p.Next.Y != p.Y &&
                    (px < (p.Next.X - p.X) * (py - p.Y) / (p.Next.Y - p.Y) + p.X))
                {
                    inside = !inside;
                }
                p = p.Next;
            }
            while (p != a);

            return inside;
        }
        private static float SignedArea(IList<float> data, int start, int end, int dim)
        {
            var sum = 0.0f;
            var j = end - dim;
            for (var i = start; i < end; i += dim)
            {
                sum += (data[j] - data[i]) * (data[i + 1] + data[j + 1]);
                j = i;
            }
            return sum;
        }
        private static bool PntInTriangle(float ax, float ay, float bx, float by, float cx, float cy, float px, float py)
        {
            return (cx - px) * (ay - py) - (ax - px) * (cy - py) >= 0 &&
               (ax - px) * (by - py) - (bx - px) * (ay - py) >= 0 &&
               (bx - px) * (cy - py) - (cx - px) * (by - py) >= 0;
        }
        private static bool Intersects(Node p1, Node q1, Node p2, Node q2)
        {
            var o1 = Sign(Area(p1, q1, p2));
            var o2 = Sign(Area(p1, q1, q2));
            var o3 = Sign(Area(p2, q2, p1));
            var o4 = Sign(Area(p2, q2, q1));
            if (o1 != o2 && o3 != o4) return true;

            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

            return false;

        }

        private static float ZOrder(float x0, float y0, float minX, float minY, float invSize)
        {
            var x = BitConverter.ToInt32(BitConverter.GetBytes(32767 * (x0 - minX) * invSize), 0);
            var y = BitConverter.ToInt32(BitConverter.GetBytes(32767 * (y0 - minY) * invSize), 0);

            x = (x | (x << 8)) & 0x00FF00FF;
            x = (x | (x << 4)) & 0x0F0F0F0F;
            x = (x | (x << 2)) & 0x33333333;
            x = (x | (x << 1)) & 0x55555555;

            y = (y | (y << 8)) & 0x00FF00FF;
            y = (y | (y << 4)) & 0x0F0F0F0F;
            y = (y | (y << 2)) & 0x33333333;
            y = (y | (y << 1)) & 0x55555555;

            var res = x | (y << 1);
            return BitConverter.ToSingle(BitConverter.GetBytes(res), 0);
        }

        private static float CompareX(Pnt2d a, Pnt2d b)
        {
            return a.X - b.X;
        }

        private static float Area(Node p, Node q, Node r)
        {
            return (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
        }
        private static float Area(Pnt2d p, Pnt2d q, Pnt2d r)
        {
            return (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">test point</param>
        /// <param name="q">segment pnt1</param>
        /// <param name="r">segment pnt2</param>
        /// <returns></returns>
        private static bool OnSegment(Node p, Node q, Node r)
        {
            return q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) 
                && q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y);
        }

        private static int Sign(float num)
        {
            return num > 0 ? 1 : num < 0 ? -1 : 0;
        }

        class Node
        {
            public Node(int index, float x, float y)
            {
                Index = index;
                X = x;
                Y = y;
            }
            public int Index { get;  }
            public float X { get;  }
            public float Y { get;  }
            public float? Z { get; set; }

            public Node Prev { get; set; }
            public Node Next { get; set; }

            public Node PrevZ { get; set; }
            public Node NextZ { get; set; }

            public bool Steiner { get; set; }
        }
    }

    struct Pnt2d : IEquatable<Pnt2d>
    {
        public Pnt2d(float x, float y)
        {
            X = x;
            Y = y;
        }
        public float X;
        public float Y;

        public bool Equals(Pnt2d other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Pnt2d)
            {
                return Equals((Pnt2d)obj);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
