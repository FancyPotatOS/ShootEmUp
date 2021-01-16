using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootEmUp.ControlHandler;
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

            HurtBox hb = new HurtBox(new float[] { -30, -30 }, new float[] { 100, 100 }, new float[] { 60, 60 });
            Player player = new Player(new KeyboardController(), hb);

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

            GLOBALSTATE.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

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
