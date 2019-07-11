using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NetGltf.Json;

namespace NetGltf
{
    public class GlbFile
    {
        public GlbHeader Header {get;set;}

        public byte[] Json {get;set;}

        public byte[] Bin {get;set;}

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

    public static class GltfFileExtensions
    {
        public static Model ToGltf(this GlbFile glb)
        {
            if (glb == null) throw new ArgumentNullException(nameof(glb));

            var json = Encoding.UTF8.GetString(glb.Json);
            var model = Util.DeserializeString<Model>(json);
            if (glb.Bin != null && glb.Bin.Length > 0)
            {
                model.Buffers[0].Uri = 
                    "data:application/octet-stream;base64," + Convert.ToBase64String(glb.Bin);
                //data:application/octet-stream;base64,
            }
            return model;
        }
    }

    public class GlbHeader
    {
        public uint Magic {get;set;}

        public uint Version {get;set;}

        public uint Length {get;set;}

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

    public class ChunkHeader
    {
        public uint Length {get;set;}

        public ChunkType ChunkType {get;set;}

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

    public enum ChunkType
    {
        Json,
        Bin
    }

    public static class GlbConsts
    {
        public const uint Magic = 0x46546C67;

        public const uint Version = 2;

        public const uint HeaderLength = 12;

        public const uint JsonType = 0x4E4F534A;

        public const uint BinType = 0x004E4942;
    }
}
