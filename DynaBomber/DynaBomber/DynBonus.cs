using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DynaBomber
{
    class DynBonus : DynIDrawable, DynICollidable
    {
        public bool active;
        public DynUtils.ObjectType type;
        DynCAnimation animation;

        public void Initialize(Texture2D texture, Vector2 position, DynUtils.ObjectType type)
        {
            animation = new DynCAnimation();
            animation.Initialize(texture, position, 16, 16, 2, 250, true);
            if (type == DynUtils.ObjectType.ExplosionBonus || type == DynUtils.ObjectType.BombBonus)
                this.type = type;
            active = true;
        }

        public Rectangle CollisionRect()
        {
            return animation.CollisionRect();
        }
        public bool Collides(DynICollidable o)
        {
            return CollisionRect().Intersects(o.CollisionRect());
        }
        

        public void OnCollision(DynUtils.ObjectType type)
        {
            switch (type)
            {
                case DynUtils.ObjectType.Explosion:
                    this.active = false;
                    break;
                case DynUtils.ObjectType.Player:
                    this.active = false;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            animation.Update(gameTime);
        }
    }
}
