using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynaBomber
{
    class DynExplosion : DynIDrawable
    {
        int top;
        int bottom;
        int left;
        int right;
        public bool active;
        public Vector2 center;
        Texture2D eTexture;
        Texture2D cTexture;

        List<DynCAnimation> animations = new List<DynCAnimation>();

        public void Initialize(Vector2 center, int top, int bottom, int left, int right, Texture2D eTexture, Texture2D cTexture)
        {
            this.top = top;
            this.bottom = bottom;
            this.left = left;
            this.right = right;
            this.center = center;
            this.eTexture = eTexture;
            int frameTime = DynUtils.explosionTime / 4 ;
            this.cTexture = cTexture;
            active = true;

            bool looping = false;

            //We add animations for each direction
            for (int i = 1; i <= right; ++i)
            {
                DynCAnimation a = new DynCAnimation();
                if (i == right)
                    a.Initialize(eTexture, new Vector2(center.X + 16 * i, center.Y), 16, 16, 4, frameTime, looping, 3);
                else
                    a.Initialize(eTexture, new Vector2(center.X + 16 * i, center.Y), 16, 16, 4, frameTime, looping, 1);
                animations.Add(a);
            }

            for (int i = 1; i <= left; ++i)
            {
                DynCAnimation a = new DynCAnimation();
                if (i == left)
                    a.Initialize(eTexture, new Vector2(center.X - 16 * i, center.Y), 16, 16, 4, frameTime, looping, 5);
                else
                    a.Initialize(eTexture, new Vector2(center.X - 16 * i, center.Y), 16, 16, 4, frameTime, looping, 1);
                animations.Add(a);
            }


            for (int i = 1; i <= top; ++i)
            {
                DynCAnimation a = new DynCAnimation();
                if (i == top)
                    a.Initialize(eTexture, new Vector2(center.X, center.Y - 16 * i), 16, 16, 4, frameTime, looping, 2);
                else
                    a.Initialize(eTexture, new Vector2(center.X, center.Y - 16 * i), 16, 16, 4, frameTime, looping);
                animations.Add(a);
            }

            for (int i = 1; i <= bottom; ++i)
            {
                DynCAnimation a = new DynCAnimation();
                if (i == bottom)
                    a.Initialize(eTexture, new Vector2(center.X, center.Y + 16 * i), 16, 16, 4, frameTime, looping, 4);
                else
                    a.Initialize(eTexture, new Vector2(center.X, center.Y + 16 * i), 16, 16, 4, frameTime, looping);
                animations.Add(a);
            }

            DynCAnimation e = new DynCAnimation();
            frameTime = DynUtils.explosionTime / 9;
            e.Initialize(cTexture, center, 16, 16, 9, frameTime, false);
            animations.Add(e);
        }

        //Iterate through animations and update them
        public void Update(GameTime gameTime)
        {
            List<DynCAnimation> nl = new List<DynCAnimation>();
            foreach (DynCAnimation a in animations)
            {
                if (a.Active)
                    nl.Add(a);
                a.Update(gameTime);
            }
            active = nl.Count > 0;
            animations = nl;
        }

        public bool Collides(DynICollidable o)
        {
            foreach (DynCAnimation a in animations)
                if (a.Collides(o))
                    return true;
            return false;
        }

  

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (DynCAnimation a in animations)
                a.Draw(spriteBatch);
        }
    }
}
