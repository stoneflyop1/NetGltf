using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Obj2Gltf.WaveFront
{
    /// <summary>
    /// represent vertex data
    /// </summary>
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
    }
}
