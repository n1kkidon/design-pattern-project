using shared;

namespace game_server.Iterator
{
    public struct GameObjectWithKey
    {
        public string Key { get; private set; }
        public CanvasObjectInfo ObjectInfo { get; private set; }

        public GameObjectWithKey(string key, CanvasObjectInfo objectInfo)
        {
            Key = key;
            ObjectInfo = objectInfo;
        }
    }

}
