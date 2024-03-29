﻿using System.Collections.Generic;

namespace BetterSkinnedSample.AnimationDataTypes
{
    /// <summary>
    ///     An animation clip is a set of keyframes with associated bones.
    /// </summary>
    public class AnimationClip
    {
        /// <summary>
        ///     The bones for this animation clip with their keyframes.
        /// </summary>
        public List<Bone> Bones { get; set; } = new List<Bone>();

        /// <summary>
        ///     Duration of the animation clip.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        ///     Name of the animation clip.
        /// </summary>
        public string Name { get; set; }
    }
}