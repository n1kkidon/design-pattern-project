using System.Globalization;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using game_client.Observer;
using shared;
using System.Collections.Generic;
using game_client.Socket;
using game_client.Models.PlayerState;
using System;
using Avalonia.Controls.Shapes;

namespace game_client.Models
{
    public class PlayerPixel : GameObject, ISubject
    {
        private IPlayerState _currentState;
        private TextBlock NameTag;
        private Image PlayerAvatar; 
        private Rectangle HealthBar;
        private ShootAlgorithm _shootAlgorithm;
        public ShootAlgorithm ShootAlgorithm => _shootAlgorithm;
        SocketService _socketService = SocketService.GetInstance();

        private int health = 10;

        public PlayerPixel(string name, Color color, Vector2 location) : base(location)
        {
            _shootAlgorithm = new Pistol(_socketService); 
            NameTag = new TextBlock
            {
                Foreground = new SolidColorBrush(Colors.White),
                Text = name,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            PlayerAvatar = new Image
            {
                Width = 30,
                Height = 30
            };

            try
            {
                PlayerAvatar.Source = new Bitmap("./Assets/player.png");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading player image: {ex.Message}");
            }

            AddToStackPanel(NameTag, true);
            AddToStackPanel(PlayerAvatar);

            HealthBar = new Rectangle
            {
                Width = 30,
                Height = 10,
                Fill = Brushes.Green,
            };
            AddToStackPanel(HealthBar, true);

            new HealthBarObserver(this, HealthBar);

            _currentState = new NormalState(this);
        }

        private List<IObserver> observers = new List<IObserver>();
        public void RegisterObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach (var observer in observers)
            {
                observer.Update(health);
            }
        }

        public void DecreaseHealth()
        {
            if (health > 0)
            {
                health--;
                NotifyObservers();
            }
            CheckForStateTransition();
        }

        private void CheckForStateTransition()
        {
            if (health <= 0 && !(_currentState is DeadState))
            {
                SetState(new DeadState(this));
            }
            else if (health < 7 && _currentState is NormalState)
            {
                SetState(new InjuredState(this));
            }
        }

        public void SetShootingAlgorithm(ShootAlgorithm shootAlgo)
        {
            _shootAlgorithm = shootAlgo;
        }

        public async void Shoot(IVector2 position)
        {
            _currentState.Shoot(position);
        }

        public void SetState(IPlayerState newState)
        {
            _currentState = newState;
        }

    }
}
