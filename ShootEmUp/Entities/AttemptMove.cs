using ShootEmUp.Hitboxes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShootEmUp.Entities
{
    // Method to move in direction and snap to edge of hitboxes
    public class AttemptMove
    {
        public float[] AttemptToMove(Hitbox hb, float[] speed)
        {
            // The amount to move total
            float[] sumChange = new float[4];

            Hitbox copied = hb.Copy();

            // Copy the hitbox and shift it
            Hitbox movedX = copied.Copy();
            movedX.pos[0] += speed[0];

            // If not null, then crosses some hitbox somewhere
            Hitbox crosses = SEU.instance.GLOBALSTATE.GetCrossesEx(movedX);
            if (crosses != null)
            {
                sumChange[2] = 1;

                // Get the difference
                sumChange[0] = crosses.ConnectX(hb) - hb.pos[0];

                // Speed stops when collides
                speed[0] = 0;
            }
            else
                // Otherwise just move the normal speed
                sumChange[0] = speed[0];

            // Apply the movement to a copy of the hitbox
            copied.pos[0] += sumChange[0];

            // Attempt to move Y
            Hitbox movedY = copied.Copy();
            movedY.pos[1] += speed[1];

            // If not null, then crosses some hitbox
            crosses = SEU.instance.GLOBALSTATE.GetCrossesEx(movedY);
            if (crosses != null)
            {
                sumChange[2] = 2;

                // Get the difference
                sumChange[1] = crosses.ConnectY(copied) - copied.pos[1];

                // Speed stops when collides
                speed[1] = 0;
            }
            else
                // Otherwise just move in direction
                sumChange[1] = speed[1];

            // Return the difference
            return sumChange;
        }
    }
}
