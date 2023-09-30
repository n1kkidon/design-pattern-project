using Avalonia.Controls;
using game_client.Views;
using shared;

namespace game_client.Models;

public class GameObject
{
    private Vector2 _location;
    public Vector2 Location { 
        get { 
            _location.Y = (float)Canvas.GetBottom(stackPanel) + GetHeight()/2;
            _location.X = (float)Canvas.GetLeft(stackPanel) + GetWidth()/2;
            return _location;
        }
        protected set {  
            Canvas.SetBottom(stackPanel, value.Y - GetHeight()/2);
            Canvas.SetLeft(stackPanel, value.X - GetWidth()/2);
        }
    }
    protected StackPanel stackPanel {get; set;}

    protected GameObject(Vector2 location)
    {
        stackPanel = new();
        Location = location;
    }

    public float GetHeight()
    {
        double total = 0;
        foreach(var child in stackPanel.Children)
        {
            if(double.IsNaN(child.Height))
                continue;
            total += child.Height;
        }
        return (float)total;
    } 
    public float GetWidth()
    {
        double total = 0;
        foreach(var child in stackPanel.Children)
        {
            if(double.IsNaN(child.Width))
                continue;
            total += child.Width;
        }
        return (float)total;
    }
    
    public void AddObjectToCanvas() => MainWindow.GetInstance().canvas.Children.Add(stackPanel);
    public void RemoveObjectFromCanvas() => MainWindow.GetInstance().canvas.Children.Remove(stackPanel);

    public void TeleportTo(Vector2 location)
    {
        Location = location;
    }
}
