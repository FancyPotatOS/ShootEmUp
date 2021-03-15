using ShootEmUp.Entities;
using ShootEmUp.Hitboxes;
using ShootEmUp.TextureHandling;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShootEmUp
{
    public class Particle : AttemptMove
    {
        readonly bool hasCollisionBox;
        readonly CollisionBox blocking;

        readonly float[] locPointer;

        // Velocity and acceleration
        readonly float[] vel;
        readonly float[] decc;
        // Speed before speed is just set to 0
        readonly float toZero;

        // Time before dies
        readonly Cooldown lifetime;

        TextureDescription texDesc;

        public Particle(float[] offset, float[] size, float[] locationPointer, float[] vel, float[] decc, float toZero, uint lifetime, bool useCollisionBox, TextureDescription td)
        {
            // Ensure decceleration is above 0
            decc[0] = Math.Max(decc[0], 0);
            decc[1] = Math.Max(decc[1], 0);

            // Ensure threshold is above 0
            toZero = Math.Max(toZero, -toZero);

            // Fill in fields
            locPointer = locationPointer;
            this.vel = vel;
            this.decc = decc;
            this.toZero = toZero;
            texDesc = td;

            // Mark with collisionBox
            hasCollisionBox = useCollisionBox;

            // Set pointer to new hitbox
            blocking = new CollisionBox(offset, locPointer, size);

            // Create lifetimme
            this.lifetime = new Cooldown(lifetime, lifetime);
        }

        // Returns whether to delete
        public virtual bool Update() {
            // Update lifetime cooldown
            lifetime.Update();

            // Apply velocity and acceleration
            vel[0] -= GetSign(vel[0]) * decc[0];
            vel[1] -= GetSign(vel[1]) * decc[1];

            /*  Attempt to move position    */
            // Assume no collision box, change by velocity
            float[] change = vel;
            if (hasCollisionBox)
            {
                // If there is a collision box, attempt to move unless blocked.
                change = AttemptToMove(blocking, vel);
            }
            // Apply decision
            locPointer[0] += change[0];
            locPointer[1] += change[1];

            // If velocities are below threshold, then make 0
            if (GetSign(vel[0]) * vel[0] < toZero)
            { vel[0] = 0; }
            if (GetSign(vel[1]) * vel[1] < toZero)
            { vel[1] = 0; }

            // Whether the lifetime has run out
            return lifetime.CanUse(); 
        }

        public bool UpdateLifetime()
        { lifetime.Update(); return lifetime.CanUse(); }

        // Get all related textures
        public TextureDescription GetTexture()
        {
            // Change location of texture
            texDesc.bound.X = (int)locPointer[0];
            texDesc.bound.Y = (int)locPointer[1];

            return texDesc;
        }

        // Returns sign of the value
        public int GetSign(float val)
        {
            return (val > 0) ? 1 : (val < 0) ? -1 : 0;
        }
    }
}
