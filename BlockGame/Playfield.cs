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
        Green,
        Yellow,
        Brown,
        MaxValue
    }

    internal class GameBlock
    {
        // eventually this will also contain what group the block belongs to
        // but for now there's only the landed group

        public readonly GameColor Color;
        public int Row { get; internal set; }
        public int Column { get; internal set; }

        public GameBlock(int row, int column, GameColor color)
        {
            this.Row = row;
            this.Column = column;
            this.Color = color;
        }

        public override string ToString()
        {
            return "[" + Row + "," + Column + "] (" + Color + ")";
        }
    }
    
    internal class Playfield
    {
        public const int HEIGHT = 6;
        public const int WIDTH = 5;

        // Use a list because we want to be able to swap or move to other
        // groups more easily. A grid would be OK if the blocks were never going
        // to have gravity, etc.
        private List<GameBlock> blocks = new List<GameBlock>();
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
        
        public Int32 MovesUsed { get; private set; }
        public GameBlock BlockHeld { get; private set; }
        public GameBlock BlockDest { get; private set; }
        public GameBlock BottomRightBlock
        {
            get
            {
                return blocks.Single(b => b.Row == HEIGHT - 1 && b.Column == WIDTH - 1);
            }
        }
        public IEnumerable<GameBlock> Blocks
        {
            get
            {
                return blocks;
            }
        }

        public void HoldBlock(GameBlock underlyingBlock)
        {
            BlockHeld = underlyingBlock;
            BlockDest = underlyingBlock;
        }

        public void TargetBlock(GameBlock underlyingBlock)
        {
            BlockDest = underlyingBlock;
        }

        public void ReleaseBlock()
        {
            if (BlockHeld != null && BlockDest != null && BlockHeld != BlockDest)
            {
                int tempRow = BlockHeld.Row;
                int tempCol = BlockHeld.Column;
                BlockHeld.Row = BlockDest.Row;
                BlockHeld.Column = BlockDest.Column;
                BlockDest.Row = tempRow;
                BlockDest.Column = tempCol;
                MovesUsed++;
            }
            
            BlockHeld = null;
            BlockDest = null;   
        }

    }
}
