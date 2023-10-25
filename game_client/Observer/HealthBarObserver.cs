using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using game_client.Models;
using game_client.Observer;

public class HealthBarObserver : IObserver
{
    private readonly PlayerPixel _playerPixel;
    private readonly Rectangle _healthBar;

    public HealthBarObserver(PlayerPixel playerPixel, Rectangle healthBar)
    {
        _playerPixel = playerPixel;
        _healthBar = healthBar;
        _playerPixel.RegisterObserver(this); // Register this observer
    }

    public void Update(int health)
    {
        // Logic to update the health bar UI based on health
        double healthBarWidth = (health / 10.0) * 30;

        _healthBar.Width = healthBarWidth;

        if (health <= 7)
        {
            _healthBar.Fill = Brushes.Red;
            Console.WriteLine("Health is critical! Health value: " + health);
        }
        else
        {
            _healthBar.Fill = Brushes.Green;
            Console.WriteLine("Health is good. Health value: " + health);
        }
    }
}

