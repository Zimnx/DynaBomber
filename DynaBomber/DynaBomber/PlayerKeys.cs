using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace DynaBomber
{
    public class PlayerKeys
    {
        Keys Up;
        Keys Down;
        Keys Left;
        Keys Right;
        Keys Bomb;

        FileManager fileManager = new FileManager();

        public Keys KeyUp
        {
            get { return Up; }
            set { Up = value; }
        }
        public Keys KeyDown
        {
            get { return Down; }
            set { Down = value; }
        }
        public Keys KeyLeft
        {
            get { return Left; }
            set { Left = value; }
        }
        public Keys KeyRight
        {
            get { return Right; }
            set { Right = value; }
        }
        public Keys KeyBomb
        {
            get { return Bomb; }
            set { Bomb = value; }
        }
        public void SetKeys(string fileName, int playerID)
        {
            fileManager.iniFile(fileName);
            KeyUp = Recognize(fileManager.GetString("Player" + playerID + "Keys", "Up", "(none)"));
            KeyDown = Recognize(fileManager.GetString("Player" + playerID + "Keys", "Down", "(none)"));
            KeyLeft = Recognize(fileManager.GetString("Player" + playerID + "Keys", "Left", "(none)"));
            KeyRight = Recognize(fileManager.GetString("Player" + playerID + "Keys", "Right", "(none)"));
            KeyBomb = Recognize(fileManager.GetString("Player" + playerID + "Keys", "Bomb", "(none)"));
        }
        private Keys Recognize(string key)
        {
            switch (key)
            {
                case "W":
                    return Keys.W;
                case "S":
                    return Keys.S;
                case "A":
                    return Keys.A;
                case "D":
                    return Keys.D;
                case "G":
                    return Keys.G;
                case "Q":
                    return Keys.Q;
                case "F":
                    return Keys.F;
                case "Up":
                    return Keys.Up;
                case "Down":
                    return Keys.Down;
                case "Left":
                    return Keys.Left;
                case "Right":
                    return Keys.Right;
                case "Space":
                    return Keys.Space;
                default:
                    return Keys.Escape;
            }
        }
    }
}
