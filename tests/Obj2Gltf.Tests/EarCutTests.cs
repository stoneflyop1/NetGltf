using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Obj2Gltf.Tests
{
    public class EarCutTests
    {
        [Fact]
        public void Indices2d_Tests()
        {
            var data = new List<float>
            {
                10, 0, 0, 50, 60, 60, 70, 10
            };
            var res = EarCut.Triangulate(data);
            var indices = new List<int>
            {
                1, 0, 3, 3, 2, 1
            };
            Assert.True(indices.Count == res.Count);
            for(var i = 0; i < res.Count;i++)
            {
                Assert.Equal(indices[i], res[i]);
            }

            data = new List<float>
            {
                661, 112,
                661, 96,
                666, 96,
                666, 87,
                743, 87,
                771, 87,
                771, 114,
                750, 114,
                750, 113,
                742, 113,
                742, 106,
                710, 106,
                710, 113,
                666, 113,
                666, 112
            };

            res = EarCut.Triangulate(data);
            Assert.Equal(13, res.Count/3);
        }
    }
}
