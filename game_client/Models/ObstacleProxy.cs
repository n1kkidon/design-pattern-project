using shared;

namespace game_client.Models
{
    public class ObstacleProxy : GameObject
    {
        private Obstacle _realObstacle;

        public ObstacleProxy(Obstacle realObstacle) : base(realObstacle.Location)
        {
            _realObstacle = realObstacle;
        }
        public void LoadImage()
        {
            _realObstacle.LoadImage();
        }

        public override void AddObjectToCanvas() 
        {
            _realObstacle.AddObjectToCanvas();
        }
    }
}