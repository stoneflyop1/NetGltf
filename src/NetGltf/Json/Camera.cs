using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NetGltf.Json
{
    /// <summary>
    /// A camera's projection.
    ///
    /// A node can reference a camera to apply a transform to place the camera in the
    /// scene.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name {get;set;}

        /// <summary>
        /// Specifies if the camera uses a perspective or orthographic projection.
        /// </summary>
        [JsonProperty("type")]
        public CheckedValue<CameraType,string> CameraType {get;set;}

        /// <summary>
        /// An orthographic camera containing properties to create an orthographic projection matrix.
        /// </summary>
        [JsonProperty("orthographic")]
        public Orthographic Orthographic {get;set;}

        /// <summary>
        /// A perspective camera containing properties to create a perspective projection matrix.
        /// </summary>
        [JsonProperty("perspective")]
        public Perspective Perspective {get;set;}
    }
    /// <summary>
    /// https://bytefish.de/blog/enums_json_net/
    /// </summary>
    public enum CameraType
    {
        /// <summary>A perspective projection.</summary>
        Perspective,
        /// <summary>A orthographic projection.</summary>
        Orthographic
    }
    /// <summary>
    /// Values for an orthographic camera.
    /// </summary>
    public class Orthographic
    {
        /// <summary>
        /// The horizontal magnification of the view.
        /// </summary>
        [JsonProperty("xmag")]
        public float Xmag {get;set;}
        /// <summary>
        /// The vertical magnification of the view.
        /// </summary>
        [JsonProperty("ymag")]
        public float Ymag {get;set;}
        /// <summary>
        /// The distance to the far clipping plane.
        /// </summary>
        [JsonProperty("zfar")]
        public float Zfar {get;set;}

        /// <summary>
        /// The distance to the near clipping plane.
        /// </summary>
        [JsonProperty("znear")]
        public float Znear {get;set;}
    }
    /// <summary>
    /// 
    /// </summary>
    public class Perspective
    {
        /// <summary>
        /// spect ratio of the field of view.
        /// </summary>
        [JsonProperty("aspectRatio")]
        public float? AspectRatio {get;set;}

        /// <summary>
        /// The vertical field of view in radians.
        /// </summary>
        [JsonProperty("yfov")]
        public float Yfov {get;set;}

        /// <summary>
        /// The distance to the far clipping plane.
        /// </summary>
        [JsonProperty("zfar")]
        public float? Zfar {get;set;}

        /// <summary>
        /// The distance to the near clipping plane.
        /// </summary>
        [JsonProperty("znear")]
        public float Znear {get;set;}
    }
}
