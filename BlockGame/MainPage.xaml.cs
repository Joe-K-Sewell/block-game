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
            GameBlock UnderlyingBlock;
            
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

            private Rect FilledRectangle
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

            public void Draw(CanvasDrawingSession session)
            {
                session.DrawRectangle(FilledRectangle, FillColor, strokeSize);
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
        static readonly Vector2 gridOrigin = new Vector2(50, 50);
        static readonly Vector2 blockWidth = new Vector2(50, 0);
        static readonly Vector2 spaceWidth = new Vector2(60, 0);
        static readonly Vector2 blockHeight = new Vector2(0, 50);
        static readonly Vector2 spaceHeight = new Vector2(0, 60);
        static readonly float spaceSize = blockWidth.X / 2.0f;
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
    }
}
