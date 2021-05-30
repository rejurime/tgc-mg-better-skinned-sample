using Microsoft.Xna.Framework;

namespace BetterSkinnedSample.AnimationDataTypes
{
    /// <summary>
    ///     An Keyframe is a rotation and Translation for a moment in time.
    ///     It would be easy to extend this to include scaling as well.
    /// </summary>
    public class Keyframe
    {
        // The rotation for the bone.
        public Quaternion Rotation { get; set; }

        // The keyframe time.
        public double Time { get; set; }

        // The Translation for the bone.
        public Vector3 Translation { get; set; }

        public Matrix Transform
        {
            get => Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Translation);
            set
            {
                var transform = value;
                transform.Right = Vector3.Normalize(transform.Right);
                transform.Up = Vector3.Normalize(transform.Up);
                transform.Backward = Vector3.Normalize(transform.Backward);
                Rotation = Quaternion.CreateFromRotationMatrix(transform);
                Translation = transform.Translation;
            }
        }
    }
}