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

            public void DrawInGrid(CanvasDrawingSession session)
            {
                // Draw outline
                if (UnderlyingBlock.Swapping)
                {
                    session.DrawRectangle(OutlineRect, Colors.Aqua, 5);
                }
                else
                {
                    session.DrawRectangle(OutlineRect, Colors.White, 1);
                }

                // Draw filled square
                if (!UnderlyingBlock.Swapping)
                {
                    session.FillRectangle(FillableRect, FillColor);
                    session.DrawRectangle(FillableRect, Colors.Gray, 2);
                }
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
            foreach (var vb in blocks)
            {
                vb.DrawInGrid(args.DrawingSession);
            }
            var held = blocks.SingleOrDefault(b => b.UnderlyingBlock.Swapping);
            if (held != null)
            {
                held.DrawAtHolp(args.DrawingSession, HeldObjectsLastPosition);
            }
            args.DrawingSession.DrawText(debugString, new Vector2(0, 0), Colors.LightGreen);
            args.DrawingSession.DrawText("HOLP: " + HeldObjectsLastPosition.ToString(), new Vector2(0, 25), Colors.LightGreen);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }

        private void canvas_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas).Position;
            debugString = String.Format("Pressed at {0},{1}", point.X, point.Y);

            foreach (var vb in blocks)
            {
                if (vb.PointHit(point))
                {
                    field.SetSourceBlock(vb.UnderlyingBlock);
                    HeldObjectsLastPosition = point.ToVector2();
                    return;
                }
            }

            field.ResetBlockSwap();
        }

        private void canvas_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas).Position;
            debugString = String.Format("Released at {0},{1}", point.X, point.Y);

            foreach (var vb in blocks)
            {
                if (vb.PointHit(point))
                {
                    field.SetDestinationBlock(vb.UnderlyingBlock);
                    return;
                }
            }

            field.ResetBlockSwap();
        }

        private void canvas_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas).Position;
            if (e.Pointer.IsInContact) // Good enough
            {
                HeldObjectsLastPosition = point.ToVector2();
            }
        }
    }
}
