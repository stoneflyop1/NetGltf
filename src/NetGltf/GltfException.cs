using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NetGltf
{
    /// <summary>
    /// custom glTF exception
    /// </summary>
    public class GltfException : Exception
    {
        private readonly List<GltfError> _errors;
        /// <summary>
        /// from errors
        /// </summary>
        /// <param name="errors"></param>
        public GltfException(IEnumerable<GltfError> errors):this(errors, null)
        {

        }
        /// <summary>
        /// from errors with inner
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="inner"></param>
        public GltfException(IEnumerable<GltfError> errors, Exception inner)
            : base(null, inner)
        {
            if (errors is List<GltfError> t)
            {
                _errors = t;
            }
            else
            {
                _errors = new List<GltfError>();
                if (errors != null)
                {
                    _errors.AddRange(errors);
                }
            }
            _mess = _errors.Count > 0 ? String.Join("; ", _errors.Select(c=>c.Message)) : String.Empty;
        }

        private readonly string _mess;
        /// <summary>
        /// error message
        /// </summary>
        public override string Message => _mess;
        /// <summary>
        /// file errors
        /// </summary>
        public IReadOnlyCollection<GltfError> Errors
        {
            get
            {
                return new ReadOnlyCollection<GltfError>(_errors);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class GltfError
    {
        /// <summary>
        /// Error Kind
        /// </summary>
        public ErrorKind Kind {get;set;}
        /// <summary>
        /// error message
        /// </summary>
        public string Message {get;set;}
    }
    /// <summary>
    /// gltf error kind
    /// </summary>
    public enum ErrorKind
    {
        /// <summary> glTF validation error. </summary>
        Validation,
        /// <summary> Base 64 decoding error. </summary>
        Base64,
        /// <summary> GLB parsing error. </summary>
        Glb,
        /// <summary> Standard I/O error. </summary>
        Io,
        /// <summary> unsupported uris </summary>
        Uri,
        /// <summary> The `BIN` chunk of binary glTF is referenced but does not exist. </summary>
        MissingBlob
    }
}
