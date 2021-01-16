using ShootEmUp.Entities;
using ShootEmUp.TextureHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootEmUp.States
{
    class InGame : IState
    {
        List<IEntity> entities;

        public InGame(Player player)
        {
            entities = new List<IEntity>();
            {
                entities.Add(player);
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
            td.OrderBy(val => val.layer);

            return td;
        }
    }
}
