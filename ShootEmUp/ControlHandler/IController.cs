using System;
using System.Collections.Generic;
using System.Text;

namespace ShootEmUp.ControlHandler
{
    interface IController
    {

        void Update();
        bool UpPressed();
        bool DownPressed();
        bool LeftPressed();
        bool RightPressed();
    }
}
