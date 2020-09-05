using System;
using System.Collections.Generic;
using System.Text;

namespace Obj2Gltf
{
    /// <summary>
    /// glTF converter options
    /// </summary>
    public class ConverterOptions
    {
        private bool _glb;
        /// <summary>
        ///  whether saved as glb file
        /// </summary>
        public bool GLB
        {
            get => _glb;
            set
            {
                _glb = value;
                if (value)
                {
                    SeparateBinary = false;
                    SeparateTextures = false;
                }
            }
        }
        /// <summary>
        /// whether to separate binary buffers to bin file
        /// </summary>
        public bool SeparateBinary { get; set; }
        /// <summary>
        /// whether to write separate texture files
        /// </summary>
        public bool SeparateTextures { get; set; } = true;
    }
}
