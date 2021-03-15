using System.Collections.Generic;
using System.Diagnostics;
using AnimationDataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BetterSkinnedSample
{
    /// <summary>
    ///     An encloser for an XNA model that we will use that includes support for bones, animation, and some manipulations.
    /// </summary>
    public class AnimatedModel
    {
        #region Animation Management

        /// <summary>
        ///     Play an animation clip.
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        public AnimationPlayer PlayClip(AnimationClip clip)
        {
            // Create a clip player and assign it to this model.
            player = new AnimationPlayer(clip, this);
            return player;
        }

        #endregion

        #region Updating

        /// <summary>
        ///     Update animation for the model.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (player != null) player.Update(gameTime);
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Draw the model.
        /// </summary>
        /// <param name="graphics">The graphics device to draw on</param>
        /// <param name="camera">A camera to determine the view</param>
        /// <param name="world">A world matrix to place the model</param>
        public void Draw(GraphicsDevice graphics, Camera camera, Matrix world)
        {
            if (model == null)
                return;

            // Compute all of the bone absolute transforms.
            var boneTransforms = new Matrix[bones.Count];

            for (var i = 0; i < bones.Count; i++)
            {
                var bone = bones[i];
                bone.ComputeAbsoluteTransform();

                boneTransforms[i] = bone.AbsoluteTransform;
            }

            // Determine the skin transforms from the skeleton
            var skeleton = new Matrix[modelExtra.Skeleton.Count];
            for (var s = 0; s < modelExtra.Skeleton.Count; s++)
            {
                var bone = bones[modelExtra.Skeleton[s]];
                skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform;
            }

            // Draw the model.
            foreach (var modelMesh in model.Meshes)
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

        #endregion

        #region Fields

        /// <summary>
        ///     The actual underlying XNA model.
        /// </summary>
        private Model model;

        /// <summary>
        ///     Extra data associated with the XNA model.
        /// </summary>
        private ModelExtra modelExtra;

        /// <summary>
        ///     The model bones.
        /// </summary>
        private readonly List<Bone> bones = new List<Bone>();

        /// <summary>
        ///     The model asset name.
        /// </summary>
        private readonly string assetName = "";

        /// <summary>
        ///     An associated animation clip player.
        /// </summary>
        private AnimationPlayer player;

        #endregion

        #region Properties

        /// <summary>
        ///     The actual underlying XNA model.
        /// </summary>
        public Model Model => model;

        /// <summary>
        ///     The underlying bones for the model.
        /// </summary>
        public List<Bone> Bones => bones;

        /// <summary>
        ///     The model animation clips.
        /// </summary>
        public List<AnimationClip> Clips => modelExtra.Clips;

        #endregion

        #region Construction and Loading

        /// <summary>
        ///     Constructor. Creates the model from an XNA model.
        /// </summary>
        /// <param name="assetName">The name of the asset for this model</param>
        public AnimatedModel(string assetName)
        {
            this.assetName = assetName;
        }

        /// <summary>
        ///     Load the model asset from content.
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>(assetName);
            modelExtra = model.Tag as ModelExtra;
            Debug.Assert(modelExtra != null);

            ObtainBones();
        }

        #endregion

        #region Bones Management

        /// <summary>
        ///     Get the bones from the model and create a bone class object for each bone. We use our bone class to do the real
        ///     animated bone work.
        /// </summary>
        private void ObtainBones()
        {
            bones.Clear();
            foreach (var bone in model.Bones)
            {
                // Create the bone object and add to the hierarchy.
                var newBone = new Bone(bone.Name, bone.Transform,
                    bone.Parent != null ? bones[bone.Parent.Index] : null);

                // Add to the bones for this model.
                bones.Add(newBone);
            }
        }

        /// <summary>
        ///     Find a bone in this model by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bone FindBone(string name)
        {
            foreach (var bone in Bones)
                if (bone.Name == name)
                    return bone;

            return null;
        }

        #endregion
    }
}