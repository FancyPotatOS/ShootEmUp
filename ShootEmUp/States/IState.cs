using System;
using System.Collections.Generic;
using System.Text;

using ShootEmUp.TextureHandling;

namespace ShootEmUp.States
{
    interface IState
    {
        void Update();


        List<TextureDescription> GetTextures();
    }
}
