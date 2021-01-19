using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.Hitboxes
{
    public class Hitbox
    {
        public readonly float[] offset;
        public readonly float[] pos;
        public readonly float[] size;

        public Hitbox(float[] o, float[] p, float[] s)
        {
            offset = o;
            pos = p;
            size = s;
        }

        // Checks whether it intersects/overlaps region
        public bool Crosses(float[] pos, float[] size)
        {
            Hitbox hb = new Hitbox(new float[2] { 0, 0 }, pos, size);
            return Crosses(hb);
        }

        // Checks whether it intersects/overlaps region
        public bool CrossesExclusive(float[] pos, float[] size)
        {
            Hitbox hb = new Hitbox(new float[2] { 0, 0 }, pos, size);
            return CrossesExclusive(hb);
        }

        // Checks if the hitboxes intersect/overlap anywhere
        public bool Crosses(Hitbox hb)
        {
            return CrossesX(hb) && CrossesY(hb);
        }

        // Checks if the hitboxes intersect/overlap anywhere
        public bool CrossesExclusive(Hitbox hb)
        {
            return CrossesXExclusive(hb) && CrossesYExclusive(hb);
        }

        // Position where hb touches this on the x axis (Returns what hb.pos[0] should be)
        public float ConnectX(Hitbox hb)
        {
            // It crosses, no need to move
            if (hb.Crosses(this))
            {
                return hb.pos[0];
            }
            // Hitbox is to the right of this
            else if (hb.pos[0] >= this.pos[0])
            {
                float rightside = this.pos[0] + this.offset[0] + this.size[0];
                return rightside - hb.offset[0];
            }
            // Hitbox is to the left of this
            else if (hb.pos[0] < this.pos[0])
            {
                float leftside = this.pos[0] + this.offset[0];
                return leftside + hb.offset[0];
            }

            // Nonsensical area
            return hb.pos[0];
        }

        // Position where hb touches this on the y axis (Returns what hb.pos[1] should be)
        public float ConnectY(Hitbox hb)
        {
            // It crosses, no need to move
            if (hb.Crosses(this))
            {
                return hb.pos[1];
            }
            // Hitbox is to the upper of this
            else if (hb.pos[1] >= this.pos[1])
            {
                float upside = this.pos[1] + this.offset[1] + this.size[1];
                return upside - hb.offset[1];
            }
            // Hitbox is to the bottom of this
            else if (hb.pos[1] < this.pos[1])
            {
                float bottomside = this.pos[1] + this.offset[1];
                return bottomside + hb.offset[1];
            }

            // Nonsensical area
            return hb.pos[1];
        }

        // Checks if the hitboxes cross in the x axis
        public bool CrossesX(Hitbox hb)
        {
            // Check if left side is left of/within the box
            float ls = pos[0] + offset[0];
            if (ls <= hb.pos[0] + hb.offset[0] + hb.size[0])
            {
                // Right side
                float rs = pos[0] + offset[0] + size[0];
                return (hb.pos[0] + hb.offset[0] <= rs);
            }
            return false;
        }

        // Checks if the hitboxes cross in the y axis
        public bool CrossesY(Hitbox hb)
        {
            // Check if lower side is below/within the box
            float ls = pos[1] + offset[1];
            if (ls <= hb.pos[1] + hb.offset[1] + hb.size[1])
            {
                // Upper side
                float us = pos[1] + offset[1] + size[1];
                // Return whether upper side is greater than/within the box
                return (hb.pos[1] + hb.offset[1] <= us);
            }
            return false;
        }


        // Checks if the hitboxes cross in the x axis
        public bool CrossesXExclusive(Hitbox hb)
        {
            // Check if left side is left of/within the box
            float ls = pos[0] + offset[0];
            if (ls < hb.pos[0] + hb.offset[0] + hb.size[0])
            {
                // Right side
                float rs = pos[0] + offset[0] + size[0];
                return (hb.pos[0] + hb.offset[0] < rs);
            }
            return false;
        }

        // Checks if the hitboxes cross in the y axis
        public bool CrossesYExclusive(Hitbox hb)
        {
            // Check if lower side is below/within the box
            float ls = pos[1] + offset[1];
            if (ls < hb.pos[1] + hb.offset[1] + hb.size[1])
            {
                // Upper side
                float us = pos[1] + offset[1] + size[1];
                // Return whether upper side is greater than/within the box
                return (hb.pos[1] + hb.offset[1] < us);
            }
            return false;
        }

        // Checks if a <= c <= b
        public bool Within(float a, float b, float c)
        {
            return (a <= c && c <= b);
        }

        public Hitbox Copy()
        {
            return new Hitbox(new float[] { offset[0], offset[1] }, new float[] { pos[0], pos[1] }, size);
        }
    }
}
