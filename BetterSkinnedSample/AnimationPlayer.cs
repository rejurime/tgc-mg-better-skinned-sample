using System.ComponentModel;
using BetterSkinnedSample.AnimationDataTypes;
using Microsoft.Xna.Framework;

namespace BetterSkinnedSample
{
    /// <summary>
    ///     Animation clip player. It maps an animation clip onto a model.
    /// </summary>
    public class AnimationPlayer
    {
        #region Construction

        /// <summary>
        ///     Constructor for the animation player. It makes the association between a clip and a model and sets up for playing.
        /// </summary>
        /// <param name="clip"></param>
        public AnimationPlayer(AnimationClip clip, AnimatedModel model)
        {
            this.clip = clip;
            this.model = model;

            // Create the bone information classes.
            boneCnt = clip.Bones.Count;
            boneInfos = new BoneInfo[boneCnt];

            for (var b = 0; b < boneInfos.Length; b++)
            {
                // Create it.
                boneInfos[b] = new BoneInfo(clip.Bones[b]);

                // Assign it to a model bone.
                boneInfos[b].SetModel(model);
            }

            Rewind();
        }

        #endregion

        #region BoneInfo class

        /// <summary>
        ///     Information about a bone we are animating. This class connects a bone in the clip to a bone in the model.
        /// </summary>
        private class BoneInfo
        {
            #region Constructor

            /// <summary>
            ///     Constructor.
            /// </summary>
            /// <param name="bone"></param>
            public BoneInfo(AnimationClip.Bone bone)
            {
                ClipBone = bone;
                SetKeyframes();
                SetPosition(0);
            }

            #endregion

            #region Fields

            /// <summary>
            ///     The current keyframe. Our position is a time such that the we are greater than or equal to this keyframe's time and
            ///     less than the next keyframes time.
            /// </summary>
            private int currentKeyframe;

            /// <summary>
            ///     Bone in a model that this keyframe bone is assigned to.
            /// </summary>
            private Bone assignedBone;

            /// <summary>
            ///     We are not valid until the rotation and translation are set.
            ///     If there are no keyframes, we will never be valid.
            /// </summary>
            public bool valid;

            /// <summary>
            ///     Current animation rotation.
            /// </summary>
            private Quaternion rotation;

            /// <summary>
            ///     Current animation translation.
            /// </summary>
            public Vector3 translation;

            /// <summary>
            ///     We are at a location between Keyframe1 and Keyframe2 such that Keyframe1's time is less than or equal to the
            ///     current position.
            /// </summary>
            public AnimationClip.Keyframe Keyframe1;

            /// <summary>
            ///     Second keyframe value.
            /// </summary>
            public AnimationClip.Keyframe Keyframe2;

            #endregion

            #region Properties

            /// <summary>
            ///     The bone in the actual animation clip.
            /// </summary>
            public AnimationClip.Bone ClipBone { get; }

            /// <summary>
            ///     The bone this animation bone is assigned to in the model.
            /// </summary>
            public Bone ModelBone => assignedBone;

            #endregion

            #region Position and Keyframes

            /// <summary>
            ///     Set the bone based on the supplied position value.
            /// </summary>
            /// <param name="position"></param>
            public void SetPosition(float position)
            {
                var keyframes = ClipBone.Keyframes;
                if (keyframes.Count == 0)
                    return;

                // If our current position is less that the first keyframe we move the position backward until we get to the right keyframe.
                while (position < Keyframe1.Time && currentKeyframe > 0)
                {
                    // We need to move backwards in time.
                    currentKeyframe--;
                    SetKeyframes();
                }

                // If our current position is greater than the second keyframe we move the position forward until we get to the right keyframe.
                while (position >= Keyframe2.Time && currentKeyframe < ClipBone.Keyframes.Count - 2)
                {
                    // We need to move forwards in time.
                    currentKeyframe++;
                    SetKeyframes();
                }

                if (Keyframe1 == Keyframe2)
                {
                    // Keyframes are equal.
                    rotation = Keyframe1.Rotation;
                    translation = Keyframe1.Translation;
                }
                else
                {
                    // Interpolate between keyframes.
                    var t = (float) ((position - Keyframe1.Time) / (Keyframe2.Time - Keyframe1.Time));
                    rotation = Quaternion.Slerp(Keyframe1.Rotation, Keyframe2.Rotation, t);
                    translation = Vector3.Lerp(Keyframe1.Translation, Keyframe2.Translation, t);
                }

                valid = true;
                if (assignedBone != null)
                {
                    // Send to the model.
                    // Make it a matrix first.
                    var m = Matrix.CreateFromQuaternion(rotation);
                    m.Translation = translation;
                    assignedBone.SetCompleteTransform(m);
                }
            }


            /// <summary>
            ///     Set the keyframes to a valid value relative to the current keyframe.
            /// </summary>
            private void SetKeyframes()
            {
                if (ClipBone.Keyframes.Count > 0)
                {
                    Keyframe1 = ClipBone.Keyframes[currentKeyframe];
                    if (currentKeyframe == ClipBone.Keyframes.Count - 1)
                        Keyframe2 = Keyframe1;
                    else
                        Keyframe2 = ClipBone.Keyframes[currentKeyframe + 1];
                }
                else
                {
                    // If there are no keyframes, set both to null.
                    Keyframe1 = null;
                    Keyframe2 = null;
                }
            }

            /// <summary>
            ///     Assign this bone to the correct bone in the model.
            /// </summary>
            /// <param name="model"></param>
            public void SetModel(AnimatedModel model)
            {
                // Find this bone
                assignedBone = model.FindBone(ClipBone.Name);
            }

            #endregion
        }

        #endregion

        #region Fields

        /// <summary>
        ///     Current position in time in the clip.
        /// </summary>
        private float position;

        /// <summary>
        ///     The clip we are playing.
        /// </summary>
        private readonly AnimationClip clip;

        /// <summary>
        ///     We maintain a BoneInfo class for each bone. This class does most of the work in playing the animation.
        /// </summary>
        private readonly BoneInfo[] boneInfos;

        /// <summary>
        ///     The number of bones.
        /// </summary>
        private readonly int boneCnt;

        /// <summary>
        ///     An assigned model.
        /// </summary>
        private readonly AnimatedModel model;

        /// <summary>
        ///     The looping option.
        /// </summary>
        private bool looping;

        #endregion

        #region Properties

        /// <summary>
        ///     The position in the animation.
        /// </summary>
        [Browsable(false)]
        public float Position
        {
            get => position;
            set
            {
                if (value > Duration)
                    value = Duration;

                position = value;
                foreach (var bone in boneInfos) bone.SetPosition(position);
            }
        }

        /// <summary>
        ///     The associated animation clip.
        /// </summary>
        [Browsable(false)]
        public AnimationClip Clip => clip;

        /// <summary>
        ///     The clip duration.
        /// </summary>
        [Browsable(false)]
        public float Duration => (float) clip.Duration;

        /// <summary>
        ///     A model this animation is assigned to. It will play on that model.
        /// </summary>
        [Browsable(false)]
        public AnimatedModel Model => model;

        /// <summary>
        ///     The looping option. Set to true if you want the animation to loop back at the end.
        /// </summary>
        public bool Looping
        {
            get => looping;
            set => looping = value;
        }

        #endregion

        #region Update and Transport Controls

        /// <summary>
        ///     Reset back to time zero.
        /// </summary>
        public void Rewind()
        {
            Position = 0;
        }

        /// <summary>
        ///     Update the clip position.
        /// </summary>
        /// <param name="delta"></param>
        public void Update(GameTime gameTime)
        {
            Position = Position + (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (looping && Position >= Duration)
                Position = 0;
        }

        #endregion
    }
}