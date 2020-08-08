using System;
using System.Collections.Generic;
using System.Text;

namespace Obj2Gltf
{
    public enum UpAxis
    {
        X,
        Y = 0,
        Z
    }
    public class ConverterOptions
    {
        /// <summary>
        ///  whether saved as glb file
        /// </summary>
        public bool Binary { get; set; }
        public bool SeparateBinary { get; set; }
        public bool SeparateTextures { get; set; } = true;

        public UpAxis InputUpAxis { get; set; }

        public UpAxis OutputUpAxis { get; set; }
    }
}
