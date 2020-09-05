using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NetGltf.Json;

namespace NetGltf
{
    /// <summary>
    /// glb file
    /// </summary>
    public class GlbFile
    {
        /// <summary>
        /// glb header
        /// </summary>
        public GlbHeader Header {get;set;}
        /// <summary>
        /// json buffer
        /// </summary>
        public byte[] Json {get;set;}
        /// <summary>
        /// bin buffer
        /// </summary>
        public byte[] Bin {get;set;}
        /// <summary>
        /// parse from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static GltfResult<GlbFile> Parse(Stream stream)
        {
            using(var br = new BinaryReader(stream))
            {
                var headerRes = GlbHeader.Parse(br);
                if (headerRes.Error != null)
                {
                    return Result.Error<GlbFile>(headerRes.Error);
                }
                var header = headerRes.Data;

                var glbLen = header.Length - GlbConsts.HeaderLength;

                var cHeaderRes = ChunkHeader.Parse(br);
                if (cHeaderRes.Error != null)
                {
                    return Result.Error<GlbFile>(cHeaderRes.Error);
                }
                var cHeader = cHeaderRes.Data;
                if (cHeader.ChunkType != ChunkType.Json)
                {
                    return Result.Error<GlbFile>(new GltfException(
                        new[] {new GltfError{Kind = ErrorKind.Glb, Message = "Missing Json Chunk"}}
                    ));
                }
                var jsonBytes = br.ReadBytes((int)cHeader.Length);

                var gFile = new GlbFile{
                    Header = header,
                    Json = jsonBytes
                };
                var binLen = glbLen - cHeader.Length;
                if (binLen > 0)
                {
                    var bHeaderRes = ChunkHeader.Parse(br);
                    if (bHeaderRes.Error != null)
                    {
                        return Result.Error<GlbFile>(bHeaderRes.Error);
                    }
                    var bHeader = bHeaderRes.Data;
                    if (bHeader.ChunkType != ChunkType.Bin)
                    {
                        return Result.Error<GlbFile>(new GltfException(
                            new[] {new GltfError{Kind = ErrorKind.Glb, Message = "Must be Bin Chunk after Json Chunk"}}
                        ));
                    }
                    gFile.Bin = br.ReadBytes((int)bHeader.Length);
                }
                return Result.Ok(gFile);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class GltfFileExtensions
    {
        /// <summary>
        /// to glTF model
        /// </summary>
        /// <param name="glb"></param>
        /// <returns></returns>
        public static Model ToGltf(this GlbFile glb)
        {
            if (glb == null) throw new ArgumentNullException(nameof(glb));

            var json = Encoding.UTF8.GetString(glb.Json);
            var model = JsonUtil.DeserializeString<Model>(json);
            if (glb.Bin != null && glb.Bin.Length > 0)
            {
                model.Buffers[0].Uri = 
                    "data:application/octet-stream;base64," + Convert.ToBase64String(glb.Bin);
            }
            return model;
        }
    }
    /// <summary>
    /// GLB Header
    /// </summary>
    public class GlbHeader
    {
        /// <summary>
        /// Header ByteCount
        /// </summary>
        public const int ByteCount = 12;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="len">buffer length</param>
        /// <returns></returns>
        public static GlbHeader GetGlbHeader(uint len)
        {
            return new GlbHeader
            {
                Magic = GlbConsts.Magic,
                Version = GlbConsts.Version,
                Length = len
            };
        }
        /// <summary>
        /// glTF magic number
        /// </summary>
        public uint Magic {get;set;}
        /// <summary>
        /// glTF version
        /// </summary>
        public uint Version {get;set;}
        /// <summary>
        /// buffer length
        /// </summary>
        public uint Length {get;set;}

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Magic);
            bw.Write(Version);
            bw.Write(Length);
        }

        internal static GltfResult<GlbHeader> Parse(BinaryReader br)
        {
            var magic = br.ReadUInt32();
                if (magic != GlbConsts.Magic)
                {
                    return Result.Error<GlbHeader>(new GltfException(
                        new[]{new GltfError{Kind = ErrorKind.Glb, Message = "Magic number not match"}}));
                }
                var version = br.ReadUInt32();
                if (version != GlbConsts.Version)
                {
                    return Result.Error<GlbHeader>(new GltfException(
                        new[]{new GltfError{Kind = ErrorKind.Glb, Message = "Version not match"}}));
                }
                var length = br.ReadUInt32();
                if (length == 0)
                {
                    return Result.Error<GlbHeader>(new GltfException(
                        new[]{new GltfError{Kind = ErrorKind.Glb, Message = "buffer length is zero"}}));
                }
                var header = new GlbHeader{
                    Magic = magic,
                    Version = version,
                    Length = length
                };
                return Result.Ok(header);
        }
    }
    /// <summary>
    /// glb chunk header
    /// </summary>
    public class ChunkHeader
    {
        /// <summary>
        /// chunk length
        /// </summary>
        public uint Length {get;set;}
        /// <summary>
        /// chunk type, json/bin
        /// </summary>
        public ChunkType ChunkType {get;set;}

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Length);
            bw.Write((uint)ChunkType);
        }

        internal static GltfResult<ChunkHeader> Parse(BinaryReader br)
        {
            var length = br.ReadUInt32();

            var type = br.ReadUInt32();
            switch(type)
            {
                case GlbConsts.JsonType:
                    return Result.Ok(new ChunkHeader{Length = length, ChunkType = ChunkType.Json});
                case GlbConsts.BinType:
                    return Result.Ok(new ChunkHeader{Length = length, ChunkType = ChunkType.Bin});
                default:
                    return Result.Error<ChunkHeader>(new GltfException(new[]{
                        new GltfError{Kind = ErrorKind.Glb, Message = "Unknown chunk type"}
                    }));
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public enum ChunkType : uint
    {
        /// <summary>
        /// glTF json
        /// </summary>
        Json = GlbConsts.JsonType,
        /// <summary>
        /// binary data, link vertices/normals/uvs/images
        /// </summary>
        Bin = GlbConsts.BinType
    }
    /// <summary>
    /// glb consts
    /// </summary>
    public static class GlbConsts
    {
        /// <summary>
        /// magic number
        /// </summary>
        public const uint Magic = 0x46546C67;
        /// <summary>
        /// glTF version
        /// </summary>
        public const uint Version = 2;
        /// <summary>
        /// GLB Header byte length
        /// </summary>
        public const uint HeaderLength = 12;
        /// <summary>
        ///  json type emum
        /// </summary>
        public const uint JsonType = 0x4E4F534A;
        /// <summary>
        /// bin type enum
        /// </summary>
        public const uint BinType = 0x004E4942;
    }
}
