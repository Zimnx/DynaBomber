using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynaBomber
{
    class DynBoard : DynIDrawable
    {
        int sizeX; // tiles horizontally
        int sizeY;

        public enum TileType { MOVABLE, IMMOVABLE, EMPTY, EXPLOSION, BOMB };
        public TileType[,] Tiles; // Board in array (access by coordinates, not pixels)

        Texture2D wallExplosionTexture;
        Texture2D immovableTileTexture;
        Texture2D emptyTileTexture;
        Texture2D movableTileTexture;
        Texture2D explosionTexture;
        Texture2D bombBonusTexture;
        Texture2D explosionBonusTexture;

        public List<Rectangle> CollisionList = new List<Rectangle>();
        public List<DynExplosion> explosionList = new List<DynExplosion>();
        public List<DynCAnimation> wallExplosionList = new List<DynCAnimation>();
        public List<DynBonus> bonuses = new List<DynBonus>();


        //Resets board for new game
        public void Reset()
        {
            CollisionList = new List<Rectangle>();

            explosionList = new List<DynExplosion>();
            wallExplosionList = new List<DynCAnimation>();

            bonuses = new List<DynBonus>();

            for (int i = 0; i < sizeX; ++i)
                for (int j = 0; j < sizeY; ++j)
                    if (i % 2 != 0 && j % 2 != 0)
                    {
                        Tiles[i, j] = TileType.IMMOVABLE;
                    }
                    else if (DynUtils.RandDouble() > 0.7)
                    {
                        Tiles[i, j] = TileType.MOVABLE;
                    }
                    else
                        Tiles[i, j] = TileType.EMPTY;

            //Making room for players
            Tiles[0, 0] = Tiles[1, 0] = Tiles[0, 1] = TileType.EMPTY;
            Tiles[0, sizeY - 1] = Tiles[0, sizeY - 2] = Tiles[1, sizeY - 1] = TileType.EMPTY;
            Tiles[sizeX - 1, 0] = Tiles[sizeX - 2, 0] = Tiles[sizeX - 1, 1] = TileType.EMPTY;
            Tiles[sizeX - 1, sizeY - 1] = Tiles[sizeX - 2, sizeY - 1] = Tiles[sizeX - 1, sizeY - 2] = TileType.EMPTY;

            //Filling up collision list
            for (int i = 0; i < sizeY; ++i)
                for (int j = 0; j < sizeX; ++j)
                    if (Tiles[i, j] == TileType.MOVABLE || Tiles[i, j] == TileType.IMMOVABLE)
                        CollisionList.Add(new Rectangle((16 * i) + 3, (16 * j) + 3, 11, 10));
                    else
                        CollisionList.Add(new Rectangle(-5, -5, 0, 0));

            for (int i = 0; i <= sizeX; ++i)
            {
                CollisionList.Add(new Rectangle(-16 + 16 * i, -16, 16, 16));   // Obramowania poza plansza
                CollisionList.Add(new Rectangle(-16, 16 * i, 16, 16));      // zeby nie wychodzic
                CollisionList.Add(new Rectangle(16 * i, sizeY * 16, 16, 16));
                CollisionList.Add(new Rectangle(sizeX * 16, 16 * i, 16, 16));
            }
        }

        public void Initialize(int sizeX, int sizeY, Texture2D imTileTexture, Texture2D emTileTexture, Texture2D movTileTexture,
                               Texture2D explosionTexture, Texture2D wallExplosionTexture, Texture2D bombBonusTexture, Texture2D explosionBonusTexture)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            Tiles = new TileType[sizeX, sizeY];

            immovableTileTexture = imTileTexture;
            emptyTileTexture = emTileTexture;
            movableTileTexture = movTileTexture;
            this.explosionTexture = explosionTexture;
            this.wallExplosionTexture = wallExplosionTexture;
            this.explosionBonusTexture = explosionBonusTexture;
            this.bombBonusTexture = bombBonusTexture;

            Reset();
        }

        public void ExplodeBomb(DynBomb b)
        {
            int power = b.explosionSize;
            int[] Erange = { 0, 0, 0, 0 };  // explosion range in each direction     :
                                            //up,down,left,right
            bool[] Dir = { false, false, false, false }; // already hit wall in each direction

            Vector2 center = DynUtils.GetTileCoords(b.position);

            SetTile((int)center.X, (int)center.Y, TileType.EMPTY);

            for (int k = 1; k <= power; ++k)
            {
                Vector2[] vectors = 
                {
                    new Vector2(center.X, center.Y-k),
                    new Vector2(center.X, center.Y + k),
                    new Vector2(center.X - k, center.Y),
                    new Vector2(center.X + k, center.Y)
                };
                int index = 0;
                foreach (Vector2 v in vectors)
                {
                    if (!Dir[index] && v.X >= 0 && v.Y >= 0 && v.X <= sizeX - 1 && v.Y <= sizeY - 1)
                    {
                        if (Tiles[(int)v.X, (int)v.Y] == TileType.EMPTY)
                            Erange[index]++;
                        else
                        {
                            Dir[index] = true;
                            if (Tiles[(int)v.X, (int)v.Y] != TileType.IMMOVABLE)
                            {
                                DynCAnimation wallExplosion = new DynCAnimation();
                                int frameTime = (DynUtils.explosionTime) / 7;
                                wallExplosion.Initialize(wallExplosionTexture, new Vector2(v.X * 16 + 8, v.Y * 16 + 8), 16, 16, 7, frameTime, false);
                                wallExplosionList.Add(wallExplosion);
                                Erange[index]++;
                            }
                        }
                    }
                    index++;
                }
            }
            DynExplosion explosion = new DynExplosion();
            explosion.Initialize(b.position, Erange[0], Erange[1], Erange[2], Erange[3], explosionTexture, b.explosionTexture);
            explosionList.Add(explosion);
        }

        public void SetTile(int x, int y, TileType newType)
        {
            if (newType == TileType.EMPTY)
            {
                if (Tiles[x, y] == TileType.MOVABLE) //Place bonus
                {
                    float rand = DynUtils.RandDouble();
                    if (rand <= DynUtils.bonusProbability)
                    {
                        if (DynUtils.RandDouble() >= 0.5f)
                        {
                            DynBonus bonus = new DynBonus();
                            bonus.Initialize(explosionBonusTexture, new Vector2(16 * x, 16 * y), DynUtils.ObjectType.ExplosionBonus);
                            bonuses.Add(bonus);
                        }
                        else
                        {
                            DynBonus bonus = new DynBonus();
                            bonus.Initialize(bombBonusTexture, new Vector2(16 * x, 16 * y), DynUtils.ObjectType.BombBonus);
                            bonuses.Add(bonus);
                        }
                    }
                }
                CollisionList[sizeY * x + y] = new Rectangle(0, 0, 0, 0);
            }
            else
                CollisionList[sizeY * x + y] = new Rectangle((16 * x) + 3, (16 * y) + 3, 13, 13);

            Tiles[x, y] = newType;
        }

        public TileType GetTile(int x, int y)
        {
            return Tiles[x, y];
        }


        public void Update(GameTime gameTime)
        {
            List<DynExplosion> nexpls = new List<DynExplosion>(); //next explosions (here we delete inactive ones)
            foreach (DynExplosion e in explosionList)
            {
                e.Update(gameTime);
                if (e.active)
                    nexpls.Add(e);
            }
            explosionList = nexpls; //Zeby usunac z pamieci juz skonczone animacje

            List<DynCAnimation> newWallExplList = new List<DynCAnimation>(); //next wall explosion list
            foreach (DynCAnimation a in wallExplosionList)
            {
                a.Update(gameTime);
                if (a.Active)
                    newWallExplList.Add(a);
                else
                {
                    //TODO make sure bonuses show, wall disappears etc
                    Vector2 v = DynUtils.GetTileCoords(a.Position);
                    SetTile((int)v.X, (int)v.Y, TileType.EMPTY);
                }
            }
            wallExplosionList = newWallExplList; //Zeby usunac z pamieci juz skonczone animacje

            List<DynBonus> newBonuses = new List<DynBonus>();
            foreach (DynBonus b in bonuses)
            {
                b.Update(gameTime);
                if (b.active)
                    newBonuses.Add(b);
            }
            bonuses = newBonuses;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < sizeX; ++i)
                for (int j = 0; j < sizeY; ++j)
                {
                    TileType t = Tiles[i, j];
                    switch (t)
                    {
                        case TileType.IMMOVABLE:
                            spriteBatch.Draw(immovableTileTexture, new Rectangle(i * 16, j * 16, 16, 16), Color.White);
                            break;
                        case TileType.EMPTY:
                            spriteBatch.Draw(emptyTileTexture, new Rectangle(i * 16, j * 16, 16, 16), Color.White);
                            break;
                        case TileType.MOVABLE:
                            spriteBatch.Draw(movableTileTexture, new Rectangle(i * 16, j * 16, 16, 16), Color.White);
                            break;
                    }
                }
            foreach (DynExplosion e in explosionList)
                e.Draw(spriteBatch);
            foreach (DynCAnimation a in wallExplosionList)
                a.Draw(spriteBatch);
            foreach (DynBonus b in bonuses)
                b.Draw(spriteBatch);
        }
    }
}
