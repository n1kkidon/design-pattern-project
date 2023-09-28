using System;
using System.Collections.Generic;
using Avalonia.Input;

namespace game_client;

public class Keyboard
{
    public static readonly HashSet<Key> Keys = new();

    //returns true while key is held down
    public static bool IsKeyPressed(Key key) => Keys.Contains(key);

}
