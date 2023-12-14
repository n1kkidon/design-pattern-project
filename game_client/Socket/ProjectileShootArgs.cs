using game_client.Models.CanvasItems;
using shared;

namespace game_client.Socket;

public record ProjectileShootArgs(Vector2 ClickPoint, WeaponType WeaponType);