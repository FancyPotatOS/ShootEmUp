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

        readonly List<Hitbox> windowBorders;

        readonly Deck<TraditionalDeck> MasterDeck;
        Animation topDeck;

        public InGame(Player player)
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

            windowBorders = new List<Hitbox>();
            {
                //windowBorders.Add(new Hitbox(new float[] { -1, -1 }, new float[] { 0, 0 }, new float[] { 1, 2 + size[1] }));
                windowBorders.Add(new Hitbox(new float[] { -1, -1 }, new float[] { 0, 0 }, new float[] { 2 + size[0], 1 }));
                windowBorders.Add(new Hitbox(new float[] { 0, -1 }, new float[] { size[0], 0 }, new float[] { 1, 2 + size[1] }));
                windowBorders.Add(new Hitbox(new float[] { -1, 0 }, new float[] { 0, size[1] }, new float[] { 2 + size[0], 1 }));
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
        }

        public void Update()
        {
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

            foreach (Hitbox hb in windowBorders)
            {
                td.Add(new TextureDescription(hb));
            }

            // Order by layer
            td = td.OrderBy(val => val.layer).ToList();

            // Get position of texture in screen
            td.ForEach(texture =>
            {
                int[] loc = camera.GetScreenLocation(new int[] { texture.bound.X, texture.bound.Y });
                texture.bound.X = loc[0];
                texture.bound.Y = loc[1];
            });

            return td;
        }

        // Whether hitbox crosses anywhere
        public Hitbox GetCrosses(Hitbox hb)
        {
            foreach (Hitbox hitbox in windowBorders)
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
            foreach (Hitbox hitbox in windowBorders)
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

        /** /
        public void GetTopDeckAnimation()
        {
            // Make a temporary card
            Card temp = new Card(MasterDeck.PeekFromTop(), "u");

            Dictionary<string, Animation> backDic;
            // Get the back of the card
            if (Card.animations.TryGetValue("still_card_back", out backDic))
            {
                backDic.TryGetValue("upright", out Animation backCard);
                
            }
            Animation top
        }
        /**/
    }
}
