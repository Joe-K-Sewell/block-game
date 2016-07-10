using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

            public void Draw(CanvasDrawingSession session)
            {
                var blockOrigin = SpaceOrigin;
                float strokeSize = blockWidth.X / 2.0f;

                session.DrawRectangle(
                    blockOrigin.X + strokeSize, blockOrigin.Y + strokeSize,
                    blockWidth.X - strokeSize, blockHeight.Y - strokeSize,
                    FillColor, strokeSize);
            }
        }

        List<VisualBlock> blocks;
        static readonly Vector2 gridOrigin = new Vector2(50, 50);
        static readonly Vector2 blockWidth = new Vector2(30, 0);
        static readonly Vector2 spaceWidth = new Vector2(40, 0);
        static readonly Vector2 blockHeight = new Vector2(0, 30);
        static readonly Vector2 spaceHeight = new Vector2(0, 40);
        
        public MainPage()
        {
            this.InitializeComponent();

            var field = new Playfield();
            field.GenerateBlocks();

            blocks = field.blocks.Select(gb => new VisualBlock(gb)).ToList();
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
