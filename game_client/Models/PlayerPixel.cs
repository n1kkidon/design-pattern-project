using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using shared;

namespace game_client.Models;

public class PlayerPixel
{
    public StackPanel PlayerObject;
    private double X, Y;
    private TextBlock NameTag;
    private Rectangle Pixel;

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
                Y -= Constants.MoveStep;
                break;
            case Direction.DOWN:
                Y += Constants.MoveStep;
                break;
            case Direction.LEFT:
                X -= Constants.MoveStep;
                break;
            case Direction.RIGHT:
                X += Constants.MoveStep;
                break;
        }
        Canvas.SetLeft(PlayerObject, X);
        Canvas.SetTop(PlayerObject, Y);
    }
}
