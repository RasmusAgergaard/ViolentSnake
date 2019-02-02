using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViolentSnake
{
    public class Wall
    {
        private static Random random = new Random();

        //Public int class
        public float x { get; set; }
        public float y { get; set; }

        //Place the wall random in the grid
        public Wall(int gridSize)
        {
            int randomX = random.Next(1, 24);
            int randomY = random.Next(1, 24);
            x = randomX * gridSize;
            y = randomY * gridSize;
        }
    }
}
