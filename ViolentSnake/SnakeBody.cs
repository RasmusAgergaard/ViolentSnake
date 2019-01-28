using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViolentSnake
{
    public class SnakeBody
    {
        public static Random random = new Random();

        //Public int class
        public float x { get; set; }
        public float y { get; set; }

        public SnakeBody()
        {
            x = 240;
            y = 240;
        }
    }
}
