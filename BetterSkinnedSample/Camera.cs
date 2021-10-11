using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BetterSkinnedSample
{
    /// <summary>
    ///     Simple class that implements a camera that can be manipulated using the mouse.
    /// </summary>
    public class Camera
    {
        private readonly float fov = MathHelper.ToRadians(35);

        private readonly GraphicsDeviceManager graphics;

        /// <summary>
        ///     The up direction.
        /// </summary>
        private readonly Vector3 up = Vector3.Up;

        /// <summary>
        ///     The location we are looking at in space.
        /// </summary>
        private Vector3 center = Vector3.Zero;

        /// <summary>
        ///     The eye position in space.
        /// </summary>
        private Vector3 eye = new Vector3(250, 200, 350);

        private MouseState lastMouseState;
        private float zfar = 10000;
        private float znear = 10;

        /// <summary>
        ///     Constructor.
        ///     Initializes the graphics field from a passed parameter.
        /// </summary>
        /// <param name="graphics">The graphics device manager for our program.</param>
        public Camera(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
        }

        /// <summary>
        ///     The computed view matrix.
        /// </summary>
        public Matrix View { get; private set; }

        /// <summary>
        ///     The computed projection matrix.
        /// </summary>
        public Matrix Projection { get; private set; }

        public Vector3 Center
        {
            get => center;
            set
            {
                center = value;
                ComputeView();
            }
        }

        public Vector3 Eye
        {
            get => eye;
            set
            {
                eye = value;
                ComputeView();
            }
        }

        public float ZNear
        {
            get => znear;
            set
            {
                znear = value;
                ComputeProjection();
            }
        }

        public float ZFar
        {
            get => zfar;
            set
            {
                zfar = value;
                ComputeProjection();
            }
        }

        public bool MousePitchYaw { get; set; } = true;

        public bool MousePanTilt { get; set; } = true;

        public void Update(GraphicsDevice graphics, GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            if (graphics.Viewport.Bounds.Contains(mouseState.X, mouseState.Y))
            {
                if (MousePitchYaw && mouseState.LeftButton == ButtonState.Pressed &&
                    lastMouseState.LeftButton == ButtonState.Pressed)
                {
                    float changeY = mouseState.Y - lastMouseState.Y;
                    Pitch(-changeY * 0.005f);

                    float changeX = mouseState.X - lastMouseState.X;
                    Yaw(changeX * 0.005f);
                }

                if (MousePanTilt && mouseState.RightButton == ButtonState.Pressed &&
                    lastMouseState.RightButton == ButtonState.Pressed)
                {
                    float changeY = mouseState.Y - lastMouseState.Y;
                    Tilt(changeY * 0.0025f);

                    float changeX = mouseState.X - lastMouseState.X;
                    Pan(changeX * 0.0025f);
                }

                lastMouseState = mouseState;
            }
        }

        public void Initialize()
        {
            ComputeView();
            ComputeProjection();
            lastMouseState = Mouse.GetState();
        }

        private void ComputeView()
        {
            View = Matrix.CreateLookAt(eye, center, up);
        }

        private void ComputeProjection()
        {
            Projection =
                Matrix.CreatePerspectiveFieldOfView(fov, graphics.GraphicsDevice.Viewport.AspectRatio, znear, zfar);
        }

        public void Pitch(float angle)
        {
            // Need a vector in the camera X direction.
            var cameraZ = eye - center;
            var cameraX = Vector3.Cross(up, cameraZ);
            var len = cameraX.LengthSquared();
            if (len > 0)
                cameraX.Normalize();
            else
                cameraX = Vector3.UnitX;

            var t1 = Matrix.CreateTranslation(-center);
            var r = Matrix.CreateFromAxisAngle(cameraX, angle);
            var t2 = Matrix.CreateTranslation(center);

            var m = t1 * r * t2;
            eye = Vector3.Transform(eye, m);
            ComputeView();
        }

        public void Yaw(float angle)
        {
            // Need a vector in the camera X direction.
            var cameraZ = eye - center;
            var cameraX = Vector3.Cross(up, cameraZ);
            var cameraY = Vector3.Cross(cameraZ, cameraX);
            var len = cameraY.LengthSquared();
            if (len > 0)
                cameraY.Normalize();
            else
                cameraY = Vector3.UnitY;

            var t1 = Matrix.CreateTranslation(-center);
            var r = Matrix.CreateFromAxisAngle(cameraY, angle);
            var t2 = Matrix.CreateTranslation(center);

            var m = t1 * r * t2;
            eye = Vector3.Transform(eye, m);
            ComputeView();
        }

        public void Tilt(float angle)
        {
            // Need a vector in the camera X direction.
            var cameraZ = eye - center;
            var cameraX = Vector3.Cross(up, cameraZ);
            var len = cameraX.LengthSquared();
            if (len > 0)
                cameraX.Normalize();
            else
                cameraX = Vector3.UnitX;

            var t1 = Matrix.CreateTranslation(-eye);
            var r = Matrix.CreateFromAxisAngle(cameraX, angle);
            var t2 = Matrix.CreateTranslation(eye);

            var m = t1 * r * t2;
            center = Vector3.Transform(center, m);
            ComputeView();
        }

        public void Pan(float angle)
        {
            // Need a vector in the camera X direction.
            var cameraZ = eye - center;
            var cameraX = Vector3.Cross(up, cameraZ);
            var cameraY = Vector3.Cross(cameraZ, cameraX);
            var len = cameraY.LengthSquared();
            if (len > 0)
                cameraY.Normalize();
            else
                cameraY = Vector3.UnitY;

            var t1 = Matrix.CreateTranslation(-eye);
            var r = Matrix.CreateFromAxisAngle(cameraY, angle);
            var t2 = Matrix.CreateTranslation(eye);

            var m = t1 * r * t2;
            center = Vector3.Transform(center, m);
            ComputeView();
        }
    }
}