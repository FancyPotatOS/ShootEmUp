using ControlsHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootEmUp.Hitboxes;
using ShootEmUp.TextureHandling;
using System.Collections.Generic;
using System.Linq;

namespace ShootEmUp.Entities
{
    class Player : IEntity
    {
        Texture2D DEBUGbox;

        IController controller;

        public HurtBox vulnerable;
        public List<CollisionBox> blocking;
        public List<DamageBox> attacking;

        public Player(IController cont, HurtBox init)
        {
            controller = cont;

            blocking = new List<CollisionBox>();
            attacking = new List<DamageBox>();
            vulnerable = init;
        }

        public void Update()
        {
            // Update controls
            controller.Update();

            if (DEBUGbox == null)
            {
                DEBUGbox = SEU.instance.Content.Load<Texture2D>("box");
            }
        }

        public List<TextureDescription> GetTextures()
        {
            List<TextureDescription> td = new List<TextureDescription>();
            
            Point pos = new Point((int)(vulnerable.pos[0] + vulnerable.offset[0]), (int)(vulnerable.pos[1] + vulnerable.offset[1]));
            Point size = new Point((int)vulnerable.size[0], (int)vulnerable.size[1]);

            td.Add(new TextureDescription(DEBUGbox, new Rectangle(pos, size), Color.Blue, 10));

            foreach (CollisionBox cb in blocking)
            {
                pos = new Point((int)(cb.pos[0] + cb.offset[0]), (int)(cb.pos[1] + cb.offset[1]));
                size = new Point((int)cb.size[0], (int)cb.size[1]);

                td.Add(new TextureDescription(DEBUGbox, new Rectangle(pos, size), Color.Green, 11));
            }

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
            return blocking;
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
