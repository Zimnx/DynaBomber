using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DynaBomber
{
    class DynPauseMenu
    {
        Texture2D background;
        Texture2D pointer;
        SpriteFont font;
        DynCAnimation bannerAnimation = new DynCAnimation();
        public Vector2 pointerPosition;
        public int pointerState; // which option in menu f.e 1 - start game , 2- settings etc.
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "RESUME GAME", new Vector2(35 * 16 / 2 - 70, 35 * 16 / 2 + 60), Color.White);

            spriteBatch.DrawString(font, "EXIT GAME", new Vector2(35 * 16 / 2 - 70, 35 * 16 / 2 + 120), Color.White);
            spriteBatch.Draw(pointer, pointerPosition, Color.White);
        }
        public void Initialize(Texture2D Background, Texture2D Pointer, SpriteFont font)
        {
            this.font = font;
            background = Background;
            pointer = Pointer;
            pointerPosition = new Vector2(35 * 16 / 2 - 90, 35 * 16 / 2 + 60);
            pointerState = 1;
        }
        public void Update(GameTime gameTime, KeyboardState curKState, KeyboardState prevKState)
        {
            if (pointerState < 2 && curKState.IsKeyDown(Keys.Down) && !prevKState.IsKeyDown(Keys.Down))
            {
                pointerPosition.Y += 60;
                pointerState++;
            }
            if (pointerState > 1 && curKState.IsKeyDown(Keys.Up) && !prevKState.IsKeyDown(Keys.Up))
            {
                pointerPosition.Y -= 60;
                pointerState--;
            }
        }
    }
}