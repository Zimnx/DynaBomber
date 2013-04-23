using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace DynaBomber
{
    class DynSettings : DynIDrawable
    {
        Texture2D backgroundTexture;
        Texture2D pointerTexture;
        Vector2 pointerPosition;
        FileManager fileManager;
        KeyboardState curKState; //Current Keyboard State
        KeyboardState prevKState; //Previous Keyboard State
        SpriteFont font;
        public List<Keys> capturedKeys;
        public int pointerState;
        public void CatchKeys(int PlayerID)
        {
            if (capturedKeys.Count == 1)
                capturedKeys.Remove(Keys.Enter);
            prevKState = curKState;
            curKState = Keyboard.GetState();
            Keys[] keys = curKState.GetPressedKeys();
            foreach (Keys k in keys)
                if (curKState.IsKeyDown(k) && !prevKState.IsKeyDown(k))
                    capturedKeys.Add(k);

        }
        public void SaveKeys(int PlayerID)
        {
            fileManager.WriteString("Player" + PlayerID + "Keys", "Up", capturedKeys[0].ToString());
            fileManager.WriteString("Player" + PlayerID + "Keys", "Down", capturedKeys[1].ToString());
            fileManager.WriteString("Player" + PlayerID + "Keys", "Left", capturedKeys[2].ToString());
            fileManager.WriteString("Player" + PlayerID + "Keys", "Right", capturedKeys[3].ToString());
            fileManager.WriteString("Player" + PlayerID + "Keys", "Bomb", capturedKeys[4].ToString());
            //foreach (Keys k in capturedKeys)
            //    Console.WriteLine(k.ToString());
        }
        public void Initialize(Texture2D BackgroundTexture, Texture2D PointerTexture, SpriteFont Font, string settingsFileName)
        {
            capturedKeys = new List<Keys>();
            backgroundTexture = BackgroundTexture;
            pointerTexture = PointerTexture;
            fileManager = new FileManager();
            fileManager.iniFile(settingsFileName);
            font = Font;
            pointerPosition = new Vector2(36 * 16 / 2 - 90, 35 * 16 / 2 + 30);
            pointerState = 1;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "PLAYER NUMBER :" + fileManager.GetString("Game", "PlayerNumber", "(none)"), new Vector2(36 * 16 / 2 - 70, 35 * 16 / 2 + 30), Color.White);
            spriteBatch.DrawString(font, "PLAYER 1 KEYS", new Vector2(36 * 16 / 2 - 70, 35 * 16 / 2 + 60), Color.White);
            spriteBatch.DrawString(font, "PLAYER 2 KEYS", new Vector2(36 * 16 / 2 - 70, 35 * 16 / 2 + 90), Color.White);
            spriteBatch.DrawString(font, "PLAYER 3 KEYS", new Vector2(36 * 16 / 2 - 70, 35 * 16 / 2 + 120), Color.White);
            spriteBatch.DrawString(font, "PLAYER 4 KEYS", new Vector2(36 * 16 / 2 - 70, 35 * 16 / 2 + 150), Color.White);
            spriteBatch.DrawString(font, "MENU", new Vector2(36 * 16 / 2 - 70, 35 * 16 / 2 + 180), Color.White);
            spriteBatch.Draw(pointerTexture, pointerPosition, Color.White);
        }
        public void Update(KeyboardState curKState, KeyboardState prevKState)
        {
            if (DynUtils.SettingState != DynUtils.SettingsStates.Capturing)
            {
                if (pointerState < 6 && curKState.IsKeyDown(Keys.Down) && !prevKState.IsKeyDown(Keys.Down))
                {
                    pointerPosition.Y += 30;
                    pointerState++;
                }
                if (pointerState > 1 && curKState.IsKeyDown(Keys.Up) && !prevKState.IsKeyDown(Keys.Up))
                {
                    pointerPosition.Y -= 30;
                    pointerState--;
                }
            }
        }
    }
}