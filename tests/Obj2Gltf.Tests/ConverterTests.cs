using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Obj2Gltf.Tests
{
    public class ConverterTests
    {
        internal static string EnsureOuputPath(string inputFile)
        {
            var name = Path.GetFileNameWithoutExtension(inputFile);
            var newFolder = Path.Combine(Path.GetTempPath(), name);
            if (!Directory.Exists(newFolder))
            {
                Directory.CreateDirectory(newFolder);
            }
            return Path.Combine(newFolder, name + ".gltf");
        }
        [Fact]
        public void Converter_Mat_Tests()
        {
            var objFile = Path.Combine(LoaderTests.SampleRootPath, "plants/indoor plant_02.obj");
            var gltfFile = EnsureOuputPath(objFile);
            var options = new ConverterOptions();
            options.SeparateBinary = true;
            options.SeparateTextures = true;
            var converter = new ModelConverter(objFile, gltfFile, options);
            var model = converter.Run();
            Assert.NotNull(model);
            Assert.True(model.Materials.Count > 0);
        }

        [Fact]
        public void Converter_Obj_Tests()
        {
            var objFile = Path.Combine(LoaderTests.SampleRootPath, "model/model.obj");
            var gltfFile = EnsureOuputPath(objFile);
            var options = new ConverterOptions();
            options.SeparateBinary = true;
            var converter = new ModelConverter(objFile, gltfFile, options);
            var model = converter.Run();
            Assert.NotNull(model);
            Assert.True(model.Materials.Count > 0);
        }
    }
}
