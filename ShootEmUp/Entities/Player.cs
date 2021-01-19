using ControlsHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootEmUp.Hitboxes;
using ShootEmUp.TextureHandling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShootEmUp.Entities
{
    class Player : IEntity
    {
        // Constant of acceleration
        public const float moveAcc = 0.375f;
        public float friction;

        public float[] speed;

        public Texture2D DEBUGbox;

        readonly IController controller;

        public HurtBox vulnerable;
        public CollisionBox blocking;
        public List<DamageBox> attacking;

        public Player(IController cont, HurtBox init)
        {
            controller = cont;

            attacking = new List<DamageBox>();
            vulnerable = init;
            speed = new float[2];
            friction = 0.984375f;
        }

        public void Update()
        {
            // Update controls
            controller.Update();

            if (controller.IsPressed("up"))
            {
                speed[1] = Math.Min(0, (speed[1] - moveAcc) * friction);
            }
            else if (controller.IsPressed("down"))
            {
                speed[1] = Math.Max(0, (speed[1] + moveAcc) * friction);
            }
            if (controller.IsPressed("left"))
            {
                speed[0] = Math.Min(0, (speed[0] - moveAcc) * friction);
            }
            else if (controller.IsPressed("right"))
            {
                speed[0] = Math.Max(0, (speed[0] + moveAcc) * friction);
            }
            speed[1] *= friction;
            speed[0] *= friction;

            if (Mouse.GetState().MiddleButton.HasFlag(ButtonState.Pressed))
            {
                { }
            }

            float[] sumChange = new float[2];
            // Attempt to move X
            Hitbox movedX = blocking.Copy();
            movedX.pos[0] += speed[0];

            // If not null, then crosses some hitbox
            Hitbox crosses = SEU.instance.GLOBALSTATE.GetCrossesEx(movedX);
            if (crosses != null)
                sumChange[0] = crosses.ConnectX(blocking) - blocking.pos[0];
            else
                sumChange[0] = speed[0];

            Hitbox movedY = blocking.Copy();
            movedY.pos[1] += speed[1];

            crosses = SEU.instance.GLOBALSTATE.GetCrossesEx(movedY);
            if (crosses != null)
                sumChange[1] = crosses.ConnectY(blocking) - blocking.pos[1];
            else
                sumChange[1] = speed[1];

            blocking.pos[0] += sumChange[0];
            blocking.pos[1] += sumChange[1];
            vulnerable.pos[0] += sumChange[0];
            vulnerable.pos[1] += sumChange[1];
            foreach (DamageBox db in attacking)
            {
                db.pos[0] += sumChange[0];
                db.pos[1] += sumChange[1];
            }
        }

        public List<TextureDescription> GetTextures()
        {
            List<TextureDescription> td = new List<TextureDescription>();
            
            Point pos = new Point((int)(vulnerable.pos[0] + vulnerable.offset[0]), (int)(vulnerable.pos[1] + vulnerable.offset[1]));
            Point size = new Point((int)vulnerable.size[0], (int)vulnerable.size[1]);

            td.Add(new TextureDescription(DEBUGbox, new Rectangle(pos, size), Color.Blue, 10));

            pos = new Point((int)(blocking.pos[0] + blocking.offset[0]), (int)(blocking.pos[1] + blocking.offset[1]));
            size = new Point((int)blocking.size[0], (int)blocking.size[1]);

            td.Add(new TextureDescription(DEBUGbox, new Rectangle(pos, size), Color.Green, 11));

            foreach (DamageBox db in attacking)
            {
                pos = new Point((int)(db.pos[0] + db.offset[0]), (int)(db.pos[1] + db.offset[1]));
                size = new Point((int)db.size[0], (int)db.size[1]);

                td.Add(new TextureDescription(DEBUGbox, new Rectangle(pos, size), Color.Red, 12));
            }

            return td;
        }

        public List<CollisionBox> GetCollisionHitboxes()
        {
            List<CollisionBox> cbs = new List<CollisionBox>();
            {
                cbs.Add(blocking);
            }
            return cbs;
        }

        public List<DamageBox> GetDamageBoxes()
        {
            return attacking;
        }

        public List<HurtBox> GetHurtboxes()
        {
            return new HurtBox[] { vulnerable }.ToList();
        }
    }
}
