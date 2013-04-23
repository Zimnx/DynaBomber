using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynaBomber
{
    class DynBomb : DynIDrawable, DynICollidable
    {
        public int explosionSize;
        public int OwnerId;
        int lifeTime; //miliseconds
        public bool active; //true when the bomb or explosion still exists
        public bool exploded;
        public bool running;
        public bool playerGotOff; //True if player has stepped off the bomb (can't go back on it)
        public Vector2 position;
        Texture2D texture;
        public Texture2D explosionTexture;
        DynCAnimation animation;

        public void Initialize(int ownerId, int size, Vector2 pos, Texture2D texture, Texture2D explosionTexture)
        {
            this.OwnerId = ownerId;
            this.explosionSize = size;
            this.position = pos;
            this.texture = texture;
            this.explosionTexture = explosionTexture;
            exploded = false;
            running = false;
            lifeTime = 3000;
            active = true;
            playerGotOff = false;
            animation = new DynCAnimation();
            animation.Initialize(this.texture, position, 16, 16, 3, 200, true);
        }

        public void Update(GameTime gameTime)
        {
            if (lifeTime > 0)
            {
                lifeTime -= (int)gameTime.ElapsedGameTime.Milliseconds;
            }
            else
                if (!exploded)
                {
                    //animation = new DynAnimation();
                    animation.Initialize(explosionTexture, position, 16, 16, 9, 60, false);
                    exploded = true;
                    running = true;
                }
            animation.Update(gameTime);
        }

        public bool Collides(DynICollidable o)
        {
            return o.CollisionRect().Intersects(animation.CollisionRect());
        }

        public Rectangle CollisionRect()
        {
            return animation.CollisionRect();
        }

        public void OnCollision(DynUtils.ObjectType o)
        {
            switch (o)
            {
                case DynUtils.ObjectType.Explosion:
                    lifeTime = 0;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);

            if (exploded)
                active = animation.Active;
        }
    }
}
