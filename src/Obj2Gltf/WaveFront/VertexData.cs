using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#pragma warning disable 1591

namespace Obj2Gltf.WaveFront
{
    public struct Point
    {
        public Point(float x, float y, float z) : this(x, y, z, null) { }
        public Point(float x, float y, float z, float? w)
        {
            X = x; Y = y; Z = z; W = w;
        }
        public float X;
        public float Y;
        public float Z;
        public float? W;

        public override string ToString()
        {
            return $"{X}, {Y}, {Z}, {W}";
        }

        public float[] ToArray()
        {
            if (W.HasValue)
            {
                return new float[] { X, Y, Z, W.Value };
            }
            return new[] { X, Y, Z };
        }
    }

    public struct Normal
    {
        public Normal(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }
        public float X;
        public float Y;
        public float Z;

        public override string ToString()
        {
            return $"{X}, {Y}, {Z}";
        }

        public float[] ToArray()
        {
            return new[] { X, Y, Z };
        }
    }

    public struct Uv
    {
        public Uv(float u) : this(u, 0.0f, null) { }
        public Uv(float u, float v) : this(u, v, null) { }
        public Uv(float u, float v, float? w)
        {
            U = u;
            V = v;
            W = w;
        }
        public float U;
        public float V;
        public float? W;

        public override string ToString()
        {
            return $"{U}, {V}, {W}";
        }

        public float[] ToArray()
        {
            if (W.HasValue)
            {
                return new[] { U, V, W.Value };
            }
            return new[] { U, V };
        }
    }
}
