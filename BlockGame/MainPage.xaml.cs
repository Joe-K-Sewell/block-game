using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BlockGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private class VisualBlock
        {
            public GameBlock UnderlyingBlock { get; private set; }
            
            public VisualBlock(GameBlock gb)
            {
                UnderlyingBlock = gb;
            }

            private Color FillColor
            {
                get
                {
                    switch (UnderlyingBlock.Color)
                    {
                        case GameColor.Empty:
                            return Colors.Black;
                        case GameColor.Red:
                            return Colors.Red;
                        case GameColor.White:
                            return Colors.White;
                        case GameColor.Blue:
                            return Colors.Blue;
                        case GameColor.Brown:
                            return Colors.BurlyWood;
                        case GameColor.Green:
                            return Colors.Green;
                        case GameColor.Yellow:
                            return Colors.Yellow;
                    }
                    throw new NotImplementedException();
                }
            }

            public static Vector2 GridOrigin = new Vector2(100, 100);
            public static Vector2 OutlineSize = new Vector2(100, 100);
            public static float FillDiameter = 85;
            public static Vector2 FillVector = new Vector2(FillDiameter, FillDiameter);

            private Vector2 Center
            {
                get
                {
                    return
                        GridOrigin + 
                        (OutlineSize * new Vector2(UnderlyingBlock.Column, UnderlyingBlock.Row));
                }
            }

            private Vector2 OutlineOrigin
            {
                get
                {
                    return Center - (OutlineSize / 2.0f);
                }
            }

            private Rect OutlineRect
            {
                get
                {
                    return new Rect(OutlineOrigin.ToPoint(), OutlineSize.ToSize());
                }
            }

            private Vector2 FillableOrigin
            {
                get { return Center - (FillVector / 2.0f); }
            }

            private Rect FillableRect
            {
                get
                {
                    return new Rect(FillableOrigin.ToPoint(), FillVector.ToSize());
                }
            }
            
            public bool PointHit(Point p)
            {
                return OutlineRect.Contains(p);
            }

            public void DrawGridline(CanvasDrawingSession session)
            {
                session.DrawRectangle(OutlineRect, Colors.White, 1);
            }

            public void DrawAtGrid(CanvasDrawingSession session)
            {
                session.FillRectangle(FillableRect, FillColor);
                session.DrawRectangle(FillableRect, Colors.Gray, 2);
            }

            public void DrawHighlightedOutline(CanvasDrawingSession session)
            {
                session.DrawRectangle(OutlineRect, Colors.Aqua, 5);
            }

            public void DrawAtHolp(CanvasDrawingSession session, Vector2 holp)
            {
                var origin = holp - (FillVector / 2.0f);
                var movingRect = new Rect(origin.ToPoint(), FillVector.ToSize());
                session.FillRectangle(movingRect, FillColor);
                session.DrawRectangle(movingRect, Colors.Gray, 2);
            }

            public Vector2 BottomRight()
            {
                return new Vector2((float) OutlineRect.Right, (float) OutlineRect.Bottom);
            }
        }

        Playfield field;
        List<VisualBlock> visualBlocks
        {
            get
            {
                return field.Blocks.Select(gb => new VisualBlock(gb)).ToList();
            }
        }
        Vector2 HeldObjectsLastPosition = Vector2.Zero;
        Vector2 TextPosition;

        public MainPage()
        {
            this.InitializeComponent();

            field = new Playfield();
            field.GenerateBlocks();
        }

        private void canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            var lowerRight = new VisualBlock(field.BottomRightBlock).BottomRight();
            TextPosition = new Vector2(10, lowerRight.Y + 10);
            sender.MinHeight = sender.ConvertDipsToPixels(TextPosition.Y + VisualBlock.OutlineSize.Y, CanvasDpiRounding.Ceiling);
            sender.MinWidth = sender.ConvertDipsToPixels(lowerRight.X, CanvasDpiRounding.Ceiling);
        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {

        }

        private void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var session = args.DrawingSession;
            
            // Draw grid
            foreach (var vb in visualBlocks)
            {
                vb.DrawGridline(session);
                if (vb.UnderlyingBlock != field.BlockHeld)
                {
                    vb.DrawAtGrid(session);
                }
            }

            // Draw extra outline
            var target = GetVisualBlock(field.BlockDest);
            if (target != null)
            {
                target.DrawHighlightedOutline(session);
            }

            // Draw held block
            var held = GetVisualBlock(field.BlockHeld);
            if (held != null)
            {
                held.DrawAtHolp(session, HeldObjectsLastPosition);
            }

#if DEBUG
            // Draw debug info
            args.DrawingSession.DrawText("Held: " + field.BlockHeld + " Target: " + field.BlockDest, new Vector2(0,0), Colors.LightGreen);
            args.DrawingSession.DrawText("HOLP: " + HeldObjectsLastPosition.ToString(), new Vector2(0, 25), Colors.LightGreen);
#endif

            // Draw text
            session.DrawText("Moves used: " + field.MovesUsed, TextPosition, Colors.White);
            session.DrawText("Solved: " + field.IsSolved, TextPosition + new Vector2(0, 50), Colors.White);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }

        private VisualBlock GetVisualBlock(GameBlock gb)
        {
            if (gb == null) { return null; }
            return visualBlocks.Single(vb => vb.UnderlyingBlock == gb);
        }

        private VisualBlock GetBlockUnder(Point p)
        {
            foreach (var b in visualBlocks)
            {
                if (b.PointHit(p))
                {
                    return b;
                }
            }
            return null;
        }

        private void canvas_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas).Position;
            var blockUnder = GetBlockUnder(point);
            
            if (blockUnder == null) { return; }
            field.HoldBlock(blockUnder.UnderlyingBlock);

            HeldObjectsLastPosition = point.ToVector2();
        }
        
        private void canvas_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas).Position;
            if (field.BlockHeld != null)
            {
                HeldObjectsLastPosition = point.ToVector2();
                var blockUnder = GetBlockUnder(point);
                if (blockUnder != null)
                {
                    field.TargetBlock(blockUnder.UnderlyingBlock);
                }
            }
        }

        private void canvas_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            field.ReleaseBlock();
        }
    }
}
