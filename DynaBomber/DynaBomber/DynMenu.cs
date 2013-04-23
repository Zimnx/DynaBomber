using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DynaBomber
{
    class DynMenu : DynIDrawable
    {
        Texture2D background;
        Texture2D pointer;
        Texture2D banner;
        SpriteFont font;
        DynCAnimation bannerAnimation = new DynCAnimation();
        public Vector2 pointerPosition;
        public int pointerState; // which option in menu f.e 1 - start game , 2- settings etc.
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            bannerAnimation.Draw(spriteBatch);
            spriteBatch.DrawString(font, "START GAME", new Vector2(35 * 16 / 2 - 70, 35 * 16 / 2 + 60), Color.White);
            spriteBatch.DrawString(font, "SETTINGS", new Vector2(35 * 16 / 2 - 70, 35 * 16 / 2 + 90), Color.White);
            spriteBatch.DrawString(font, "EXIT GAME", new Vector2(35 * 16 / 2 - 70, 35 * 16 / 2 + 120), Color.White);
            spriteBatch.Draw(pointer, pointerPosition, Color.White);
        }
        public void Initialize(Texture2D Background, Texture2D Pointer, Texture2D Banner, SpriteFont font)
        {
            this.font = font;
            background = Background;
            pointer = Pointer;
            banner = Banner;
            bannerAnimation.Initialize(Banner, new Vector2(300, 0), 263, 71, 1, 500, true);
            pointerPosition = new Vector2(35 * 16 / 2 - 90, 35 * 16 / 2 + 60);
            pointerState = 1;
        }
        public void Update(GameTime gameTime, KeyboardState curKState, KeyboardState prevKState)
        {
            if (pointerState < 3 && curKState.IsKeyDown(Keys.Down) && !prevKState.IsKeyDown(Keys.Down))
            {
                pointerPosition.Y += 30;
                pointerState++;
            }
            if (pointerState > 1 && curKState.IsKeyDown(Keys.Up) && !prevKState.IsKeyDown(Keys.Up))
            {
                pointerPosition.Y -= 30;
                pointerState--;
            }

            if (bannerAnimation.Position.Y <= 35 * 16 / 2 - 150)
                bannerAnimation.Position.Y += 1f;
            bannerAnimation.Update(gameTime);
        }
    }
}