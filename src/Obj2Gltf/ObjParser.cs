using System;
using System.Collections.Generic;
using System.Text;
using Obj2Gltf.WaveFront;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;

namespace Obj2Gltf
{
    internal class GroupBuilder
    {
        public GroupBuilder(ObjModel model)
        {
            Model = model;
        }
        public ObjModel Model { get; }

        public string Current { get; set; }

        public Dictionary<string, Group> Result { get; } = new Dictionary<string, Group>();

        public void Start(string input)
        {
            var count = Model.CurrentCounter;
            if (Current != null)
            {
                if (Current == input) return;
                if (Result.TryGetValue(Current, out var grp))
                {
                    if (grp.End(count))
                    {
                        Result.Remove(Current);
                    }
                }
            }
            if (Result.TryGetValue(input, out var group))
            {
                group.Start(count);
            }
            else
            {
                Result.Add(input, Group.Create(count));
            }
            Current = input;
        }

        public void End()
        {
            if (Current != null)
            {
                if (Result.TryGetValue(Current, out var group))
                {
                    if (group.End(Model.CurrentCounter))
                    {
                        Result.Remove(Current);
                    }
                }
            }
            Current = null;
        }
    }
    /// <summary>
    /// parse obj file
    /// </summary>
    public class ObjParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="encoding">default utf-8</param>
        /// <returns></returns>
        public static ObjModel Parse(string inputFile, Encoding encoding = null)
        {
            encoding = encoding ?? TextParser.DefaultEncoding;
            // https://stackoverflow.com/questions/2161895/reading-large-text-files-with-streams-in-c-sharp
            using (var fs = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using(var bs = new BufferedStream(fs))
            using(var sr = new StreamReader(bs, encoding))
            {
                var nameBuilder = new StringBuilder();
                var model = new ObjModel();
                var groupBuilder = model.GetMapBuilder("default");
                var objectBuilder = model.GetMapBuilder("default");
                var meshBuilder = model.GetMapBuilder("");
                TextParser.Lex(sr, (key, args) =>
                {
                    switch (key)
                    {
                        case "mtllib":
                            model.MaterialLibaries.AddRange(args);
                            break;
                        case "v":
                            args = TextParser.ParseArgs(args);
                            model.Positions.Add(ParsePoint(args));
                            break;
                        case "vt":
                            args = TextParser.ParseArgs(args);
                            model.TextureCoords.Add(ParseUv(args));
                            break;
                        case "vn":
                            args = TextParser.ParseArgs(args);
                            model.Normals.Add(ParseNormal(args));
                            break;
                        case "vp":
                            break;
                        case "p":
                            args = TextParser.ParseArgs(args);
                            foreach (var a in args)
                            {
                                var index = FindIndex(model.Positions, a);
                                model.Points.Add(index);
                            }
                            break;
                        case "l":
                            args = TextParser.ParseArgs(args);
                            FillLineSegments(model, args);
                            break;
                        case "fo":
                        case "f":
                            args = TextParser.ParseArgs(args);
                            FillPolygon(model, args);
                            break;
                        case "g":
                            args = TextParser.ParseArgs(args);
                            groupBuilder.Start(args[0]);
                            break;
                        case "o":
                            args = TextParser.ParseArgs(args);
                            objectBuilder.Start(args[0]);
                            if(String.IsNullOrEmpty(model.Name))
                            {
                                model.Name = args[0];
                            }
                            break;
                        case "usemtl":
                            meshBuilder.Start(args[0]);
                            break;
                    }
                });
                objectBuilder.End();
                groupBuilder.End();
                meshBuilder.End();
                model.Objects = objectBuilder.Result;
                model.Groups = groupBuilder.Result;
                model.Meshes = meshBuilder.Result;

                return model;
            }
        }

        private static uint FindIndex<T>(IList<T> list, string inputIndex)
        {
            if (int.TryParse(inputIndex, out var index))
            {
                var len = list.Count;
                if (index < 0)
                {
                    index += len;
                }
                return (uint)index; //TODO: check out of range
            }
            throw new ArgumentException("inputIndex out of range or parse int failed", nameof(inputIndex));
        }

        private static Point ParsePoint(IList<string> args)
        {
            switch (args.Count)
            {
                case 4:
                    return new Point(float.Parse(args[0]), float.Parse(args[1]), 
                        float.Parse(args[2]), float.Parse(args[3]));
                case 3:
                    return new Point(float.Parse(args[0]), float.Parse(args[1]),
                        float.Parse(args[2]));
                default:
                    throw new ArgumentException("position should have 3 or 4 parts", nameof(args));
            }
        }

        private static Uv ParseUv(IList<string> args)
        {
            switch (args.Count)
            {
                case 3:
                    return new Uv(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]));
                case 2:
                    return new Uv(float.Parse(args[0]), float.Parse(args[1]));
                case 1:
                    return new Uv(float.Parse(args[0]));
                default:
                    throw new ArgumentException("texture_coord should have 1, 2 or 3 parts", nameof(args));
            }
        }

        private static Normal ParseNormal(IList<string> args)
        {
            switch (args.Count)
            {
                case 3:
                    return new Normal(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]));
                default:
                    throw new ArgumentException("normal should have 3 parts", nameof(args));
            }
        }

        private static void FillLineSegments(ObjModel model, IList<string> args)
        {
            if (args.Count < 2) return;
            var segments = new LineSegments();
            foreach(var a in args)
            {
                var numStrs = a.Trim().Split('/');
                var vIndex = FindIndex(model.Positions, numStrs[0]);
                uint? vtIndex = null;
                if (numStrs.Length == 2)
                {
                    if (!String.IsNullOrEmpty(numStrs[1]))
                    {
                        vtIndex = FindIndex(model.TextureCoords, numStrs[1]);
                    }
                }
                segments.Vertices.Add(new LineVertex(vIndex, vtIndex));
            }
            if (segments.Vertices.Count > 0)
            {
                model.Lines.Add(segments);
            }
        }

        private static void FillPolygon(ObjModel model, IList<string> args)
        {
            if (args.Count <= 2)
            {
                return;
            }
            var polygon = new Polygon();
            foreach(var a in args)
            {
                var numStrs = a.Trim().Split('/');
                var vIndex = FindIndex(model.Positions, numStrs[0]);
                uint? vtIndex = null;
                if (numStrs.Length >= 2)
                {
                    if (!String.IsNullOrEmpty(numStrs[1]))
                    {
                        vtIndex = FindIndex(model.TextureCoords, numStrs[1]);
                    }
                }
                uint? vnIndex = null;
                if (numStrs.Length == 3)
                {
                    if (!String.IsNullOrEmpty(numStrs[2]))
                    {
                        vnIndex = FindIndex(model.Normals, numStrs[2]);
                    }
                }
                polygon.Vertices.Add(new PolygonVertex(vIndex, vtIndex, vnIndex));
            }
            if (polygon.Vertices.Count > 0)
            {
                model.Polygons.Add(polygon);
            }
        }
    }
}
