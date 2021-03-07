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

        readonly List<Hitbox> windowBorders;

        public InGame(Player player)
        {
            entities = new List<IEntity>();
            {
                entities.Add(player);
            }

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
                windowBorders.Add(new Hitbox(new float[] { -1, -1 }, new float[] { 0, 0 }, new float[] { 1, 2 + size[1] }));
                windowBorders.Add(new Hitbox(new float[] { -1, -1 }, new float[] { 0, 0 }, new float[] { 2 + size[0], 1 }));
                windowBorders.Add(new Hitbox(new float[] { 0, -1 }, new float[] { size[0], 0 }, new float[] { 1, 2 + size[1] }));
                windowBorders.Add(new Hitbox(new float[] { -1, 0 }, new float[] { 0, size[1] }, new float[] { 2 + size[0], 1 }));
            }
        }

        public void Update()
        {
            // Update all entities
            foreach (IEntity ent in entities)
            {
                ent.Update();
            }
        }


        public List<TextureDescription> GetTextures()
        {
            List<TextureDescription> td = new List<TextureDescription>();

            // Get all entity textures
            foreach (IEntity ent in entities)
            {
                td.AddRange(ent.GetTextures());
            }

            // Order by layer
            td = td.OrderBy(val => val.layer).ToList();

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
    }
}
