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
        Playfield field = new Playfield();
        Vector2 gridOrigin = new Vector2(50, 50);
        Vector2 blockWidth = new Vector2(30, 0);
        Vector2 spaceWidth = new Vector2(40, 0);
        Vector2 blockHeight = new Vector2(0, 30);
        Vector2 spaceHeight = new Vector2(0, 40);
        
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            sender.TargetElapsedTime = TimeSpan.FromMilliseconds(250);
        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            field.GenerateBlocks();
        }

        private void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            for (int r = 0; r < Playfield.HEIGHT; r++)
            {
                for (int c = 0; c < Playfield.WIDTH; c++)
                {
                    var blockOrigin = gridOrigin
                        + (spaceWidth * c)
                        + (spaceHeight * r);

                    Color color;
                    switch (field.BlockAt(r, c))
                    {
                        case BlockTypes.Empty:
                            color = Colors.Black;
                            break;
                        case BlockTypes.Red:
                            color = Colors.Red;
                            break;
                        case BlockTypes.White:
                            color = Colors.White;
                            break;
                        case BlockTypes.Blue:
                            color = Colors.Blue;
                            break;
                    }

                    float strokeSize = blockWidth.X / 2.0f;

                    args.DrawingSession.DrawRectangle(
                        blockOrigin.X + strokeSize, blockOrigin.Y + strokeSize,
                        blockWidth.X - strokeSize, blockHeight.Y - strokeSize, 
                        color, strokeSize);
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }
    }
}
