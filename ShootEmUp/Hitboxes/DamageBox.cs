using System;
using System.Collections.Generic;
using System.Text;

using ShootEmUp.Entities;

namespace ShootEmUp.Hitboxes
{
    // Damage box is what does the damage
    public class DamageBox : Hitbox
    {
        readonly float[] knockback;
        readonly int damage;
        public readonly int priority;
        public int ttl;

        readonly List<IEntity> accounted;

        public DamageBox(float[] kb, int d, float[] o, float[] p, float[] s) : base(o, p, s)
        {
            knockback = kb;
            damage = d;

            accounted = new List<IEntity>();
        }

        float[] GetKnockback()
        {
            return knockback;
        }

        public void Update()
        {

        }
    }
}
