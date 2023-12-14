using shared;

namespace game_client.ChainOfResponsibility;

public class PlayerJoinRequest
{
    public string? Name { get; set; }
    public WeaponType SelectedWeapon { get; set; }
    public bool IsValid { get; set; } = true;

}