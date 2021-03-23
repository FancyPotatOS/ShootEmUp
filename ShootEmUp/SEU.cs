using ControlsHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootEmUp.Entities;
using ShootEmUp.Hitboxes;
using ShootEmUp.States;
using ShootEmUp.TextureHandling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShootEmUp
{
    public class SEU : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static SEU instance;

        internal IState GLOBALSTATE;

        public bool isFullScreen = false;

        public UInt64 updateID;

        /*  Keyboard inputs */
        // List of accounted keys
        public List<Keys> accKeys;
        // List of pressed keys
        public List<Keys> preKeys;
        // List of newly pressed keys
        public List<Keys> newKeys;

        /*  Control Handler */
        Texture2D[] udlr;
        bool hasConnected = false;

        Player player;

        public SEU()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // 60 FPS
            TargetElapsedTime = TimeSpan.FromTicks(166667);
            IsFixedTimeStep = true;

            // Initialize keyboard input
            accKeys = new List<Keys>();
            preKeys = new List<Keys>();
            newKeys = new List<Keys>();

            instance = this;

            updateID = 0;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            IController.Load(Content);

            udlr = new Texture2D[] { 
                Content.Load<Texture2D>("up"), 
                Content.Load<Texture2D>("down"),
                Content.Load<Texture2D>("left"), 
                Content.Load<Texture2D>("right"),
                Content.Load<Texture2D>("run"),
                Content.Load<Texture2D>("shoot"),
                Content.Load<Texture2D>("run")
            };

            Player.DEBUGbox = Content.Load<Texture2D>("box");

            IEntity.LoadAllAnimations(Content);

            /** /
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            isFullScreen = true;
            /**/
            _graphics.ApplyChanges();
            player = new Player(null, new float[] { 250, 250 }, null);

            GLOBALSTATE = new InGame(player, 10);

            List<StringTypePair<Keys>> defaults = new List<StringTypePair<Keys>>();
            {
                defaults.Add(new StringTypePair<Keys>("up", Keys.W));
                defaults.Add(new StringTypePair<Keys>("down", Keys.S));
                defaults.Add(new StringTypePair<Keys>("left", Keys.A));
                defaults.Add(new StringTypePair<Keys>("right", Keys.D));
                defaults.Add(new StringTypePair<Keys>("run", Keys.LeftShift));
                defaults.Add(new StringTypePair<Keys>("shoot", Keys.Space));
                defaults.Add(new StringTypePair<Keys>("reset", Keys.T));
            }
            KeyboardController.SetDefault(defaults);
        }

        protected override void Update(GameTime gameTime)
        {
            updateID++;

            // Keys that are being pressed
            preKeys = Keyboard.GetState().GetPressedKeys().ToList();

            // New keys that are pressed this tick
            newKeys = preKeys.FindAll(key => !accKeys.Contains(key));

            // Account for only the pressed keys
            accKeys = preKeys;

            /**/
            // Kill condition
            if (newKeys.Contains(Keys.Q))
                Exit();
            // Look for controller reconnection attempt
            else if (preKeys.Contains(Keys.Back))
            { 
                // Attempt controls
                hasConnected = false;

                // End update
                base.Update(gameTime);
                return;
            }
            /**/

            // Attempt controls
            if (!hasConnected)
            {
                // Attempted Exit from connecting
                if (IController.IsExit())
                {
                    hasConnected = true;
                    base.Update(gameTime);
                    return;
                }

                bool finished = IController.Connect(1, new string[] { "up", "down", "left", "right", "run", "shoot" });

                if (finished)
                {
                    hasConnected = true;
                    player.controller = IController.controllersInOrder[0];
                }
                else
                {
                    base.Update(gameTime);
                    return;
                }
            }

            GLOBALSTATE.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            List<TextureDescription> liszt = GLOBALSTATE.GetTextures();

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, null);

            foreach (TextureDescription td in liszt)
            {
                _spriteBatch.Draw(td.tex, td.bound, td.color);
            }

            if (!hasConnected)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);

                foreach (ToDraw td in IController.GetDrawing(this, 1, udlr))
                {
                    Texture2D tex = td.tex;
                    Rectangle bound = td.bound;
                    Color color = td.color;

                    _spriteBatch.Draw(tex, bound, color);
                }

                _spriteBatch.End();

                base.Draw(gameTime);

                return;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
