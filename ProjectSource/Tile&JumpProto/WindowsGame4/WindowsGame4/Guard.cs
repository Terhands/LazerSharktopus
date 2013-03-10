using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class Guard : ADynamicGameObject, IDynamicGameObject
    {

        protected int LOSRadius;
        protected int hearingRadius;

        Vector2 eyePos;

        public Guard(Game game, Texture2D texture, int xStart, int yStart) : base(game)
        {

        }

        public override Rectangle GetPosition()
        {
            return new Rectangle();
        }

        public override void HandleCollision(IList<ITile> obj)
        {

        }

        protected Direction determineRadialCollision(Rectangle r, float radius)
        {
            Direction direction = Direction.none;

            // hopefully close enough & easier than having to handle circle/rectangle collisions
            // let recRadius be half the average of the width & height of the rectangle
            float recRadius = 0.25f * ((float)(r.Width + r.Height));

            float distance = (float)Math.Sqrt(Math.Pow(eyePos.X - (r.X + (r.Width/2)), 2) + Math.Pow(eyePos.Y - (r.Y + (r.Height/2)), 2));

            // if the distance is less than the two radiai, then the rectange is in collision with this Guard's collision radius
            if (distance <= recRadius + radius)
            {
                // now to get the collision direction l\u/r u=up, b=bottom, r=right, l=left
                //                                    l/b\r
                Vector2 destination = new Vector2((r.X + r.Width)/2, (r.Y + r.Height)/2);
                Vector2 vecDir = destination - eyePos;
                vecDir.Normalize();
                float angle = VectorToAngle(vecDir);

                // may have to tweak the angle values it depends on how xna stores angles againsts world coords
                if ((angle <= 45 && angle >= 0) || (angle >= 315))
                {
                    direction = Direction.right;
                }
                else if (angle >= 45 && angle <= 135)
                {
                    direction = Direction.top;
                }
                else if (angle >= 135 && angle <= 225)
                {
                    direction = Direction.left;
                }
                else
                {
                    direction = Direction.bottom;
                }
            }

            return direction;
        }

        protected float VectorToAngle(Vector2 v)
        {
            return (float)Math.Atan2(v.Y, v.X);
        }

        // get the normalized vector from the given angle
        protected Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public override void Update(Action action, int velocity)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }

    }
}
