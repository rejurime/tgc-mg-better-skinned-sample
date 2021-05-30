using BetterSkinnedSample.AnimationDataTypes;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BetterSkinnedSample.AnimationPipelineExtension
{
    [ContentTypeWriter]
    public class ModelExtraWriter : ContentTypeWriter<ModelExtra>
    {
        protected override void Write(ContentWriter output, ModelExtra value)
        {
            output.WriteObject(value.Skeleton);
            output.WriteObject(value.Clips);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ModelExtraReader).AssemblyQualifiedName;
        }
    }
}