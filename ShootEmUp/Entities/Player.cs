using ControlsHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootEmUp.Hitboxes;
using ShootEmUp.TextureHandling;
using System;
using System.Collections.Generic;
using System.Linq;

using CardsDevelopment;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace ShootEmUp.Entities
{
    class Player : IEntity
    {
        public static Dictionary<string, Dictionary<string, Animation>> animations;
        Animation currAnimation;
        string animationName;


        public float[] speed;
        const float maxWalkSpeed = 5;
        const float maxRunSpeed = 8;

        public static Texture2D DEBUGbox;

        public IController controller;

        float[] posPointer;
        public HurtBox vulnerable;
        public CollisionBox blocking;
        public List<DamageBox> attacking;

        public string facing;
        public int facingChange;
        bool running;

        public Player(IController cont, float[] pos)
        {
            // Fill hitboxes from pos
            posPointer = pos;
            controller = cont;

            attacking = new List<DamageBox>();
            blocking = CollisionBox.FromHitbox(new Hitbox(new float[] { -25, -25 }, posPointer, new float[] { 50, 60 }));
            vulnerable = new HurtBox(new float[] { -20, -20 }, posPointer, new float[] { 40, 40 });
            speed = new float[2];

            currAnimation = GetAnimation("stand", "down");
            animationName = "stand";
            facing = "down";
            facingChange = 0;
            running = false;
        }

        public void Update()
        {
            // Update current animation to loop
            currAnimation.Update();
            if (currAnimation.Expired())
                currAnimation.Reset();

            // Update controls
            controller.Update();

            // Save which buttons are pressed
            int[] buttonVec = { 0, 0 };

            // Whether animation should update
            bool changedAnim = false;

            // Whether run is still pressed
            bool newRun = controller.IsPressed("run");
            string newAnim = "stand";

            // Set max speed, depends on if running
            float maxSpeed = maxWalkSpeed;
            if (running)
                maxSpeed = maxRunSpeed;

            if (controller.IsPressed("up"))
            {
                newAnim = (newRun) ? "run" : "walk";
                buttonVec[1] = -1;
                speed[1] = Math.Max(-maxSpeed, Math.Min(0, speed[1] - 1));
            }
            else if (controller.IsPressed("down"))
            {
                newAnim = (newRun) ? "run" : "walk";
                buttonVec[1] = 1;
                speed[1] = Math.Min(maxSpeed, Math.Max(0, speed[1] + 1));
            }
            else
            {
                speed[1] -= GetMag(speed[1]);
            }
            if (controller.IsPressed("left"))
            {
                newAnim = (newRun) ? "run" : "walk";
                buttonVec[0] = -1;
                speed[0] = Math.Max(-maxSpeed, Math.Min(0, speed[0] - 1));
            }
            else if (controller.IsPressed("right"))
            {
                newAnim = (newRun) ? "run" : "walk";
                buttonVec[0] = 1;
                speed[0] = Math.Min(maxSpeed, Math.Max(0, speed[0] + 1));
            }
            else
            {
                speed[0] -= GetMag(speed[0]);
            }

            string dir = VectorToDirection(buttonVec);

            // Update animation if direction changed or running has changed and not walking
            changedAnim = (facing  != dir) || (animationName != newAnim) || (animationName != "walk" && (newRun != running));

            running = newRun;
            animationName = newAnim;
            facing = dir;

            // If direction changed
            if (changedAnim)
            {
                // Reset current animation
                currAnimation.Reset();

                // Set to right animation
                currAnimation = GetAnimation(animationName, facing);
            }

            float[] sumChange = new float[2];

            // Attempt to move X
            Hitbox movedX = blocking.Copy();
            movedX.pos[0] += speed[0];

            // If not null, then crosses some hitbox
            Hitbox crosses = SEU.instance.GLOBALSTATE.GetCrossesEx(movedX);
            if (crosses != null)
            {
                sumChange[0] = crosses.ConnectX(blocking) - blocking.pos[0];
                speed[0] = 0;
            }
            else
                sumChange[0] = speed[0];


            // Change position pointer
            posPointer[0] += sumChange[0];

            // Attempt to move X
            Hitbox movedY = blocking.Copy();
            movedY.pos[1] += speed[1];

            // If not null, then crosses some hitbox
            crosses = SEU.instance.GLOBALSTATE.GetCrossesEx(movedY);
            if (crosses != null)
            {
                sumChange[1] = crosses.ConnectY(blocking) - blocking.pos[1];
                speed[1] = 0;
            }
            else
                sumChange[1] = speed[1];
           
            // Change position pointer
            posPointer[1] += sumChange[1];
        }

        public List<TextureDescription> GetTextures()
        {
            List<TextureDescription> td = new List<TextureDescription>();
            /** /
            {
                td.Add(new TextureDescription(vulnerable));

                td.Add(new TextureDescription(blocking));
            }

            attacking.ForEach(db =>
            {
                td.Add(new TextureDescription(db));
            });
            td.Clear();
            /**/

            // Get texture from animation
            int[] currAnimSize = currAnimation.GetSizeOfCurrentAnimation();
            Point pos = new Point(
                (int)(blocking.pos[0] + blocking.offset[0] + (blocking.size[0] / 2) - (currAnimSize[0] / 2)), 
                (int)(blocking.pos[1] + blocking.offset[1] + blocking.size[1] - currAnimSize[1])
            );
            Point size = new Point((int)currAnimSize[0], (int)currAnimSize[1]);
            Rectangle bound = new Rectangle(pos, size);
            td.Add(new TextureDescription(currAnimation.GetTexture(), bound, Color.White, 10));

            return td;
        }

        public List<CollisionBox> GetCollisionHitboxes()
        {
            List<CollisionBox> cbs = new List<CollisionBox>();
            {
                cbs.Add(blocking);
            }
            return cbs;
        }

        public List<DamageBox> GetDamageBoxes()
        {
            return attacking;
        }

        public List<HurtBox> GetHurtboxes()
        {
            return new HurtBox[] { vulnerable }.ToList();
        }

        public static void LoadAnimations(ContentManager Content)
        {
            // Do not reload animations
            if (animations != null)
            {
                return;
            }

            // Dictionary by type
            animations = new Dictionary<string, Dictionary<string, Animation>>();

            // Load file from resources
            XmlDocument doc = new XmlDocument();
            doc.Load(EmbeddedFileHandler.GetStreamReader("Animations/player_animations.xml"));

            foreach (XmlNode node in doc.SelectNodes("/Animations/child"))
            {
                // Get name of animation
                string name = node.Attributes.GetNamedItem("name").InnerText;

                // Animation organized by direction
                Dictionary<string, Animation> directionalAnimation = new Dictionary<string, Animation>();

                // Get each frame
                List<int> frames = new List<int>();
                foreach (XmlNode frm in node.SelectSingleNode("frames").ChildNodes)
                {
                    frames.Add(Int32.Parse(frm.InnerText));
                }
                int[] frameArr = frames.ToArray();

                // For each direction
                foreach (XmlNode child in node.SelectNodes("child"))
                {
                    // Get name of direction
                    string direction = child.Attributes.GetNamedItem("name").InnerText;

                    // Get each path for direction
                    List<Texture2D> texs = new List<Texture2D>();
                    foreach (XmlNode pathNode in child.SelectNodes("child"))
                    {
                        string path = pathNode.InnerText;

                        // Get texture and save
                        Texture2D tex = Content.Load<Texture2D>(path);
                        texs.Add(tex);
                    }

                    // Get textures in array format
                    Texture2D[] texsArr = texs.ToArray();

                    // Add animation to direction
                    directionalAnimation.Add(direction, new Animation(frameArr, texsArr));
                }

                // Add direction animation dictionary to full dictionary
                animations.Add(name, directionalAnimation);
            }
        }

        public Animation GetAnimation(string type, string direction)
        {
            if (animations.TryGetValue(type, out Dictionary<string, Animation> anim))
            {
                if (anim.TryGetValue(direction, out Animation attempt))
                {
                    return attempt;
                }
                else
                {
                    throw new Exception("'" + direction + "' is not a valid direction!");
                }
            }
            else
            {
                throw new Exception("'" + type + "' is not a valid animation!");
            }
        }

        // Turns 2d vector to string direction, defaults to facing if no direction
        string VectorToDirection(int[] vec)
        {
            string coll = "";
            coll += (vec[1] < 0) ? "up" : (vec[1] > 0) ? "down" : "";
            coll += (vec[0] > 0) ? "right" : (vec[0] < 0) ? "left" : "";

            return (coll == "") ? facing : coll;
        }

        int GetMag(float n)
        {
            return (n > 0) ? 1 : (n < 0) ? -1 : 0;
        }
    }
}
