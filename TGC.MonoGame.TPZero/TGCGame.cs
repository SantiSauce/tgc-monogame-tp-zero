using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Content.Models;

namespace TGC.MonoGame.TP
{
    public class TGCGame : Game


    {

        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/"; 

        private GraphicsDeviceManager Graphics { get; }
        private CityScene City { get; set; }
        private Model CarModel { get; set; }
        private Matrix CarWorld { get; set; }
        private FollowCamera FollowCamera { get; set; }



        public TGCGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;

            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            Graphics.ApplyChanges();

            FollowCamera = new FollowCamera(GraphicsDevice.Viewport.AspectRatio);
            CarWorld = Matrix.Identity;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            City = new CityScene(Content);
            CarModel = Content.Load<Model>("Models/scene/car");

            base.LoadContent();
        }

    

        private const float GRAVITY = 5000f;
        private const float JUMP = 1500f;
        private bool _onGround = true;

        private Vector3 carPosition = Vector3.Zero;
        private Vector3 carRotation = Vector3.Zero;



        private const float velocity = 100f;
        private const float rotationVelocity = 1f;

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var time = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            float carVelocity = 0f;

            float rotationAngle = 0f;


            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                carVelocity = velocity;

            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                carVelocity = -velocity;

            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                rotationAngle += rotationVelocity * time;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                rotationAngle -= rotationVelocity * time;
            }

            CarWorld = Matrix.CreateFromAxisAngle(CarWorld.Up, rotationAngle) * CarWorld;

            if (!_onGround)
            {
                carPosition.Y += -GRAVITY * time;
            }

            if (CarWorld.Translation.Y <= 0)
            {
                _onGround = true;
                carPosition.Y = 0;
            }

            if (keyboardState.IsKeyDown(Keys.Space) && _onGround)
            {
                carPosition.Y = JUMP;
                _onGround = false;
            }

            carPosition += CarWorld.Forward * carVelocity * time;

            CarWorld *= Matrix.CreateTranslation(carPosition * time);

            FollowCamera.Update(gameTime, CarWorld);

            base.Update(gameTime);
        }




        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            City.Draw(gameTime, FollowCamera.View, FollowCamera.Projection);

            CarModel.Draw(CarWorld, FollowCamera.View, FollowCamera.Projection);

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            Content.Unload();

            base.UnloadContent();
        }
    }
}



