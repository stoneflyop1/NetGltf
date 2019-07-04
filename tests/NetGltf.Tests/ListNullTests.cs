using System.Collections.Generic;
using Xunit;

namespace NetGltf.Tests
{
    public class ListNullTests
    {
        [Fact]
        public void NullAndEmpty_Test()
        {
            List<int> null_t = null;
            var empty_t = new List<int>();
            Assert.False(null_t is List<int>);
            Assert.True(empty_t is List<int>);
        }
    }
}
