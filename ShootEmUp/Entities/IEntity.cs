using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShootEmUp.Hitboxes;
using ShootEmUp.TextureHandling;

namespace ShootEmUp.Entities
{
    public interface IEntity
    {
        void Update();

        List<TextureDescription> GetTextures();

        List<CollisionBox> GetCollisionHitboxes();

        List<HurtBox> GetHurtboxes();

        List<DamageBox> GetDamageBoxes();

        public static void LoadAllAnimations(ContentManager Content)
        {
            Player.LoadAnimations(Content);

            Card.LoadAnimations(Content);

            Blob.LoadAnimations(Content);
        }

        public static Dictionary<string, Dictionary<string, Animation>> LoadAnimations(ContentManager Content, string xmlpath)
        {
            // Dictionary by type
            Dictionary<string, Dictionary<string, Animation>>  animations = new Dictionary<string, Dictionary<string, Animation>>();

            // Load file from resources
            XmlDocument doc = new XmlDocument();
            doc.Load(EmbeddedFileHandler.GetStreamReader(xmlpath));

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

            return animations;
        }
    }
}
