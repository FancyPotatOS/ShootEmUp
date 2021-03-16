using CardsDevelopment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootEmUp.Entities;
using ShootEmUp.Hitboxes;
using ShootEmUp.TextureHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootEmUp.States
{
    class InGame : IState
    {
        readonly List<IEntity> entities;
        readonly List<IEntity> toRemove;
        readonly List<IEntity> toAdd;

        readonly Camera camera;

        readonly Deck<TraditionalDeck> MasterDeck;

        Particle[] deckAnimations;

        public Map map;

        uint pause;

        public InGame(Player player, uint pause)
        {
            entities = new List<IEntity>();
            {
                entities.Add(player);
            }
            toRemove = new List<IEntity>();
            toAdd = new List<IEntity>();

            int[] size;
            if (SEU.instance.isFullScreen)
            { 
                size = new int[] { GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height }; 
            }
            else
            {
                size = new int[] { SEU.instance.Window.ClientBounds.Width, SEU.instance.Window.ClientBounds.Height };
            }

            // Initalize camera with position
            camera = new Camera(size, player, 0.2f);

            MasterDeck = Deck<TraditionalDeck>.MakeDeck();
            for (int i = 0; i < 10; i++)
            {
                MasterDeck.ChunkShuffle(3);
                MasterDeck.RiffleShuffle(3);
            }

            player.MasterDeck = MasterDeck;

            // Create top card animations
            CreateDeckAnimation();

            this.pause = pause;

            map = new Map();
        }

        public void Update()
        {
            if (pause > 0)
            { pause--; return; }

            // Add entities saved to add
            entities.AddRange(toAdd);
            // Reset list
            toAdd.Clear();

            // Remove all entities that are to be removed
            entities.RemoveAll(ent => toRemove.Contains(ent));
            // Reset the list
            toRemove.Clear();

            // Update all entities
            foreach (IEntity ent in entities)
            {
                // If the entity will be removed, do not update it
                if (toRemove.Contains(ent))
                    continue;

                // Otherwise the entity gets to update
                ent.Update();
            }

            UpdateDeckAnimations();

            camera.Recenter();
        }


        public List<TextureDescription> GetTextures()
        {
            List<TextureDescription> td = new List<TextureDescription>();

            // Get all entity textures
            foreach (IEntity ent in entities)
            {
                td.AddRange(ent.GetTextures());
            }

            // Get all entity textures
            foreach (CollisionBox cb in map.walls)
            {
                td.Add(new TextureDescription(cb));
            }

            // Get position of texture in screen
            td.ForEach(texture =>
            {
                int[] loc = camera.GetScreenLocation(new int[] { texture.bound.X, texture.bound.Y });
                texture.bound.X = loc[0];
                texture.bound.Y = loc[1];
            });

            // Add deck animations after camera change
            foreach (Particle particle in deckAnimations)
            {
                td.AddRange(particle.GetTextures());
            }

            // Order by layer
            td = td.OrderBy(val => val.layer).ToList();

            return td;
        }

        // Whether hitbox crosses anywhere
        public Hitbox GetCrosses(Hitbox hb)
        {
            foreach (Hitbox hitbox in map.walls)
            {
                if (hitbox.Crosses(hb))
                {
                    return hitbox;
                }
            }

            return null;
        }

        // Whether hitbox crosses exclusively anywhere
        public Hitbox GetCrossesEx(Hitbox hb)
        {
            foreach (Hitbox hitbox in map.walls)
            {
                if (hitbox.CrossesExclusive(hb))
                {
                    return hitbox;
                }
            }

            return null;
        }

        public void AddEntity(IEntity ent)
        {
            toAdd.Add(ent);
        }

        // Remove all entities that match the predicate
        public void RemoveEntity(Predicate<IEntity> match)
        {
            toRemove.AddRange(entities.FindAll(match));
        }

        // Remove all entities in list
        public void RemoveEntity(List<IEntity> ents)
        {
            // Sort to entities that actually exist
            ents = ents.FindAll(entity => entities.Contains(entity));
            toRemove.AddRange(ents);
        }

        public List<IEntity> FindEntities(Predicate<IEntity> match)
        {
            return entities.FindAll(match);
        }

        public Animation GetBackCard(string dir)
        {
            string animType = "still_card_back";
            string direction = ((dir == "u" || dir == "d") ? "upright" : "sideways");
            if (Card.animations.TryGetValue(animType, out Dictionary<string, Animation> anims))
            {
                if (anims.TryGetValue(direction, out Animation anim))
                {
                    return anim;
                }
                else
                    throw new Exception("Could not find directional animation '" + direction + "' in '" + animType + "'!");
            }
            else
                throw new Exception("Could not find animation '" + animType + "' in cards!");
        }

        public void CreateDeckAnimation()
        {
            Animation backCard = GetBackCard("u");

            deckAnimations = new Particle[3];
            int max = 3;
            for (int i = 0; i < max; i++)
            {
                // Amount design shifts
                int shift = 10;
                int boxSize = 60;

                float[] offset = new float[] { 10 + (i * shift), -(10 + ((max - 1) * shift) + (boxSize)) + (i * shift) };
                float[] pos = new float[] { 0, camera.screenSize[1] };
                float[] s = new float[] { boxSize, boxSize };

                List<Animation> anims = new List<Animation>();
                if (i < max - 1)
                {
                    anims.Add(backCard);
                }
                else if (i == max - 1)
                {
                    anims.AddRange(GetCardAnimation(MasterDeck.PeekFromTop(), "u"));
                }

                deckAnimations[i] = new Particle(offset, s, pos, new float[] { 0, 0 }, new float[] { 0, 0 }, 100000, uint.MaxValue, false, anims);
            }
        }

        public void UpdateDeckAnimations()
        {
            // Count the cards
            int mdCount = MasterDeck.GetCount();

            // If the deck is empty
            if (mdCount == 0)
            {
                // Wipe it
                deckAnimations = new Particle[0];
                return;
            }

            // Find top card
            List<Animation> topCard = GetCardAnimation(MasterDeck.PeekFromTop(), "u");
            
            // If there is cards
            if (deckAnimations.Length != 0)
                // Update top card animation
                deckAnimations[^1].ChangeAnimations(topCard);

            // If the picture is larger than should be
            if (deckAnimations.Length > Math.Min(mdCount, 3))
            {
                // Create new array to replace
                Particle[] newDA = new Particle[mdCount];

                // Copy them over
                for (int i = 0; i < mdCount; i++)
                {
                    newDA[i] = deckAnimations[i];
                }

                // Set last one
                newDA[^1] = deckAnimations[^1];

                deckAnimations = newDA;
            }
            // If the picture is smaller than should be
            else if (deckAnimations.Length < Math.Min(mdCount, 3))
            {
                deckAnimations = new Particle[Math.Min(mdCount, 3)];

                // Get back card animation
                Animation backCard = GetBackCard("u");

                // Fill them in the right position
                int max = Math.Min(mdCount, 3);
                for (int i = 0; i < max; i++)
                {
                    // Amount design shifts
                    int shift = 10;
                    int boxSize = 60;

                    float[] offset = new float[] { 10 + (i * shift), -(10 + ((max - 1) * shift) + (boxSize)) + (i * shift) };
                    float[] pos = new float[] { 0, camera.screenSize[1] };
                    float[] s = new float[] { boxSize, boxSize };

                    List<Animation> anims = new List<Animation>();
                    if (i < max - 1)
                    {
                        anims.Add(backCard);
                    }
                    else if (i == max - 1)
                    {
                        anims.AddRange(topCard);
                    }

                    deckAnimations[i] = new Particle(offset, s, pos, new float[] { 0, 0 }, new float[] { 0, 0 }, 100000, uint.MaxValue, false, anims);
                }
            }
        }

        public List<Animation> GetCardAnimation(TraditionalDeck card, string dir)
        {
            Card temp = new Card(card, dir);

            List<Animation> liszt = new List<Animation>();
            {
                liszt.Add(temp.GetStillCardAnimation());
                liszt.Add(temp.GetStillRankAnimation());
                liszt.Add(temp.GetStillSuitAnimation());
            }

            return liszt;
        }
    }
}
