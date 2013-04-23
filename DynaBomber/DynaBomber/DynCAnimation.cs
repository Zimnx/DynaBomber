using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DynaBomber
{
    public class DynCAnimation : DynIDrawable, DynICollidable
    {
        public Texture2D spriteStrip; // Image with all sprites for animation
        float scale; // Scale used to display sprite
        int elapsedTime; // Time since we last updated the frame
        int frameTime; // Time we display a frame until the next one
        int frameCount; // Number of frames in animation
        int curFrame; // Current frame
        int row; //Which row of spriteStrip we're drawing
        Color color; //The color of the frame

        Rectangle srcRect = new Rectangle(); //Area of the stip we're displaying
        Rectangle destRect = new Rectangle(); // Area where we'll display strip in-game

        public int FrameWidth;
        public int FrameHeight; //Size of frame

        public bool Active; //State of animation
        public bool Looping; // Shall we loop animation or deactivate after one run
        public Vector2 Position;

        //TODO delete rotation, scale and color parameters
        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount, int frameTime, bool looping, int row = 0)
        {
            spriteStrip = texture;
            this.scale = 1.0f;
            this.frameTime = frameTime;
            this.frameCount = frameCount;
            this.color = Color.White;

            this.Position = DynUtils.GetTileCenter(position);

            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.Looping = looping;
            this.row = row;

            elapsedTime = 0;
            curFrame = 0;
            Active = true;
        }

        //Updates animation state (position in-game, frame)
        public void Update(GameTime gameTime, float newX = -1, float newY = -1, int row = 0, bool nextFrame = false)
        {
            if (!Active)
                return;

            if (newX != -1) //As an option, we change position and row for player's animation
            {
                this.Position.X = newX;
                this.Position.Y = newY;
                this.row = row;
                if (nextFrame)
                    elapsedTime = frameTime + 1;
                else
                    elapsedTime = 0;
            }
            else
            {
                elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds; // Update elapsed time
                row = this.row;
            }

            //Change frame
            if (elapsedTime > frameTime)
            {
                curFrame++;
                if (curFrame >= frameCount)
                {
                    curFrame = 0;
                    Active = Looping; // If we don't loop, we deactivate animation
                }
                elapsedTime = 0;
            }
            
            // Get new frame
            srcRect = new Rectangle(curFrame * FrameWidth, FrameHeight * row, FrameWidth, FrameHeight);
            destRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,  // X
                                     (int)Position.Y - (int)(FrameHeight * scale) / 2, // Y
                                     (int)(FrameWidth * scale),                        // Width 
                                     (int)(FrameHeight * scale));                      // Heigth
        }

        //Provides some DynICollidable methods
        public bool Collides(DynICollidable o)
        {
            return (o.CollisionRect().Intersects(CollisionRect()));
        }
        public Rectangle CollisionRect()
        {
            Rectangle returnRect = destRect;
            returnRect.X += 1;
            returnRect.Y += -1;
            returnRect.Width -= 1;
            returnRect.Height += 1;
            return returnRect;
        }
        public void OnCollision(DynUtils.ObjectType o)
        {
            return;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
                spriteBatch.Draw(spriteStrip, destRect, srcRect, color);
        }

    }
}
