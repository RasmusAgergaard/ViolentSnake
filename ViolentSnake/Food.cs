using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViolentSnake
{
    public class Food
    {
        //Init
        private static Random random = new Random();
        public float x { get; set; }
        public float y { get; set; }
        public float foodAngle { get; set; }

        //Constructor
        public Food(int gridSize)
        {
            int randomX = random.Next(1, 24);
            int randomY = random.Next(1, 24);
            x = randomX * gridSize;
            y = randomY * gridSize;

            foodAngle = 0;
        }


        /********************************** Methods **********************************/

        //Moves food to a random position, and reduce timer and make the game more difficult 
        public float MoveFood(float timeBetweenMoves, int gridSize)
        {
            Random random = new Random();
            int randomX = random.Next(1, 24);
            int randomY = random.Next(1, 24);
            x = randomX * gridSize;
            y = randomY * gridSize;

            timeBetweenMoves = timeBetweenMoves - 0.0025f;
            return timeBetweenMoves;
        }

        //Well yeah... rotate the stuff
        public void RotateFood()
        {
            foodAngle = foodAngle + 0.01f;
        }
    }
}
