using System;
using System.Collections.Generic;
using System.Text;

using ShootEmUp.Entities;

namespace ShootEmUp.Hitboxes
{
    // Hurtbox is where you get hurt
    public class HurtBox : Hitbox
    {
        public HurtBox(float[] o, float[] p, float[] s) : base(o, p, s)
        {
        }
    }
}