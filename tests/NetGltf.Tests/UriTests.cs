using System;
using System.IO;
using Xunit;

namespace NetGltf.Tests
{
    public class UriTests
    {
        [Fact]
        public void WellFormUri_Test()
        {
            var isOK = Uri.IsWellFormedUriString("CM.gltf", UriKind.Absolute);
            Assert.False(isOK);
            var isOK2 = Uri.IsWellFormedUriString("test/CM.gltf", UriKind.Absolute);
            Assert.False(isOK2);            
            var isOK4 = Uri.IsWellFormedUriString("file:///test.jpg", UriKind.Absolute);
            Assert.True(isOK4);
            var isOK5 = Uri.IsWellFormedUriString("https://microsoft.com/robots.txt", UriKind.Absolute);
            Assert.True(isOK5);
            var dataUrl = "data:application/octet-stream;base64,QPnrOD1Ef7+g65o9AAAAAAAAgD80Wd44ROfAuAAAAAAhj684oOuaPTpEfz8AAAAAf7Klu01iUr1oUy2";
            var isOKD = Uri.IsWellFormedUriString(dataUrl, UriKind.Absolute);
            Assert.True(isOKD);
            var isOK3 = Uri.IsWellFormedUriString("/test.jpg", UriKind.Absolute);
            Assert.False(isOK3);
            
        }

        [Fact]
        public void FileUri_Test()
        {
            var uri = new Uri("/Users/Test/test.txt");
            Assert.True(uri.IsFile);
            Assert.Equal("file", uri.Scheme);
        }

        [Fact]
        public void DataUri_Test()
        {
            var uri = new Uri("data:application/octet-stream;base64,QPnrOD1Ef7+g65o9AAAAAAAAgD80Wd44ROfAuAAAAAAhj684oOuaPTpEfz8AAAAAf7Klu01iUr1oUy2");
            Assert.True(!uri.IsFile);
            Assert.Equal("data", uri.Scheme);
        }

        [Fact]
        public void HttpUri_Test()
        {
            var uri = new Uri("https://microsoft.com/robots.txt");
            Assert.True(!uri.IsFile);
            Assert.Equal("https", uri.Scheme);
        }

        [Fact]
        public void Path_Test()
        {
            var host = "https://microsoft.com";
            var path = "like/1";
            var u = Path.Combine(host, path);
            Assert.Equal(host+"/"+path, u);

            var pDir = "/a/b";
            var sDir = "c";
            var dir = Path.Combine(pDir, sDir);
            Assert.Equal(pDir + "/" + sDir, dir);
        }
    }
}
