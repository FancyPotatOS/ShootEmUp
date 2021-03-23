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
using ShootEmUp.Animations;

namespace ShootEmUp.Entities
{
    public class Blob : AttemptMove, IEntity
    {
        Color color;

        Timing<int> movementTimings;

        static Dictionary<string, Dictionary<string, Animation>> animations;
        readonly Animation idle;

        Animation currAnimation;

        readonly float[] posPointer;

        readonly CollisionBox blocking;

        string facing;

        readonly FollowBehaviour FOLLOW;

        // Whether to wait for hop to finish before resetting to idle animation
        bool goToIdle;

        public Blob(float[] pos, string facing, Predicate<IEntity> target, Color c)
        {
            this.facing = facing;
            posPointer = pos;

            // Default to idle animation
            idle = GetAnimation("idle", "idle");
            currAnimation = idle;

            blocking = CollisionBox.FromHitbox(new Hitbox(new float[] { -25, -25 }, posPointer, new float[] { 50, 50 }));

            int viewSize = 500; // 5 player hitboxes on either side
            uint resetTarget = 30; // 0.5 seconds
            uint stopFollowing = 600; // 10 seconds
            uint tooClose = 0; // Half the player's hit

            FOLLOW = new FollowBehaviour(target, viewSize, this, resetTarget, stopFollowing, tooClose);

            int[] movement = new int[] { 1, 2, 4, 2, 1, 0 };
            int[] timing = new int[] { 4, 6, 12, 6, 4, 50 };
            movementTimings = new Timing<int>(timing, movement);

            color = c;

            // Assume idle by default
            goToIdle = true;
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
                td.Add(new TextureDescription(currAnimation.GetTexture(), bound, color, 10));
            }

            return td;
        }

        public void Update()
        {
            // Loop animation
            currAnimation.Update();
            if (currAnimation.Expired())
            {
                currAnimation.Reset();

                // Set as idle animation once finished if marked to 
                if (goToIdle)
                    currAnimation = idle;
            }

            // Update behaviours
            FOLLOW.Update();

            // If there is a target
            if (FOLLOW.HasCurrTarget())
            {
                goToIdle = false;

                // Get target position
                float[] target = FOLLOW.GetTarget();

                // Calculate necessary change
                float[] dV = new float[] { target[0] - posPointer[0], target[1] - posPointer[1] };

                /*  Movement Logic  */
                // Update the movement
                movementTimings.Update();

                // Loop movement
                if (movementTimings.IsExpired())
                    movementTimings.Reset();

                int mag = movementTimings.GetCurrElement();
                // Don't move if doesn't need to
                if (mag == 0)
                    return;

                // Get direction
                int[] sign = new int[] { InGame.GetSign(dV[0]), InGame.GetSign(dV[1]) };

                // Cap the movement
                dV = new float[] { InGame.GetAbsMin(dV[0], sign[0] * mag), InGame.GetAbsMin(dV[1], sign[1] * mag) };

                // Face the right direction
                // Face right direction
                string nowFacing = facing;
                SetFacing(sign);
                // Update animation if changed
                if (nowFacing != facing)
                {
                    // Get right animation and synchronize to current animation
                    Animation newAnim = GetAnimation("attack", facing);
                    currAnimation.Synchronize(newAnim);

                    // Reset and swap
                    currAnimation.Reset();
                    currAnimation = newAnim;
                }
                // If just started attacking
                else if (FOLLOW.ChangedState())
                {
                    // Reset current animation
                    currAnimation.Reset();

                    // Set to right animation
                    currAnimation = GetAnimation("attack", facing);
                }

                // Get possible change
                float[] change = AttemptToMove(blocking, dV);

                // Apply change to position
                posPointer[0] += change[0];
                posPointer[1] += change[1];
            }
            /**/
            // No target anymore
            else
            {
                // Update animation if changed state
                if (FOLLOW.ChangedState())
                {
                    // Reset current animation and become idle
                    movementTimings.Reset();

                    goToIdle = true;
                }
            }
            /**/
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

        public void SetFacing(int[] sign)
        {
            if (sign[0] < 0)
            {
                facing = "l";
            }
            else
                facing = "r";
        }
    }
}
