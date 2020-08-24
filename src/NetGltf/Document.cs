using System;
using System.Collections.Generic;
using System.IO;
using NetGltf.Json;

namespace NetGltf
{
    public class Document
    {
        private readonly string _filePath;

        internal Document() : this(null) { }

        internal Document(string filePath)
        {
            _filePath = filePath;
        }

        public static Document Create()
        {
            return new Document();
        }

        public static Document FromPath(string filePath)
        {
            if (filePath == null)
            {
                throw new System.ArgumentNullException(
                    nameof(filePath), "Must provide gltf filePath");
            }
            return new Document(filePath);
        }

        public GltfResult<Model> Parse()
        {
            try
            {
                var model = JsonUtil.Deserialize<Model>(_filePath);
                model.Uri = _filePath;
                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                return Result.Error<Model>(ex);
            }
        }

        public static void PaddingSpace(List<byte> buffer, int boundarySize)
        {
            var count = buffer.Count;
            var res = count % boundarySize;
            if (res != 0)
            {
                var padding = boundarySize - res;
                for (var i = 0; i < padding; i++)
                {
                    buffer.Add(0x20);
                }
            }
        }
        public static void Padding(List<byte> buffer, int boundarySize)
        {
            var count = buffer.Count;
            var res = count % boundarySize;
            if (res != 0)
            {
                var padding = boundarySize - res;
                for (var i = 0; i < padding; i++)
                {
                    buffer.Add(0);
                }
            }
        }

        public void WriteModel(Model model, string filePath)
        {
            if (UriUtil.IsValidUri(model.Uri))
            {
                model.WriteUriFiles(filePath);
            }
            var ext = Path.GetExtension(filePath).TrimStart('.').ToUpper();
            if (ext == "GLB")
            {
                using(var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
                using(var bw = new BinaryWriter(fs))
                {
                    var binCount = 0;
                    var byteLength = 0;
                    if (model.BinBuffers != null)
                    {
                        byteLength = model.BinBuffers.Count;
                        Padding(model.BinBuffers, 4);
                        binCount += model.BinBuffers.Count;
                        if (model.Buffers.Count == 0)
                        {
                            var buffer = new Json.Buffer();
                            model.Buffers.Add(buffer);
                        }
                        model.Buffers[0].ByteLength = byteLength;
                    }
                    var json = JsonUtil.ToJson(model);
                    var jsonBytes = new List<byte>(JsonUtil.StrEncoding.GetBytes(json));
                    PaddingSpace(jsonBytes, 4);
                    var len = GlbHeader.ByteCount + jsonBytes.Count + binCount + 8 + (binCount > 0 ? 8 : 0);
                    var header = GlbHeader.GetGlbHeader((uint)len);
                    header.Write(bw);
                    var jsonChunk = new ChunkHeader
                    {
                        ChunkType = ChunkType.Json,
                        Length = (uint)jsonBytes.Count
                    };
                    jsonChunk.Write(bw);
                    for(var i = 0; i < jsonBytes.Count; i++)
                    {
                        bw.Write(jsonBytes[i]);
                    }
                    if (model.BinBuffers != null)
                    {
                        var binChunk = new ChunkHeader
                        {
                            ChunkType = ChunkType.Bin,
                            Length = (uint)model.BinBuffers.Count
                        };
                        binChunk.Write(bw);
                        for (var i = 0; i < model.BinBuffers.Count; i++)
                        {
                            bw.Write(model.BinBuffers[i]);
                        }
                    }
                    bw.Flush();
                }
            }
            else
            {
                JsonUtil.Serialize(model, filePath);
            }
        }
    }
}
