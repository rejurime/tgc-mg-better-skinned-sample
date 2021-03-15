using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace AnimationDataTypes
{
    public class ModelExtraReader : ContentTypeReader<ModelExtra>
    {
        protected override ModelExtra Read(ContentReader input, ModelExtra existingInstance)
        {
            var extra = new ModelExtra
            {
                Skeleton = input.ReadObject<List<int>>(),
                Clips = input.ReadObject<List<AnimationClip>>()
            };

            return extra;
        }
    }
}