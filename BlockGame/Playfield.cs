using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockGame
{
    public enum GameColor
    {
        Empty,
        Red,
        White,
        Blue,
        MaxValue
    }

    public class GameBlock
    {
        // eventually this will also contain what group the block belongs to
        // but for now there's only the landed group

        public readonly GameColor Color;
        public int Row { get; private set; }
        public int Column { get; private set; }

        public GameBlock(int row, int column, GameColor color)
        {
            this.Row = row;
            this.Column = column;
            this.Color = color;
        }
    }
    
    public class Playfield
    {
        public const int HEIGHT = 6;
        public const int WIDTH = 5;

        // Use a list because we want to be able to swap or move to other
        // groups more easily. A grid would be OK if the blocks were never going
        // to have gravity, etc.
        public List<GameBlock> blocks = new List<GameBlock>();
        private Random rand = new Random();

        public void GenerateBlocks()
        {
            for (int r = 0; r < HEIGHT; r++)
            {
                for (int c = 0; c < WIDTH; c++)
                {
                    blocks.Add(
                        new GameBlock(r, c, (GameColor) rand.Next((int)GameColor.Red, (int)GameColor.MaxValue)));
                }
            }
        }
    }
}
