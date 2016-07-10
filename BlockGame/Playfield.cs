﻿using System;
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

    public class GameBlock
    {
        // eventually this will also contain what group the block belongs to
        // but for now there's only the landed group

        public readonly GameColor Color;
        public int Row { get; internal set; }
        public int Column { get; internal set; }
        public bool Swapping { get; internal set; }

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

        internal void SetBlockSwapping(GameBlock underlyingBlock)
        {
            // find out if any existing blocks are swapping
            GameBlock preExistingBlock = null;
            foreach (var gb in blocks)
            {
                if (gb.Swapping)
                {
                    preExistingBlock = gb;
                }
            }
            
            if (preExistingBlock == null)
            {
                // If not, mark the block as swapping
                underlyingBlock.Swapping = true;
            }
            else
            {
                // If so, swap the blocks!
                int tempRow = preExistingBlock.Row;
                int tempCol = preExistingBlock.Column;
                preExistingBlock.Row = underlyingBlock.Row;
                preExistingBlock.Column = underlyingBlock.Column;
                underlyingBlock.Row = tempRow;
                underlyingBlock.Column = tempCol;
                preExistingBlock.Swapping = false;
            }
        }
    }
}
