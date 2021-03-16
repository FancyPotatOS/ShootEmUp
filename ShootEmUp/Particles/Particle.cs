using Microsoft.Xna.Framework;
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

        public float[] locPointer;

        // Velocity and acceleration
        readonly float[] vel;
        readonly float[] decc;
        // Speed before speed is just set to 0
        readonly float toZero;

        // Time before dies
        readonly Cooldown lifetime;

        List<Animation> animations;

        public Particle(float[] offset, float[] size, float[] locationPointer, float[] vel, float[] decc, float toZero, uint lifetime, bool useCollisionBox, List<Animation> anims)
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
            animations = anims;

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

            // Update the textures
            foreach (Animation anim in animations)
            {
                anim.Update();

                // Loop if expired
                if (anim.Expired())
                    anim.Reset();
            }

            // Whether the lifetime has run out
            return lifetime.CanUse(); 
        }

        public bool UpdateLifetime()
        { lifetime.Update(); return lifetime.CanUse(); }

        // Get all related textures
        public List<TextureDescription> GetTextures()
        {
            List<TextureDescription> td = new List<TextureDescription>();

            foreach (Animation anim in animations)
            {
                // Get texture from animation
                int[] currAnimSize = anim.GetSizeOfCurrentAnimation();
                Point pos = new Point(
                    (int)(blocking.pos[0] + blocking.offset[0] + (blocking.size[0] / 2) - (currAnimSize[0] / 2)),
                    (int)(blocking.pos[1] + blocking.offset[1] + blocking.size[1] - currAnimSize[1])
                );
                Point size = new Point((int)currAnimSize[0], (int)currAnimSize[1]);
                Rectangle bound = new Rectangle(pos, size);
                td.Add(new TextureDescription(anim.GetTexture(), bound, Color.White, 10));
            }

            return td;
        }

        public void ChangeAnimations(List<Animation> newAnims)
        {
            animations = newAnims;
        }

        // Returns sign of the value
        public int GetSign(float val)
        {
            return (val > 0) ? 1 : (val < 0) ? -1 : 0;
        }
    }
}
