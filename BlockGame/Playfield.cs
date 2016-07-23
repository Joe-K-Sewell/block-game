using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //Blue,
        //Green,
        //Yellow,
        //Brown,
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

        internal bool IsAdjacentTo(GameBlock other)
        {
            int diff;
            if (this.Row == other.Row)
            {
                diff = this.Column - other.Column;
            }
            else if (this.Column == other.Column)
            {
                diff = this.Row - other.Row;
            }
            else
            {
                return false;
            }

            return Math.Abs(diff) == 1;
        }

        public override bool Equals(object obj)
        {
            var other = obj as GameBlock;
            if (other != null)
            {
                return this.Column == other.Column
                    && this.Row == other.Row
                    && this.Color == other.Color;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Column + 7 * Row;
        }
    }
    
    internal class Playfield
    {
        public const int HEIGHT = 3;
        public const int WIDTH = 3;

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
                CachedSolveValue = null;
            }
            
            BlockHeld = null;
            BlockDest = null;   
        }

        private bool? CachedSolveValue = null;

        public bool IsSolved
        {
            get
            {
                if (CachedSolveValue.HasValue)
                {
                    return CachedSolveValue.Value;
                }

                foreach (GameColor color in Enum.GetValues(typeof(GameColor)))
                {
                    if (!IsColorSolved(color))
                    {
                        CachedSolveValue = false;
                        return false;
                    }
                }
                CachedSolveValue = true;
                return true;
            }
        }

        private bool IsColorSolved(GameColor color)
        {
            // A color is solved if each block can reach every other block of the same
            // color by travelling vertically or horizontally only over its own colors
            if (color == GameColor.Empty || color == GameColor.MaxValue)
            {
                return true;
            }

            var blocksOfThisColor = blocks.Where(b => b.Color == color);

            foreach (var source in blocksOfThisColor)
            {
                foreach (var dest in blocksOfThisColor)
                {
                    var isPath = IsPathBetween(blocksOfThisColor, source, dest);
                    if (!isPath) { return false; }
                }
            }

            return true;
        }

        private class TraversedNode
        {
            internal GameBlock block;
            internal bool wasDiscovered;

            public TraversedNode(GameBlock b)
            {
                block = b;
                wasDiscovered = false;
            }

            public override string ToString()
            {
                return block.ToString() + (wasDiscovered ? "!" : ".");
            }
        }

        private bool IsPathBetween(IEnumerable<GameBlock> blocks, GameBlock source, GameBlock dest)
        {
            var graph = blocks.Select(b => new TraversedNode(b)).ToList();
            Debug.WriteLine("IsPathBetween {0} {1}", source, dest);
            LogGraph(graph);

            var stack = new Stack<TraversedNode>();
            stack.Push(graph.Single(n => n.block == source));

            while (stack.Count != 0)
            {
                var cur = stack.Pop();
                Debug.WriteLine(" Popped {0}", cur);
                if (!cur.wasDiscovered && !stack.Contains(cur))
                {
                    cur.wasDiscovered = true;
                    LogGraph(graph);

                    if (cur.block == dest) { return true; }

                    foreach (var next in graph.Where(n => n.block.IsAdjacentTo(cur.block)))
                    {
                        if (!next.wasDiscovered)
                        {
                            Debug.WriteLine(" Pushed {0}", next);
                            stack.Push(next);
                        }
                    }
                }
            }

            return false;
        }

        private void LogGraph(IEnumerable<TraversedNode> graph)
        {
            foreach (var node in graph)
            {
                Debug.WriteLine(node);
            }
        }
    }
}
