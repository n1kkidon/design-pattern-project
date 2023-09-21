using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using shared;

namespace game_client.Views;

public class PlayerPixel
{
    public StackPanel PlayerObject;
    private double X, Y;
    private TextBlock NameTag;
    private Rectangle Pixel;
    private readonly double MoveStep = 10;

    public PlayerPixel(string name, Color color, double x, double y){
        X = x;
        Y = y;
        PlayerObject = new();
        NameTag = new(){
            Foreground = new SolidColorBrush(Colors.White),
            Text = name,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };
        Pixel = new(){
            Fill = new SolidColorBrush(color),
            Width = 15,
            Height = 15
        };
        PlayerObject.Children.Add(NameTag);
        PlayerObject.Children.Add(Pixel);
        Canvas.SetLeft(PlayerObject, X);
        Canvas.SetTop(PlayerObject, Y);
    }

    public void Move(Direction direction){
        switch(direction){
            case Direction.UP: 
                Y -= MoveStep;
                break;
            case Direction.DOWN:
                Y += MoveStep;
                break;
            case Direction.LEFT:
                X -= MoveStep;
                break;
            case Direction.RIGHT:
                X += MoveStep;
                break;
        }
        Canvas.SetLeft(PlayerObject, X);
        Canvas.SetTop(PlayerObject, Y);
    }
}
