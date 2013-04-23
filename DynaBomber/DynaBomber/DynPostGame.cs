using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DynaBomber
{
    class DynPostGame : DynIDrawable
    {
        Texture2D background;
        Texture2D pointer;
        SpriteFont font;
        DynCAnimation bannerAnimation = new DynCAnimation();
        public Vector2 pointerPosition;
        public int pointerState; // which option in menu f.e 1 - start game , 2- settings etc.
        List<int> playersScores;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "REVENGE!", new Vector2(35 * 16 / 2 - 70, 35 * 16 / 2 + 60), Color.White);

            spriteBatch.DrawString(font, "QUIT FOR NOW...", new Vector2(35 * 16 / 2 - 70, 35 * 16 / 2 + 120), Color.White);
            spriteBatch.Draw(pointer, pointerPosition, Color.White);

            string result = "";
            for (int i = 0; i < playersScores.Count; ++i)
                result += "PLAYER " + (i + 1) + " : " + playersScores[i] + "\n";
            spriteBatch.DrawString(font, result, new Vector2(10 * 16 / 2 - 70, 2 * 16 / 2 + 60), Color.White);

        }

        public void Initialize(Texture2D Background, Texture2D Pointer, SpriteFont font, List<int> playersScores)
        {
            this.font = font;
            background = Background;
            pointer = Pointer;
            pointerPosition = new Vector2(35 * 16 / 2 - 90, 35 * 16 / 2 + 60);
            pointerState = 1;
            this.playersScores = playersScores;
        }

        public void SetScores(List<int> newScores)
        {
            playersScores = newScores;
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