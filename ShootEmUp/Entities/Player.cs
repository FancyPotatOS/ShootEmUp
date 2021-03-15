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

namespace ShootEmUp.Entities
{
    class Player : AttemptMove, IEntity
    {
        public static Dictionary<string, Dictionary<string, Animation>> animations;
        Animation currAnimation;
        string animationName;

        public Deck<TraditionalDeck> MasterDeck;

        public float[] speed;
        const float maxWalkSpeed = 5;
        const float maxRunSpeed = 8;

        public static Texture2D DEBUGbox;

        public IController controller;

        readonly float[] posPointer;
        public HurtBox vulnerable;
        public CollisionBox blocking;
        public List<DamageBox> attacking;

        public string facing;
        public int facingChange;
        bool running;

        bool hasShot = false;
        readonly Cooldown shotCooldown;

        public Player(IController cont, float[] pos, Deck<TraditionalDeck> masterDeck)
        {
            // Fill hitboxes from pos
            posPointer = pos;
            controller = cont;

            attacking = new List<DamageBox>();
            blocking = CollisionBox.FromHitbox(new Hitbox(new float[] { -25, -25 }, posPointer, new float[] { 50, 60 }));
            vulnerable = new HurtBox(new float[] { -20, -20 }, posPointer, new float[] { 40, 40 });
            speed = new float[2];

            currAnimation = GetAnimation("stand", "down");
            animationName = "stand";
            facing = "down";
            facingChange = 0;
            running = false;

            // Create deck of cards
            MasterDeck = masterDeck;
            shotCooldown = new Cooldown(0, 20);
        }

        public void Update()
        {
            // Update current animation to loop
            currAnimation.Update();
            if (currAnimation.Expired())
                currAnimation.Reset();

            // Update controls
            controller.Update();

            // Save which buttons are pressed
            int[] buttonVec = { 0, 0 };

            // Whether run is still pressed
            bool newRun = controller.IsPressed("run");
            string newAnim = "stand";

            // Set max speed, depends on if running
            float maxSpeed = maxWalkSpeed;
            if (running)
                maxSpeed = maxRunSpeed;

            if (controller.IsPressed("up"))
            {
                newAnim = (newRun) ? "run" : "walk";
                buttonVec[1] = -1;
                speed[1] = Math.Max(-maxSpeed, Math.Min(0, speed[1] - 1));
            }
            else if (controller.IsPressed("down"))
            {
                newAnim = (newRun) ? "run" : "walk";
                buttonVec[1] = 1;
                speed[1] = Math.Min(maxSpeed, Math.Max(0, speed[1] + 1));
            }
            else
            {
                speed[1] -= GetMag(speed[1]);
            }
            if (controller.IsPressed("left"))
            {
                newAnim = (newRun) ? "run" : "walk";
                buttonVec[0] = -1;
                speed[0] = Math.Max(-maxSpeed, Math.Min(0, speed[0] - 1));
            }
            else if (controller.IsPressed("right"))
            {
                newAnim = (newRun) ? "run" : "walk";
                buttonVec[0] = 1;
                speed[0] = Math.Min(maxSpeed, Math.Max(0, speed[0] + 1));
            }
            else
            {
                speed[0] -= GetMag(speed[0]);
            }

            string dir = VectorToDirection(buttonVec);

            // Whether animation should update
            // Update animation if direction changed or running has changed and not walking
            bool changedAnim = facing != dir || animationName != newAnim || animationName != "walk" && newRun != running;

            running = newRun;
            animationName = newAnim;
            facing = dir;

            // If direction changed
            if (changedAnim)
            {
                // Reset current animation
                currAnimation.Reset();

                // Set to right animation
                currAnimation = GetAnimation(animationName, facing);
            }

            // Attempt to move with speed; Speed is changed within function
            float[] hbChange = AttemptToMove(blocking, speed);
            // Change hitbox positions
            posPointer[0] += hbChange[0];
            posPointer[1] += hbChange[1];

            // Shoot a card
            int mag = 15;
            // Update cooldown
            shotCooldown.Update();
            // No cooldown and shooting
            if (shotCooldown.CanUse() && controller.IsPressed("shoot"))
            {
                if (!hasShot)
                {
                    float[] facingVec = GetFacingVector();
                    if (!MasterDeck.IsEmpty())
                    {
                        shotCooldown.Reset();

                        Card newCard = new Card(new float[] { facingVec[0] * mag, facingVec[1] * mag }, new float[] { posPointer[0], posPointer[1] }, facing.Substring(0, 1), MasterDeck.TakeFromTop());

                        SEU.instance.GLOBALSTATE.AddEntity(newCard);
                    }
                }

                // Save as shooting
                hasShot = true;
            }
            else
                // No longer pressing shoot button
                hasShot = false;

            // If touching a card
            List<IEntity> cardsTouching = SEU.instance.GLOBALSTATE.FindEntities(ent => 
                                                                            ent.GetType().Equals(typeof(Card)) &&
                                                                            ent.GetCollisionHitboxes().Exists(cb => blocking.Crosses(cb)));
            foreach (Card card in cardsTouching)
            {
                if (card.grabCooldown.CanUse())
                {
                    // Add card back into this.cards
                    MasterDeck.AddToTop(card.card);

                    // Remove the card from the world
                    SEU.instance.GLOBALSTATE.RemoveEntity(temp => temp == card);
                }
            }
        }

        public List<TextureDescription> GetTextures()
        {
            List<TextureDescription> td = new List<TextureDescription>();

            // Get texture from animation
            int[] currAnimSize = currAnimation.GetSizeOfCurrentAnimation();
            Point pos = new Point(
                (int)(blocking.pos[0] + blocking.offset[0] + (blocking.size[0] / 2) - (currAnimSize[0] / 2)), 
                (int)(blocking.pos[1] + blocking.offset[1] + blocking.size[1] - currAnimSize[1])
            );
            Point size = new Point((int)currAnimSize[0], (int)currAnimSize[1]);
            Rectangle bound = new Rectangle(pos, size);
            td.Add(new TextureDescription(currAnimation.GetTexture(), bound, Color.White, 10));

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

        public static void LoadAnimations(ContentManager Content)
        {
            animations = IEntity.LoadAnimations(Content, "Animations/player_animations.xml");
        }

        public Animation GetAnimation(string type, string direction)
        {
            if (animations.TryGetValue(type, out Dictionary<string, Animation> anim))
            {
                if (anim.TryGetValue(direction, out Animation attempt))
                {
                    return attempt;
                }
                else
                {
                    throw new Exception("'" + direction + "' is not a valid direction!");
                }
            }
            else
            {
                throw new Exception("'" + type + "' is not a valid animation!");
            }
        }

        // Turns 2d vector to string direction, defaults to facing if no direction
        string VectorToDirection(int[] vec)
        {
            string coll = "";
            coll += (vec[1] < 0) ? "up" : (vec[1] > 0) ? "down" : "";
            coll += (vec[0] > 0) ? "right" : (vec[0] < 0) ? "left" : "";

            return (coll == "") ? facing : coll;
        }

        int GetMag(float n)
        {
            return (n > 0) ? 1 : (n < 0) ? -1 : 0;
        }

        // 1 or -1 for each axis facing
        public float[] GetFacingVector()
        {
            float ud = 0;
            if (Regex.Matches(facing, ".*up.*").Count > 0)
                ud = -1;
            else if (Regex.Matches(facing, ".*down.*").Count > 0)
                ud = 1;
            float lr = 0;
            if (Regex.Matches(facing, ".*right.*").Count > 0)
                lr = 1;
            else if (Regex.Matches(facing, ".*left.*").Count > 0)
                lr = -1;
            return new float[] { lr, ud };
        }
    }
}
