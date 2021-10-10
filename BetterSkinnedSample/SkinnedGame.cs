using System.Collections.Generic;
using BetterSkinnedSample.AnimationModel;
using BetterSkinnedSample.AnimationPipelineExtension;
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
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Some basic setup for the display window
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Graphics.PreferredBackBufferWidth = 1024;
            Graphics.PreferredBackBufferHeight = 768;

            // Create a simple mouse-based Camera
            Camera = new Camera(Graphics);
            Camera.Eye = new Vector3(190, 247, 1000);
            Camera.Center = new Vector3(-20, 86, 500);

            // File names of models and animations
            ModelFileNames = new List<string> { "michelle/michelle", "ninja/ninja", "xbot/xbot", "ybot/ybot" };
            ModelAnimationFileNames = new List<string>
            {
                "xbot/xbot - idle", "ybot/ybot - idle", "ybot/ybot - running", "ybot/ybot - silly dancing",
                "ybot/ybot - walking", "michelle/michelle - jumping"
            };
        }

        /// <summary>
        ///     The Camera we use
        /// </summary>
        private Camera Camera { get; }

        /// <summary>
        ///     This graphics device we are drawing on in this program
        /// </summary>
        private GraphicsDeviceManager Graphics { get; }

        private List<string> ModelAnimationFileNames { get; }

        private List<string> ModelFileNames { get; }

        /// <summary>
        ///     This Model is loaded solely for the Animation animation
        /// </summary>
        private AnimatedModel Animation { get; set; }

        /// <summary>
        ///     The animated Model we are displaying
        /// </summary>
        private AnimatedModel Model { get; set; }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic related content.  Calling
        ///     base.Initialize will enumerate through any components and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            var manager = CustomPipelineManager.CreateCustomPipelineManager();
            manager.BuildAnimationContent(ModelFileNames[0]);
            manager.BuildAnimationContent(ModelAnimationFileNames[5]);

            Camera.Initialize();

            base.Initialize();
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load the Model we will display.
            Model = new AnimatedModel(ModelFileNames[0]);
            Model.LoadContent(Content);

            // Load the Model that has an animation clip it in.
            Animation = new AnimatedModel(ModelAnimationFileNames[5]);
            Animation.LoadContent(Content);

            // Obtain the clip we want to play. I'm using an absolute index, because XNA 4.0 won't allow you to have more than one animation associated with a Model, anyway.
            // It would be easy to add code to look up the clip by name and to index it by name in the Model.
            var clip = Animation.Clips[0];

            // And play the clip.
            var player = Model.PlayClip(clip);
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

            Model.Update(gameTime);

            Camera.Update(Graphics.GraphicsDevice, gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            Model.Draw(Graphics.GraphicsDevice, Camera, Matrix.Identity);

            base.Draw(gameTime);
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }
    }
}