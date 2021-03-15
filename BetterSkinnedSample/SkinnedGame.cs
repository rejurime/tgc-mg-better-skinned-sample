using AnimationPipelineExtension;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BetterSkinnedSample
{
    /// <summary>
    ///     This is the main class for your game
    /// </summary>
    public class SkinnedGame : Game
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public SkinnedGame()
        {
            // XNA startup
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Some basic setup for the display window
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

            // Create a simple mouse-based camera
            camera = new Camera(graphics);
            camera.Eye = new Vector3(190, 247, 1000);
            camera.Center = new Vector3(-20, 86, 500);
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic related content.  Calling
        ///     base.Initialize will enumerate through any components and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            var manager = CustomPipelineManager.CreateCustomPipelineManager();
            manager.BuildAnimationContent(modelFilename);
            manager.BuildAnimationContent(animationFilename);

            camera.Initialize();

            base.Initialize();
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load the model we will display.
            model = new AnimatedModel(modelFilename);
            model.LoadContent(Content);

            // Load the model that has an animation clip it in.
            dance = new AnimatedModel(animationFilename);
            dance.LoadContent(Content);

            // Obtain the clip we want to play. I'm using an absolute index, because XNA 4.0 won't allow you to have more than one animation associated with a model, anyway. It would be easy to add code to look up the clip by name and to index it by name in the model.
            var clip = dance.Clips[0];

            // And play the clip.
            var player = model.PlayClip(clip);
            player.Looping = true;
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing
        ///     audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            model.Update(gameTime);

            camera.Update(graphics.GraphicsDevice, gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.LightGray);

            model.Draw(graphics.GraphicsDevice, camera, Matrix.Identity);

            base.Draw(gameTime);
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #region Fields

        /// <summary>
        ///     This graphics device we are drawing on in this program
        /// </summary>
        private readonly GraphicsDeviceManager graphics;

        /// <summary>
        ///     The camera we use
        /// </summary>
        private readonly Camera camera;

        /// <summary>
        ///     The animated model we are displaying
        /// </summary>
        private AnimatedModel model;

        //private string modelFilename = "michelle";
        //private string modelFilename = "ninja";
        //private string modelFilename = "xbot";
        private readonly string modelFilename = "ybot";


        /// <summary>
        ///     This model is loaded solely for the dance animation
        /// </summary>
        private AnimatedModel dance;

        //private string animationFilename = "ninja - running";
        //private string animationFilename = "ninja - silly dancing";
        //private string animationFilename = "ninja - walking";
        //private string animationFilename = "xbot - front twist flip";
        //private string animationFilename = "ybot - front flip";
        private readonly string animationFilename = "ybot - kneeling pointing";

        #endregion
    }
}