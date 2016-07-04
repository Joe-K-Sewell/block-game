using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockGame
{
    public enum BlockTypes
    {
        Empty,
        Red,
        White,
        Blue,
        MaxValue
    }

    public class Playfield
    {
        public const int HEIGHT = 6;
        public const int WIDTH = 5;
        public BlockTypes[,] Grid = new BlockTypes[HEIGHT, WIDTH];
        private Random rand = new Random();

        public void GenerateBlocks()
        {
            for (int r = 0; r < HEIGHT; r++)
            {
                for (int c = 0; c < WIDTH; c++)
                {
                    Grid[r, c] = (BlockTypes) rand.Next((int)BlockTypes.Red, (int)BlockTypes.MaxValue);
                }
            }
        }

        public BlockTypes BlockAt(int row, int column)
        {
            return Grid[row, column];
        }


    }
}
