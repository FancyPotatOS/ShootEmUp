using System;
using System.Collections.Generic;
using System.Text;

namespace ShootEmUp.Hitboxes
{
    public class CollisionBox : Hitbox
    {
        public CollisionBox(float[] o, float[] p, float[] s) : base(o, p, s) { }

        public static CollisionBox FromHitbox(Hitbox hb)
        {
            return new CollisionBox(hb.offset, hb.pos, hb.size);
        }
    }
}
