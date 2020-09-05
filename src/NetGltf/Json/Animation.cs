using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NetGltf.Json
{
    /// <summary>
    /// A keyframe animation.
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// An array of channels, each of which targets an animation's sampler at a
        /// node's property.
        ///
        /// Different channels of the same animation must not have equal targets.
        /// </summary>

        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; }
        /// <summary>
        /// An array of samplers that combine input and output accessors with an
        /// interpolation algorithm to define a keyframe graph (but not its target).
        /// </summary>

        [JsonProperty("samplers")]
        public List<AnimationSampler> Samplers { get; set; }
    }
    /// <summary>
    /// Targets an animation's sampler at a node's property.
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// The index of a sampler in this animation used to compute the value for the target
        /// </summary>
        [JsonProperty("sampler")]
        public Index<AnimationSampler> Sampler { get; set; }
        /// <summary>
        /// The index of the node and TRS property to target.
        /// </summary>
        [JsonProperty("target")]
        public Target Target { get; set; }

    }
    /// <summary>
    /// The index of the node and TRS property that an animation channel targets.
    /// </summary>
    public class Target
    {

        /// <summary>
        /// The index of the node to target.
        /// </summary>
        [JsonProperty("node")]
        public Index<Node> Node { get; set; }
        /// <summary>
        /// The name of the node's property to modify or the 'weights' of the
        /// morph targets it instantiates.
        /// </summary>
        [JsonProperty("path")]
        //[JsonConverter(typeof(StringEnumConverter), true)]
        public CheckedValue<TargetProperty,string> Path { get; set; }
    }
    /// <summary>
    /// Specifies a property to animate.
    /// </summary>
    public enum TargetProperty
    {
        /// <summary>
        /// XYZ translation vector, VEC3
        /// </summary>
        Translation,
        /// <summary>
        /// XYZW rotation quaternion, VEC4
        /// </summary>
        Rotation,
        /// <summary>
        /// XYZ scale vector, VEC3
        /// </summary>
        Scale,
        /// <summary>
        /// Weights of morph targets, SCALAR
        /// </summary>
        Weights
    }
    /// <summary>
    /// Specifies an interpolation algorithm.
    /// </summary>
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
        LINEAR,
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
        CATMULLROMSPLINE, //CatmullRomSpline,
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
    /// <summary>
    /// Defines a keyframe graph but not its target.
    /// </summary>
    public class AnimationSampler
    {
        /// <summary>
        /// The index of an accessor containing keyframe input values, e.g., time.
        /// </summary>
        [JsonProperty("input")]
        public Index<Accessor> Input { get; set; }
        /// <summary>
        /// The index of an accessor containing keyframe output values.
        /// </summary>
        [JsonProperty("output")]
        public Index<Accessor> Output { get; set; }
        /// <summary>
        /// The interpolation algorithm.
        /// </summary>
        [JsonProperty("interpolation")]
        //[JsonConverter(typeof(StringEnumConverter))]
        public CheckedValue<Interpolation, string> Interpolation { get; set; }
    }
}
