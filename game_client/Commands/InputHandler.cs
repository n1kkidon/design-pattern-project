using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input;

namespace game_client;

public static class InputHandler
{
    private static readonly Dictionary<Key, ICommand> keyMapping = new();

    public static IEnumerable<Key> KeysWithRegistedCommands { get => keyMapping.Keys; }

    public static void SetCommand(Key key, ICommand command)
    {
        keyMapping[key] = command;
    }

    public static async Task HandleInput(Key key)
    {
        if (keyMapping.TryGetValue(key, out ICommand? value))
        {
            await value.Execute();
        }
    }

    public static bool RemoveCommand(Key key) => keyMapping.Remove(key);
}
