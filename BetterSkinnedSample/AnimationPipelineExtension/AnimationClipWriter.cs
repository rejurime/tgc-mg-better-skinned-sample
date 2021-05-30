using BetterSkinnedSample.AnimationDataTypes;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BetterSkinnedSample.AnimationPipelineExtension
{
    [ContentTypeWriter]
    public class AnimationClipWriter : ContentTypeWriter<AnimationClip>
    {
        protected override void Write(ContentWriter output, AnimationClip value)
        {
            output.Write(value.Name);
            output.Write(value.Duration);
            output.Write(value.Bones.Count);
            foreach (var bone in value.Bones)
            {
                output.Write(bone.Name);
                output.Write(bone.Keyframes.Count);
                foreach (var keyframe in bone.Keyframes)
                {
                    output.Write(keyframe.Time);
                    output.Write(keyframe.Rotation);
                    output.Write(keyframe.Translation);
                }
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(AnimationClipReader).AssemblyQualifiedName;
        }
    }
}