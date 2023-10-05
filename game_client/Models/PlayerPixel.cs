using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using shared;

namespace game_client.Models;

public class PlayerPixel : GameObject
{
    private TextBlock NameTag;
    private Rectangle Pixel;
    private Rectangle HealthBar;

    private int health = 10;

    public PlayerPixel(string name, Color color, Vector2 location) : base(location){ //initial location 
        NameTag = new(){
            Foreground = new SolidColorBrush(Colors.White),
            Text = name,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };
        Pixel = new(){
            Fill = new SolidColorBrush(color),
            Width = 16,
            Height = 16
        };
        AddToStackPanel(NameTag, true);
        AddToStackPanel(Pixel);

        HealthBar = new Rectangle
        {
            Width = 30,
            Height = 10,
            Fill = Brushes.Green, // Initial health bar color
        };
        AddToStackPanel(HealthBar, true);
    }

    public void UpdateHealthBar()
    {
        // Calculate the width of the health bar based on the player's health
        double healthBarWidth = (health / 10.0) * 30; // Assuming max health is 10 and health bar width is 30

        // Update the health bar's width
        HealthBar.Width = healthBarWidth;

        // Change the color of the health bar based on the player's health
        if (health <= 7)
        {
            HealthBar.Fill = Brushes.Red; // Low health is red
        }
        else
        {
            HealthBar.Fill = Brushes.Green; // Normal health is green
        }
    }

    public void DecreaseHealth()
    {
        if (health > 0)
        {
            health--;
        }
    }
}
