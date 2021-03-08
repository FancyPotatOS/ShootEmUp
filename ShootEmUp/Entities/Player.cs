using ControlsHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootEmUp.Hitboxes;
using ShootEmUp.TextureHandling;
using System;
using System.Collections.Generic;
using System.Linq;

using CardsDevelopment;

namespace ShootEmUp.Entities
{
    class Player : IEntity
    {
        Animation currAnimation;

        // Constant of acceleration
        public const float moveAcc = 0.375f;
        public float friction;

        public float[] speed;

        public static Texture2D DEBUGbox;

        public IController controller;

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

            currAnimation = new Animation(new int[] { 50, 100, 25, 75 }, new Texture2D[] { IController.emptySlot, IController.GUI, IController.keyGameUI[0], IController.keyGameUI[1] });
        }

        public void Update()
        {
            // Update current animation
            currAnimation.Update();
            if (currAnimation.Expired())
                currAnimation.Reset();

            // Update controls
            controller.Update();

            if (controller.IsPressed("up"))
            {
                speed[1] = Math.Min(-speed[1] / 3, (speed[1] - moveAcc) * friction);
            }
            else if (controller.IsPressed("down"))
            {
                speed[1] = Math.Max(-speed[1] / 3, (speed[1] + moveAcc) * friction);
            }
            else
            {
                speed[1] /= 1.25f;
            }
            if (controller.IsPressed("left"))
            {
                speed[0] = Math.Min(-speed[0] / 3, (speed[0] - moveAcc) * friction);
            }
            else if (controller.IsPressed("right"))
            {
                speed[0] = Math.Max(-speed[0] / 3, (speed[0] + moveAcc) * friction);
            }
            else
            {
                speed[0] /= 1.25f;
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
            {
                sumChange[0] = crosses.ConnectX(blocking) - blocking.pos[0];
                speed[0] /= 1.10f;
            }
            else
                sumChange[0] = speed[0];

            // Update all X's hitboxes -> this means x will have 'precedence' in corner cases
            blocking.pos[0] += sumChange[0];
            vulnerable.pos[0] += sumChange[0];
            foreach (DamageBox db in attacking)
            {
                db.pos[0] += sumChange[0];
            }

            // Attempt to move X
            Hitbox movedY = blocking.Copy();
            movedY.pos[1] += speed[1];

            // If not null, then crosses some hitbox
            crosses = SEU.instance.GLOBALSTATE.GetCrossesEx(movedY);
            if (crosses != null)
            {
                sumChange[1] = crosses.ConnectY(blocking) - blocking.pos[1];
                speed[1] /= 1.10f;
            }
            else
                sumChange[1] = speed[1];

            blocking.pos[1] += sumChange[1];
            vulnerable.pos[1] += sumChange[1];
            foreach (DamageBox db in attacking)
            {
                db.pos[1] += sumChange[1];
            }

            if (SEU.instance.GLOBALSTATE.GetCrossesEx(blocking) != null)
            {
                { }
            }
        }

        public List<TextureDescription> GetTextures()
        {
            List<TextureDescription> td = new List<TextureDescription>();

            td.Add(new TextureDescription(vulnerable));

            td.Add(new TextureDescription(blocking));

            attacking.ForEach(db =>
            {
                td.Add(new TextureDescription(db));
            });

            // Get texture from animation
            td.Clear();
            Point pos = new Point((int)(blocking.pos[0] + blocking.offset[0]), (int)(blocking.pos[1] + blocking.offset[1]));
            Point size = new Point((int)blocking.size[0], (int)blocking.size[1]);
            Rectangle bound = new Rectangle(pos, size);
            td.Add(new TextureDescription(currAnimation.GetTexture(), bound, Color.White, 1));

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
