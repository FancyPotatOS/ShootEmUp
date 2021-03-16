using Microsoft.Xna.Framework;
using ShootEmUp.Hitboxes;
using SimpleDungeonGenerator;
using System;
using System.Collections.Generic;
using System.Text;



namespace ShootEmUp
{
    public class Map
    {
        public readonly List<CollisionBox> walls;

        readonly int[] offset;

        public readonly Dungeon dungeon;

        public Map()
        {
            int boxSize = 200;

            dungeon = new Dungeon(50, 50);
            dungeon.StartGeneration(1, 3);

            walls = new List<CollisionBox>();

            offset = new int[] { 0, 0 };

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    if (!dungeon.map[i, j])
                    {
                        walls.Add(new CollisionBox(new float[] { 0, 0 }, new float[] { i * boxSize, j * boxSize }, new float[] { boxSize, boxSize }));
                    }
                }
            }

            int tempOffset = 10;
            walls.Add(new CollisionBox(new float[] { 0, 0 }, new float[] { -tempOffset, 0 }, new float[] { tempOffset, 100000 }));
            walls.Add(new CollisionBox(new float[] { 0, 0 }, new float[] { 0, -tempOffset }, new float[] { 1000000, tempOffset }));
            walls.Add(new CollisionBox(new float[] { 0, 0 }, new float[] { 50 * boxSize, 0 }, new float[] { tempOffset, 100000 }));
            walls.Add(new CollisionBox(new float[] { 0, 0 }, new float[] { 0, 50 * boxSize }, new float[] { 100000, tempOffset }));
        }
    }
}
