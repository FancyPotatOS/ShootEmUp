using System;
using System.Collections.Generic;
using System.Text;

using ShootEmUp.Hitboxes;
using ShootEmUp.TextureHandling;

namespace ShootEmUp.Entities
{
    interface IEntity
    {
        void Update();
        List<CollisionBox> GetCollisionHitboxes();

        List<HurtBox> GetHurtboxes();

        List<DamageBox> GetDamageBoxes();

        List<TextureDescription> GetTextures();
    }
}
