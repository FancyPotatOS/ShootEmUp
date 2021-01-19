using ControlsHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootEmUp.Entities;
using ShootEmUp.Hitboxes;
using ShootEmUp.States;
using ShootEmUp.TextureHandling;
using System.Collections.Generic;
using System.Linq;

namespace ShootEmUp
{
    public class SEU : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static SEU instance;

        IState GLOBALSTATE;

        /*  Keyboard inputs */
        // List of accounted keys
        public List<Keys> accKeys;
        // List of pressed keys
        public List<Keys> preKeys;
        // List of newly pressed keys
        public List<Keys> newKeys;

        /*  Control Handler */
        Texture2D controls_handler_gui;
        Texture2D[] udlr;
        bool hasConnected = false;
        IController playerCont;

        public SEU()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Initialize keyboard input
            accKeys = new List<Keys>();
            preKeys = new List<Keys>();
            newKeys = new List<Keys>();

            instance = this;

            playerCont = new KeyboardController();

            HurtBox hb = new HurtBox(new float[] { -30, -30 }, new float[] { 100, 100 }, new float[] { 60, 60 });
            Player player = new Player(playerCont, hb);

            // Where player collision is
            player.blocking.Add(CollisionBox.FromHitbox(new Hitbox(new float[] { -25, -25 }, new float[] { 250, 250 }, new float[] { 50, 50 })));

            player.vulnerable = new HurtBox(new float[] { -20, -20 }, new float[] { 250, 235 }, new float[] { 40, 40 });

            GLOBALSTATE = new InGame(player);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // NECESSARY
            controls_handler_gui = Content.Load<Texture2D>("controls_handler_gui");
            // SYMBOLS ASSIGNED TO EACH CONTROL
            udlr = new Texture2D[] { Content.Load<Texture2D>("up"), Content.Load<Texture2D>("down"), Content.Load<Texture2D>("left"), Content.Load<Texture2D>("right") };

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Keys that are being pressed
            preKeys = Keyboard.GetState().GetPressedKeys().ToList();

            // New keys that are pressed this tick
            newKeys = preKeys.FindAll(key => !accKeys.Contains(key));

            // Account for only the pressed keys
            accKeys = preKeys;

            if (newKeys.Contains(Keys.Q))
                Exit();

            // Attempt controls
            if (!hasConnected)
            {
                bool finished = IController.Connect(new IController[] { playerCont }, new string[] { "up", "down", "left", "right" });

                if (finished)
                {
                    hasConnected = true;
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
            
            if (!hasConnected)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);

                _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, null);

                foreach (ToDraw td in IController.GetDrawing(this, new IController[] { playerCont }, controls_handler_gui, udlr))
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

            List<TextureDescription> liszt = GLOBALSTATE.GetTextures();

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null,null,null);

            foreach (TextureDescription td in liszt)
            {
                _spriteBatch.Draw(td.tex, td.bound, td.color);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
