using Microsoft.Graphics.Canvas;
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

            private Vector2 SpaceOrigin
            {
                get
                {
                    return gridOrigin
                        + (spaceWidth * UnderlyingBlock.Column)
                        + (spaceHeight * UnderlyingBlock.Row);
                }
            }

            private Rect MainRect
            {
                get
                {
                    return new Rect(
                        SpaceOrigin.X + spaceSize,
                        SpaceOrigin.Y + spaceSize,
                        blockWidth.X - spaceSize,
                        blockHeight.Y - spaceSize);
                }
            }

            private Rect BackRect
            {
                get
                {
                    return new Rect(SpaceOrigin.X, SpaceOrigin.Y,
                        spaceWidth.X, spaceHeight.Y);
                }
            }

            public bool PointInMainRect(Point p)
            {
                return MainRect.Contains(p);
            }

            public void Draw(CanvasDrawingSession session)
            {
                if (UnderlyingBlock.Swapping)
                {
                    session.DrawRectangle(BackRect, Colors.Aqua, 2);
                }
                session.DrawRectangle(MainRect, FillColor, strokeSize);
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

        static readonly Vector2 gridOrigin = new Vector2(50, 60);
        static readonly Vector2 blockWidth = new Vector2(50, 0);
        static readonly Vector2 spaceWidth = new Vector2(60, 0);
        static readonly Vector2 blockHeight = new Vector2(0, 50);
        static readonly Vector2 spaceHeight = new Vector2(0, 60);
        static readonly float spaceSize = blockWidth.X / 2.0f; // denom can't be > 2
        static readonly float strokeSize = blockWidth.X - spaceSize;

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
            args.DrawingSession.DrawText(debugString, new Vector2(0, 0), Colors.LightGreen);
            foreach (var vb in blocks)
            {
                vb.Draw(args.DrawingSession);
            }
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
                if (vb.PointInMainRect(point))
                {
                    field.SetBlockSwapping(vb.UnderlyingBlock);
                    break;
                }
            }
        }
    }
}
