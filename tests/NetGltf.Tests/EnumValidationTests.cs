using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using NetGltf.Json;

namespace NetGltf.Tests
{
    public class EnumValidationTests
    {
        [Fact]
        public void Enum_Parse_Test()
        {
            var type = (TargetType)1;
            Assert.False(Enum.IsDefined(typeof(TargetType), type));

            var ok = Enum.TryParse<TargetType>("Test", out var res);
            Assert.False(ok);
            Assert.False(Enum.IsDefined(typeof(TargetType), res));
        }
    }
}
