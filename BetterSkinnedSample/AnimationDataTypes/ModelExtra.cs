using System.Collections.Generic;

namespace BetterSkinnedSample.AnimationDataTypes
{
    /// <summary>
    ///     Class that contains additional information attached to the model and shared with the runtime.
    /// </summary>
    public class ModelExtra
    {
        /// <summary>
        ///     Animation clips associated with this model.
        /// </summary>
        public List<AnimationClip> Clips { get; set; } = new List<AnimationClip>();

        /// <summary>
        ///     The bone indices for the skeleton associated with any skinned model.
        /// </summary>
        public List<int> Skeleton { get; set; } = new List<int>();
    }
}