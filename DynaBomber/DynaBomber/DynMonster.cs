using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace DynaBomber
{
    class DynMonster : DynIDrawable, DynICollidable
    {
        public Texture2D deathTexture;
        public Texture2D monsterTexture;
        public Vector2 position;
        public bool ghost;
        public Vector2 lastMove; // ( +/- 1/0 , +/- 1/0) depends on direction f.e (1,0) - right, (0,1) - down
        public bool alive;
        int changeDirIn;
        public int monsterID;
        public Vector2[] possibleMoves;
        public DynCAnimation animation;

        public float speedLevel = 0.7f;

        int distTraveled;
        bool nextFrame;


        public void Update(GameTime gameTime)
        {
            if (alive)
                animation.Update(gameTime, position.X, position.Y, 0, nextFrame);
            else
                animation.Update(gameTime);
        }
        public void Move(List<Rectangle> collisionList)
        {
            Vector2 new_position = position;
            new_position.X += speedLevel * lastMove.X;
            new_position.Y += speedLevel * lastMove.Y;
            bool moved = true;
            foreach (Rectangle r in collisionList)
                if (!ghost && r.Intersects(new Rectangle((int)new_position.X - 5, (int)new_position.Y - 5, 13, 13)) ||
                    new_position.Y <= 8 || new_position.X <= 8 || new_position.X >= 34 * 16 + 9 || new_position.Y >= 34 * 16 + 9) //czeba zmienic na po Tiles
                    moved = false;                                                                  // bo chodzi po szarych scianach

            if (moved)
                position = new_position;
            else
                lastMove = possibleMoves[DynUtils.Rand(0, 4)]; // [0,4)

            if (moved)
                ++distTraveled;

            if (distTraveled > 13)
            {
                distTraveled = 0;
                nextFrame = true;
                --changeDirIn;
                if (changeDirIn <= 0)
                {
                    changeDirIn = DynUtils.Rand(3, 7);
                    lastMove = possibleMoves[DynUtils.Rand(0, 4)]; // [0,4)
                }
            }
            else
                nextFrame = false;
        }

        public void Initialize(Vector2 pos, Texture2D deathTexture, Texture2D monsterTexture, int ID, bool ghost = false)
        {
            alive = true;
            position = pos;
            this.deathTexture = deathTexture;
            this.monsterTexture = monsterTexture;
            possibleMoves = new Vector2[]
            {
                new Vector2(0,1),
                new Vector2(0,-1),
                new Vector2(-1,0),
                new Vector2(1,0)
            };
            lastMove = possibleMoves[DynUtils.Rand(0, 3)];
            animation = new DynCAnimation();
            animation.Initialize(monsterTexture, position, 16, 16, 3, 400, true);
            this.ghost = ghost;
            monsterID = ID;
            changeDirIn = DynUtils.Rand(3, 8);
        }

        public void OnCollision(DynUtils.ObjectType c)
        {
            switch (c)
            {
                case DynUtils.ObjectType.Explosion:
                    die();
                    break;
                case DynUtils.ObjectType.Monster:
                    lastMove *= -1;
                    break;
            }
        }

        public Rectangle CollisionRect()
        {
            return animation.CollisionRect();
        }
        
        public bool Collides(DynICollidable o)
        {
            return o.CollisionRect().Intersects(CollisionRect());
        }

        void die()
        {
            if (alive)
            {
                alive = false;
                animation = new DynCAnimation();
                animation.Initialize(deathTexture, position, 16, 16, 6, 400, false);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
        }
    }

}
