﻿using Microsoft.Xna.Framework.Input;
using ShootEmUp.Hitboxes;
using ShootEmUp.TextureHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShootEmUp.Entities
{
    public class FollowBehaviour
    {
        // How to find target, who we see, and where we last saw it
        readonly Predicate<IEntity> target;
        IEntity currTarg;
        float[] lastPointSeen;

        // Distance away from follow position before uninterested
        uint distanceIgnore;

        // Sight hitbox
        readonly Hitbox sight;

        // How long before reevaluates target
        readonly Cooldown resetTarget;
        readonly Cooldown stopFollowing;

        readonly IEntity me;
        readonly float[] posPointer;

        bool changedFollowing;

        public FollowBehaviour(Predicate<IEntity> targ, int viewSize, IEntity me, uint resTarg, uint stpFollow, uint distIgnore)
        {
            posPointer = me.GetCollisionHitboxes()[0].pos;

            lastPointSeen = null;
            currTarg = null;
            target = targ;

            sight = new Hitbox(new float[] { -viewSize / 2, -viewSize / 2 }, posPointer, new float[] { viewSize, viewSize });

            resetTarget = new Cooldown(0, resTarg);

            this.me = me;

            distanceIgnore = distIgnore;

            stopFollowing = new Cooldown(stpFollow, stpFollow);

            changedFollowing = true;
        }

        public bool HasCurrTarget()
        {
            return lastPointSeen != null;
        }

        public float[] GetTarget()
        {
            return new float[] { lastPointSeen[0], lastPointSeen[1] };
        }

        // Update given myself
        public void Update()
        {
            // Assume not changed following state
            changedFollowing = false;

            // Update cooldown for new target
            resetTarget.Update();
            
            // Attempt to find new target if none
            if (currTarg == null)
                ResetTarget();

            // Set following if has a target
            UpdateIfHasTarget();

            // If following behind with no target
            if (currTarg == null && lastPointSeen != null)
            {
                // Update the cooldown to stop
                stopFollowing.Update();
                
                // If not done following
                if (!stopFollowing.CanUse())
                {
                    // Get euclidean distance from last point seen
                    double distFromLastSeen = Math.Sqrt(
                        ((lastPointSeen[0] - posPointer[0]) * (lastPointSeen[0] - posPointer[0])) *
                        ((lastPointSeen[1] - posPointer[1]) * (lastPointSeen[1] - posPointer[1])));

                    // Ignore now that too close
                    if (distFromLastSeen < distanceIgnore)
                    {
                        lastPointSeen = null;

                        changedFollowing = true;
                    }
                    // Otherwise just keep swimming
                    else
                    {
                        return;
                    }
                }
                // Otherwise just forget the point
                else
                {
                    changedFollowing = true;

                    lastPointSeen = null;
                }
            }
        }

        // Update what last point seen is
        void UpdateIfHasTarget()
        {
            // If has a target
            if (currTarg != null)
            {
                // For each of the target's collision boxes
                foreach (Hitbox hb in currTarg.GetCollisionHitboxes())
                {
                    // Set the follow to position
                    lastPointSeen = new float[] { hb.pos[0], hb.pos[1] };

                    // If in sights
                    if (hb.Crosses(sight))
                    {
                        // Done updating
                        return;
                    }
                }

                // Cannot see the target
                currTarg = null;

                // Start the following cooldown
                stopFollowing.Reset();
            }

        }

        // Reevaluate trying to find a target
        void ResetTarget()
        {

            // If going to reset target
            if (resetTarget.CanUse())
            {
                // Reset cooldown
                resetTarget.Reset();

                // Find all potential targets that are not me
                List<IEntity> targetable = SEU.instance.GLOBALSTATE.FindEntities(target).FindAll(ent => ent != me);

                // If there is none
                if (targetable.Count == 0)
                {
                    // Not chasing
                    currTarg = null;
                }
                else
                {
                    // For each potential target
                    foreach (IEntity potTarg in targetable)
                    {
                        // For each collision box
                        foreach (Hitbox hb in potTarg.GetCollisionHitboxes())
                        {
                            // If can see
                            if (hb.Crosses(sight))
                            {
                                // Found the target
                                lastPointSeen = new float[] { hb.pos[0], hb.pos[1] };
                                currTarg = potTarg;

                                changedFollowing = true;

                                return;
                            }
                        }
                    }
                }
            }
        }

        public bool ChangedState()
        {
            return changedFollowing;
        }
    }
}
