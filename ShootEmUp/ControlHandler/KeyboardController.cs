using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShootEmUp.ControlHandler
{
    class KeyboardController : IController
    {
        public bool DownPressed()
        {
            return SEU.instance.preKeys.Contains(Keys.S);
        }

        public bool LeftPressed()
        {
            return SEU.instance.preKeys.Contains(Keys.A);
        }

        public bool RightPressed()
        {
            return SEU.instance.preKeys.Contains(Keys.D);
        }

        public void Update() { }

        public bool UpPressed()
        {
            return SEU.instance.preKeys.Contains(Keys.W);
        }
    }
}
