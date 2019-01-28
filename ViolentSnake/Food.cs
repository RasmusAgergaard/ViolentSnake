using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViolentSnake
{
    public class Food
    {
        private static Random random = new Random();

        //Public int class
        public float x { get; set; }
        public float y { get; set; }

        public Food()
        {
            int gridSize = 20;

            int gridX = random.Next(20, 480) / gridSize;
            int gridY = random.Next(20, 480) / gridSize;
            x = gridX * gridSize;
            y = gridY * gridSize;
        }
    }
}
