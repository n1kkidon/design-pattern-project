using System;
using System.Collections.Generic;
using Avalonia.Input;

namespace game_client;

public class Keyboard
{
    public static readonly HashSet<Key> Keys = new();

    /// <summary>
    /// returns true while key is held down
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsKeyPressed(Key key) => Keys.Contains(key);

}
