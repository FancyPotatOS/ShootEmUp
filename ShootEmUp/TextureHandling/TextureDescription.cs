using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootEmUp.Entities;
using ShootEmUp.Hitboxes;

namespace ShootEmUp.TextureHandling
{
    public class TextureDescription
    {
        public readonly Texture2D tex;
        public Rectangle bound;
        public Color color;
        public int layer;
        public TextureDescription(Texture2D t, Rectangle b, Color c, int layer)
        {
            tex = t;
            bound = b;
            color = c;
            this.layer = layer;
        }

        public TextureDescription(DamageBox db)
        {
            tex = Player.DEBUGbox;

            Point pos = new Point((int)(db.pos[0] + db.offset[0]), (int)(db.pos[1] + db.offset[1]));
            Point size = new Point((int)db.size[0], (int)db.size[1]);

            bound = new Rectangle(pos, size);

            color = Color.Red;

            layer = 8;
        }

        public TextureDescription(CollisionBox cb)
        {
            tex = Player.DEBUGbox;

            Point pos = new Point((int)(cb.pos[0] + cb.offset[0]), (int)(cb.pos[1] + cb.offset[1]));
            Point size = new Point((int)cb.size[0], (int)cb.size[1]);

            bound = new Rectangle(pos, size);

            color = Color.Green;

            layer = 9;
        }

        public TextureDescription(HurtBox hb)
        {
            tex = Player.DEBUGbox;

            Point pos = new Point((int)(hb.pos[0] + hb.offset[0]), (int)(hb.pos[1] + hb.offset[1]));
            Point size = new Point((int)hb.size[0], (int)hb.size[1]);

            bound = new Rectangle(pos, size);

            color = Color.Blue;

            layer = 10;
        }

        public TextureDescription(Hitbox hb)
        {
            tex = Player.DEBUGbox;

            Point pos = new Point((int)(hb.pos[0] + hb.offset[0]), (int)(hb.pos[1] + hb.offset[1]));
            Point size = new Point((int)hb.size[0], (int)hb.size[1]);

            bound = new Rectangle(pos, size);

            color = Color.Purple;

            layer = 7;
        }
    }
}
