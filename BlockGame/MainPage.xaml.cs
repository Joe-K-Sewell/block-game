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

            private static Vector2 GridOrigin = new Vector2(150, 150);
            private static Vector2 OutlineSize = new Vector2(100, 100);
            private static float FillDiameter = 80;
            private static Vector2 FillVector = new Vector2(FillDiameter, FillDiameter);

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
        }

        Playfield field;
        List<VisualBlock> blocks
        {
            get
            {
                return field.blocks.Select(gb => new VisualBlock(gb)).ToList();
            }
        }
        string debugString = "Debug String Initial Value";
        Vector2 HeldObjectsLastPosition = Vector2.Zero;

        public MainPage()
        {
            this.InitializeComponent();

            field = new Playfield();
            field.GenerateBlocks();
        }

        private void canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {

        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {

        }

        private void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var session = args.DrawingSession;
            
            // Draw grid
            foreach (var vb in blocks)
            {
                vb.DrawGridline(session);
                if (vb.UnderlyingBlock != field.HeldBlock)
                {
                    vb.DrawAtGrid(session);
                }
            }

            // Draw extra outline
            var target = GetVisualBlock(field.TargetBlock);
            if (target != null)
            {
                target.DrawHighlightedOutline(session);
            }

            // Draw held block
            var held = GetVisualBlock(field.HeldBlock);
            if (held != null)
            {
                held.DrawAtHolp(session, HeldObjectsLastPosition);
            }

            // Draw debug info
            args.DrawingSession.DrawText("Held: " + field.HeldBlock + " Target: " + field.TargetBlock, new Vector2(0,0), Colors.LightGreen);
            args.DrawingSession.DrawText("HOLP: " + HeldObjectsLastPosition.ToString(), new Vector2(0, 25), Colors.LightGreen);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }

        private VisualBlock GetVisualBlock(GameBlock gb)
        {
            if (gb == null) { return null; }
            return blocks.Single(vb => vb.UnderlyingBlock == gb);
        }

        private VisualBlock GetBlockUnder(Point p)
        {
            foreach (var b in blocks)
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
            field.HeldBlock = GetBlockUnder(point).UnderlyingBlock;
            field.TargetBlock = field.HeldBlock;

            HeldObjectsLastPosition = point.ToVector2();
        }
        
        private void canvas_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas).Position;
            var blockUnder = GetBlockUnder(point);
            if (e.Pointer.IsInContact) // Good enough
            {
                HeldObjectsLastPosition = point.ToVector2();
                if (blockUnder != null)
                {
                    field.TargetBlock = blockUnder.UnderlyingBlock;
                }
            }
        }

        private void canvas_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas).Position;
            field.SwapBlocks();
        }
    }
}
