using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NetGltf.Json
{
    public class Animation
    {

        [JsonProperty("name")]
        public string Name {get;set;}

        [JsonProperty("channels")]
        public List<Channel> Channels {get;set;}

        [JsonProperty("samplers")]
        public List<AnimationSampler> Samplers {get;set;}
    }

    public class Channel
    {
        /// <summary>
        /// The index of a sampler in this animation used to compute the value for the target
        /// </summary>
        [JsonProperty("sampler")]
        public Index<AnimationSampler> Sampler {get;set;}
        /// <summary>
        /// The index of the node and TRS property to target.
        /// </summary>
        [JsonProperty("target")]
        public Target Target {get;set;}

    }

    public class Target{

        /// <summary>
        /// The index of the node to target.
        /// </summary>
        [JsonProperty("node")]
        public Index<Node> Node {get;set;}
        /// <summary>
        /// The name of the node's property to modify or the 'weights' of the
        /// morph targets it instantiates.
        /// </summary>
        [JsonProperty("path")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TargetProperty Path {get;set;}
    }

    public enum TargetProperty
    {
        /// <summary>
        /// XYZ translation vector, VEC3
        /// </summary>
        translation,
        /// <summary>
        /// XYZW rotation quaternion, VEC4
        /// </summary>
        rotation,
        /// <summary>
        /// XYZ scale vector, VEC3
        /// </summary>
        scale,
        /// <summary>
        /// Weights of morph targets, SCALAR
        /// </summary>
        weights
    }

    public enum Interpolation
    {
        /// <summary>
        /// Linear interpolation.
        ///
        /// The animated values are linearly interpolated between keyframes.
        /// When targeting a rotation, spherical linear interpolation (slerp) should be
        /// used to interpolate quaternions. The number output of elements must equal
        /// the number of input elements.
        /// </summary>
        LINEAR = 1,
        /// <summary>
        /// Step interpolation.
        ///
        /// The animated values remain constant to the output of the first keyframe,
        /// until the next keyframe. The number of output elements must equal the number
        /// of input elements.
        /// </summary>
        STEP,
        /// <summary>
        /// Uniform Catmull-Rom spline interpolation.
        ///
        /// The animation's interpolation is computed using a uniform Catmull-Rom spline.
        /// The number of output elements must equal two more than the number of input
        /// elements. The first and last output elements represent the start and end
        /// tangents of the spline. There must be at least four keyframes when using this
        /// interpolation.
        /// </summary>
        CATMULLROMSPLINE,        //CatmullRomSpline,
        /// <summary>
        /// Cubic spline interpolation.
        ///
        /// The animation's interpolation is computed using a uniform Catmull-Rom spline.
        /// The number of output elements must equal two more than the number of input
        /// elements. The first and last output elements represent the start and end
        /// tangents of the spline. There must be at least four keyframes when using this
        /// interpolation.
        /// </summary>
        CUBICSPLINE,
    }

    public class AnimationSampler
    {
        /// <summary>
        /// The index of an accessor containing keyframe input values, e.g., time.
        /// </summary>
        [JsonProperty("input")]
        public Index<Accessor> Input {get;set;}
        /// <summary>
        /// The index of an accessor containing keyframe output values.
        /// </summary>
        [JsonProperty("output")]
        public Index<Accessor> Output {get;set;}

        [JsonProperty("interpolation")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Interpolation Interpolation {get;set;} = Interpolation.LINEAR;
    }
}