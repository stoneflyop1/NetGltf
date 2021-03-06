using System;
using System.IO;
using Obj2Gltf.WaveFront;
using Xunit;

namespace Obj2Gltf.Tests
{
    public class LoaderTests
    {
        /// <summary>
        /// relative to bin/Debug/netcoreapp3.1
        /// </summary>
        internal static readonly string SampleRootPath = @"../../../../../objsamples";

        [Fact]
        public void ObjLoad_Tests()
        {
            var objFile = Path.Combine(SampleRootPath, "model/model.obj");

            var objModel = ObjParser.Parse(objFile);
            Assert.NotNull(objModel);
            Assert.True(objModel.Groups.Count == 1);
            Assert.Equal(92, objModel.Positions.Count);
            Assert.Equal(19, objModel.Normals.Count);
            Assert.Equal(32, objModel.TextureCoords.Count);
        }

        [Fact]
        public void MtlLoad_Tests()
        {
            var mtlFile = Path.Combine(SampleRootPath, "model/model.mtl");

            var model = MtlParser.Parse(mtlFile);
            Assert.NotNull(model);
            Assert.Equal(4, model.Materials.Count);
        }

        [Fact]
        public void Comments_Tests()
        {
            var line = "# WaveFront *.obj file (generated by CINEMA 4D)";
            line = line.Trim();
            var strs = line.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            Assert.Single(strs);
        }

    }
}
