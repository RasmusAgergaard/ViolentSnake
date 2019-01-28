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

        //Textures
        Texture2D TextureBG;
        Texture2D TextureSnakePart;
        Texture2D TextureFoodPart;
        Texture2D TextureFoodShadowPart;
        Texture2D TextureWallPart;
        Texture2D TextureWallPartShadow;

        //Score
        SpriteFont font;
        int currentScore;
        int bestScore;

        //Snake
        private List<SnakeBody> Snake;
        float SnakeSpeed;
        float MoveTimer;
        float TimeBetweenMoves;
        enum SnakeDirection {up, down, left, right };
        int CurrentDirection;

        //Food
        Food SnakeFood;
        float FoodAngle;

        //Wall
        private List<Wall> Walls;

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

        private void CreateWalls()
        {
            Walls = new List<Wall>();

            for (int i = 0; i < 10; i++)
            {
                Wall wall = new Wall();
                Walls.Add(wall);
            }
        }

        private void CreateFood()
        {
            SnakeFood = new Food();

            Random random = new Random();
            FoodAngle = random.Next(0, 360);
        }

        private void Init()
        {
            MoveTimer = 0f;
            TimeBetweenMoves = 0.15f;
            SnakeSpeed = 20f;
            CurrentDirection = (int)SnakeDirection.up;
            currentScore = 0;
        }

        private void CreateSnake()
        {
            Snake = new List<SnakeBody>();
            for (int i = 0; i < 3; i++)
            {
                SnakeBody body = new SnakeBody();
                Snake.Add(body);

                if (i != 0)
                {
                    Snake[i].y = Snake[i - 1].y + 20;
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
            TextureBG = Content.Load<Texture2D>("background_desert");
            TextureSnakePart = Content.Load<Texture2D>("snake");
            TextureFoodPart = Content.Load<Texture2D>("sheep");
            TextureFoodShadowPart = Content.Load<Texture2D>("sheep_shadow");
            TextureWallPart = Content.Load<Texture2D>("wall");
            TextureWallPartShadow = Content.Load<Texture2D>("wall_shadow");

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

            //Update
            base.Update(gameTime);
        }

        private void CollisionCheck()
        {
            // If Snakehead and food is in the same location
            if (Snake[0].x == SnakeFood.x && Snake[0].y == SnakeFood.y)
            {
                MoveFood();
                AddSnakeBody();
            }

            //Walls
            for (int i = 0; i < Walls.Count; i++)
            {
                // If Snakehead collides with walls
                if (Snake[0].x == Walls[i].x && Snake[0].y == Walls[i].y)
                {
                    Die();
                }

                // If food collide with walls
                if (SnakeFood.x == Walls[i].x && SnakeFood.y == Walls[i].y)
                {
                    MoveFood();
                }
            }

            //Leaving game area
            if (Snake[0].x < 0 || Snake[0].x > 500)
            {
                Die();
            }

            if (Snake[0].y < 0 || Snake[0].y > 500)
            {
                Die();
            }

        }

        private void AddSnakeBody()
        {
            SnakeBody body = new SnakeBody();
            Snake.Add(body);
            int snakeLenght = Snake.Count - 1;

            Snake[snakeLenght].x = Snake[0].x;
            Snake[snakeLenght].y = Snake[0].y;

            //Add to score
            currentScore = currentScore + 1;
        }

        private void MoveFood()
        {
            Random random = new Random();
            int gridSize = 20;
            int gridX = random.Next(20, 480) / gridSize;
            int gridY = random.Next(20, 480) / gridSize;
            SnakeFood.x = gridX * gridSize;
            SnakeFood.y = gridY * gridSize;

            FoodAngle = random.Next(0, 360);

        }

        private void MoveSnake(GameTime gameTime)
        {
            MoveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (MoveTimer >= TimeBetweenMoves)
            {
                MoveSnakeBodys();
                MoveSnakeHead();
                MoveTimer -= TimeBetweenMoves;
            }
        }

        private void ChangeSnakeDirection()
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Up) && CurrentDirection != (int)SnakeDirection.down)
            {
                CurrentDirection = (int)SnakeDirection.up;
            }

            if (keyState.IsKeyDown(Keys.Down) && CurrentDirection != (int)SnakeDirection.up)
            {
                CurrentDirection = (int)SnakeDirection.down;
            }

            if (keyState.IsKeyDown(Keys.Left) && CurrentDirection != (int)SnakeDirection.right)
            {
                CurrentDirection = (int)SnakeDirection.left;
            }

            if (keyState.IsKeyDown(Keys.Right) && CurrentDirection != (int)SnakeDirection.left)
            {
                CurrentDirection = (int)SnakeDirection.right;
            }
        }

        private void MoveSnakeHead()
        {
            switch (CurrentDirection)
            {
                case (int)SnakeDirection.up:
                    Snake[0].y -= SnakeSpeed;
                    break;

                case (int)SnakeDirection.down:
                    Snake[0].y += SnakeSpeed;
                    break;

                case (int)SnakeDirection.left:
                    Snake[0].x -= SnakeSpeed;
                    break;

                case (int)SnakeDirection.right:
                    Snake[0].x += SnakeSpeed;
                    break;
            }
        }

        private void MoveSnakeBodys()
        {
            for (int i = Snake.Count - 1; i > 0; i--)
            {
                Snake[i].x = Snake[i - 1].x;
                Snake[i].y = Snake[i - 1].y;
            }
        }

        private void Die()
        {
            SetBestScore();
            CreateSnake();
            CreateFood();
            CreateWalls();
            Init();
        }

        private void SetBestScore()
        {
            //If the current score is better than the best, update it
            if (currentScore > bestScore)
            {
                bestScore = currentScore;
            }
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //Draw Background
            Vector2 DrawBG = new Vector2(0,0);
            spriteBatch.Draw(TextureBG, DrawBG, null, Color.White, 0f, new Vector2(0, 0), Vector2.One, SpriteEffects.None, 0f);

            //Draw snake
            for (int i = 0; i < Snake.Count; i++)
            {
                Vector2 DrawBody = new Vector2(Snake[i].x, Snake[i].y);
                spriteBatch.Draw(TextureSnakePart, DrawBody, null, Color.White, 0f, new Vector2(TextureSnakePart.Width / 2, TextureSnakePart.Height / 2), Vector2.One, SpriteEffects.None, 0f);
            }

            //Draw food
            Vector2 DrawFood = new Vector2(SnakeFood.x, SnakeFood.y);
            Vector2 DrawFoodShadow = new Vector2(SnakeFood.x - 3, SnakeFood.y + 3);
            spriteBatch.Draw(TextureFoodShadowPart, DrawFoodShadow, null, Color.White, FoodAngle, new Vector2(TextureFoodShadowPart.Width / 2, TextureFoodShadowPart.Height / 2), Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureFoodPart, DrawFood, null, Color.White, FoodAngle, new Vector2(TextureFoodPart.Width / 2, TextureFoodPart.Height / 2), Vector2.One, SpriteEffects.None, 0f);

            //Draw wall
            for (int i = 0; i < Walls.Count; i++)
            {
                Vector2 DrawWall = new Vector2(Walls[i].x, Walls[i].y);
                Vector2 DrawWallShadow = new Vector2(Walls[i].x-3, Walls[i].y+3);
                spriteBatch.Draw(TextureWallPartShadow, DrawWallShadow, null, Color.White, 0f, new Vector2(TextureWallPartShadow.Width / 2, TextureWallPartShadow.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureWallPart, DrawWall, null, Color.White, 0f, new Vector2(TextureWallPart.Width / 2, TextureWallPart.Height / 2), Vector2.One, SpriteEffects.None, 0f);
            }

            //Draw score
            spriteBatch.DrawString(font, "Score: " + currentScore, new Vector2(20, 20), Color.Black);

            if (bestScore > 0)
            {
                spriteBatch.DrawString(font, "Best: " + bestScore, new Vector2(20, 40), Color.Black);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
