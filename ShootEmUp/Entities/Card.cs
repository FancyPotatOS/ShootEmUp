using CardsDevelopment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ShootEmUp.Hitboxes;
using ShootEmUp.TextureHandling;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShootEmUp.Entities
{
    public class Card : AttemptMove, IEntity
    {
        public static Dictionary<string, Dictionary<string, Animation>> animations;

        Animation currRankAnimation;
        Animation currSuitAnimation;
        Animation currCardAnimation;

        public TraditionalDeck card;
        string facing;

        public List<HurtBox> attacking;
        public CollisionBox stop;
        public DamageBox hurtable;

        public float[] posPointer;
        public float[] speed;

        bool hitWall = false;

        public static readonly float[] friction = new float[] { -0.25f, -0.25f };

        public Cooldown grabCooldown;

        public Card(float[] dir, float[] pos, string direction, TraditionalDeck card)
        {
            attacking = new List<HurtBox>();
            stop = new CollisionBox(new float[] { -7, -7 }, pos, new float[] { 14, 14 });
            hurtable = new DamageBox(dir, 0, new float[] { -7, -7 }, pos, new float[] { 14, 14 });

            this.card = card;

            posPointer = pos;
            speed = new float[] { dir[0], dir[1] };

            facing = direction;

            currCardAnimation = GetCardAnimation();
            currRankAnimation = GetRankAnimation();
            currSuitAnimation = GetSuitAnimation();

            // Synchronize the card animations
            currCardAnimation.Synchronize(currRankAnimation);
            currCardAnimation.Synchronize(currSuitAnimation);

            grabCooldown = new Cooldown(65, 65);
        }

        public Card(TraditionalDeck card, string dir)
        {
            facing = dir;
            this.card = card;
        }

        public void Update()
        {
            // Update cooldown
            grabCooldown.Update();

            // Update animations
            currRankAnimation.Update();
            currCardAnimation.Update();
            currSuitAnimation.Update();

            // Loop the animations
            if (currCardAnimation.Expired())
                currCardAnimation.Reset();
            if (currRankAnimation.Expired())
                currRankAnimation.Reset();
            if (currSuitAnimation.Expired())
                currSuitAnimation.Reset();


            /**/
            // Attempt to move with speed; speed is changed automatically
            float[] posChange = AttemptToMove(stop, speed);
            // Apply change to position
            posPointer[0] += posChange[0];
            posPointer[1] += posChange[1];
            if (posChange[2] > 0)
            {
                // Set max cooldown after hitting wall
                grabCooldown.AtMost(30);

                hitWall = true;

                // If the direction it cannot move is u/d
                if (posChange[2] == 2)
                {
                    if (posChange[0] > 0)
                        facing = "r";
                    else
                        facing = "l";
                }
                // If direction cannot move is l/r
                else if (posChange[2] == 1)
                {
                    if (posChange[1] > 0)
                        facing = "d";
                    else
                        facing = "u";
                }

                // Reset the old animations
                currCardAnimation.Reset();
                currRankAnimation.Reset();
                currSuitAnimation.Reset();

                // Set as still animation
                currCardAnimation = GetStillCardAnimation();
                currRankAnimation = GetStillRankAnimation();
                currSuitAnimation = GetStillSuitAnimation();

                // Synchronize the card animations
                currCardAnimation.Synchronize(currRankAnimation);
                currCardAnimation.Synchronize(currSuitAnimation);
            }
            // if not moving anymore
            else if (posChange[0] == 0 && posChange[1] == 0)
            {
                // Reset the old animations
                currCardAnimation.Reset();
                currRankAnimation.Reset();
                currSuitAnimation.Reset();

                // Set as still animation
                currCardAnimation = GetStillCardAnimation();
                currRankAnimation = GetStillRankAnimation();
                currSuitAnimation = GetStillSuitAnimation();

                // Synchronize the card animations
                currCardAnimation.Synchronize(currRankAnimation);
                currCardAnimation.Synchronize(currSuitAnimation);
            }
            /**/

            // Slow down if possible
            if (speed[0] != 0)
            {
                if (Math.Abs(speed[0]) < 1)
                    speed[0] = 0;
                else if (hitWall)
                    speed[0] += GetMag(speed[0]) * friction[0] * 2;
                else
                    speed[0] += GetMag(speed[0]) * friction[0];
            }
            if (speed[1] != 0)
            {
                if (Math.Abs(speed[1]) < 1)
                    speed[1] = 0;
                else if (hitWall)
                    speed[1] += GetMag(speed[1]) * friction[1] * 2;
                else
                    speed[1] += GetMag(speed[1]) * friction[1];
            }
        }

        public List<TextureDescription> GetTextures()
        {
            List<TextureDescription> texs = new List<TextureDescription>();
            {
                int[] currAnimSize = currCardAnimation.GetSizeOfCurrentAnimation();
                Point pos = new Point(
                    (int)(stop.pos[0] + stop.offset[0] + (stop.size[0] / 2) - (currAnimSize[0] / 2)),
                    (int)(stop.pos[1] + stop.offset[1] + (stop.size[1] / 2) - (currAnimSize[1] / 2))
                );
                Point size = new Point((int)currAnimSize[0], (int)currAnimSize[1]);
                Rectangle bound = new Rectangle(pos, size);
                texs.Add(new TextureDescription(currCardAnimation.GetTexture(), bound, Color.White, 10));

                currAnimSize = currSuitAnimation.GetSizeOfCurrentAnimation();
                pos = new Point(
                     (int)(stop.pos[0] + stop.offset[0] + (stop.size[0] / 2) - (currAnimSize[0] / 2)),
                     (int)(stop.pos[1] + stop.offset[1] + (stop.size[1] / 2) - (currAnimSize[1] / 2))
                 );
                size = new Point((int)currAnimSize[0], (int)currAnimSize[1]);
                bound = new Rectangle(pos, size);
                texs.Add(new TextureDescription(currSuitAnimation.GetTexture(), bound, Color.White, 10));

                currAnimSize = currRankAnimation.GetSizeOfCurrentAnimation();
                pos = new Point(
                     (int)(stop.pos[0] + stop.offset[0] + (stop.size[0] / 2) - (currAnimSize[0] / 2)),
                     (int)(stop.pos[1] + stop.offset[1] + (stop.size[1] / 2) - (currAnimSize[1] / 2))
                 );
                size = new Point((int)currAnimSize[0], (int)currAnimSize[1]);
                bound = new Rectangle(pos, size);
                texs.Add(new TextureDescription(currRankAnimation.GetTexture(), bound, Color.White, 10));
            }

            return texs;
        }

        public List<CollisionBox> GetCollisionHitboxes()
        {
            List<CollisionBox> temp = new List<CollisionBox>();
            {
                temp.Add(stop);
            }
            
            return temp;
        }

        public List<DamageBox> GetDamageBoxes()
        {
            List<DamageBox> temp = new List<DamageBox>();
            {
                temp.Add(hurtable);
            }

            return temp;
        }

        public List<HurtBox> GetHurtboxes()
        {
            return attacking;
        }

        public static void LoadAnimations(ContentManager Content)
        {
            animations = IEntity.LoadAnimations(Content, "Animations/card_animations.xml");
        }

        public Animation GetRankAnimation()
        {
            // Get referencable color and rank
            string color = GetColor();
            string name = GetRank();
            string reference = color + "_" + name + "_" + facing;

            // Get spinning xml
            if (animations.TryGetValue("spin_rank", out Dictionary<string, Animation> anim))
            {
                // Get rank by color and name
                if (anim.TryGetValue(reference, out Animation attempt))
                {
                    return attempt;
                }
                else
                {
                    throw new Exception("'" + reference + "' is not a valid card reference!");
                }
            }
            else
            {
                throw new Exception("'spin_rank' is not recognized!");
            }
        }

        public Animation GetStillRankAnimation()
        {
            // Get referencable color and rank
            string color = GetColor();
            string name = GetRank();
            string reference = color + "_" + name + "_" + facing;

            // Get spinning xml
            if (animations.TryGetValue("still_rank", out Dictionary<string, Animation> anim))
            {
                // Get rank by color and name
                if (anim.TryGetValue(reference, out Animation attempt))
                {
                    return attempt;
                }
                else
                {
                    throw new Exception("'" + reference + "' is not a valid card reference!");
                }
            }
            else
            {
                throw new Exception("'spin_rank' is not recognized!");
            }
        }

        public Animation GetSuitAnimation()
        {
            // Get referencable color and rank
            string suit = GetSuit();
            string reference = suit + "_" + facing;

            // Get spinning xml
            if (animations.TryGetValue("spin_suit", out Dictionary<string, Animation> anim))
            {
                // Get rank by color and name
                if (anim.TryGetValue(reference, out Animation attempt))
                {
                    return attempt;
                }
                else
                {
                    throw new Exception("'" + reference + "' is not a valid card reference!");
                }
            }
            else
            {
                throw new Exception("'spin_suit' is not recognized!");
            }
        }

        public Animation GetStillSuitAnimation()
        {
            // Get referencable color and rank
            string suit = GetSuit();
            string reference = suit + "_" + facing;

            // Get spinning xml
            if (animations.TryGetValue("still_suit", out Dictionary<string, Animation> anim))
            {
                // Get rank by color and name
                if (anim.TryGetValue(reference, out Animation attempt))
                {
                    return attempt;
                }
                else
                {
                    throw new Exception("'" + reference + "' is not a valid card reference!");
                }
            }
            else
            {
                throw new Exception("'still_suit' is not recognized!");
            }
        }

        public Animation GetCardAnimation()
        {
            // Get referencable orientation
            string reference = GetOrientation();

            // Get spinning xml
            if (animations.TryGetValue("spin_card", out Dictionary<string, Animation> anim))
            {
                // Get rank by color and name
                if (anim.TryGetValue(reference, out Animation attempt))
                {
                    return attempt;
                }
                else
                {
                    throw new Exception("'" + reference + "' is not a valid card reference!");
                }
            }
            else
            {
                throw new Exception("'spin_card' is not recognized!");
            }
        }

        public Animation GetStillCardAnimation()
        {
            // Get referencable orientation
            string reference = GetOrientation();

            // Get spinning xml
            if (animations.TryGetValue("still_card", out Dictionary<string, Animation> anim))
            {
                // Get rank by color and name
                if (anim.TryGetValue(reference, out Animation attempt))
                {
                    return attempt;
                }
                else
                {
                    throw new Exception("'" + reference + "' is not a valid card reference!");
                }
            }
            else
            {
                throw new Exception("'still_card' is not recognized!");
            }
        }

        string GetColor()
        {
            return (card.suit == TraditionalDeck.Suits.Diamonds || card.suit == TraditionalDeck.Suits.Hearts) ? "red" : "black";
        }

        string GetSuit()
        {
            if (card.suit == TraditionalDeck.Suits.Diamonds)
                return "diamonds";
            if (card.suit == TraditionalDeck.Suits.Hearts)
                return "hearts";
            if (card.suit == TraditionalDeck.Suits.Spades)
                return "spades";
            else
                return "clubs";
        }

        string GetRank()
        {
            if (card.rank == TraditionalDeck.Ranks.Two)
                return "two";
            if (card.rank == TraditionalDeck.Ranks.Three)
                return "three";
            if (card.rank == TraditionalDeck.Ranks.Four)
                return "four";
            if (card.rank == TraditionalDeck.Ranks.Five)
                return "five";
            if (card.rank == TraditionalDeck.Ranks.Six)
                return "six";
            if (card.rank == TraditionalDeck.Ranks.Seven)
                return "seven";
            if (card.rank == TraditionalDeck.Ranks.Eight)
                return "eight";
            if (card.rank == TraditionalDeck.Ranks.Nine)
                return "nine";
            if (card.rank == TraditionalDeck.Ranks.Ten)
                return "ten";
            if (card.rank == TraditionalDeck.Ranks.Jack)
                return "jack";
            if (card.rank == TraditionalDeck.Ranks.Queen)
                return "queen";
            if (card.rank == TraditionalDeck.Ranks.King)
                return "king";
            else
                return "ace";
        }

        string GetOrientation()
        {
            if (facing == "u" || facing == "d")
                return "upright";
            return "sideways";
        }

        int GetMag(float x)
        {
            if (x < 0)
                return -1;
            if (x > 0)
                return 1;
            return 0;
        }
    }
}
