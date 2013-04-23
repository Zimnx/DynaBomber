using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DynaBomber
{
    class DynPlayer : DynIDrawable, DynICollidable
    {
        enum direction { DOWN, RIGHT, LEFT, UP };
        public Texture2D deathTexture;
        public Vector2 position;
        public int bombsLeft = 3;
        public int bombPower = 3;
        public int playerID; //0/1/2/3
        public bool alive;

        // movement variables
        public float speedLevel = 1.2f;
        int curDir;
        int distTraveled;
        bool nextFrame;

        public DynCAnimation animation;

        public PlayerKeys playerKeys;

        public void Initialize(Texture2D playerTexture, Texture2D deathTexture, Vector2 playerPosition, int playerId)
        {
            position = playerPosition;
            this.playerID = playerId;
            alive = true;
            playerKeys = new PlayerKeys();
            this.deathTexture = deathTexture;
            
            distTraveled = 0;

            animation = new DynCAnimation();
            animation.Initialize(playerTexture, position, 23, 23, 3, 400, true);
        }

        public void Move(KeyboardState keybState, List<Rectangle> collisionList, List<DynBomb> bombs)
        {
            bool moved = false;
            //Where would player move if he moved
            Vector2 new_position = new Vector2(position.X, position.Y);

            if (keybState.IsKeyDown(playerKeys.KeyUp))
            {
                new_position.Y -= speedLevel;
                curDir = (int)direction.UP;
                moved = true;
            }
            if (keybState.IsKeyDown(playerKeys.KeyDown))
            {
                new_position.Y += speedLevel;
                curDir = (int)direction.DOWN;
                moved = true;
            }
            if (keybState.IsKeyDown(playerKeys.KeyLeft))
            {
                new_position.X -= speedLevel;
                curDir = (int)direction.LEFT;
                moved = true;
            }
            if (keybState.IsKeyDown(playerKeys.KeyRight))
            {
                new_position.X += speedLevel;
                curDir = (int)direction.RIGHT;
                moved = true;
            }

            bool will_move = true;
            //Determining movement, considering bombs
            Rectangle playerRec = new Rectangle((int)new_position.X - 3, (int)new_position.Y - 3, 12, 12);

            foreach (Rectangle c in collisionList)
                if (c.Intersects(playerRec))
                {
                    will_move = false;
                    bool is_bomb = false;
                    foreach (DynBomb b in bombs)
                    {
                        if (c.Intersects(b.CollisionRect()))
                        {
                            is_bomb = true;
                            if (b.OwnerId == playerID & !b.playerGotOff)
                                will_move = true;
                        }
                    }
                    if (!is_bomb)
                        break;
                }
            foreach (DynBomb b in bombs)
                if (!b.Collides(this) && b.OwnerId == playerID)
                    b.playerGotOff = true;

            if (will_move)
                position = new_position;
            
            if (moved) 
                ++distTraveled;

            if (distTraveled > 13) //Co jaki dystans zmieni się klatka
            {
                distTraveled = 0;
                nextFrame = true;
            }
            else
                nextFrame = false;
        }

        void die()
        {
            if (alive) //Once we die (and only once) we switch to death animation
            {
                alive = false;
                animation = new DynCAnimation();
                animation.Initialize(deathTexture, position, 24, 24, 7, 300, false);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (alive)
                animation.Update(gameTime, position.X, position.Y, (int)curDir, nextFrame);
            else
                animation.Update(gameTime);
        }

        public void OnCollision(DynUtils.ObjectType c)
        {
            switch (c)
            {
                case DynUtils.ObjectType.BombBonus:
                    ++bombsLeft;
                    break;
                case DynUtils.ObjectType.ExplosionBonus:
                    ++bombPower;
                    break;
                case DynUtils.ObjectType.Monster:
                    die();
                    break;
                case DynUtils.ObjectType.Explosion:
                    die();
                    break;
            }
        }

        public Rectangle CollisionRect()
        {
            return new Rectangle((int)this.position.X - 7, (int)this.position.Y - 5, 10, 10);
        }

        public bool Collides(DynICollidable o)
        {
            return o.CollisionRect().Intersects(CollisionRect());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
        }

    }
}