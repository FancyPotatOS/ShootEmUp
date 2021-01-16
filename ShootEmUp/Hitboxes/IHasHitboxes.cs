using System;
using System.Collections.Generic;
using System.Text;

namespace ShootEmUp.Hitboxes
{
    interface IHasHitboxes
    {
        // Get all HurtBoxes
        List<HurtBox> GetHurtboxes();

        // Get all DamageBoxes
        List<DamageBox> GetDamageBoxes();

        // Get all CollisionBoxes
        List<CollisionBox> GetCollisionBoxes();

        // Whether DamageBox collides with HurtBox
        HurtBox CollidesWithHurtBox(DamageBox db);

        // Whether HurtBox collides with DamageBox
        DamageBox CollidesWithDamageBox(HurtBox hb);

        // Whether CollisionBox collides with CollisionBox
        CollisionBox CollidesWithCollisionBox(CollisionBox cb);

        // Whether CollisionBox collides with World
        CollisionBox CollidesWithWorld(CollisionBox cb);


    }
}
