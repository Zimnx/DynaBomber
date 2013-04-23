using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DynaBomber
{
    /// <summary>
    /// Class responsible for managing single game
    /// </summary>
    class DynGame : DynIDrawable
    {

        //Game elements
        List<DynPlayer> players; //List of players
        List<DynMonster> monsters; //List of monsters
        List<DynBomb> bombs; //List of bombs
        List<DynExplosion> explosions; //List of explosions

        DynBoard board;

        //Textures, graphics and animations
        List<Texture2D> deathTextures;
        List<Texture2D> playerTextures;
        List<Texture2D> monsterTextures;
        List<Texture2D> monsterDeathTextures;
        Texture2D bombTexture;
        Texture2D bombExplosionTexture;
        Texture2D explosionETexture;
        Texture2D wallExplosionTexture;
        Texture2D imTile;
        Texture2D movTile;
        Texture2D grass;
        Texture2D explosionBonusTexture;
        Texture2D bombBonusTexture;

        List<Vector2> playersStartingPositions;

        //Game options
        int boardSize; //should be an odd number
        int playerNumber;
        int monsterNumber;
        string settingsFileName;
        //-------------

        /// <summary>
        /// Loads all graphics etc
        /// </summary>
        public void LoadContent(ContentManager Content)
        {

            bombTexture = Content.Load<Texture2D>("env/bomb");
            bombExplosionTexture = Content.Load<Texture2D>("env/bombExplosion");
            explosionETexture = Content.Load<Texture2D>("env/Explosion");
            wallExplosionTexture = Content.Load<Texture2D>("env/wallExplosion");

            imTile = Content.Load<Texture2D>("env/imTile");
            movTile = Content.Load<Texture2D>("env/movTile");
            grass = Content.Load<Texture2D>("env/bGrass");

            for (int i = 1; i <= 4; ++i)
            {
                playerTextures.Add(Content.Load<Texture2D>("player/player" + i + "Animation"));
                deathTextures.Add(Content.Load<Texture2D>("player/player" + i + "Death"));

                if (i < 3)
                {
                    monsterTextures.Add(Content.Load<Texture2D>("env/monster_" + i));
                    monsterDeathTextures.Add(Content.Load<Texture2D>("env/monster_" + i + "_d"));
                }
            }

            bombBonusTexture = Content.Load<Texture2D>("env/bombBonus");
            explosionBonusTexture = Content.Load<Texture2D>("env/explBonus");

            board.Initialize(boardSize, boardSize, imTile, grass, movTile, explosionETexture, wallExplosionTexture, bombBonusTexture, explosionBonusTexture);
        }

        /// <summary>
        /// Will begin a new game
        /// </summary>
        public DynGame(DynGameOptions gameOptions, ContentManager Content)
        {
            setGameOptions(gameOptions);
            //-----------------------------------------------
            //Create lists 
            playerTextures = new List<Texture2D>();
            deathTextures = new List<Texture2D>();
            players = new List<DynPlayer>();
            monsters = new List<DynMonster>();
            bombs = new List<DynBomb>();
            explosions = new List<DynExplosion>();
            monsterTextures = new List<Texture2D>();
            monsterDeathTextures = new List<Texture2D>();

            //-----------------------------
            //Create objects
            board = new DynBoard();

            playersStartingPositions = new List<Vector2>(); //Initial positions for players
            playersStartingPositions.Add(new Vector2(10, 10));
            playersStartingPositions.Add(new Vector2(boardSize * 16 - 10, 10));
            playersStartingPositions.Add(new Vector2(10, boardSize * 16 - 10));
            playersStartingPositions.Add(new Vector2(boardSize * 16 - 10, boardSize * 16 - 10));

            LoadContent(Content);
            StartGame();
        }

        public void StartGame()
        {
            board.Reset();
            //Add monsters
            monsters.Clear();
            for (int i = 0; i < monsterNumber; ++i)
                monsters.Add(new DynMonster());

            for (int i = 0; i < monsterNumber; ++i)
            {
                Vector2 monsterPosition = new Vector2(15, 15);
                while (board.Tiles[(int)monsterPosition.X, (int)monsterPosition.Y] != DynBoard.TileType.EMPTY)
                {
                    monsterPosition.X = DynUtils.Rand(10, boardSize - 3);
                    monsterPosition.Y = DynUtils.Rand(10, boardSize - 3);

                }
                monsterPosition *= 16;
                monsterPosition.X += 8;
                monsterPosition.Y += 8;


                int random = DynUtils.Rand(0, 2);
                bool ghost = random == 0 ? true : false;
                monsters[i].Initialize(monsterPosition, monsterDeathTextures[random], monsterTextures[random], i, ghost);
            }
            /////////////////////////////////////////////
            bombs.Clear();

            //Clear and place players
            players.Clear();
            for (int i = 0; i < playerNumber; ++i)
            {
                players.Add(new DynPlayer());
                players[i].Initialize(playerTextures[i], deathTextures[i], playersStartingPositions[i], i);
                players[i].playerKeys.SetKeys(settingsFileName, (i + 1));
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            board.Draw(spriteBatch);
            foreach (DynBomb b in bombs)
                b.Draw(spriteBatch);
            foreach (DynPlayer p in players)
                p.Draw(spriteBatch);
            foreach (DynMonster m in monsters)
                m.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime, KeyboardState prevKState, KeyboardState curKState)
        {
            //Updating bombs
            int j = bombs.Count;
            for (int i = 0; i < j; ++i)
            {
                DynBomb b = bombs[i];
                b.Update(gameTime);
                if (!b.active)
                {
                    players[b.OwnerId].bombsLeft += 1;
                    bombs.RemoveAt(i);
                    --j;
                    --i;
                }
                if (b.running)
                {
                    board.ExplodeBomb(b);
                    b.running = false;
                }
            }

            //Updating players
            foreach (DynPlayer p in players)
            {
                if (p.alive)
                    p.Move(curKState, board.CollisionList, bombs);
                p.Update(gameTime);
                Vector2 coords = DynUtils.GetTileCoords(p.position);
                //Placing bomb
                if (p.alive &&
                    curKState.IsKeyDown(p.playerKeys.KeyBomb) && (board.GetTile((int)coords.X, (int)coords.Y) != DynBoard.TileType.BOMB)) 
                {
                    if (p.bombsLeft > 0)
                    {
                        DynBomb bomb = new DynBomb();
                        bomb.Initialize(p.playerID, p.bombPower, DynUtils.GetTileCenter(p.position), bombTexture, bombExplosionTexture);
                        bombs.Add(bomb);
                        board.SetTile((int)coords.X, (int)coords.Y, DynBoard.TileType.BOMB);
                        --p.bombsLeft;
                    }
                }

            }

            foreach (DynMonster m in monsters)
            {
                if (m.alive)
                    m.Move(board.CollisionList);
                m.Update(gameTime);
            }

            //Collisions checks

            //player <-> ()
            foreach (DynPlayer p in players)
            {
                foreach (DynExplosion d in board.explosionList)  //Player <-> Bomb
                    if (d.Collides(p))
                    {
                        bool is_wall = false;
                        foreach (DynCAnimation we in board.wallExplosionList)
                            if (p.Collides(we))
                                is_wall = true;
                        if (!is_wall)
                            p.OnCollision(DynUtils.ObjectType.Explosion);
                    }

                foreach (DynMonster m in monsters) // Player <-> Monster
                    if (m.alive && m.animation.CollisionRect().Intersects(p.CollisionRect()))
                        p.OnCollision(DynUtils.ObjectType.Explosion);

                foreach (DynBonus b in board.bonuses) //Player <-> Bonus
                    if (b.Collides(p) && b.active)
                    {
                        p.OnCollision(b.type);
                        b.OnCollision(DynUtils.ObjectType.Player);
                    }
            }

            foreach (DynMonster m1 in monsters)
                foreach (DynMonster m2 in monsters)
                    if (m1.monsterID != m2.monsterID && m1.Collides(m2))
                    {
                        m1.OnCollision(DynUtils.ObjectType.Monster);
                        m2.OnCollision(DynUtils.ObjectType.Monster);
                    }
            //explosion <-> ()
            foreach (DynExplosion e in board.explosionList)
            {
                foreach (DynBomb b in bombs)
                    if (e.Collides(b))
                        b.OnCollision(DynUtils.ObjectType.Explosion);
                foreach (DynMonster m in monsters)
                    if (e.Collides(m))
                        m.OnCollision(DynUtils.ObjectType.Explosion);
                foreach (DynBonus b in board.bonuses)
                    if (e.Collides(b))
                        b.OnCollision(DynUtils.ObjectType.Explosion);
            }

            board.Update(gameTime);
        }

        public void setGameOptions(DynGameOptions gameOptions)
        {
            playerNumber = gameOptions.playerNumber;
            monsterNumber = gameOptions.monsterNumber;
            boardSize = gameOptions.boardSize;
            settingsFileName = gameOptions.settingsFileName;
        }

        /// <summary>
        /// Returns ID of winning player or 0 at draw, -1 at not finished game
        /// </summary>
        public int GetResult()
        {
            int pCount = 0;
            int won = 0;
            foreach (DynPlayer p in players)
                if (p.alive)
                {
                    ++pCount;
                    won = p.playerID;
                }
                else
                    if (p.animation.Active)
                        ++pCount;
            if (pCount > 1)
                return -1;
            else
                if (pCount == 0) //draw
                    return 0;
                else
                    return won + 1;
        }
    }
}

