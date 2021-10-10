using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace BetterSkinnedSample.AnimationPipelineExtension
{
    // TODO. This class needs a refactor.
    public class CustomPipelineManager : PipelineManager
    {
        private const string BinFolder = "bin/";
        private const string ObjFolder = "obj/";
        private const string ContentExtension = ".xnb";
        private const string ContentFolder = "Content/";
        private const string FbxExtension = ".fbx";
        private const string FbxImporterName = "FbxImporter";
        private const string ProcessorName = "Animation Processor";

        public CustomPipelineManager(string projectDir, string outputDir, string intermediateDir) : base(projectDir,
            outputDir, intermediateDir)
        {
        }

        // Provides methods for writing compiled binary format.
        public ContentCompiler Compiler { get; private set; }

        public static CustomPipelineManager CreateCustomPipelineManager()
        {
            // This code is from MonoGame.Content.Builder.BuildContent.Build(out int successCount, out int errorCount)
            var projectDirectory = PathHelper.Normalize(Directory.GetCurrentDirectory());
            var projectContentDirectory =
                PathHelper.Normalize(Path.GetFullPath(Path.Combine(projectDirectory, "../../../" + ContentFolder)));
            var outputPath = projectDirectory + "/" + ContentFolder;
            var projectDirectoryParts = projectDirectory.Split(new[] { BinFolder }, StringSplitOptions.None);
            var intermediatePath = PathHelper.Normalize(Path.GetFullPath(Path.Combine(projectContentDirectory,
                "../" + ObjFolder + projectDirectoryParts[projectDirectoryParts.Length - 1])));

            return new CustomPipelineManager(projectContentDirectory, outputPath, intermediatePath);
        }

        public void BuildAnimationContent(string modelFilename)
        {
            var importContext = new PipelineImporterContext(this);
            var importer = new FbxImporter();
            var nodeContent = importer.Import(ProjectDirectory + modelFilename + FbxExtension, importContext);
            var animationProcessor = new AnimationProcessor();

            var parameters = new OpaqueDataDictionary
            {
                { "ColorKeyColor", "0,0,0,0" },
                { "ColorKeyEnabled", "True" },
                { "DefaultEffect", "BasicEffect" },
                { "GenerateMipmaps", "True" },
                { "GenerateTangentFrames", "False" },
                { "PremultiplyTextureAlpha", "True" },
                { "PremultiplyVertexColors", "True" },
                { "ResizeTexturesToPowerOfTwo", "False" },
                { "RotationX", "0" },
                { "RotationY", "0" },
                { "RotationZ", "0" },
                { "Scale", "1" },
                { "SwapWindingOrder", "False" },
                { "TextureFormat", "Compressed" }
            };

            // Record what we're building and how.
            var pipelineEvent = new PipelineBuildEvent
            {
                SourceFile = modelFilename,
                DestFile = OutputDirectory + modelFilename + ContentExtension,
                Importer = FbxImporterName,
                Processor = ProcessorName,
                Parameters = ValidateProcessorParameters(ProcessorName, parameters)
            };

            var processContext = new PipelineProcessorContext(this, pipelineEvent);
            var modelContent = animationProcessor.Process(nodeContent, processContext);

            // Write the content to disk.
            WriteXnb(modelContent, pipelineEvent);
        }

        private void WriteXnb(object content, PipelineBuildEvent pipelineEvent)
        {
            // Make sure the output directory exists.
            var outputFileDir = Path.GetDirectoryName(pipelineEvent.DestFile);

            Directory.CreateDirectory(outputFileDir);

            Compiler ??= new ContentCompiler();

            // Write the XNB.
            using (var stream =
                new FileStream(pipelineEvent.DestFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Compiler.Compile(stream, content, Platform, Profile, CompressContent, OutputDirectory, outputFileDir);
            }

            // Store the last write time of the output XNB here so we can verify it hasn't been tampered with.
            pipelineEvent.DestTime = File.GetLastWriteTime(pipelineEvent.DestFile);
        }
    }
}