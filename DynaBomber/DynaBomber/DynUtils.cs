using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynaBomber
{
    //Interfaces
    public interface DynIDrawable
    {
        void Draw(SpriteBatch spriteBatch);
    }
    public interface DynICollidable
    {
        Rectangle CollisionRect();
        bool Collides(DynICollidable o);
        void OnCollision(DynUtils.ObjectType c);
    }



    /// <summary>
    /// Class for common methods etc
    /// </summary>
    public static class DynUtils
    {
        public enum ObjectType { Player, Monster, BombBonus, Bomb, ExplosionBonus, MovableWall, ImmovableWall, Explosion };
        public enum GameStates { Menu, Settings, PreGame, Game, PostGame, Pause };
        public enum SettingsStates { Capturing, Choice };

        private static Random Random = new Random();

        public static GameStates GameState; //Current game state
        public static SettingsStates SettingState;

        public static float bonusProbability = 0.35f;

        public static int explosionTime = 1200;

        /// <summary>
        /// Gives vector of pixels in the centre of tile at coords (coords are [1..boardSize, 1..boardSize], ie. (4,5). GetTileCenter returns pixels in the centre of (4,5) piece)
        /// </summary>
        public static Vector2 GetTileCenter(Vector2 coords)
        {
            Vector2 rv = new Vector2();
            rv.X = (((int)coords.X) / 16) * 16 + 8;
            rv.Y = (((int)coords.Y) / 16) * 16 + 8;
            return rv;
        }

        /// <summary>
        /// Gets coordinates of tile at pixels given
        /// </summary>
        public static Vector2 GetTileCoords(Vector2 coords)
        {
            Vector2 rv = new Vector2();
            rv.X = (int)coords.X / 16;
            rv.Y = (int)coords.Y / 16;
            return rv;
        }

        public static int Rand(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static float RandDouble()
        {
            return (float)Random.NextDouble();
        }

        /// <summary>
        /// Sets current game state
        /// </summary>
        public static void SetGameState(GameStates NewGameState)
        {
            GameState = NewGameState;
        }

        /// <summary>
        /// Sets currents settings state
        /// </summary>
        public static void ChangeSettingsState(SettingsStates NewSettingsState)
        {
            SettingState = NewSettingsState;
        }
    }
}
