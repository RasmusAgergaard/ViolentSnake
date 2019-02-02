using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViolentSnake
{
    public class SnakeBody
    {
        //Init
        private static Random random = new Random();
        public float x { get; set; }
        public float y { get; set; }
        public float snakeSpeed { get; set; }
        public float moveTimer { get; set; }
        public enum snakeDirection { up, down, left, right } 
        public snakeDirection currentDirection { get; set; }

        //Constructor
        public SnakeBody()
        {
            x = 240;
            y = 240;
        }

        //Makes sure the snake only moves at the given rate
        public void MoveSnake(GameTime gameTime, List<SnakeBody> snake, float timeBetweenMoves)
        {
            moveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (moveTimer >= timeBetweenMoves)
            {
                MoveSnakeBodys(snake);
                MoveSnakeHead(snake);
                moveTimer -= timeBetweenMoves;
            }
        }

        //Sets the snake direction based on keyboard input
        public void ChangeSnakeDirection()
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Up) && currentDirection != snakeDirection.down)
            {
                currentDirection = snakeDirection.up;
            }

            if (keyState.IsKeyDown(Keys.Down) && currentDirection != snakeDirection.up)
            {
                currentDirection = snakeDirection.down;
            }

            if (keyState.IsKeyDown(Keys.Left) && currentDirection != snakeDirection.right)
            {
                currentDirection = snakeDirection.left;
            }

            if (keyState.IsKeyDown(Keys.Right) && currentDirection != snakeDirection.left)
            {
                currentDirection = snakeDirection.right;
            }
        }

        //Moves the Snakehead in the selected direction
        public void MoveSnakeHead(List<SnakeBody> snake)
        {
            switch (currentDirection)
            {
                case snakeDirection.up:
                    snake[0].y -= snakeSpeed;
                    break;

                case snakeDirection.down:
                    snake[0].y += snakeSpeed;
                    break;

                case snakeDirection.left:
                    snake[0].x -= snakeSpeed;
                    break;

                case snakeDirection.right:
                    snake[0].x += snakeSpeed;
                    break;
            }
        }

        //Loop though the snake list, and move the elements
        public void MoveSnakeBodys(List<SnakeBody> snake)
        {
            for (int i = snake.Count - 1; i > 0; i--)
            {
                snake[i].x = snake[i - 1].x;
                snake[i].y = snake[i - 1].y;
            }
        }
    }
}
