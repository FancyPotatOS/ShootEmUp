using System;
using System.Collections.Generic;
using System.Text;
using ShootEmUp.Hitboxes;
using ShootEmUp.TextureHandling;

namespace ShootEmUp.States
{
    internal interface IState
    {
        void Update();

        Hitbox GetCrosses(Hitbox hb);
        Hitbox GetCrossesEx(Hitbox hb);

        List<TextureDescription> GetTextures();
    }
}
