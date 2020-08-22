using System;
using System.Collections.Generic;
using System.Text;

namespace Obj2Gltf
{
    public class ConverterOptions
    {
        /// <summary>
        ///  whether saved as glb file
        /// </summary>
        public bool GLB { get; set; }
        public bool SeparateBinary { get; set; }
        public bool SeparateTextures { get; set; } = true;
    }
}
