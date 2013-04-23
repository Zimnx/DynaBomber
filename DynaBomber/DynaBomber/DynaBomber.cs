using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DynaBomber
{

    public class DynaBomber : Microsoft.Xna.Framework.Game
    {
        //Variables for settings and implementation
        SpriteFont font;
        FileManager fileManager = new FileManager();
        string settingsFileName = ".\\settings.ini";
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch; // kontener obrazkow do rysowania 
        KeyboardState curKState; //Current Keyboard State
        KeyboardState prevKState; //Previous Keyboard State

        //interface and game
        DynMenu menu;
        DynSettings settings;
        DynGame game;
        DynGameOptions gameOptions;
        DynPauseMenu pauseMenu;
        DynPostGame postGame;

        List<int> playersScores = new List<int>();
        int playerNumber;

        public DynaBomber()
        {
            fileManager.iniFile(settingsFileName); //Initialization of file with settings
            gameOptions = new DynGameOptions( //Loads options to gameOptions object
                        Int32.Parse(fileManager.GetString("Game", "BoardSize", "(none)")),
                        Int32.Parse(fileManager.GetString("Game", "PlayerNumber", "(none)")),
                        Int32.Parse(fileManager.GetString("Game", "MonsterNumber", "(none)"))
                        );

            //Graphics initialization
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = gameOptions.boardSize * 16;
            graphics.PreferredBackBufferHeight = gameOptions.boardSize * 16;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent() //LoadContent will be called once per game and is the place to load all of your content.
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("5px2bus");
            Texture2D bannerTexture = Content.Load<Texture2D>("DynMenu/banner");
            Texture2D pointerTexture = Content.Load<Texture2D>("DynMenu/pointer");
            Texture2D backgroundTexture = Content.Load<Texture2D>("DynMenu/background");

            menu = new DynMenu();
            menu.Initialize(backgroundTexture, pointerTexture, bannerTexture, font);

            settings = new DynSettings();
            settings.Initialize(backgroundTexture, pointerTexture, font, settingsFileName);

            pauseMenu = new DynPauseMenu();
            pauseMenu.Initialize(backgroundTexture, pointerTexture, font);

            game = new DynGame(gameOptions, Content);


            postGame = new DynPostGame();
            postGame.Initialize(backgroundTexture, pointerTexture, font, playersScores);


            for (int i = 0; i < gameOptions.playerNumber; ++i)
                playersScores.Add(0);
        }


        protected override void Initialize()
        {
            DynUtils.SetGameState(DynUtils.GameStates.Menu);
            DynUtils.ChangeSettingsState(DynUtils.SettingsStates.Choice);
            base.Initialize(); // Calling base.Initialize will enumerate through any components and initialize them as well.
        }


        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions or gathering input
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            prevKState = curKState;
            curKState = Keyboard.GetState();

            //Update depending on current game state (menus/pause/game)
            switch (DynUtils.GameState)
            {
                //Main menu
                case DynUtils.GameStates.Menu:
                    //Change menu pointer
                    if (curKState.IsKeyDown(Keys.Enter) && !prevKState.IsKeyDown(Keys.Enter))
                    {
                        switch (menu.pointerState)
                        {
                            case 1:
                                DynUtils.SetGameState(DynUtils.GameStates.Game);
                                game.StartGame();
                                break;
                            case 2:
                                DynUtils.SetGameState(DynUtils.GameStates.Settings);
                                DynUtils.ChangeSettingsState(DynUtils.SettingsStates.Choice);
                                break;
                            case 3:
                                this.Exit();
                                break;
                        }
                    }
                    //Update menu
                    menu.Update(gameTime, curKState, prevKState);
                    break;

                //Settings menu
                case DynUtils.GameStates.Settings:
                    //Catching keys to configure
                    //Not working
                    if (DynUtils.SettingState == DynUtils.SettingsStates.Capturing)
                    {
                        settings.CatchKeys(settings.pointerState - 1);
                        if (settings.capturedKeys.Count == 5)
                        {
                            settings.SaveKeys(settings.pointerState - 1);
                            DynUtils.ChangeSettingsState(DynUtils.SettingsStates.Choice);
                            settings.capturedKeys.Clear();
                        }
                    }

                    //Change menu pointer / display player numbers
                    if (DynUtils.SettingState == DynUtils.SettingsStates.Choice)
                    {
                        if (settings.pointerState == 1 && playerNumber > 1 && curKState.IsKeyDown(Keys.Left) && !prevKState.IsKeyDown(Keys.Left))
                        {
                            playerNumber--;
                            fileManager.WriteString("Game", "PlayerNumber", playerNumber.ToString());
                        }
                        else if (settings.pointerState == 1 && playerNumber < 4 && curKState.IsKeyDown(Keys.Right) && !prevKState.IsKeyDown(Keys.Right))
                        {
                            playerNumber++;
                            fileManager.WriteString("Game", "PlayerNumber", playerNumber.ToString());
                        }
                        else if (curKState.IsKeyDown(Keys.Enter) && !prevKState.IsKeyDown(Keys.Enter))
                        {
                            if (settings.pointerState == 6)
                            {
                                DynUtils.SetGameState(DynUtils.GameStates.Menu);
                            }
                            else if (settings.pointerState < 5 && settings.pointerState > 1)
                                DynUtils.ChangeSettingsState(DynUtils.SettingsStates.Capturing);
                        }
                    }
                    //Update settings meni
                    settings.Update(curKState, prevKState);
                    break;

                //Paused game
                case DynUtils.GameStates.Pause:
                    //Continue game on ESC
                    if (curKState.IsKeyDown(Keys.Escape) && !prevKState.IsKeyDown(Keys.Escape))
                        DynUtils.SetGameState(DynUtils.GameStates.Game);
                    //Continue / Quit game on Enter
                    if (curKState.IsKeyDown(Keys.Enter))
                        if (pauseMenu.pointerState == 1)
                            DynUtils.SetGameState(DynUtils.GameStates.Game);
                        else
                            DynUtils.SetGameState(DynUtils.GameStates.Menu);
                    //Update paused menu
                    pauseMenu.Update(gameTime, curKState, prevKState);
                    break;

                //Game
                case DynUtils.GameStates.Game:
                    //Switch pause
                    if (curKState.IsKeyDown(Keys.Escape) && !prevKState.IsKeyDown(Keys.Escape))
                    {
                        DynUtils.SetGameState(DynUtils.GameStates.Pause);
                        break;
                    }
                    //Update game
                    game.Update(gameTime, prevKState, curKState);

                    //Grab end of game
                    if (game.GetResult() > -1)
                    {
                        //If not draw
                        if (game.GetResult() > 0)
                            //Add points to player
                            playersScores[game.GetResult() - 1] += 1;
                        postGame.SetScores(playersScores);
                        //Set state to post-game
                        DynUtils.SetGameState(DynUtils.GameStates.PostGame);
                    }
                    break;

                //Post-game
                case DynUtils.GameStates.PostGame:
                    //Continue or end game
                    if (curKState.IsKeyDown(Keys.Enter))
                        if (postGame.pointerState == 1)
                        {
                            DynUtils.SetGameState(DynUtils.GameStates.Game);
                            game.StartGame();
                        }
                        else if (postGame.pointerState == 2)
                        {
                            Reset();
                            postGame.SetScores(playersScores);
                            DynUtils.SetGameState(DynUtils.GameStates.Menu);
                        }
                    //Update post-game menu
                    postGame.Update(gameTime, curKState, prevKState);
                    break;
            }
            base.Update(gameTime);
        }

        public void Reset()
        {   
            playersScores.Clear();
            for (int i = 0; i < playerNumber; ++i)
                playersScores.Add(0);
        }

        /// <summary>
        /// Draws what is necessary
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            switch (DynUtils.GameState)
            {
                case (DynUtils.GameStates.Menu):
                menu.Draw(spriteBatch);
                break;

                case (DynUtils.GameStates.Game):
                game.Draw(spriteBatch);
                break;

                case (DynUtils.GameStates.Pause):
                game.Draw(spriteBatch);
                pauseMenu.Draw(spriteBatch);
                break;

                case(DynUtils.GameStates.Settings):
                settings.Draw(spriteBatch);
                break;

                case (DynUtils.GameStates.PostGame):
                postGame.Draw(spriteBatch);
                break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
