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
using System.Xml;
using Microsoft.Xna.Framework.Content;
using System.Text.RegularExpressions;
using ShootEmUp.States;

namespace ShootEmUp.Entities
{
    public class Blob : IEntity
    {

        static Dictionary<string, Dictionary<string, Animation>> animations;
        readonly Animation idle;

        Animation currAnimation;

        readonly float[] posPointer;

        readonly CollisionBox blocking;

        string facing;

        readonly FollowBehaviour FOLLOW;

        public Blob(float[] pos, string facing, Predicate<IEntity> target)
        {
            this.facing = facing;
            posPointer = pos;

            // Default to idle animation
            idle = GetAnimation("idle", "idle");
            currAnimation = idle;

            blocking = CollisionBox.FromHitbox(new Hitbox(new float[] { -25, -25 }, posPointer, new float[] { 50, 50 }));

            int viewSize = 600; // 6 player hitboxes on either side
            uint resetTarget = 120; // 2 seconds
            uint stopFollowing = 300; // 5 seconds
            uint tooClose = 0; // Half the player's hit

            FOLLOW = new FollowBehaviour(target, viewSize, this, resetTarget, stopFollowing, tooClose);
        }

        public List<CollisionBox> GetCollisionHitboxes()
        {
            return new CollisionBox[] { blocking }.ToList();
        }

        public List<DamageBox> GetDamageBoxes()
        {
            return new List<DamageBox>();
        }

        public List<HurtBox> GetHurtboxes()
        {
            return new List<HurtBox>();
        }

        public List<TextureDescription> GetTextures()
        {
            List<TextureDescription> td = new List<TextureDescription>();
            {
                // Get texture from animation
                int[] currAnimSize = currAnimation.GetSizeOfCurrentAnimation();
                Point pos = new Point(
                    (int)(blocking.pos[0] + blocking.offset[0] + (blocking.size[0] / 2) - (currAnimSize[0] / 2)),
                    (int)(blocking.pos[1] + blocking.offset[1] + blocking.size[1] - currAnimSize[1])
                );
                Point size = new Point((int)currAnimSize[0], (int)currAnimSize[1]);
                Rectangle bound = new Rectangle(pos, size);
                td.Add(new TextureDescription(currAnimation.GetTexture(), bound, Color.White, 10));
            }

            return td;
        }

        public void Update()
        {
            // Loop animation
            currAnimation.Update();
            if (currAnimation.Expired())
                currAnimation.Reset();

            // Update behaviours
            FOLLOW.Update();

            // If there is a target
            if (FOLLOW.HasCurrTarget())
            {
                // Get target position
                float[] target = FOLLOW.GetTarget();

                // Calculate necessary change
                float[] dV = new float[] { target[0] - posPointer[0], target[1] - posPointer[1] };

                // Movement logic
            }
            // No target anymore
            else
            {
                // If not idle animation
                if (currAnimation != idle)
                {
                    // Reset current animation and become idle
                    currAnimation.Reset();
                    currAnimation = idle;
                }
            }
        }

        public static void LoadAnimations(ContentManager Content)
        {
            animations = IEntity.LoadAnimations(Content, "Animations/blob_animations.xml");
        }

        public Animation GetAnimation(string type, string direction)
        {
            if (type == "idle")
                direction = "idle";

            if (animations.TryGetValue(type, out Dictionary<string, Animation> anim))
            {
                if (anim.TryGetValue(direction, out Animation attempt))
                {
                    return attempt;
                }
                else
                {
                    throw new Exception("'" + direction + "' is not a valid animation name!");
                }
            }
            else
            {
                throw new Exception("'" + type + "' is not a valid animation!");
            }
        }
    }
}
