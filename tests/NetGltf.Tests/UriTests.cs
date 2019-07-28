using System;
using System.IO;
using Xunit;

namespace NetGltf.Tests
{
    public class UriTests
    {
        [Fact]
        public void UriUtil_ValidUri_Test()
        {
            var relativeUrl = "test/1.html";
            Assert.True(UriUtil.IsValidUri(relativeUrl));
            var absoluteUrl = "/test/1.html";
            Assert.True(UriUtil.IsValidUri(absoluteUrl));
            var dataUrl = "data:application/octet-stream;base64,QPnrOD1Ef7+g65o9AAAAAAAAgD80Wd44ROfAuAAAAAAhj684oOuaPTpEfz8AAAAAf7Klu01iUr1oUy2";
            Assert.True(UriUtil.IsValidUri(dataUrl));
            var httpUrl = "http://test.com/1";
            Assert.True(UriUtil.IsValidUri(httpUrl));
            if (UriUtil.IsWin())
            {
                var windowsUrl = @"c:\1\2\";
                Assert.True(UriUtil.IsValidUri(windowsUrl));
                var windowsUrl2 = @"\1\2\"; 
                Assert.True(UriUtil.IsValidUri(windowsUrl2));
            }
            else
            {
                var unixUrl = "/1/2/";
                Assert.True(UriUtil.IsValidUri(unixUrl));
            }
            
            var fileUrl = "file:///d:/1/2";
            Assert.True(UriUtil.IsValidUri(fileUrl));
        }

        [Fact]
        public void UriUtil_Normalize_Test()
        {
            var http = "http://test.com";
            Assert.Equal(http, UriUtil.Normalize(http));
            var https = "https://test.com";
            Assert.Equal(https, UriUtil.Normalize(https));
            var ftp = "ftp://test.com";
            Assert.Equal(ftp, UriUtil.Normalize(ftp));
            if (UriUtil.IsWin())
            {
                var windowsUrl = @"c:\1\2"; //@"c:\folder\myfile.txt";
                var nUrl = UriUtil.Normalize(windowsUrl);
                Assert.Equal("file:///c:/1/2", nUrl);
            }
            else
            {
                var unixUrl = "/1/2/";
                var nUrl = UriUtil.Normalize(unixUrl);
                Assert.Equal("file:///1/2", nUrl);
            }
        }

        [Fact]
        public void UriUtil_Combine_Test()
        {
            var http = "http:/test.com";
            var rPath = "test/test.html";
            Assert.Equal(http + "/" + rPath, UriUtil.Combine(http, rPath));

            if (UriUtil.IsWin())
            {
                var root = @"\test\test";
                var rPath2 = @"\test\test.html";
                Assert.Equal("/test/test/test/test.html", UriUtil.Combine(root, rPath2));
                var rPath3 = @"..\test\test.html";
                var cPath = UriUtil.Combine(root, rPath3);
                var pRoot = Path.GetPathRoot(Path.GetFullPath("/"));
                Assert.Equal($@"{pRoot}test\test\test.html", Path.GetFullPath(cPath));
            }
            else
            {
                var root = "/test/test";
                var rPath2 = "/test/test.html";
                Assert.Equal("/test/test/test/test.html", UriUtil.Combine(root, rPath2));
                var rPath3 = "../test/test.html";
                var cPath = UriUtil.Combine(root, rPath3);
                var pRoot = Path.GetPathRoot(Path.GetFullPath("/"));
                Assert.Equal($@"{pRoot}test/test/test.html", Path.GetFullPath(cPath));
            }
            
        }

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
            var url = "/Users/Test/test.txt";
            if (UriUtil.IsWin()) // for windows
            {
                url = "c:"+url;
            }
            var uri = new Uri(url, UriKind.Absolute);
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
            var u = Path.Combine(host, path).Replace(Path.DirectorySeparatorChar, '/');
            Assert.Equal(host+"/"+path, u);

            var pDir = "/a/b";
            var sDir = "c";
            var dir = Path.Combine(pDir, sDir).Replace(Path.DirectorySeparatorChar, '/');
            Assert.Equal(pDir + "/" + sDir, dir);
        }
    }
}
