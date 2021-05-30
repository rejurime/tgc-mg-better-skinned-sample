using Microsoft.Xna.Framework.Content;

namespace BetterSkinnedSample.AnimationDataTypes
{
    public class AnimationClipReader : ContentTypeReader<AnimationClip>
    {
        protected override AnimationClip Read(ContentReader input, AnimationClip existingInstance)
        {
            var clip = new AnimationClip {Name = input.ReadString(), Duration = input.ReadDouble()};

            var boneCnt = input.ReadInt32();
            for (var i = 0; i < boneCnt; i++)
            {
                var bone = new Bone();
                clip.Bones.Add(bone);

                bone.Name = input.ReadString();

                var cnt = input.ReadInt32();

                for (var j = 0; j < cnt; j++)
                {
                    var keyframe = new Keyframe
                    {
                        Time = input.ReadDouble(),
                        Rotation = input.ReadQuaternion(),
                        Translation = input.ReadVector3()
                    };

                    bone.Keyframes.Add(keyframe);
                }
            }

            return clip;
        }
    }
}