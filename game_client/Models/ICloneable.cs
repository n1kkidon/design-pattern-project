namespace game_client.Models;

public interface ICloneable {
    ICloneable Clone();
    ICloneable ShallowClone();
    ICloneable DeepCopy();
}


