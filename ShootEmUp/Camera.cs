using Microsoft.Xna.Framework;
using ShootEmUp.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShootEmUp
{
    public class Camera
    {
        public Point center;

        public int[] screenSize;

        IEntity target;

        float alpha;

        public Camera(int[] ss, IEntity targ, float al)
        {
            alpha = al;

            screenSize = ss;
            target = targ;

            // Center camera to target's position
            float[] hb = target.GetHurtboxes()[0].pos;
            center = new Point((int)hb[0] - (ss[0] / 2), (int)hb[1] - (ss[1] / 2));
        }

        public void Recenter()
        {
            // Center camera to target's position
            float[] hb = target.GetHurtboxes()[0].pos;
            Point newCenter = new Point((int)(hb[0] - (screenSize[0] / 2)), (int)(hb[1] - (screenSize[1] / 2)));
            newCenter = newCenter - center;

            /**/
            newCenter.X = (int)(newCenter.X * alpha);
            newCenter.Y = (int)(newCenter.Y * alpha);
            /**/

            center += newCenter;
        }

        public Point GetScreenPos(Point p)
        {
            return p - center;
        }

        public int[] GetScreenLocation(int[] location)
        {
            return new int[] { location[0] - center.X, location[1] - center.Y };
        }
    }
}
