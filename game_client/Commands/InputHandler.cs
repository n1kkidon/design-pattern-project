using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input;
using game_client.Models;

namespace game_client;

public static class InputHandler
{
    private static readonly Dictionary<Key, ICommand> keyMapping = new();

    public static IEnumerable<Key> KeysWithRegistedCommands { get => keyMapping.Keys; }

    public static event Action<Key>? KeyDown;
    public static event Action<Key>? KeyUp;

    public static void SetCommand(Key key, ICommand command)
    {
        keyMapping[key] = command;
    }

    public static async Task HandleContinuousInput(Key key)
    {
        if (keyMapping.TryGetValue(key, out ICommand? value) && value.ContinuosExecuteOnKeyDown)
        {
            await value.Execute();
        }
    }

    public static bool RemoveCommand(Key key) => keyMapping.Remove(key);

    private static readonly HashSet<Key> Keys = new();

    /// <summary>
    /// returns true while key is held down
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsKeyPressed(Key key) => Keys.Contains(key);

    public static void AddKey(Key key) //on key press
    {
        Keys.Add(key);
        KeyDown?.Invoke(key);
        if(keyMapping.TryGetValue(key, out var command))
        {
            command.Execute();
        }
    } 
    public static void RemoveKey(Key key) //on key up
    {
        Keys.Remove(key);
        KeyUp?.Invoke(key);
        if(keyMapping.TryGetValue(key, out var command))
        {
            command.OnKeyUp();
        }
    }
}
