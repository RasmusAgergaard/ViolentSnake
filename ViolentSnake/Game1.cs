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
        Texture2D TextureWallPart;

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

        /********** Methods **********/
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
            TextureFoodPart = Content.Load<Texture2D>("food");
            TextureWallPart = Content.Load<Texture2D>("wall");

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

            //Update
            base.Update(gameTime);
        }

        /********** Methods **********/
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
        }

        private void RotateFood()
        {
            FoodAngle = FoodAngle + 0.01f;
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
            //Clear the screen
            GraphicsDevice.Clear(Color.White);

            //Colors
            Color shadowColor = Color.Lerp(Color.Black, Color.Transparent, 0.6f);

            //Start of draw
            spriteBatch.Begin();

            //Draw Background
            DrawSprite(250f, 250f, TextureBG, 0f);

            //Draw food
            DrawSprite(SnakeFood.x, SnakeFood.y, TextureFoodPart, FoodAngle, shadowColor);

            //Draw snake
            for (int i = 0; i < Snake.Count; i++)
            {
                DrawSprite(Snake[i].x, Snake[i].y, TextureSnakePart, 0f, shadowColor);
            }

            //Draw wall
            for (int i = 0; i < Walls.Count; i++)
            {
                DrawSprite(Walls[i].x, Walls[i].y, TextureWallPart, 0f, shadowColor);
            }

            //Draw score
            spriteBatch.DrawString(font, "Score: " + currentScore, new Vector2(20, 20), Color.Black);

            if (bestScore > 0)
            {
                spriteBatch.DrawString(font, "Best: " + bestScore, new Vector2(20, 40), Color.Black);
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
