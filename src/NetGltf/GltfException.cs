using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NetGltf
{
    public class GltfException : Exception
    {
        private readonly List<GltfError> _errors;

        public GltfException(IEnumerable<GltfError> errors):this(errors, null)
        {

        }

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

        public override string Message
        {
            get 
            {
                return _mess;
            }
        }

        public IReadOnlyCollection<GltfError> Errors
        {
            get
            {
                return new ReadOnlyCollection<GltfError>(_errors);
            }
        }
    }

    public class GltfError
    {
        public ErrorKind Kind {get;set;}

        public string Message {get;set;}
    }

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
