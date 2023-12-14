using System;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using game_client.Models;
using game_client.Models.CanvasItems;
using game_client.Observer;

public class EnemyHealthBarObserver : IObserver
{
    private readonly EnemyPixel _enemyPixel;
    private readonly Rectangle _healthBar;

    public EnemyHealthBarObserver(EnemyPixel enemyPixel, Rectangle healthBar)
    {
        _enemyPixel = enemyPixel;
        _healthBar = healthBar;
        _enemyPixel.RegisterObserver(this); 
    }

    public void Update(int health)
    {
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