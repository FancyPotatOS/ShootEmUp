using System;
using System.Collections.Generic;
using System.Text;

using ShootEmUp.Hitboxes;
using ShootEmUp.TextureHandling;

namespace ShootEmUp.Entities
{
    public interface IEntity
    {
        void Update();

        List<TextureDescription> GetTextures();

        List<CollisionBox> GetCollisionHitboxes();

        List<HurtBox> GetHurtboxes();

        List<DamageBox> GetDamageBoxes();
    }
}
