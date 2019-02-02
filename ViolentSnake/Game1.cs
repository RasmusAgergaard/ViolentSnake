using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ViolentSnake
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Level settings
        int gridSize;

        //Texture items
        Texture2D textureBG;
        Texture2D textureDeath;
        Texture2D textureSnakePart;
        Texture2D textureFoodPart;
        Texture2D textureWallPart;

        //Score
        SpriteFont font;
        int currentScore;
        int bestScore;

        //Snake
        public List<SnakeBody> snake;
        public float timeBetweenMoves;

        //Food
        Food snakeFood;

        //Wall
        private List<Wall> walls;

        //Death splash
        bool drawDeathSplash;
        int drawDeathSplashTime;
        int drawDeathSplashTimer;

        //Constructor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 500;    // Width of window
            graphics.PreferredBackBufferHeight = 500;   // Height of window
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            SetupLevel();
            CreateSnake();
            CreateFood();
            CreateWalls();
            Init();

            bestScore = 0; //This is separate, so not to update it upon death

            base.Initialize();
        }

        
        /********** Methods **********/

        //Create random walls, and add them to the wall list
        private void CreateWalls()
        {
            walls = new List<Wall>();

            for (int i = 0; i < 10; i++)
            {
                Wall wall = new Wall(gridSize);
                walls.Add(wall);
            }
        }

        //Create random food
        private void CreateFood()
        {
            snakeFood = new Food(gridSize);

            Random random = new Random();
        }

        //Create snake elements, and add them to the snake list
        private void CreateSnake()
        {
            snake = new List<SnakeBody>();
            for (int i = 0; i < 3; i++)
            {
                SnakeBody body = new SnakeBody();
                snake.Add(body);

                if (i != 0)
                {
                    snake[i].y = snake[i - 1].y + 20;
                }
            }
        }

        //Used to both set and reset upon death
        private void Init()
        {
            snake[0].moveTimer = 0f;
            timeBetweenMoves = 0.15f;
            snake[0].snakeSpeed = 20f;
            //snake[0].currentDirection = snake[0].snakeDirection.up;
            currentScore = 0;

            //Death splash
            drawDeathSplash = false;
            drawDeathSplashTime = 20;
            drawDeathSplashTimer = drawDeathSplashTime;
        }

        //Set level settings
        private void SetupLevel()
        {
            gridSize = 20;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            textureBG = Content.Load<Texture2D>("background_desert");
            textureDeath = Content.Load<Texture2D>("death");
            textureSnakePart = Content.Load<Texture2D>("snake");
            textureFoodPart = Content.Load<Texture2D>("food");
            textureWallPart = Content.Load<Texture2D>("wall");

            font = Content.Load<SpriteFont>("Score");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Exit on ESC
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // TODO: Add your update logic here
            snake[0].ChangeSnakeDirection();
            snake[0].MoveSnake(gameTime, snake, timeBetweenMoves);
            CollisionCheck();
            snakeFood.RotateFood();
            ShowOrHideDeathSplash();

            //Update
            base.Update(gameTime);
        }

        /********** Methods **********/

        //All collision is handled here
        private void CollisionCheck()
        {
            //Snakehead VS Food collision - Reduce time between moves, and move the food
            if (snake[0].x == snakeFood.x && snake[0].y == snakeFood.y)
            {
                timeBetweenMoves = snakeFood.MoveFood(timeBetweenMoves, gridSize);
                AddSnakeBody();
            }

            //Loop though walls
            for (int i = 0; i < walls.Count; i++)
            {
                //Snakehead VS Walls - Death is all around us
                if (snake[0].x == walls[i].x && snake[0].y == walls[i].y)
                {
                    Die();
                }

                //Food VS Walls - Replace food, so it's not unreachable
                if (snakeFood.x == walls[i].x && snakeFood.y == walls[i].y)
                {
                    snakeFood.MoveFood(timeBetweenMoves, gridSize);
                }
            }

            //Snakehead leaving game area
            if (snake[0].x < 20 || snake[0].x > 480 || snake[0].y < 20 || snake[0].y > 480) 
            {
                Die();
            }

            //Snakehead VS Snakebody - Die! Break added as the snake.count is reset upon death, and that causes the list to fail
            for (int i = snake.Count - 1; i > 0; i--)
            {
                if (snake[0].x == snake[i].x && snake[0].y == snake[i].y)
                {
                    Die();
                    break;
                }
            }
        }

        //Add a new snake element to the list and increase the score
        private void AddSnakeBody()
        {
            SnakeBody body = new SnakeBody();
            snake.Add(body);
            int snakeLenght = snake.Count - 1;

            snake[snakeLenght].x = snake[1].x;
            snake[snakeLenght].y = snake[1].y;

            //Add to score
            currentScore = currentScore + 1;
        }

        //Death, resert all stuff, and display the death-splash
        private void Die()
        {
            SetBestScore();
            CreateSnake();
            CreateFood();
            CreateWalls();
            Init();

            drawDeathSplash = true;
        }

        //If the current score is better than the best, update it
        private void SetBestScore()
        {
            if (currentScore > bestScore)
            {
                bestScore = currentScore;
            }
        }

        //Only shows the death-splash for a short amount
        private void ShowOrHideDeathSplash()
        {
            //If the splash is drawn
            if (drawDeathSplash == true)
            {
                //Decrease timer
                drawDeathSplashTimer = drawDeathSplashTimer - 1;

                //Hide splash and reset timer
                if (drawDeathSplashTimer <= 0)
                {
                    drawDeathSplashTimer = drawDeathSplashTime;
                    drawDeathSplash = false;
                }
            }


        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Clear the screen
            GraphicsDevice.Clear(Color.White);

            //Colors
            Color shadowColor = Color.Lerp(Color.Black, Color.Transparent, 0.6f);

            //Start of draw
            spriteBatch.Begin();

            //Draw Background
            DrawSprite(250f, 250f, textureBG, 0f);

            //Draw food
            DrawSprite(snakeFood.x, snakeFood.y, textureFoodPart, snakeFood.foodAngle, shadowColor);

            //Draw snake
            for (int i = 0; i < snake.Count; i++)
            {
                DrawSprite(snake[i].x, snake[i].y, textureSnakePart, 0f, shadowColor);
            }

            //Draw wall
            for (int i = 0; i < walls.Count; i++)
            {
                DrawSprite(walls[i].x, walls[i].y, textureWallPart, 0f, shadowColor);
            }

            //Draw score
            spriteBatch.DrawString(font, "Score: " + currentScore, new Vector2(20, 20), Color.Black);
        
            if (bestScore > 0)
            {
                spriteBatch.DrawString(font, "Best: " + bestScore, new Vector2(20, 40), Color.Black);
            }

            //Draw death splash
            if (drawDeathSplash == true)
            {
                DrawSprite(250f, 250f, textureDeath, 0f);
            }

            //End of draw
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /********** Draw Methods **********/

        //Standard draw
        private void DrawSprite(float xpos, float ypos, Texture2D texture, float angle)
        {
            Vector2 DrawSprite = new Vector2(xpos, ypos);
            spriteBatch.Draw(texture, DrawSprite, null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
        }
        //Draw with shadow
        private void DrawSprite(float xpos, float ypos, Texture2D texture, float angle, Color shadowColor)
        {
            Vector2 DrawSpriteShadow = new Vector2(xpos - 3, ypos + 3);
            Vector2 DrawSprite = new Vector2(xpos, ypos);
            spriteBatch.Draw(texture, DrawSpriteShadow, null, shadowColor, angle, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, DrawSprite, null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
        }
    }
}
