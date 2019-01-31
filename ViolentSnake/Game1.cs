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
        private List<SnakeBody> snake;
        float snakeSpeed;
        float moveTimer;
        float timeBetweenMoves;
        enum snakeDirection {up, down, left, right };
        int currentDirection;

        //Food
        Food snakeFood;
        float foodAngle;

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
            CreateSnake();
            CreateFood();
            CreateWalls();
            Init();

            bestScore = 0; //This is separate, so not to update it upon death

            base.Initialize();
        }

        /********** Methods **********/
        private void CreateWalls()
        {
            walls = new List<Wall>();

            for (int i = 0; i < 10; i++)
            {
                Wall wall = new Wall();
                walls.Add(wall);
            }
        }

        private void CreateFood()
        {
            snakeFood = new Food();

            Random random = new Random();
        }

        private void Init()
        {
            moveTimer = 0f;
            timeBetweenMoves = 0.15f;
            snakeSpeed = 20f;
            currentDirection = (int)snakeDirection.up;
            currentScore = 0;

            //Death splash
            drawDeathSplash = false;
            drawDeathSplashTime = 20;
            drawDeathSplashTimer = drawDeathSplashTime;
        }

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
            ChangeSnakeDirection();
            MoveSnake(gameTime);
            CollisionCheck();
            RotateFood();
            ShowOrHideDeathSplash();

            //Update
            base.Update(gameTime);
        }

        /********** Methods **********/
        private void CollisionCheck()
        {
            //Food
            if (snake[0].x == snakeFood.x && snake[0].y == snakeFood.y)
            {
                MoveFood();
                AddSnakeBody();
            }

            //Walls
            for (int i = 0; i < walls.Count; i++)
            {
                // If Snakehead collides with walls
                if (snake[0].x == walls[i].x && snake[0].y == walls[i].y)
                {
                    Die();
                }

                // If food collide with walls
                if (snakeFood.x == walls[i].x && snakeFood.y == walls[i].y)
                {
                    MoveFood();
                }
            }

            //Leaving game area
            if (snake[0].x < 20 || snake[0].x > 480)
            {
                Die();
            }

            if (snake[0].y < 20 || snake[0].y > 480)
            {
                Die();
            }

            //Snake head vs snake body
            for (int i = snake.Count - 1; i > 0; i--)
            {
                if (snake[0].x == snake[i].x && snake[0].y == snake[i].y)
                {
                    Die();
                    break;
                }
            }
        }

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

        private void MoveFood()
        {
            //Move food
            Random random = new Random();
            int gridSize = 20;
            int gridX = random.Next(20, 480) / gridSize;
            int gridY = random.Next(20, 480) / gridSize;
            snakeFood.x = gridX * gridSize;
            snakeFood.y = gridY * gridSize;

            //Reduce timer and make the game more difficult
            timeBetweenMoves = timeBetweenMoves - 0.0025f;
        }

        private void RotateFood()
        {
            foodAngle = foodAngle + 0.01f;
        }

        private void MoveSnake(GameTime gameTime)
        {
            moveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (moveTimer >= timeBetweenMoves)
            {
                MoveSnakeBodys();
                MoveSnakeHead();
                moveTimer -= timeBetweenMoves;
            }
        }

        private void ChangeSnakeDirection()
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Up) && currentDirection != (int)snakeDirection.down)
            {
                currentDirection = (int)snakeDirection.up;
            }

            if (keyState.IsKeyDown(Keys.Down) && currentDirection != (int)snakeDirection.up)
            {
                currentDirection = (int)snakeDirection.down;
            }

            if (keyState.IsKeyDown(Keys.Left) && currentDirection != (int)snakeDirection.right)
            {
                currentDirection = (int)snakeDirection.left;
            }

            if (keyState.IsKeyDown(Keys.Right) && currentDirection != (int)snakeDirection.left)
            {
                currentDirection = (int)snakeDirection.right;
            }
        }

        private void MoveSnakeHead()
        {
            switch (currentDirection)
            {
                case (int)snakeDirection.up:
                    snake[0].y -= snakeSpeed;
                    break;

                case (int)snakeDirection.down:
                    snake[0].y += snakeSpeed;
                    break;

                case (int)snakeDirection.left:
                    snake[0].x -= snakeSpeed;
                    break;

                case (int)snakeDirection.right:
                    snake[0].x += snakeSpeed;
                    break;
            }
        }

        private void MoveSnakeBodys()
        {
            for (int i = snake.Count - 1; i > 0; i--)
            {
                snake[i].x = snake[i - 1].x;
                snake[i].y = snake[i - 1].y;
            }
        }

        private void Die()
        {
            //Reset game
            SetBestScore();
            CreateSnake();
            CreateFood();
            CreateWalls();
            Init();

            //Show splash
            drawDeathSplash = true;
        }

        private void SetBestScore()
        {
            //If the current score is better than the best, update it
            if (currentScore > bestScore)
            {
                bestScore = currentScore;
            }
        }

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
            DrawSprite(snakeFood.x, snakeFood.y, textureFoodPart, foodAngle, shadowColor);

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

        /********** Methods **********/
        private void DrawSprite(float xpos, float ypos, Texture2D texture, float angle)
        {
            Vector2 DrawSprite = new Vector2(xpos, ypos);
            spriteBatch.Draw(texture, DrawSprite, null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
        }
        private void DrawSprite(float xpos, float ypos, Texture2D texture, float angle, Color shadowColor)
        {
            Vector2 DrawSpriteShadow = new Vector2(xpos - 3, ypos + 3);
            Vector2 DrawSprite = new Vector2(xpos, ypos);
            spriteBatch.Draw(texture, DrawSpriteShadow, null, shadowColor, angle, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, DrawSprite, null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
        }
    }
}
