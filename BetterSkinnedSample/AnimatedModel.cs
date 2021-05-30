using System.Collections.Generic;
using System.Diagnostics;
using BetterSkinnedSample.AnimationDataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BetterSkinnedSample
{
    /// <summary>
    ///     An encloser for an XNA Model that we will use that includes support for Bones, animation, and some manipulations.
    /// </summary>
    public class AnimatedModel
    {
        /// <summary>
        ///     Creates the Model from an XNA Model.
        /// </summary>
        /// <param name="assetName">The name of the asset for this Model</param>
        public AnimatedModel(string assetName)
        {
            AssetName = assetName;
        }

        /// <summary>
        ///     The Model asset name.
        /// </summary>
        private string AssetName { get; }

        /// <summary>
        ///     The underlying Bones for the Model.
        /// </summary>
        private List<Bone> Bones { get; } = new List<Bone>();

        /// <summary>
        ///     The actual underlying XNA Model.
        /// </summary>
        private Model Model { get; set; }

        /// <summary>
        ///     Extra data associated with the XNA Model.
        /// </summary>
        private ModelExtra ModelExtra { get; set; }

        /// <summary>
        ///     An associated animation clip Player.
        /// </summary>
        private AnimationPlayer Player { get; set; }

        /// <summary>
        ///     The Model animation clips.
        /// </summary>
        public List<AnimationClip> Clips => ModelExtra.Clips;

        /// <summary>
        ///     Play an animation clip.
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The Player that will play this clip</returns>
        public AnimationPlayer PlayClip(AnimationClip clip)
        {
            // Create a clip Player and assign it to this Model.
            Player = new AnimationPlayer(clip, this);
            return Player;
        }

        /// <summary>
        ///     Update animation for the Model.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (Player != null) Player.Update(gameTime);
        }

        /// <summary>
        ///     Draw the Model.
        /// </summary>
        /// <param name="graphics">The graphics device to draw on</param>
        /// <param name="camera">A camera to determine the view</param>
        /// <param name="world">A world matrix to place the Model</param>
        public void Draw(GraphicsDevice graphics, Camera camera, Matrix world)
        {
            if (Model == null)
                return;

            // Compute all of the bone absolute transforms.
            var boneTransforms = new Matrix[Bones.Count];

            for (var i = 0; i < Bones.Count; i++)
            {
                var bone = Bones[i];
                bone.ComputeAbsoluteTransform();

                boneTransforms[i] = bone.AbsoluteTransform;
            }

            // Determine the skin transforms from the skeleton
            var skeleton = new Matrix[ModelExtra.Skeleton.Count];
            for (var s = 0; s < ModelExtra.Skeleton.Count; s++)
            {
                var bone = Bones[ModelExtra.Skeleton[s]];
                skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform;
            }

            // Draw the Model.
            foreach (var modelMesh in Model.Meshes)
            {
                foreach (var effect in modelMesh.Effects)
                {
                    if (effect is BasicEffect)
                    {
                        var beffect = effect as BasicEffect;
                        beffect.World = boneTransforms[modelMesh.ParentBone.Index] * world;
                        beffect.View = camera.View;
                        beffect.Projection = camera.Projection;
                        beffect.EnableDefaultLighting();
                        beffect.PreferPerPixelLighting = true;
                    }

                    if (effect is SkinnedEffect)
                    {
                        var seffect = effect as SkinnedEffect;
                        seffect.World = boneTransforms[modelMesh.ParentBone.Index] * world;
                        seffect.View = camera.View;
                        seffect.Projection = camera.Projection;
                        seffect.EnableDefaultLighting();
                        seffect.PreferPerPixelLighting = true;
                        seffect.SetBoneTransforms(skeleton);
                    }
                }

                modelMesh.Draw();
            }
        }

        /// <summary>
        ///     Load the Model asset from content.
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>(AssetName);
            ModelExtra = Model.Tag as ModelExtra;
            Debug.Assert(ModelExtra != null);

            ObtainBones();
        }

        /// <summary>
        ///     Get the Bones from the Model and create a bone class object for each bone. We use our bone class to do the real
        ///     animated bone work.
        /// </summary>
        private void ObtainBones()
        {
            Bones.Clear();
            foreach (var bone in Model.Bones)
            {
                // Create the bone object and add to the hierarchy.
                var newBone = new Bone(bone.Name, bone.Transform,
                    bone.Parent != null ? Bones[bone.Parent.Index] : null);

                // Add to the Bones for this Model.
                Bones.Add(newBone);
            }
        }

        /// <summary>
        ///     Find a bone in this Model by name.
        /// </summary>
        public Bone FindBone(string name)
        {
            foreach (var bone in Bones)
                if (bone.Name == name)
                    return bone;

            return null;
        }
    }
}